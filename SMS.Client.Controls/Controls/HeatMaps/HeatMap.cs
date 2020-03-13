using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SMS.Client.Controls
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SMS.Client.Controls.Controls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SMS.Client.Controls.Controls;assembly=SMS.Client.Controls.Controls"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:HeatMap/>
    ///
    /// </summary>
    public class HeatMap : Control
    {
        #region Fields

        private Image _heatMapImage = null;
        private ConcurrentDictionary<int, double[]> _heatPointCoefficientMatrixDict = new ConcurrentDictionary<int, double[]>();

        #endregion

        #region Properties

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty HeatMapWidthProperty =
            DependencyProperty.Register("HeatMapWidth", typeof(int), typeof(HeatMap), new PropertyMetadata(1920, OnHeatMapWidthPropertyChanged));
        public static readonly DependencyProperty HeatMapHeightProperty =
            DependencyProperty.Register("HeatMapHeight", typeof(int), typeof(HeatMap), new PropertyMetadata(1080, OnHeatMapHeightPropertyChanged));
        public static readonly DependencyProperty ColorPointsProperty =
            DependencyProperty.Register("ColorPoints", typeof(ObservableCollection<HeatMapPaletteColorPoint>), typeof(HeatMap), new PropertyMetadata(null, OnColorPointsChanged));

        #endregion

        #region Dependency Property Wrappers

        public int HeatMapWidth
        {
            get { return (int)GetValue(HeatMapWidthProperty); }
            set { SetValue(HeatMapWidthProperty, value); }
        }

        public int HeatMapHeight
        {
            get { return (int)GetValue(HeatMapHeightProperty); }
            set { SetValue(HeatMapHeightProperty, value); }
        }

        public ObservableCollection<HeatMapPaletteColorPoint> ColorPoints
        {
            get { return (ObservableCollection<HeatMapPaletteColorPoint>)GetValue(ColorPointsProperty); }
            set { SetValue(ColorPointsProperty, value); }
        }
        #endregion

        #region Constructors

        static HeatMap()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HeatMap), new FrameworkPropertyMetadata(typeof(HeatMap)));
        }

        #endregion

        #region Private Methods

        private static void OnHeatMapHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeatMap heatMap = d as HeatMap;
            if (heatMap != null && heatMap._heatMapImage != null)
            {
                heatMap._heatMapImage.Height = (double)e.NewValue;
            }
        }

        private static void OnHeatMapWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeatMap heatMap = d as HeatMap;
            if (heatMap != null && heatMap._heatMapImage != null)
            {
                heatMap._heatMapImage.Width = (double)e.NewValue;
            }
        }

        private static void OnColorPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeatMap heatMap = d as HeatMap;
            ObservableCollection<HeatMapPaletteColorPoint> oldColorPoints = e.OldValue as ObservableCollection<HeatMapPaletteColorPoint>;
            ObservableCollection<HeatMapPaletteColorPoint> newColorPoints = e.NewValue as ObservableCollection<HeatMapPaletteColorPoint>;
            if (newColorPoints != null)
            {

            }
        }

        private double GetDecayFactor(int radius, double pointCenterHeatValue)
        {
            return Math.Pow(-Math.Pow(radius, 2) / (2 * Math.Log(1.0 / 255.0 / pointCenterHeatValue, Math.E)), 0.5);
        }

        private BitmapImage DrawHeatMapImage(IList<HeatPoint> heatPointList)
        {
            if (heatPointList == null || heatPointList.Count == 0)
            {
                return null;
            }

            //1 通过热力点计算图上每一个像素点的热力值
            int arrayLength = 0;
            Dispatcher.Invoke(() =>
            {
                arrayLength = HeatMapWidth * HeatMapHeight;
            });

            double[] heatValueArray = new double[arrayLength];
            Parallel.ForEach(heatPointList, heatPoint =>
            {
                if (heatPoint.X <= 0 || heatPoint.Y <= 0)
                {
                    return;
                }

                //计算单点的热力点影响力系数矩阵
                double[] heatValueCoefficientMatrixOfPoint = null;
                if (_heatPointCoefficientMatrixDict.ContainsKey(heatPoint.EffectiveRadius))
                {
                    heatValueCoefficientMatrixOfPoint = _heatPointCoefficientMatrixDict[heatPoint.EffectiveRadius];
                }
                else
                {
                    heatValueCoefficientMatrixOfPoint = GetHeatValueCoefficientMatrixOfPoint(heatPoint.EffectiveRadius);
                    if (heatPoint.EffectiveRadius < 100)
                    {
                        _heatPointCoefficientMatrixDict.AddOrUpdate(heatPoint.EffectiveRadius, heatValueCoefficientMatrixOfPoint, (k, v) => v);
                    }
                }

                //计算单点的热力值
                double[] heatValueArrayOfPoint = GetHeatValuesOfPoint(heatPoint.Weight, heatValueCoefficientMatrixOfPoint);

                //将单点热力值叠加到总热力值上
                AddPointHeatValuesToTotalHeatValues(heatPoint.EffectiveRadius, heatValueArray, heatValueArrayOfPoint, heatPoint);
            });

            //标准化热力值
            NormalizeHeatValues(heatValueArray);

            //2 将热力值映射到渐变调色板的颜色
            byte[] colorizedHeatValueArray = ColorizeHeatValue(heatValueArray);

            //3 画图
            BitmapImage bitmapImage = null;
            Dispatcher.Invoke(() =>
            {
                bitmapImage = ImageProcessHelper.ArgbBytesToBitmapImage(colorizedHeatValueArray, HeatMapWidth, HeatMapHeight);
            });

            return bitmapImage;
        }

        private void NormalizeHeatValues(double[] heatValueArray)
        {
            for (int i = 0; i < heatValueArray.Length; i++)
            {
                if (heatValueArray[i] > 1)
                {
                    heatValueArray[i] = 1;
                }
            }
        }

        private Color GetHeatPointColorByHeatValue(double heatValue, IList<HeatMapPaletteColorPoint> colorPoints)
        {
            for (int i = 0; i < colorPoints.Count - 1; i++)
            {
                if (IsHeatValueBetweenTwoColorPoints(heatValue, colorPoints[i], colorPoints[i + 1]))
                {
                    return GetInterpolationColorByHeatValue(heatValue, colorPoints[i], colorPoints[i + 1]);
                }
            }

            return new Color();
        }

        private Color GetInterpolationColorByHeatValue(double heatValue, HeatMapPaletteColorPoint frontColorPoint, HeatMapPaletteColorPoint backColorPoint)
        {
            double offsetDifference = (heatValue - frontColorPoint.Offset) / (backColorPoint.Offset - frontColorPoint.Offset);
            byte colorR = (byte)(frontColorPoint.Color.R + (backColorPoint.Color.R - frontColorPoint.Color.R) * offsetDifference);
            byte colorG = (byte)(frontColorPoint.Color.G + (backColorPoint.Color.G - frontColorPoint.Color.G) * offsetDifference);
            byte colorB = (byte)(frontColorPoint.Color.B + (backColorPoint.Color.B - frontColorPoint.Color.B) * offsetDifference);

            return Color.FromRgb(colorR, colorG, colorB);
        }

        private bool IsHeatValueBetweenTwoColorPoints(double heatValue, HeatMapPaletteColorPoint frontColorPoint, HeatMapPaletteColorPoint backColorPoint)
        {
            return frontColorPoint.Offset <= heatValue && backColorPoint.Offset >= heatValue;
        }

        private byte[] ColorizeHeatValue(double[] heatValueArray)
        {
            if (!HasPalette())
            {
                return null;
            }

            int pixelCount = 0;
            List<HeatMapPaletteColorPoint> colorPoints = null;
            Dispatcher.Invoke(() =>
            {
                pixelCount = HeatMapWidth * HeatMapHeight;
                colorPoints = ColorPoints.ToList();
            });

            byte[] colorizedHeatValueArray = new byte[pixelCount * 4];
            for (int i = 0; i < pixelCount; i++)
            {
                double heatValue = heatValueArray[i];

                Color color = GetHeatPointColorByHeatValue(heatValue, colorPoints);

                colorizedHeatValueArray[i * 4 + 0] = color.B;
                colorizedHeatValueArray[i * 4 + 1] = color.G;
                colorizedHeatValueArray[i * 4 + 2] = color.R;
                colorizedHeatValueArray[i * 4 + 3] = GetColorAlphaValue(heatValue);

                //Console.WriteLine(string.Format("H R G B : {0} - {1} - {2} - {3}", heatValue,
                //colorizedHeatValueArray[i * 4 + 2], colorizedHeatValueArray[i * 4 + 1], colorizedHeatValueArray[i * 4 + 0]));
            }

            return colorizedHeatValueArray;
        }

        private bool HasPalette()
        {
            bool hasPalette = false;
            Dispatcher.Invoke(() =>
            {
                hasPalette = ColorPoints != null && ColorPoints.Count > 0;
            });

            return hasPalette;
        }

        private byte GetColorAlphaValue(double heatValue)
        {
            double normalOpacity = 0.7;
            double normalOpacityThreshold = 0.4;
            double tailOpacityCoefficient = normalOpacity / normalOpacityThreshold;
            byte opacity;
            if (heatValue == 0)
            {
                opacity = 0;
            }
            else if (heatValue > normalOpacityThreshold)
            {
                opacity = (byte)(normalOpacity * 255);
            }
            else
            {
                opacity = (byte)(heatValue * 255 * tailOpacityCoefficient);
            }

            return opacity;
        }

        private void AddPointHeatValuesToTotalHeatValues(int radius, double[] heatValueArray, double[] heatValueArrayOfPoint, HeatPoint heatPoint)
        {
            int height = 0;
            int width = 0;

            Dispatcher.Invoke(() =>
            {
                height = HeatMapHeight;
                width = HeatMapWidth;
            });

            for (int row = -radius; row <= radius; row++)
            {
                for (int column = -radius; column <= radius; column++)
                {
                    if (heatPoint.Y + row < 0 || heatPoint.Y + row >= height
                        || heatPoint.X + column < 0 || heatPoint.X + column >= width)
                    {
                        continue;
                    }

                    int heatValueIndex = (heatPoint.Y + row) * width + heatPoint.X + column;
                    heatValueArray[heatValueIndex] += heatValueArrayOfPoint[(row + radius) * (2 * radius + 1) + column + radius];
                }
            }
        }

        private double[] GetHeatValueCoefficientMatrixOfPoint(int radius)
        {
            double decayFactor = GetDecayFactor(radius, 1);
            double[] heatValueCoefficientMatrix = new double[(2 * radius + 1) * (2 * radius + 1)];
            for (int row = -radius; row <= radius; row++)
            {
                for (int column = -radius; column <= radius; column++)
                {
                    double distanceToHeatPoint = Math.Pow(Math.Pow(row, 2) + Math.Pow(column, 2), 0.5);

                    int heatValueIndexInArray = (row + radius) * (2 * radius + 1) + column + radius;

                    //热力半径范围之外
                    if (distanceToHeatPoint > radius)
                    {
                        heatValueCoefficientMatrix[heatValueIndexInArray] = 0;
                        continue;
                    }

                    double heatValue = Math.Pow(Math.E, -Math.Pow(distanceToHeatPoint, 2) / (2 * Math.Pow(decayFactor, 2)));
                    //double heatValue = 1 - distanceToHeatPoint / radius;
                    if (heatValue < 0)
                    {
                        heatValueCoefficientMatrix[heatValueIndexInArray] = 0;
                        continue;
                    }

                    heatValueCoefficientMatrix[heatValueIndexInArray] = heatValue;
                }
            }

            return heatValueCoefficientMatrix;
        }

        private double[] GetHeatValuesOfPoint(double pointCenterHeatValue, double[] heatValueCoefficientMatrix)
        {
            double[] heatValueArrayOPoint = new double[heatValueCoefficientMatrix.Length];
            for (int i = 0; i < heatValueArrayOPoint.Length; i++)
            {
                double heatValue = pointCenterHeatValue * heatValueCoefficientMatrix[i];
                heatValueArrayOPoint[i] = heatValue;
            }

            return heatValueArrayOPoint;
        }

        #endregion

        #region Public Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _heatMapImage = GetTemplateChild("PART_HeatMapImage") as Image;
            if (_heatMapImage != null)
            {
                _heatMapImage.Width = HeatMapWidth;
                _heatMapImage.Height = HeatMapHeight;
            }
        }

        public void RefreshHeatMapImage(List<HeatPoint> heatMapList)
        {
            if (_heatMapImage == null)
            {
                return;
            }


            BitmapImage bitmap = DrawHeatMapImage(heatMapList);
            Dispatcher.Invoke(() =>
            {
                _heatMapImage.Source = bitmap;
            });
        }

        #endregion

    }
}

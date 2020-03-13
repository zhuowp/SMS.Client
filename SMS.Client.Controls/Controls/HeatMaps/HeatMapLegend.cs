using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
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
    ///     <MyNamespace:HeatMapLegend/>
    ///
    /// </summary>
    public class HeatMapLegend : Control
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty ColorPointsProperty =
            DependencyProperty.Register("ColorPoints", typeof(ObservableCollection<HeatMapPaletteColorPoint>), typeof(HeatMapLegend), new PropertyMetadata(null, OnColorPointsChanged));

        public static readonly DependencyProperty LegendTitleProperty =
            DependencyProperty.Register("LegendTitle", typeof(string), typeof(HeatMapLegend), new PropertyMetadata(""));

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(string), typeof(HeatMapLegend), new PropertyMetadata(""));

        public static readonly DependencyProperty LegendBrushProperty =
            DependencyProperty.Register("LegendBrush", typeof(Brush), typeof(HeatMapLegend), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public static readonly DependencyProperty TipsCollectionProperty =
            DependencyProperty.Register("TipsCollection", typeof(ObservableCollection<HeatMapLegendTip>), typeof(HeatMapLegend), new PropertyMetadata(null));

        #endregion

        #region Dependency Property Wrappers

        public ObservableCollection<HeatMapPaletteColorPoint> ColorPoints
        {
            get { return (ObservableCollection<HeatMapPaletteColorPoint>)GetValue(ColorPointsProperty); }
            set { SetValue(ColorPointsProperty, value); }
        }

        public string LegendTitle
        {
            get { return (string)GetValue(LegendTitleProperty); }
            set { SetValue(LegendTitleProperty, value); }
        }

        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        public Brush LegendBrush
        {
            get { return (Brush)GetValue(LegendBrushProperty); }
            set { SetValue(LegendBrushProperty, value); }
        }

        public ObservableCollection<HeatMapLegendTip> TipsCollection
        {
            get { return (ObservableCollection<HeatMapLegendTip>)GetValue(TipsCollectionProperty); }
            set { SetValue(TipsCollectionProperty, value); }
        }

        #endregion

        #region Constructors

        static HeatMapLegend()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HeatMapLegend), new FrameworkPropertyMetadata(typeof(HeatMapLegend)));
        }

        public HeatMapLegend()
        {
            TipsCollection = new ObservableCollection<HeatMapLegendTip>();
            SizeChanged += HeatMapLegend_SizeChanged;
        }

        #endregion

        #region Private Methods

        private static void OnColorPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeatMapLegend heatMapLegend = d as HeatMapLegend;
            ObservableCollection<HeatMapPaletteColorPoint> oldColorPoints = e.NewValue as ObservableCollection<HeatMapPaletteColorPoint>;
            if (oldColorPoints != null)
            {
                oldColorPoints.CollectionChanged -= heatMapLegend.HeatMapColorPointCollection_CollectionChanged;
            }

            heatMapLegend.TipsCollection.Clear();
            heatMapLegend.LegendBrush = null;
            ObservableCollection<HeatMapPaletteColorPoint> newColorPoints = e.NewValue as ObservableCollection<HeatMapPaletteColorPoint>;
            if (newColorPoints != null)
            {
                newColorPoints.CollectionChanged += heatMapLegend.HeatMapColorPointCollection_CollectionChanged;
                heatMapLegend.UpdateLegendLayoutByColorPoints(newColorPoints);
            }
        }

        private void HeatMapColorPointCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        private void HeatMapLegend_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                UpdateLegendLayoutByColorPoints(ColorPoints);
            }
        }

        private void UpdateLegendLayoutByColorPoints(IEnumerable<HeatMapPaletteColorPoint> colorPoints)
        {
            LegendBrush = GetLegendColorBarBrush(colorPoints);

            List<HeatMapLegendTip> heatMapLegendTips = GetLegendTips(colorPoints);
            TipsCollection.Clear();
            foreach (var legendTip in heatMapLegendTips)
            {
                TipsCollection.Add(legendTip);
            }
        }

        private LinearGradientBrush GetLegendColorBarBrush(IEnumerable<HeatMapPaletteColorPoint> colorPoints)
        {
            LinearGradientBrush brush = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 0) };
            if (colorPoints == null)
            {
                return brush;
            }

            foreach (HeatMapPaletteColorPoint colorPoint in colorPoints)
            {
                GradientStop gradientStop = new GradientStop() { Color = colorPoint.Color, Offset = colorPoint.Offset };
                brush.GradientStops.Add(gradientStop);
            }

            return brush;
        }

        private List<HeatMapLegendTip> GetLegendTips(IEnumerable<HeatMapPaletteColorPoint> colorPoints)
        {
            List<HeatMapLegendTip> heatMapLegendTips = new List<HeatMapLegendTip>();
            if (colorPoints == null || colorPoints.Count() == 0)
            {
                return heatMapLegendTips;
            }

            foreach (HeatMapPaletteColorPoint colorPoint in colorPoints)
            {
                HeatMapLegendTip legendTip = new HeatMapLegendTip();
                legendTip.Tips = colorPoint.Value.ToString();
                legendTip.Margin = new Thickness((ActualWidth - 30) * colorPoint.Offset + 5, 0, 0, 0);

                heatMapLegendTips.Add(legendTip);
            }

            return heatMapLegendTips;
        }

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        #endregion
    }
}

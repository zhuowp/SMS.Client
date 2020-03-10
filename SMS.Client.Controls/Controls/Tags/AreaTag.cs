using System;
using System.Collections.Generic;
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
    ///     xmlns:MyNamespace="clr-namespace:SMS.Client.Controls.Controls.Tags"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SMS.Client.Controls.Controls.Tags;assembly=SMS.Client.Controls.Controls.Tags"
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
    ///     <MyNamespace:AreaTag/>
    ///
    /// </summary>
    public class AreaTag : LineTextTagBase
    {
        #region Fields

        private Canvas _areaCanvas = null;
        private LineTextTag _lineTextTag = null;

        private Point _startPoint;
        private Point _endPoint;

        #endregion

        #region Properties

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty AreaPointsProperty =
            DependencyProperty.Register("AreaPoints", typeof(IList<Point>), typeof(AreaTag), new PropertyMetadata(null, OnAreaPointsChanged));

        public static readonly DependencyProperty AreaColorProperty =
            DependencyProperty.Register("AreaColor", typeof(Color), typeof(AreaTag), new PropertyMetadata(Colors.Blue));

        #endregion

        #region Dependency Property Wrappers

        public IList<Point> AreaPoints
        {
            get { return (IList<Point>)GetValue(AreaPointsProperty); }
            set { SetValue(AreaPointsProperty, value); }
        }

        public Color AreaColor
        {
            get { return (Color)GetValue(AreaColorProperty); }
            set { SetValue(AreaColorProperty, value); }
        }

        #endregion

        #region Constructors

        static AreaTag()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AreaTag), new FrameworkPropertyMetadata(typeof(AreaTag)));
        }

        #endregion

        #region Private Methods

        private static void OnAreaPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AreaTag areaTag = d as AreaTag;
            areaTag.UpdateTagLocationInScreen();
        }

        private void UpdateAreaShape()
        {
            if (_areaCanvas == null)
            {
                return;
            }
            _areaCanvas.Children.Clear();

            List<Point> localPoints = AreaPointsToLocalPoints();
            Path path = GetAreaShape(localPoints);
            if (path == null)
            {
                return;
            }

            _areaCanvas.Children.Add(path);
        }

        private List<Point> AreaPointsToLocalPoints()
        {
            List<Point> localPoints = new List<Point>();
            foreach (Point point in AreaPoints)
            {
                Point localPoint = new Point(point.X - _startPoint.X, point.Y - _startPoint.Y);
                localPoints.Add(localPoint);
            }

            return localPoints;
        }

        private Path GetAreaShape(List<Point> localPoints)
        {
            if (localPoints == null || localPoints.Count == 0)
            {
                return null;
            }

            Path path = new Path() { IsHitTestVisible = false };
            PathFigure pathFigure = new PathFigure();
            pathFigure.IsClosed = true;
            pathFigure.StartPoint = localPoints[0];
            for (int i = 1; i < localPoints.Count; i++)
            {
                LineSegment lineSegment = new LineSegment();
                lineSegment.Point = localPoints[i];
                pathFigure.Segments.Add(lineSegment);
            }

            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = new PathFigureCollection();
            pathGeometry.Figures.Add(pathFigure);

            path.Data = pathGeometry;
            path.Stroke = new SolidColorBrush(AreaColor);
            path.StrokeThickness = 2;
            path.StrokeDashArray = new DoubleCollection(new double[] { 4 });
            path.Fill = new SolidColorBrush(Color.FromArgb(0x66, AreaColor.R, AreaColor.G, AreaColor.B));
            return path;
        }

        private Point GetStartPoint(IList<Point> areaPoints)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;

            foreach (Point point in areaPoints)
            {
                if (point.X < minX)
                {
                    minX = point.X;
                }

                if (point.Y < minY)
                {
                    minY = point.Y;
                }
            }

            return new Point(minX, minY);
        }

        private Point GetEndPoint(IList<Point> areaPoints)
        {
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (Point point in areaPoints)
            {
                if (point.X > maxX)
                {
                    maxX = point.X;
                }

                if (point.Y > maxY)
                {
                    maxY = point.Y;
                }
            }

            return new Point(maxX, maxY);
        }

        private void UpdateTagSelfLocation()
        {
            Canvas.SetLeft(this, _startPoint.X);
            Canvas.SetTop(this, _startPoint.Y);
        }

        private void UpdateLineTextTagLocation()
        {
            double textTagLocationX = (_endPoint.X - _startPoint.X) / 2;
            double textTagLocationY = (_endPoint.Y - _startPoint.Y) / 2;

            TextTagLocation = new Point(textTagLocationX, textTagLocationY);
        }

        #endregion

        #region Protected Methods

        protected override void UpdateTagLocationInScreen()
        {
            if (_areaCanvas == null || _lineTextTag == null)
            {
                return;
            }

            _startPoint = GetStartPoint(AreaPoints);
            _endPoint = GetEndPoint(AreaPoints);

            UpdateAreaShape();
            UpdateTextTagLocation();
            UpdateLineTextTagLocation();
            UpdateTagSelfLocation();
        }

        #endregion

        #region Public Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _areaCanvas = GetTemplateChild("PART_Area") as Canvas;
            _lineTextTag = GetTemplateChild("PART_TextTag") as LineTextTag;
        }

        public override void StartWarningAnimation()
        {
            _lineTextTag?.StartWarningAnimation();
        }

        public override void StartLocateAnimation()
        {
            _lineTextTag?.StartLocateAnimation();
        }

        #endregion
    }
}

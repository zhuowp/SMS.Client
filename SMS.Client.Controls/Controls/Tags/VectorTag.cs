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
    ///     <MyNamespace:VectorTag/>
    ///
    /// </summary>
    public class VectorTag : LineTextTagBase
    {
        #region Fields

        private Canvas _vectorCanvas = null;
        private LineTextTag _lineTextTag = null;
        private Path _arrowShape = null;

        private Vector _vector;

        #endregion

        #region Properties

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty AreaPointsProperty =
            DependencyProperty.Register("AreaPoints", typeof(IList<Point>), typeof(VectorTag), new PropertyMetadata(null, OnAreaPointsChanged));

        public static readonly DependencyProperty ArrowBrushProperty =
            DependencyProperty.Register("ArrowBrush", typeof(Brush), typeof(VectorTag),
                new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#88FFFF00")), OnArrowBrushChanged));

        #endregion

        #region Dependency Property Wrappers

        public IList<Point> AreaPoints
        {
            get { return (IList<Point>)GetValue(AreaPointsProperty); }
            set { SetValue(AreaPointsProperty, value); }
        }

        public Brush ArrowBrush
        {
            get { return (Brush)GetValue(ArrowBrushProperty); }
            set { SetValue(ArrowBrushProperty, value); }
        }

        #endregion

        #region Constructors

        static VectorTag()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VectorTag), new FrameworkPropertyMetadata(typeof(VectorTag)));
        }

        #endregion

        #region Private Methods

        private static void OnAreaPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VectorTag vectorTag = d as VectorTag;
            vectorTag.UpdateTagLocationInScreen();
        }

        private static void OnArrowBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VectorTag vectorTag = d as VectorTag;
            vectorTag.UpdateArrowShapeBrush();
        }

        private void UpdateArrowShapeBrush()
        {
            if (_arrowShape == null)
            {
                return;
            }

            _arrowShape.Fill = ArrowBrush;
        }

        private void UpdateArrowShape(double headWidth = 30, double headHeigh = 70)
        {
            if (_vectorCanvas == null)
            {
                return;
            }

            _vectorCanvas.Children.Clear();
            List<Point> arrowKeyPoints = GetArrowShapeKeyPoints(headWidth, headHeigh);

            _arrowShape = GetArrowShape(arrowKeyPoints);
            _vectorCanvas.Children.Add(_arrowShape);
        }

        private Path GetArrowShape(List<Point> arrowKeyPoints)
        {
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = arrowKeyPoints[0];
            pathFigure.IsClosed = true;

            for (int i = 1; i < arrowKeyPoints.Count; i++)
            {
                LineSegment lineSegment = new LineSegment();
                lineSegment.Point = arrowKeyPoints[i];
                pathFigure.Segments.Add(lineSegment);
            }

            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = new PathFigureCollection();
            pathGeometry.Figures.Add(pathFigure);

            Path arrowShape = new Path
            {
                Data = pathGeometry,
                Stroke = ArrowBrush,
                StrokeThickness = 0,
                Fill = ArrowBrush
            };

            return arrowShape;
        }

        private List<Point> GetArrowShapeKeyPoints(double headWidth, double headHeigh)
        {
            List<Point> arrowShapePoints = new List<Point>();

            Point p1 = new Point(0, 0);
            Point p2 = p1 + _vector;
            double theta = Math.Atan2(p1.Y - p2.Y, p1.X - p2.X);
            double sint = Math.Sin(theta);
            double cost = Math.Cos(theta);

            Point p3 = new Point(
            p2.X + (headHeigh * cost - headWidth * sint),
            p2.Y + (headHeigh * sint + headWidth * cost));

            Point p4 = new Point(
            p2.X + (headHeigh * cost - headWidth / 2 * sint),
            p2.Y + (headHeigh * sint + headWidth / 2 * cost));

            Point p5 = new Point(
            p2.X + (headHeigh * cost + headWidth * sint),
            p2.Y - (headWidth * cost - headHeigh * sint));

            Point p6 = new Point(
            p2.X + (headHeigh * cost + headWidth / 2 * sint),
            p2.Y - (headWidth / 2 * cost - headHeigh * sint));

            arrowShapePoints.Add(p1);
            arrowShapePoints.Add(p4);
            arrowShapePoints.Add(p3);
            arrowShapePoints.Add(p2);
            arrowShapePoints.Add(p5);
            arrowShapePoints.Add(p6);

            return arrowShapePoints;
        }

        #endregion

        #region Protected Methods

        protected override void UpdateTagLocationInScreen()
        {
            if (AreaPoints == null || AreaPoints.Count != 2)
            {
                return;
            }

            _vector = AreaPoints[1] - AreaPoints[0];
            Location = AreaPoints[0];

            UpdateArrowShape();
            UpdateTextTagLocation();

            Canvas.SetLeft(this, Location.X);
            Canvas.SetTop(this, Location.Y);
        }

        #endregion

        #region Public Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _vectorCanvas = GetTemplateChild("PART_Area") as Canvas;
            _lineTextTag = GetTemplateChild("PART_TextTag") as LineTextTag;

            UpdateTagLocationInScreen();
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

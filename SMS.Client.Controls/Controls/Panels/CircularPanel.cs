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
    ///     xmlns:MyNamespace="clr-namespace:SMS.Client.Controls.Controls.Panels"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SMS.Client.Controls.Controls.Panels;assembly=SMS.Client.Controls.Controls.Panels"
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
    ///     <MyNamespace:CircularPanel/>
    ///
    /// </summary>
    public class CircularPanel : Panel
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register("RadiusX", typeof(double), typeof(CircularPanel), new PropertyMetadata(100.0, OnRadiusChanged));

        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register("RadiusY", typeof(double), typeof(CircularPanel), new PropertyMetadata(100.0, OnRadiusChanged));

        #endregion

        #region Dependency Property Wrappers

        public double RadiusX
        {
            get { return (double)GetValue(RadiusXProperty); }
            set { SetValue(RadiusXProperty, value); }
        }

        public double RadiusY
        {
            get { return (double)GetValue(RadiusYProperty); }
            set { SetValue(RadiusYProperty, value); }
        }

        #endregion

        #region Constructors

        static CircularPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularPanel), new FrameworkPropertyMetadata(typeof(CircularPanel)));
        }

        #endregion

        #region Private Methods

        private static void OnRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularPanel circularPanel = (CircularPanel)d;
            circularPanel.InvalidateArrange();
        }

        #endregion

        #region Protected Methods

        protected override Size MeasureOverride(Size availableSize)
        {
            Size s = base.MeasureOverride(availableSize);
            foreach (UIElement element in Children)
            {
                element.Measure(availableSize);
            }
            return s;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Clip = new RectangleGeometry { Rect = new Rect(0, 0, finalSize.Width, finalSize.Height) };

            int i = 0;
            double degreesOffset = 360.0 / Children.Count;

            foreach (FrameworkElement element in this.Children)
            {
                double centerX = element.DesiredSize.Width / 2.0;
                double centerY = element.DesiredSize.Height / 2.0;

                double degreesAngle = degreesOffset * i++;

                RotateTransform rotateTransform = new RotateTransform();
                rotateTransform.CenterX = centerX;
                rotateTransform.CenterY = centerY;

                rotateTransform.Angle = degreesAngle;
                element.RenderTransform = rotateTransform;

                double radianAngle = (Math.PI * degreesAngle) / 180.0;

                double x = RadiusX * Math.Cos(radianAngle);
                double y = RadiusY * Math.Sin(radianAngle);

                var rectX = x + (finalSize.Width / 2.0) - centerX;
                var rectY = y + (finalSize.Height / 2.0) - centerY;

                element.Arrange(new Rect(rectX, rectY, element.DesiredSize.Width, element.DesiredSize.Height));
            }
            return finalSize;
        }

        #endregion

        #region Public Methods

        #endregion

    }
}

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
    ///     xmlns:MyNamespace="clr-namespace:SMS.Client.Controls.Controls.VideoPlayers"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SMS.Client.Controls.Controls.VideoPlayers;assembly=SMS.Client.Controls.Controls.VideoPlayers"
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
    ///     <MyNamespace:PtzController/>
    ///
    /// </summary>
    public class PtzController : Control
    {
        #region Fields

        private ItemsControl _directionItemsControl = null;
        private FrameworkElement _zoomInElement = null;
        private FrameworkElement _zoomOutElement = null;
        private Slider _speedSlider = null;

        #endregion

        #region Properties

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty MaxSpeedProperty
            = DependencyProperty.Register("MaxSpeed", typeof(double), typeof(PtzController), new PropertyMetadata(255.0));

        public static readonly DependencyProperty MinSpeedProperty
            = DependencyProperty.Register("MinSpeed", typeof(double), typeof(PtzController), new PropertyMetadata(1.0));

        public static readonly DependencyProperty SpeedTickFrequencyProperty
            = DependencyProperty.Register("SpeedTickFrequency", typeof(double), typeof(PtzController), new PropertyMetadata(1.0));

        public static readonly DependencyProperty OuterRadiusXProperty
            = DependencyProperty.Register("OuterRaidusX", typeof(double), typeof(PtzController), new PropertyMetadata(200.0));

        public static readonly DependencyProperty OuterRadiusYProperty
            = DependencyProperty.Register("OuterRadiusY", typeof(double), typeof(PtzController), new PropertyMetadata(200.0));

        public static readonly DependencyProperty InnerRadiusXProperty
            = DependencyProperty.Register("InnerRaidusX", typeof(double), typeof(PtzController), new PropertyMetadata(80.0));

        public static readonly DependencyProperty InnerRadiusYProperty
            = DependencyProperty.Register("InnerRadiusY", typeof(double), typeof(PtzController), new PropertyMetadata(80.0));

        public static readonly DependencyProperty DirectionControllerOffsetXProperty
            = DependencyProperty.Register("DirectionControllerOffsetX", typeof(double), typeof(PtzController), new PropertyMetadata(70.0));

        public static readonly DependencyProperty DirectionControllerOffsetYProperty
            = DependencyProperty.Register("DirectionControllerOffsetY", typeof(double), typeof(PtzController), new PropertyMetadata(70.0));

        public static readonly DependencyProperty ControllerNormalBrushProperty
            = DependencyProperty.Register("ControllerNormalBrush", typeof(Brush), typeof(PtzController), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public static readonly DependencyProperty ControllerMouseOverBrushProperty
            = DependencyProperty.Register("ControllerMouseOverBrush", typeof(Brush), typeof(PtzController), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        #endregion

        #region Dependency Property Wrappers

        public double MaxSpeed
        {
            get { return (double)GetValue(MaxSpeedProperty); }
            set { SetValue(MaxSpeedProperty, value); }
        }

        public double MinSpeed
        {
            get { return (double)GetValue(MinSpeedProperty); }
            set { SetValue(MinSpeedProperty, value); }
        }

        public double SpeedTickFrequency
        {
            get { return (double)GetValue(SpeedTickFrequencyProperty); }
            set { SetValue(SpeedTickFrequencyProperty, value); }
        }

        public double OuterRadiusX
        {
            get { return (double)GetValue(OuterRadiusXProperty); }
            set { SetValue(OuterRadiusXProperty, value); }
        }

        public double OuterRadiusY
        {
            get { return (double)GetValue(OuterRadiusYProperty); }
            set { SetValue(OuterRadiusYProperty, value); }
        }

        public double InnerRadiusX
        {
            get { return (double)GetValue(InnerRadiusXProperty); }
            set { SetValue(InnerRadiusXProperty, value); }
        }

        public double InnerRadiusY
        {
            get { return (double)GetValue(InnerRadiusYProperty); }
            set { SetValue(InnerRadiusYProperty, value); }
        }

        public double DirectionControllerOffsetX
        {
            get { return (double)GetValue(DirectionControllerOffsetXProperty); }
            set { SetValue(DirectionControllerOffsetXProperty, value); }
        }

        public double DirectionControllerOffsetY
        {
            get { return (double)GetValue(DirectionControllerOffsetYProperty); }
            set { SetValue(DirectionControllerOffsetYProperty, value); }
        }

        public Brush ControllerNormalBrush
        {
            get { return (Brush)GetValue(ControllerNormalBrushProperty); }
            set { SetValue(ControllerNormalBrushProperty, value); }
        }

        public Brush ControllerMouseOverBrush
        {
            get { return (Brush)GetValue(ControllerMouseOverBrushProperty); }
            set { SetValue(ControllerMouseOverBrushProperty, value); }
        }

        #endregion

        #region Routed Events

        public static readonly RoutedEvent PtzControlRoutedEvent
            = EventManager.RegisterRoutedEvent("PtzControl", RoutingStrategy.Bubble, typeof(EventHandler<PtzControlRoutedEventArgs>), typeof(PtzController));

        #endregion

        #region Routed Event Wrappers

        public event RoutedEventHandler PtzControl
        {
            add { AddHandler(PtzControlRoutedEvent, value); }
            remove { RemoveHandler(PtzControlRoutedEvent, value); }
        }

        #endregion

        #region Constructors

        static PtzController()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PtzController), new FrameworkPropertyMetadata(typeof(PtzController)));
        }

        public PtzController()
        {
            Loaded += PtzController_Loaded;
        }

        #endregion

        #region Override Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _directionItemsControl = GetTemplateChild("PART_DirectionOperators") as ItemsControl;
            if (_directionItemsControl != null)
            {
                _directionItemsControl.ItemsSource = new List<PTZControlType> { PTZControlType.Right, PTZControlType.DownRight, PTZControlType.Down, PTZControlType.DownLeft,
                    PTZControlType.Left, PTZControlType.UpLeft, PTZControlType.Up, PTZControlType.UpRight};
            }

            _zoomInElement = GetTemplateChild("PART_ZoomIn") as FrameworkElement;
            if (_zoomInElement != null)
            {
                _zoomInElement.Tag = PTZControlType.ZoomIn;
                _zoomInElement.MouseLeftButtonDown -= Item_MouseLeftButtonOperate;
                _zoomInElement.MouseLeftButtonUp -= Item_MouseLeftButtonOperate;

                _zoomInElement.MouseLeftButtonDown += Item_MouseLeftButtonOperate;
                _zoomInElement.MouseLeftButtonUp += Item_MouseLeftButtonOperate;
            }

            _zoomOutElement = GetTemplateChild("PART_ZoomOut") as FrameworkElement;
            if (_zoomOutElement != null)
            {
                _zoomOutElement.Tag = PTZControlType.ZoomOut;
                _zoomOutElement.MouseLeftButtonDown -= Item_MouseLeftButtonOperate;
                _zoomOutElement.MouseLeftButtonUp -= Item_MouseLeftButtonOperate;

                _zoomOutElement.MouseLeftButtonDown += Item_MouseLeftButtonOperate;
                _zoomOutElement.MouseLeftButtonUp += Item_MouseLeftButtonOperate;
            }

            _speedSlider = GetTemplateChild("PART_SpeedSlider") as Slider;
        }

        #endregion

        #region Private Methods

        private void PtzController_Loaded(object sender, RoutedEventArgs e)
        {
            if (_directionItemsControl != null)
            {
                List<FrameworkElement> elementList = LVTreeHelper.FindVisualChildren<FrameworkElement>(_directionItemsControl, "DirectionController");
                if (elementList == null || elementList.Count == 0)
                {
                    return;
                }

                //注册方向控制按钮点击事件
                foreach (FrameworkElement element in elementList)
                {
                    element.MouseLeftButtonDown -= Item_MouseLeftButtonOperate;
                    element.MouseLeftButtonUp -= Item_MouseLeftButtonOperate;

                    element.MouseLeftButtonDown += Item_MouseLeftButtonOperate;
                    element.MouseLeftButtonUp += Item_MouseLeftButtonOperate;
                }
            }
        }

        private void Item_MouseLeftButtonOperate(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element == null || element.Tag == null)
            {
                return;
            }

            PTZControlType ptzCtrlType = (PTZControlType)element.Tag;

            int stopFlag = 1;
            MouseButtonState mouseState = e.ButtonState;
            if (mouseState == MouseButtonState.Pressed)
            {
                element.CaptureMouse();
                stopFlag = 0;
            }
            else
            {
                element.ReleaseMouseCapture();
            }

            int speed = 0;
            if (_speedSlider != null)
            {
                speed = (int)_speedSlider.Value;
            }

            PtzControlRoutedEventArgs eventArgs = new PtzControlRoutedEventArgs(PtzControlRoutedEvent, this)
            {
                PtzControlType = ptzCtrlType,
                StopFlag = stopFlag,
                Speed = speed,
            };

            RaiseEvent(eventArgs);
        }

        #endregion

        #region Public Methods

        #endregion
    }
}

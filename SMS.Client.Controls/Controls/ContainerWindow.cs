using System;
using System.Windows;
using System.Windows.Controls;

namespace SMS.Client.Controls
{
    public class ContainerWindow : Window
    {
        #region Fields

        private bool _isClosed = false;
        private readonly Control _hostView = null;

        #endregion

        #region Properties

        public Window ParentWindow { get; private set; }

        #endregion

        #region Constructors

        public ContainerWindow(Control hostView, bool allowsTransparency = true)
        {
            InitWindowProperty(allowsTransparency);

            _hostView = hostView ?? throw new Exception("The host view cannot be null!");

            _hostView.DataContextChanged += HostView_DataContextChanged;
            _hostView.FocusableChanged += HostView_FocusableChanged;
            _hostView.IsEnabledChanged += HostView_IsEnabledChanged;
            _hostView.IsHitTestVisibleChanged += HostView_IsHitTestVisibleChanged;
            _hostView.IsMouseCapturedChanged += HostView_IsMouseCapturedChanged;
            _hostView.IsVisibleChanged += HostView_IsVisibleChanged;
            _hostView.SizeChanged += HostView_SizeChanged;

            Loaded += ContainerWindow_Loaded;
            Closed += ContainerWindow_Closed;
        }

        #endregion

        #region Event Methods

        private void ContainerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateUILayout();
        }

        private void ContainerWindow_Closed(object sender, EventArgs e)
        {
            _isClosed = true;
            if (ParentWindow != null && ParentWindow.IsActive)
            {
                ParentWindow.LocationChanged -= ParentWindow_LocationChanged;
            }
        }

        private void HostView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateUILayout();
        }

        private void HostView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            Visibility visibility = element.Visibility;
            UpdateVisibility(visibility);
            if (visibility == Visibility.Visible)
            {
                UpdateUILayout();
            }
        }

        private void HostView_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isMouseCaputred = (bool)e.NewValue;
            if (isMouseCaputred)
            {
                CaptureMouse();
            }
            else
            {
                ReleaseMouseCapture();
            }
        }

        private void HostView_IsHitTestVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            IsHitTestVisible = (bool)e.NewValue;
        }

        private void HostView_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            IsEnabled = (bool)e.NewValue;
        }

        private void HostView_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Focusable = (bool)e.NewValue;
        }

        private void HostView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DataContext = e.NewValue;
        }

        private void ParentWindow_LocationChanged(object sender, EventArgs e)
        {
            UpdateUILayout();
        }

        #endregion

        #region Private Methods

        private void InitWindowProperty(bool allowsTransparency)
        {
            AllowsTransparency = allowsTransparency;
            ShowInTaskbar = false;
            WindowStyle = WindowStyle.None;
            Background = null;
            ResizeMode = ResizeMode.NoResize;
        }

        private void UpdateVisibility(Visibility visibility)
        {
            if (_isClosed)
            {
                return;
            }

            if (visibility == Visibility.Hidden || visibility == Visibility.Collapsed)
            {
                Visibility = Visibility.Hidden;
            }
            else if (visibility == Visibility.Visible)
            {
                Visibility = Visibility.Visible;
            }
        }

        private void UpdateParentWindow()
        {
            Window newParentWindow = GetWindow(_hostView);
            if (newParentWindow == null)
            {
                throw new Exception("The parent window of container window cannot be null");
            }

            if (newParentWindow == ParentWindow)
            {
                return;
            }

            if (ParentWindow != null)
            {
                ParentWindow.LocationChanged -= ParentWindow_LocationChanged;
            }

            newParentWindow.LocationChanged += ParentWindow_LocationChanged;
            ParentWindow = newParentWindow;
            Owner = ParentWindow;
        }

        private void UpdateSize()
        {
            Width = _hostView.ActualWidth - _hostView.Padding.Left - _hostView.Padding.Right;
            Height = _hostView.ActualHeight - _hostView.Padding.Top - _hostView.Padding.Bottom;
        }

        private void UpdateLocation()
        {
            if (ParentWindow == null || _hostView == null)
            {
                throw new Exception("The parent window and host cannot be null.");
            }

            Point hostRelativePointToParentWindow = _hostView.TranslatePoint(new Point(), ParentWindow);

            double parentLeft = ParentWindow.Left;
            double parentTop = ParentWindow.Top;

            //定位窗口的左边
            if (_hostView.HorizontalContentAlignment == HorizontalAlignment.Left || _hostView.HorizontalContentAlignment == HorizontalAlignment.Stretch)
            {
                Left = parentLeft + hostRelativePointToParentWindow.X + _hostView.Padding.Left;
            }
            else if (_hostView.HorizontalContentAlignment == HorizontalAlignment.Right)
            {
                Left = parentLeft + hostRelativePointToParentWindow.X + _hostView.ActualWidth - ActualWidth - _hostView.Padding.Right;
            }
            else if (_hostView.HorizontalContentAlignment == HorizontalAlignment.Center)
            {
                Left = parentLeft + hostRelativePointToParentWindow.X + _hostView.ActualWidth / 2 - ActualWidth / 2;
            }

            //定位窗口的顶部
            if (_hostView.VerticalContentAlignment == VerticalAlignment.Top || _hostView.VerticalContentAlignment == VerticalAlignment.Stretch)
            {
                Top = parentTop + hostRelativePointToParentWindow.Y + _hostView.Padding.Top;
            }
            else if (_hostView.VerticalContentAlignment == VerticalAlignment.Bottom)
            {
                Top = parentTop + hostRelativePointToParentWindow.Y + _hostView.ActualHeight - ActualHeight - _hostView.Padding.Bottom;
            }
            else if (_hostView.VerticalContentAlignment == VerticalAlignment.Center)
            {
                Top = parentTop + hostRelativePointToParentWindow.Y + _hostView.ActualHeight / 2 - ActualHeight / 2;
            }
        }

        #endregion

        #region Public Methods

        public void UpdateUILayout()
        {
            UpdateParentWindow();
            UpdateLocation();
            UpdateSize();
        }

        #endregion

    }
}

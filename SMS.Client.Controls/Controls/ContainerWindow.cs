using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SMS.Client.Controls
{
    public class ContainerWindow : Window
    {
        #region Fields

        private readonly Control _hostView = null;
        private readonly object _lock = new object();

        #endregion

        #region Properties

        public Window ParentWindow { get; private set; }

        #endregion

        #region Constructors

        public ContainerWindow(Control hostView)
        {
            _hostView = hostView ?? throw new Exception("The host view is null!");

            ShowInTaskbar = false;

            Width = 0;
            Height = 0;

            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            AllowsTransparency = true;
            Background = null;

            UpdateUILayout();

            Closed += ContainerWindow_Closed;
        }

        #endregion

        #region Event Methods

        private void ContainerWindow_Closed(object sender, EventArgs e)
        {
            if (ParentWindow != null && ParentWindow.IsActive)
            {
                ParentWindow.LocationChanged -= ParentWindow_LocationChanged; ;
                ParentWindow.SizeChanged -= ParentWindow_SizeChanged; ;
                ParentWindow.StateChanged -= ParentWindow_StateChanged;
            }
        }

        private void ParentWindow_LocationChanged(object sender, EventArgs e)
        {
            UpdateUILayout();
        }

        private void ParentWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateUILayout();
        }

        private void ParentWindow_StateChanged(object sender, EventArgs e)
        {
            UpdateUILayout();
        }

        #endregion

        #region Private Methods

        private void UpdateParentWindow()
        {
            Window newParentWindow = GetWindow(_hostView);
            if (newParentWindow == ParentWindow)
            {
                return;
            }

            if (ParentWindow != null)
            {
                ParentWindow.LocationChanged -= ParentWindow_LocationChanged; ;
                ParentWindow.SizeChanged -= ParentWindow_SizeChanged; ;
                ParentWindow.StateChanged -= ParentWindow_StateChanged;
            }

            if (newParentWindow != null)
            {
                newParentWindow.LocationChanged += ParentWindow_LocationChanged; ;
                newParentWindow.SizeChanged += ParentWindow_SizeChanged; ;
                newParentWindow.StateChanged += ParentWindow_StateChanged;
            }

            ParentWindow = newParentWindow;
            Owner = ParentWindow;
        }

        private void UpdateVisibility()
        {
            if (ParentWindow == null)
            {
                return;
            }

            if (ParentWindow.WindowState == WindowState.Minimized)
            {
                Visibility = Visibility.Hidden;
            }
            else
            {
                if (Visibility == Visibility.Hidden)
                {
                    Visibility = Visibility.Visible;
                }
            }
        }

        private void UpdateSize()
        {
            Width = _hostView.ActualWidth;
            Height = _hostView.ActualHeight;
        }

        private void UpdateLocation()
        {
            if (ParentWindow == null || _hostView == null)
            {
                return;
            }

            Point relativePoint = _hostView.TranslatePoint(new Point(), ParentWindow); ;

            double parentLeft = ParentWindow.Left;
            double parentTop = ParentWindow.Top;
            double parentWidth = ParentWindow.ActualWidth;
            double parentHeight = ParentWindow.ActualHeight;
            if (ParentWindow.WindowState == WindowState.Maximized)
            {
                parentLeft = 0;
                parentTop = 0;
                parentWidth = SystemParameters.FullPrimaryScreenWidth;
                parentHeight = SystemParameters.FullPrimaryScreenHeight;
            }

            //定位窗口的左边
            if (_hostView.HorizontalContentAlignment == HorizontalAlignment.Left || _hostView.HorizontalContentAlignment == HorizontalAlignment.Stretch)
            {
                Left = parentLeft + relativePoint.X + _hostView.Padding.Left;
            }
            else if (_hostView.HorizontalContentAlignment == HorizontalAlignment.Right)
            {
                Left = parentLeft + relativePoint.X + _hostView.ActualWidth - ActualWidth - _hostView.Padding.Right;
            }
            else if (_hostView.HorizontalContentAlignment == HorizontalAlignment.Center)
            {
                Left = parentLeft + relativePoint.X + _hostView.ActualWidth / 2 - ActualWidth / 2;
            }

            //定位窗口的顶部
            if (_hostView.VerticalContentAlignment == VerticalAlignment.Top || _hostView.VerticalContentAlignment == VerticalAlignment.Stretch)
            {
                Top = parentTop + relativePoint.Y + _hostView.Padding.Top;
            }
            else if (_hostView.VerticalContentAlignment == VerticalAlignment.Bottom)
            {
                Top = parentTop + relativePoint.Y + _hostView.ActualHeight - ActualHeight - _hostView.Padding.Bottom;
            }
            else if (_hostView.VerticalContentAlignment == VerticalAlignment.Center)
            {
                Top = parentTop + relativePoint.Y + _hostView.ActualHeight / 2 - ActualHeight / 2;
            }
        }

        #endregion

        #region Public Methods

        public void UpdateUILayout()
        {
            lock (_lock)
            {
                UpdateParentWindow();
                UpdateVisibility();
                UpdateLocation();
                UpdateSize();
            }
        }

        #endregion

    }
}

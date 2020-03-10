using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SMS.Client.Controls
{
    public class TagBase : Control
    {
        #region Fields

        private FrameworkElement _locateAnimationElement;
        private FrameworkElement _warningAnimationElement;

        #endregion

        #region Properties

        public string Id { get; set; }
        public bool IsExpired { get; set; }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty TagNameProperty
            = DependencyProperty.Register("TagName", typeof(string), typeof(TagBase), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty TagNameVisibilityProperty
            = DependencyProperty.Register("TagNameVisibility", typeof(Visibility), typeof(TagBase), new PropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty LocationProperty
            = DependencyProperty.Register("Location", typeof(Point), typeof(TagBase), new PropertyMetadata(new Point(), OnLocationPropertyChanged));

        public static readonly DependencyProperty IsCheckableProperty =
            DependencyProperty.Register("IsCheckable", typeof(bool), typeof(TagBase), new PropertyMetadata(true));

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(TagBase), new PropertyMetadata(false));

        public static readonly DependencyProperty IsTagEnabledProperty =
            DependencyProperty.Register("IsTagEnabled", typeof(bool), typeof(TagBase), new PropertyMetadata(true, OnIsTagEnabledChanged));

        #endregion

        #region Dependency Property Wrappers

        public string TagName
        {
            get { return (string)GetValue(TagNameProperty); }
            set { SetValue(TagNameProperty, value); }
        }

        public Visibility TagNameVisibility
        {
            get { return (Visibility)GetValue(TagNameVisibilityProperty); }
            set { SetValue(TagNameVisibilityProperty, value); }
        }

        public Point Location
        {
            get { return (Point)GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }

        public bool IsCheckable
        {
            get { return (bool)GetValue(IsCheckableProperty); }
            set { SetValue(IsCheckableProperty, value); }
        }

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public bool IsTagEnabled
        {
            get { return (bool)GetValue(IsTagEnabledProperty); }
            set { SetValue(IsTagEnabledProperty, value); }
        }

        #endregion

        #region Routed Events

        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(TagBase));

        #endregion

        #region Routed Event Wrappers

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        #endregion

        #region Private Methods

        private static void OnLocationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TagBase tag = d as TagBase;

            Point newPoint = (Point)e.NewValue;
            tag.UpdateTagLocationInScreen();
        }

        private static void OnIsTagEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #region Protected Methods

        protected virtual void UpdateTagLocationInScreen()
        {
            double offsetX = ActualWidth / 2;
            double offsetY = ActualHeight;

            Canvas.SetLeft(this, Location.X - offsetX);
            Canvas.SetTop(this, Location.Y - offsetY);
        }

        protected void RaiseTagClickEvent(TagBase element, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(ClickEvent, element);
            RaiseEvent(args);
        }

        #endregion

        #region Public Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _locateAnimationElement = GetTemplateChild("PART_LocateAnimation") as FrameworkElement;
            _warningAnimationElement = GetTemplateChild("PART_WarningAnimation") as FrameworkElement;
        }

        public virtual void StartWarningAnimation()
        {
            if (_warningAnimationElement == null)
            {
                return;
            }

            Task.Factory.StartNew(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    _warningAnimationElement.Visibility = Visibility.Visible;
                });

                Thread.Sleep(5000);

                Dispatcher.Invoke(() =>
                {
                    _warningAnimationElement.Visibility = Visibility.Collapsed;
                });
            });
        }

        public virtual void StartLocateAnimation()
        {
            if (_locateAnimationElement == null)
            {
                return;
            }

            Task.Factory.StartNew(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    _locateAnimationElement.Visibility = Visibility.Visible;
                });

                Thread.Sleep(5000);

                Dispatcher.Invoke(() =>
                {
                    _locateAnimationElement.Visibility = Visibility.Collapsed;
                });
            });
        }

        #endregion
    }
}

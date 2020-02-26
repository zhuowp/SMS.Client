using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
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
    ///     <MyNamespace:TopmostPanel/>
    ///
    /// </summary>
    [ContentProperty("Child")]
    public class TopmostPanel : ContentControl
    {
        #region Properties

        public ContainerWindow ContentHodler { get; private set; }
        public Window ParentWindow { get; private set; }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty IsTopmostProperty
            = DependencyProperty.Register("IsTopmost", typeof(bool), typeof(TopmostPanel), new PropertyMetadata(false, new PropertyChangedCallback(OnIsTopmostPropertyChangedCallback)));

        public static readonly DependencyProperty ChildProperty
            = DependencyProperty.Register("Child", typeof(FrameworkElement), typeof(TopmostPanel), new PropertyMetadata(null, new PropertyChangedCallback(OnChildPropertyChangedCallback)));

        #endregion

        #region Dependency Property Wrappers

        public bool IsTopmost
        {
            get { return (bool)GetValue(IsTopmostProperty); }
            set { SetValue(IsTopmostProperty, value); }
        }

        public FrameworkElement Child
        {
            get { return (FrameworkElement)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }

        #endregion

        #region Constructors

        static TopmostPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TopmostPanel), new FrameworkPropertyMetadata(typeof(TopmostPanel)));
        }

        public TopmostPanel()
        {
            Loaded += TopmostPanel_Loaded;
            Unloaded += TopmostPanel_Unloaded;
        }

        #endregion

        #region Dependency Property Changed Callbacks

        private static void OnIsTopmostPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            TopmostPanel topMostPanel = d as TopmostPanel;
            if (topMostPanel.ContentHodler != null)
            {
                topMostPanel.ContentHodler.Topmost = topMostPanel.IsTopmost;
            }
        }

        private static void OnChildPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TopmostPanel topMostPanel = d as TopmostPanel;
            if (topMostPanel.ContentHodler == null)
            {
                topMostPanel.Content = topMostPanel.Child;
            }
            else
            {
                topMostPanel.ContentHodler.Content = topMostPanel.Child;
            }
        }

        #endregion

        #region Event Methods

        private void TopmostPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            UpdateContentHolderVisibility();
        }

        private void TopmostPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (ContentHodler == null)
            {
                ContentHodler = new ContainerWindow(this)
                {
                    Topmost = IsTopmost
                };

                Panel.SetZIndex(ContentHodler, Panel.GetZIndex(this));
                ContentHodler.Visibility = Visibility;
                ContentHodler.Content = this.Child;
                ContentHodler.DataContext = this.DataContext;
            }

            LayoutUpdated -= TopmostPanel_LayoutUpdated;
            LayoutUpdated += TopmostPanel_LayoutUpdated;

            IsVisibleChanged -= TopmostPanel_IsVisibleChanged;
            IsVisibleChanged += TopmostPanel_IsVisibleChanged;

            SizeChanged -= TopmostPanel_SizeChanged;
            SizeChanged += TopmostPanel_SizeChanged;

            UpdateContentHolderVisibility();
        }

        private void TopmostPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ActualHeight != double.NaN && ActualWidth != double.NaN)
            {
                ContentHodler.UpdateUILayout();
                UpdateLayout();
            }
        }

        private void TopmostPanel_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateContentHolderVisibility();
        }

        private void TopmostPanel_LayoutUpdated(object sender, EventArgs e)
        {
            if (ParentWindow != null && ContentHodler != null && this.IsLoaded && this.IsVisible)
            {
                ContentHodler.UpdateUILayout();
            }
        }

        #endregion

        #region Private Methods

        private void UpdateContentHolderVisibility()
        {
            if (ContentHodler == null)
            {
                return;
            }
            if (IsLoaded && IsVisible)
            {
                ContentHodler.Opacity = 100;
            }
            else
            {
                ContentHodler.Opacity = 0;
            }
        }

        #endregion

        #region Public Methods

        public void Dispose()
        {
            if (ContentHodler != null)
            {
                ContentHodler.Close();
            }
        }

        #endregion
    }
}

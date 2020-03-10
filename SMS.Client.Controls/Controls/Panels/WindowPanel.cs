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
    ///     <MyNamespace:WindowPanel/>
    ///
    /// </summary>
    [ContentProperty("Child")]
    public class WindowPanel : ContentControl
    {
        #region Fields

        private ContainerWindow _containerWindow = null;

        #endregion

        #region Properties

        public Window ContainerWindow
        {
            get
            {
                return _containerWindow;
            }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty IsTopmostProperty =
            DependencyProperty.Register("IsTopmost", typeof(bool), typeof(WindowPanel), new PropertyMetadata(false, OnIsTopmostChanged));

        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.Register("Child", typeof(FrameworkElement), typeof(WindowPanel), new PropertyMetadata(null, OnChildChanged));

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

        static WindowPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowPanel), new FrameworkPropertyMetadata(typeof(WindowPanel)));
        }

        public WindowPanel()
        {
            InitContainerWindow();

            DataContextChanged += WindowPanel_DataContextChanged;
        }

        #endregion

        #region Private Methods

        private static void OnIsTopmostChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WindowPanel windowPanel = d as WindowPanel;
            if (windowPanel._containerWindow != null)
            {
                windowPanel._containerWindow.Topmost = (bool)e.NewValue;
            }
        }

        private static void OnChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WindowPanel windowPanel = d as WindowPanel;
            if (windowPanel._containerWindow != null)
            {
                windowPanel._containerWindow.Content = e.NewValue;
            }
        }

        private void WindowPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_containerWindow != null)
            {
                _containerWindow.DataContext = DataContext;
            }
        }

        private void InitContainerWindow()
        {
            _containerWindow = new ContainerWindow(this)
            {
                Topmost = IsTopmost,
                Visibility = Visibility,
                Content = Child,
                DataContext = DataContext,
            };

            Panel.SetZIndex(_containerWindow, Panel.GetZIndex(this));
        }

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        #endregion
    }
}

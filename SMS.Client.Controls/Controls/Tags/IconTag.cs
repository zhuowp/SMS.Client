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
    ///     <MyNamespace:IconTag/>
    ///
    /// </summary>
    public class IconTag : TagBase
    {
        #region Fields

        private ImageButton _iconButton = null;

        #endregion

        #region Properties

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register("IconWidth", typeof(double), typeof(IconTag), new PropertyMetadata(44.0));

        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.Register("IconHeight", typeof(double), typeof(IconTag), new PropertyMetadata(44.0));

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(IconTag), new PropertyMetadata(null));

        public static readonly DependencyProperty MouseOverIconProperty =
            DependencyProperty.Register("MouseOverIcon", typeof(ImageSource), typeof(IconTag), new PropertyMetadata(null));

        public static readonly DependencyProperty MouseDownIconProperty =
            DependencyProperty.Register("MouseDownIcon", typeof(ImageSource), typeof(IconTag), new PropertyMetadata(null));

        #endregion

        #region Dependency Property Wrappers

        public double IconWidth
        {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }

        public double IconHeight
        {
            get { return (double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }

        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public ImageSource MouseOverIcon
        {
            get { return (ImageSource)GetValue(MouseOverIconProperty); }
            set { SetValue(MouseOverIconProperty, value); }
        }

        public ImageSource MouseDownIcon
        {
            get { return (ImageSource)GetValue(MouseDownIconProperty); }
            set { SetValue(MouseDownIconProperty, value); }
        }

        #endregion

        #region Constructors

        static IconTag()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconTag), new FrameworkPropertyMetadata(typeof(IconTag)));
        }

        #endregion

        #region Private Methods

        #endregion

        #region Protected Methods

        private void IconButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseTagClickEvent(this, e);
        }

        #endregion

        #region Public Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _iconButton = GetTemplateChild("PART_Icon") as ImageButton;
            if (_iconButton != null)
            {
                _iconButton.Click += IconButton_Click;
            }

            UpdateTagLocationInScreen();
        }

        #endregion
    }
}

using SMS.Client.Controls;
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

namespace SMS.Client.Business
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SMS.Client.Business.RealtimeMonitor"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SMS.Client.Business.RealtimeMonitor;assembly=SMS.Client.Business.RealtimeMonitor"
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
    ///     <MyNamespace:RealtimePlayWindow/>
    ///
    /// </summary>
    public class RealtimePlayWindow : Window
    {
        #region Fields

        private Grid _header = null;

        #endregion

        #region Properties

        public RealtimePlayer RealPlayer { get; private set; }

        #endregion

        #region Constructors

        static RealtimePlayWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RealtimePlayWindow), new FrameworkPropertyMetadata(typeof(RealtimePlayWindow)));
        }

        public RealtimePlayWindow()
        {
            AllowsTransparency = true;
            WindowStyle = WindowStyle.None;
            Background = null;
        }

        public RealtimePlayWindow(double width, double height) : this()
        {
            Width = width;
            Height = height;
        }

        #endregion

        #region Private Methods

        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            RealPlayer = GetTemplateChild("PART_Player") as RealtimePlayer;
            _header = GetTemplateChild("PART_Header") as Grid;
            if (_header != null)
            {
                _header.MouseDown += Header_MouseDown;
            }
        }

        #endregion
    }
}

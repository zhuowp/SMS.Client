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
    ///     xmlns:MyNamespace="clr-namespace:SMS.Client.BusinessComponent.RealtimeMonitor"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SMS.Client.BusinessComponent.RealtimeMonitor;assembly=SMS.Client.BusinessComponent.RealtimeMonitor"
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
    ///     <MyNamespace:RealtimeMonitorView/>
    ///
    /// </summary>
    public class RealtimeMonitorView : Control, IDisposable
    {
        #region Fields

        private PtzController _ptzController = null;
        private WindowPanel _winPanel = null;
        private Grid _gridTitle = null;
        private Button _btnClose = null;

        public event Action CloseWindow;

        #endregion

        #region Properties

        public RealtimePlayer Player { get; private set; }
        public Window ContainerWindow { get; private set; }
        public TagContainer TagContainer { get; private set; }

        #endregion

        #region Dependency Properties

        #endregion

        #region Dependency Property Wrappers

        #endregion

        #region Constructors

        static RealtimeMonitorView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RealtimeMonitorView), new FrameworkPropertyMetadata(typeof(RealtimeMonitorView)));
        }

        #endregion

        #region Private Methods

        private void PtzController_PtzControl(object sender, RoutedEventArgs e)
        {
            PtzControlRoutedEventArgs args = e as PtzControlRoutedEventArgs;
            Player?.PTZControl(args.PtzControlType, args.StopFlag, args.Speed);
        }

        private void GridTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window window = LVTreeHelper.GetVisualParent<Window>(this);
            window?.DragMove();
        }

        private void _btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow?.Invoke();
        }

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Player = GetTemplateChild("PART_Player") as RealtimePlayer;

            _ptzController = GetTemplateChild("PART_PtzController") as PtzController;
            if (_ptzController != null)
            {
                _ptzController.PtzControl += PtzController_PtzControl;
            }

            _winPanel = GetTemplateChild("PART_TopmostPanel") as WindowPanel;
            if (_winPanel != null)
            {
                ContainerWindow = _winPanel.ContainerWindow;
            }

            TagContainer = GetTemplateChild("PART_TagContainer") as TagContainer;

            _gridTitle = GetTemplateChild("PART_Title") as Grid;
            if (_gridTitle != null)
            {
                _gridTitle.MouseLeftButtonDown += GridTitle_MouseLeftButtonDown;
            }

            _btnClose = GetTemplateChild("PART_Close") as Button;
            if (_btnClose != null)
            {
                _btnClose.Click += _btnClose_Click;
            }
        }

        public void Dispose()
        {
            TagContainer?.Dispose();
        }

        #endregion
    }
}

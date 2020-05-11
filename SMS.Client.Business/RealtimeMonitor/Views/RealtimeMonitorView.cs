using SMS.Client.Common.Caches;
using SMS.Client.Common.Models;
using SMS.Client.Common.Utilities;
using SMS.Client.Controls;
using SMS.Client.Log;
using SMS.StreamMedia.ClientSDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        private SpaceTransformer _spaceTransformer = null;

        #endregion

        #region Events

        public event Action CloseWindow;

        #endregion

        #region Properties

        public RealtimePlayer Player { get; private set; }
        public Window ContainerWindow { get; private set; }
        public TagContainer TagContainer { get; private set; }

        #endregion

        #region Dependency Properties

        public bool IsShowAddTagContextMenu
        {
            get { return (bool)GetValue(IsShowAddTagContextMenuProperty); }
            set { SetValue(IsShowAddTagContextMenuProperty, value); }
        }

        #endregion

        #region Dependency Property Wrappers

        public static readonly DependencyProperty IsShowAddTagContextMenuProperty =
            DependencyProperty.Register("IsShowAddTagContextMenu", typeof(bool), typeof(RealtimeMonitorView), new PropertyMetadata(false));

        #endregion

        #region Constructors

        static RealtimeMonitorView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RealtimeMonitorView), new FrameworkPropertyMetadata(typeof(RealtimeMonitorView)));
        }

        public RealtimeMonitorView()
        {
            Loaded += RealtimeMonitorView_Loaded;
        }

        #endregion

        #region Private Methods

        private void RealtimeMonitorView_Loaded(object sender, RoutedEventArgs e)
        {
            _spaceTransformer = new SpaceTransformer(ActualWidth, ActualHeight);
            SizeChanged += RealtimeMonitorView_SizeChanged;
        }

        private void RealtimeMonitorView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _spaceTransformer = new SpaceTransformer(ActualWidth, ActualHeight);
        }

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

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow?.Invoke();
        }

        private void TagContainer_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            TagContainer tagContainer = sender as TagContainer;
            Point mouseClickPosition = Mouse.GetPosition(tagContainer);

            string devId = "test";
            HolographicInfo holographicInfo = HolographicInfoCache.Instance.GetHolographicInfoByDeviceId(devId);
            if (string.IsNullOrEmpty(holographicInfo.Id))
            {
                LogHelper.Default.DebugFormatted("获取设备id：{0} 的全息信息失败，无法计算标签位置", devId);
                return;
            }

            PTZ ptz = _spaceTransformer.ScreenLocationToAngleLocation(mouseClickPosition, holographicInfo.CameraParameter);
        }

        private void TagContainer_TagLocationChanged(TagBase arg1, ITagModel arg2)
        {
        }

        private void TagContainer_TagClick(TagBase arg1, RoutedEventArgs arg2)
        {
            RealtimePlayWindow realPlayWindow = new RealtimePlayWindow(400, 300);

            realPlayWindow.Loaded += (s, e) =>
            {
                VideoPlayModel playModel = new VideoPlayModel
                {
                    Ip = "192.168.28.136",
                    Port = 8000,
                    Channel = 1,
                    UserName = "admin",
                    Password = "admin12345",
                    StreamType = 0
                };

                realPlayWindow.RealPlayer.PlayHelper = new VideoPlayHelper();
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                    Dispatcher.Invoke(() =>
                    {
                        realPlayWindow.RealPlayer.StartPlay(playModel);
                    });
                });
            };

            realPlayWindow.Show();
        }

        private void TagContainer_TagPreviewMouseMove(TagBase arg1, MouseEventArgs arg2)
        {
        }

        private void TagContainer_TagPreviewMouseLeftButtonUp(TagBase arg1, MouseButtonEventArgs arg2)
        {
        }

        private void TagContainer_TagPreviewMouseLeftButtonDown(TagBase arg1, MouseButtonEventArgs arg2)
        {
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
            if (TagContainer != null)
            {
                TagContainer.MouseRightButtonUp += TagContainer_MouseRightButtonUp;
                TagContainer.TagPreviewMouseLeftButtonDown += TagContainer_TagPreviewMouseLeftButtonDown;
                TagContainer.TagPreviewMouseLeftButtonUp += TagContainer_TagPreviewMouseLeftButtonUp;
                TagContainer.TagPreviewMouseMove += TagContainer_TagPreviewMouseMove;
                TagContainer.TagClick += TagContainer_TagClick;
                TagContainer.TagLocationChanged += TagContainer_TagLocationChanged;
            }

            _gridTitle = GetTemplateChild("PART_Title") as Grid;
            if (_gridTitle != null)
            {
                _gridTitle.MouseLeftButtonDown += GridTitle_MouseLeftButtonDown;
            }

            _btnClose = GetTemplateChild("PART_Close") as Button;
            if (_btnClose != null)
            {
                _btnClose.Click += BtnClose_Click;
            }
        }

        public void Dispose()
        {
            TagContainer?.Dispose();
        }

        #endregion
    }
}

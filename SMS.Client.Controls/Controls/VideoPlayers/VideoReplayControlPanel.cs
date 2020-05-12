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

namespace SMS.Client.Controls
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SMS.Client.Business.Views.VideoReplay"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SMS.Client.Business.Views.VideoReplay;assembly=SMS.Client.Business.Views.VideoReplay"
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
    ///     <MyNamespace:VideoReplayControlPanel/>
    ///
    /// </summary>
    public class VideoReplayControlPanel : Control
    {
        #region Fields

        private Button _btnPlay = null;
        private Button _btnPause = null;
        private Button _btnStop = null;
        private Button _btnFastForward = null;
        private Button _btnSlowForward = null;

        #endregion

        #region Events

        public event Func<PlayStatus, bool> ReplayControl;

        #endregion

        #region Properties

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty CurrentDateTimeProperty =
            DependencyProperty.Register("CurrentDateTime", typeof(DateTime), typeof(VideoReplayControlPanel));

        public static readonly DependencyProperty ReplayControlCommandProperty =
            DependencyProperty.Register("ReplayControlCommand", typeof(ICommand), typeof(VideoReplayControlPanel), new PropertyMetadata(null));

        #endregion

        #region Dependency Property Wrappers

        public DateTime CurrentDateTime
        {
            get { return (DateTime)GetValue(CurrentDateTimeProperty); }
            set { SetValue(CurrentDateTimeProperty, value); }
        }

        public ICommand ReplayControlCommand
        {
            get { return (ICommand)GetValue(ReplayControlCommandProperty); }
            set { SetValue(ReplayControlCommandProperty, value); }
        }

        #endregion

        #region Routed Events

        #endregion

        #region Routed Event Wrappers

        #endregion

        #region Constructors

        static VideoReplayControlPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VideoReplayControlPanel), new FrameworkPropertyMetadata(typeof(VideoReplayControlPanel)));
        }

        #endregion

        #region Private Methods

        #endregion

        #region Protected Methods

        private void TimelineBand_TimelineBandDoubleClick(object sender, TimelineBandDoubleClickEventArgs e)
        {
            //timelineBandMain.CurrentDateTime = e.ClickDateTime;
        }

        private void SlowForward_Click(object sender, RoutedEventArgs e)
        {
            RaiseReplayControlEvent(PlayStatus.SlowForward);
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            RaiseReplayControlEvent(PlayStatus.Stop);

            _btnPlay.Visibility = Visibility.Visible;
            _btnPause.Visibility = Visibility.Collapsed;
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            if (RaiseReplayControlEvent(PlayStatus.Pause) == true)
            {
                _btnPause.Visibility = Visibility.Collapsed;
                _btnPlay.Visibility = Visibility.Visible;
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (RaiseReplayControlEvent(PlayStatus.Play) == true)
            {
                _btnPlay.Visibility = Visibility.Collapsed;
                _btnPause.Visibility = Visibility.Visible;
            }
        }

        private void Fastforward_Click(object sender, RoutedEventArgs e)
        {
            RaiseReplayControlEvent(PlayStatus.FastForward);
        }

        private bool? RaiseReplayControlEvent(PlayStatus status)
        {
            return ReplayControl?.Invoke(status);
        }

        #endregion

        #region Public Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _btnPlay = GetTemplateChild("PART_PlayButton") as Button;
            if (_btnPlay != null)
            {
                _btnPlay.Click += Play_Click;
            }

            _btnPause = GetTemplateChild("PART_PauseButton") as Button;
            if (_btnPause != null)
            {
                _btnPause.Click += Pause_Click;
            }

            _btnStop = GetTemplateChild("PART_StopButton") as Button;
            if (_btnStop != null)
            {
                _btnStop.Click += Stop_Click;
            }

            _btnFastForward = GetTemplateChild("PART_FastForwardButton") as Button;
            if (_btnFastForward != null)
            {
                _btnFastForward.Click += Fastforward_Click;
            }

            _btnSlowForward = GetTemplateChild("PART_SlowForwardButton") as Button;
            if (_btnSlowForward != null)
            {
                _btnSlowForward.Click += SlowForward_Click;
            }
        }

        #endregion
    }
}

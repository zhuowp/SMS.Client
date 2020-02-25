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
    ///     xmlns:MyNamespace="clr-namespace:SMS.Client.Controls.Controls.VideoPlayers"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SMS.Client.Controls.Controls.VideoPlayers;assembly=SMS.Client.Controls.Controls.VideoPlayers"
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
    ///     <MyNamespace:Replayer/>
    ///
    /// </summary>
    public class Replayer : PlayerBase
    {
        #region Fields

        //快放最大倍率
        private double _maxFastSpeed = 8;
        //慢放最小倍率
        private double _minSlowSpeed = 1.0 / 8.0;

        //录像回放帮助类
        private IReplayHelper _recordPlayHelper = null;

        #endregion

        #region Properties

        public IReplayHelper ReplayHelper
        {
            get { return _recordPlayHelper; }
            set
            {
                if (PlayHandle != -1 && _recordPlayHelper != null)
                {
                    _recordPlayHelper.StopReplay(PlayHandle);
                    _recordPlayHelper.PlayPositionChangedEvent -= _recordPlayHelper_PlayPositionChangedEvent;
                }

                _recordPlayHelper = value;
                _recordPlayHelper.PlayPositionChangedEvent += _recordPlayHelper_PlayPositionChangedEvent;
            }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyPropertyKey ReplayStatusKey =
            DependencyProperty.RegisterReadOnly("ReplayStatus", typeof(PlayStatus), typeof(Replayer), new PropertyMetadata(PlayStatus.Unknown));
        public static readonly DependencyPropertyKey ReplaySpeedKey =
            DependencyProperty.RegisterReadOnly("ReplaySpeed", typeof(double), typeof(Replayer), new PropertyMetadata(1.0));
        public static readonly DependencyPropertyKey ReplayTimeKey =
            DependencyProperty.RegisterReadOnly("ReplayTime", typeof(DateTime), typeof(Replayer), new PropertyMetadata(DateTime.MinValue));

        #endregion

        #region Dependency Property Wrappers

        public PlayStatus ReplayStatus
        {
            get { return (PlayStatus)GetValue(ReplayStatusKey.DependencyProperty); }
        }

        public double ReplaySpeed
        {
            get { return (double)GetValue(ReplaySpeedKey.DependencyProperty); }
        }

        public DateTime ReplayTime
        {
            get { return (DateTime)GetValue(ReplayTimeKey.DependencyProperty); }
        }

        #endregion

        #region Constructors

        static Replayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Replayer), new FrameworkPropertyMetadata(typeof(Replayer)));
        }

        #endregion

        #region Private Methods

        private void _recordPlayHelper_PlayPositionChangedEvent(IntPtr arg1, DateTime arg2)
        {
            if (arg1 == ScreenHandle)
            {
                SetValue(ReplayTimeKey, arg2);
            }
        }

        private bool StartReplay(IReplayModel replayModel)
        {
            //回放参数必须不能为空
            if (replayModel == null || replayModel.BeginTime == default(DateTime))
            {
                return false;
            }

            InitializePlayScreen();

            replayModel.ScreenHandle = ScreenHandle;
            int replayHandle = _recordPlayHelper.StartReplay(replayModel);
            if (replayHandle >= 0)
            {
                SetValue(ReplayStatusKey, PlayStatus.Play);
                PlayHandle = replayHandle;
                return true;
            }

            return false;
        }

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        public bool StopReplay()
        {
            if (_recordPlayHelper == null || PlayHandle == -1)
            {
                return false;
            }

            bool isStopSuccess = _recordPlayHelper.StopReplay(PlayHandle);
            DisposePlayScreen();
            SetValue(ReplaySpeedKey, 1.0);
            SetValue(ReplayStatusKey, PlayStatus.Stop);

            return isStopSuccess;
        }

        public bool PauseReplay()
        {
            if (_recordPlayHelper == null || PlayHandle == -1)
            {
                return false;
            }

            bool isPauseReplaySuccess = _recordPlayHelper.PauseReplay(PlayHandle);

            SetValue(ReplayStatusKey, PlayStatus.Pause);
            return isPauseReplaySuccess;
        }

        public bool ContinueReplay()
        {
            if (_recordPlayHelper == null || PlayHandle == -1)
            {
                return false;
            }

            bool isContinueReplaySuccess = _recordPlayHelper.ContinueReplay(PlayHandle);

            SetValue(ReplayStatusKey, PlayStatus.Play);
            return isContinueReplaySuccess;
        }

        public bool FastReplay()
        {
            if (_recordPlayHelper == null || PlayHandle == -1)
            {
                return false;
            }

            double newReplaySpeed = ReplaySpeed * 2;
            if (newReplaySpeed > _maxFastSpeed)
            {
                return false;
            }

            if (_recordPlayHelper.SetReplaySpeed(PlayHandle, newReplaySpeed))
            {
                SetValue(ReplaySpeedKey, newReplaySpeed);
                return true;
            }

            return false;
        }

        public bool SlowReplay()
        {
            if (_recordPlayHelper == null || PlayHandle == -1)
            {
                return false;
            }

            double newReplaySpeed = ReplaySpeed / 2;
            if (newReplaySpeed < _minSlowSpeed)
            {
                return false;
            }

            if (_recordPlayHelper.SetReplaySpeed(PlayHandle, newReplaySpeed))
            {
                SetValue(ReplaySpeedKey, newReplaySpeed);
                return true;
            }

            return false;
        }

        public bool SetReplayPosition(DateTime replayTime)
        {
            if (_recordPlayHelper == null || PlayHandle == -1)
            {
                return false;
            }

            bool isSetReplayPositionSuccess = _recordPlayHelper.SetReplayPosition(PlayHandle, replayTime);
            return isSetReplayPositionSuccess;
        }

        public bool ReplayControl(IReplayModel replayModel)
        {
            if (replayModel == null)
            {
                return false;
            }

            switch (replayModel.ReplayStatus)
            {
                case PlayStatus.Play:
                    if (ReplayStatus == PlayStatus.Unknown || ReplayStatus == PlayStatus.Stop)
                    {
                        return StartReplay(replayModel);
                    }
                    else
                    {
                        return ContinueReplay();
                    }
                case PlayStatus.Stop:
                    return StopReplay();
                case PlayStatus.Pause:
                    return PauseReplay();
                case PlayStatus.FastForward:
                    return FastReplay();
                case PlayStatus.SlowForward:
                    return SlowReplay();
                //case PlayStatus.SetPos:
                //    return SetReplayPosition(replayModel.BeginTime);
                default:
                    return false;
            }
        }

        public void Dispose()
        {
            StopReplay();
        }

        #endregion

    }
}

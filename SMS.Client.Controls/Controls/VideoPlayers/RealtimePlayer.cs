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
    ///     <MyNamespace:RealtimePlayer/>
    ///
    /// </summary>
    public class RealtimePlayer : PlayerBase
    {
        #region Fields

        //播放辅助类
        private IRealtimePlayHelper _playHelper = null;

        #endregion

        #region Properties

        public IRealtimePlayHelper PlayHelper
        {
            get
            {
                return _playHelper;
            }
            set
            {
                if (PlayHandle != -1 && _playHelper != null
                    && (PlayStatus != PlayStatus.Stop && PlayStatus != PlayStatus.Unknown))
                {
                    throw new Exception("Video cannot be playing while changing the PlayHelper.");
                }

                _playHelper = value;
            }
        }

        #endregion

        #region Dependency Properties

        #endregion

        #region Dependency Property Wrappers

        #endregion

        #region Routed Events

        #endregion

        #region Constructors

        static RealtimePlayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RealtimePlayer), new FrameworkPropertyMetadata(typeof(RealtimePlayer)));
        }

        #endregion

        #region Private Methods

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        /// <summary>
        /// 开始播放实时预览
        /// </summary>
        /// <param name="playModel"></param>
        /// <returns></returns>
        public int StartPlay(IPlayModel playModel)
        {
            if (PlayHelper == null)
            {
                return -1;
            }

            //在开始一个新预览之前关闭前一次预览
            if (PlayHandle >= 0)
            {
                StopPlay();
            }

            //初始化播放屏
            InitializePlayScreen();

            playModel.ScreenHandle = ScreenHandle;
            PlayHandle = PlayHelper.StartPlay(playModel);

            return PlayHandle;
        }

        /// <summary>
        /// 停止播放实时预览
        /// </summary>
        /// <returns></returns>
        public bool StopPlay()
        {
            if (PlayHelper == null || PlayHandle < 0)
            {
                return false;
            }

            //停止视频播放
            bool stopResult = PlayHelper.StopPlay(PlayHandle);

            //销毁播放屏控件
            DisposePlayScreen();
            PlayHandle = -1;

            return stopResult;
        }

        public bool PTZControl(PTZControlType controlType, int stopFlag, int ptzControlSpeed)
        {
            if (PlayHandle < 0)
            {
                return false;
            }

            return _playHelper.PTZControl(PlayHandle, controlType, stopFlag, ptzControlSpeed);
        }

        public bool CapturePicture(string fileName)
        {
            if (PlayHandle < 0)
            {
                return false;
            }

            return _playHelper.CapturePicture(PlayHandle, fileName);
        }

        public void Dispose()
        {
            StopPlay();
        }

        #endregion
    }
}

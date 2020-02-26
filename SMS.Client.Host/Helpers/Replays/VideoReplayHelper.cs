using SMS.Client.Controls;
using SMS.Client.Host.Interfaces.Replays;
using SMS.Client.Host.Models;
using SMS.Client.Host.Models.Replays;
using SMS.Client.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SMS.Client.Host.Helpers.Replays
{
    public class VideoReplayHelper : IReplayHelper
    {
        #region Fields

        private readonly double _maxReplaySpeed = 8;
        private readonly double _minReplaySpeed = 0.125;

        private IVideoReplayAdapter _replayAdapter = null;
        private IVideoRecordCache _videoRecordCache = null;

        private bool _isQueryReplayPosition = true;

        //正在播放的录像状态字典，key为播放控件句柄
        private ConcurrentDictionary<IntPtr, RelayStatusModel> _replayingStatusModelDict = new ConcurrentDictionary<IntPtr, RelayStatusModel>();

        //录像播放进度查询线程
        private Thread _videoReplayPositionQueryThread = null;

        #endregion

        #region Properties

        public IVideoReplayAdapter ReplayAdapter
        {
            get
            {
                return _replayAdapter;
            }

            set
            {
                _replayAdapter = value;
            }
        }

        public IVideoRecordCache VideoRecordCache
        {
            get
            {
                return _videoRecordCache;
            }

            set
            {
                _videoRecordCache = value;
            }
        }

        #endregion

        #region Events

        public event Action<IntPtr, DateTime> ReplayPositionChangedEvent;

        #endregion

        #region Constructors

        public VideoReplayHelper(IVideoReplayAdapter replayAdapter = null, IVideoRecordCache videoRecordCache = null)
        {
            ReplayAdapter = replayAdapter ?? new VideoReplayAdapter();
            VideoRecordCache = videoRecordCache ?? Caches.VideoRecordCache.Instance;

            InitAndStartReplayPositionQueryThread();
        }

        #endregion

        #region Private Methods

        private void InitAndStartReplayPositionQueryThread()
        {
            _videoReplayPositionQueryThread = new Thread(ContinuousQueryReplayPosition);
            _videoReplayPositionQueryThread.IsBackground = true;
            _videoReplayPositionQueryThread.Start();
        }

        private void ContinuousQueryReplayPosition()
        {
            while (_isQueryReplayPosition)
            {
                foreach (KeyValuePair<IntPtr, RelayStatusModel> playStatusKVPair in _replayingStatusModelDict)
                {
                    try
                    {
                        RelayStatusModel replayStatus = playStatusKVPair.Value;
                        QueryReplayStatus(replayStatus);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.DebugFormatted("录像进度查询失败，播放句柄：{0}， 错误原因：{1}", playStatusKVPair.Value.PlayingHandle, ex.ToString());
                    }
                    finally
                    {
                        Thread.Sleep(500);
                    }
                }
            }
        }

        /// <summary>
        /// 查询单个录像播放的进度
        /// </summary>
        /// <param name="replayStatus"></param>
        private void QueryReplayStatus(RelayStatusModel replayStatus)
        {
            if (replayStatus == null)
            {
                return;
            }

            //非播放状态下的录像回放无需进行进度查询
            if (!IsNeedQueryReplayPosition(replayStatus))
            {
                return;
            }

            //查询失败或得到错误的结果时不做任何处理
            DateTime replayDateTime = ReplayAdapter.GetReplayPositon(replayStatus.PlayingHandle);
            if (replayDateTime == DateTime.MinValue)
            {
                return;
            }

            //如果播放时间进度已经超过录像文件的最大时间，则通知消息订阅者停止播放
            if (VideoRecordCache.IsTimeBeyondTimeRangeOfVideoFiles(replayStatus.Camera.ChannelID, replayDateTime))
            {
                ReplayPositionChangedEvent?.Invoke(replayStatus.PlayingScreenHandle, DateTime.MaxValue);
                return;
            }

            ReplayPositionChangedEvent?.Invoke(replayStatus.PlayingScreenHandle, replayDateTime);
        }

        /// <summary>
        /// 判断是否需要进行录像播放进度查询
        /// </summary>
        /// <param name="replayStatus"></param>
        /// <returns></returns>
        private bool IsNeedQueryReplayPosition(RelayStatusModel replayStatus)
        {
            //只有处于播放状态（普通播放，快放，慢放，快退，慢退等）中的回放才需要查询进度
            return replayStatus.PlayingStatus == PlayStatus.Play
                || replayStatus.PlayingStatus == PlayStatus.FastForward
                || replayStatus.PlayingStatus == PlayStatus.SlowForward
                || replayStatus.PlayingStatus == PlayStatus.FastBackward;
        }

        /// <summary>
        /// 录像回放参数校验
        /// </summary>
        /// <param name="replayModel"></param>
        /// <returns></returns>
        private bool ValidateReplayParam(ReplayModel replayModel)
        {
            if (replayModel.ReplayCamera == null)
            {
                LogHelper.DebugFormatted("录像播放参数错误，设备信息为空");
                return false;
            }

            if (replayModel.ScreenHandle == IntPtr.Zero)
            {
                LogHelper.DebugFormatted("录像播放参数错误，没有播放控件句柄，通道Id：{0}，IP地址：{1}",
                    replayModel.ReplayCamera.ChannelID, replayModel.ReplayCamera.Ip);
                return false;
            }

            return true;
        }

        private bool TryGetReplayStatusByScreenHandle(IntPtr screenHandle, out RelayStatusModel replayHandle)
        {
            replayHandle = null;

            if (_replayingStatusModelDict.ContainsKey(screenHandle))
            {
                replayHandle = _replayingStatusModelDict[screenHandle];
                return true;
            }

            return false;
        }

        private bool ValidateSettingReplaySpeed(double replaySpeed)
        {
            if (replaySpeed > _maxReplaySpeed)
            {
                LogHelper.DebugFormatted("录像回放快放失败，设置的速度大于系统所允许的最大速度");
                return false;
            }

            if (replaySpeed < _minReplaySpeed)
            {
                LogHelper.DebugFormatted("录像回放慢放失败，设置的速度小于系统所允许的最小速度");
                return false;
            }

            return true;
        }

        private bool SetReplaySpeed(RelayStatusModel replayStatus, double replaySpeed)
        {
            if (replayStatus.PlayingStatus != PlayStatus.Play)
            {
                LogHelper.Debug("设置回放速度失败，当前回放不处于播放状态中，无法设置播放速度");
                return false;
            }

            if (!ValidateSettingReplaySpeed(replaySpeed))
            {
                return false;
            }

            bool result = ReplayAdapter.SetReplaySpeed(replayStatus.PlayingHandle, replaySpeed);
            if (result)
            {
                replayStatus.PlayingSpeed = replaySpeed;
            }

            return result;
        }

        private bool DirectlySetReplayPosition(RelayStatusModel replayStatus, DateTime replayTime)
        {
            if (replayStatus.PlayingStatus == PlayStatus.Pause)
            {
                ResumeReplay(replayStatus.PlayingScreenHandle);
                Thread.Sleep(200);
            }

            bool result = ReplayAdapter.SetReplayPositon(replayStatus.PlayingHandle, replayTime);
            if (result)
            {
                replayStatus.PlayingStatus = PlayStatus.Play;
            }

            return true;
        }

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        public RelayStatusModel GetReplayStatus(IntPtr screenHandle)
        {
            RelayStatusModel replayStatus;
            _replayingStatusModelDict.TryGetValue(screenHandle, out replayStatus);
            return replayStatus;
        }

        public long StartReplay(IReplayModel iReplayModel)
        {
            ReplayModel replayModel = iReplayModel as ReplayModel;
            if (!ValidateReplayParam(replayModel))
            {
                return -1;
            }

            if (!VideoRecordCache.HasTimeSpecificRecordFile(replayModel.ReplayCamera.ChannelID, replayModel.ReplayTimeRange.BeginTime))
            {
                return -1;
            }

            long replayHandle = ReplayAdapter.StartReplay(replayModel);
            if (replayHandle < 0)
            {
                return -1;
            }

            RelayStatusModel playStatus = new RelayStatusModel
            {
                Camera = replayModel.ReplayCamera,
                PlayingScreenHandle = replayModel.ScreenHandle,
                StartPlayDateTime = replayModel.ReplayTimeRange.BeginTime,
                PlayingHandle = replayHandle,
                PlayingStatus = PlayStatus.Play,
                PlayingSpeed = 1,
            };
            _replayingStatusModelDict.TryAdd(replayModel.ScreenHandle, playStatus);

            return replayHandle;
        }

        public bool PauseReplay(IntPtr screenHandle)
        {
            RelayStatusModel replayStatus;
            if (!TryGetReplayStatusByScreenHandle(screenHandle, out replayStatus))
            {
                return false;
            }

            bool result = ReplayAdapter.PauseReplay(replayStatus.PlayingHandle);
            if (result)
            {
                _replayingStatusModelDict[screenHandle].PlayingStatus = PlayStatus.Pause;
            }

            return result;
        }

        public bool ResumeReplay(IntPtr screenHandle)
        {
            RelayStatusModel replayStatus;
            if (!TryGetReplayStatusByScreenHandle(screenHandle, out replayStatus))
            {
                return false;
            }

            bool result = ReplayAdapter.ResumeReplay(replayStatus.PlayingHandle);
            if (result)
            {
                _replayingStatusModelDict[screenHandle].PlayingStatus = PlayStatus.Play;
            }

            return result;
        }

        public bool StopReplay(IntPtr screenHandle)
        {
            RelayStatusModel replayStatus;
            if (!TryGetReplayStatusByScreenHandle(screenHandle, out replayStatus))
            {
                return false;
            }

            bool result = ReplayAdapter.StopReplay(replayStatus.PlayingHandle);
            if (result)
            {
                _replayingStatusModelDict.TryRemove(screenHandle, out replayStatus);
            }

            return result;
        }

        public bool RestartReplay(IntPtr screenHandle, DateTime replayTime)
        {
            RelayStatusModel replayStatus;
            if (!TryGetReplayStatusByScreenHandle(screenHandle, out replayStatus))
            {
                return false;
            }

            //停止当前播放
            bool stopResult = StopReplay(screenHandle);
            if (!stopResult)
            {
                return false;
            }

            //开始新的播放
            ReplayModel replayModel = new ReplayModel()
            {
                ReplayTimeRange = new TimeRange(replayTime, replayTime.Date.AddDays(1).AddSeconds(-1)),
                ReplayCamera = replayStatus.Camera,
                ScreenHandle = screenHandle,
            };

            long handle = StartReplay(replayModel);
            if (handle >= 0)
            {
                RelayStatusModel newReplayStatus;
                if (TryGetReplayStatusByScreenHandle(screenHandle, out newReplayStatus))
                {
                    //重新设置播放速度
                    SetReplaySpeed(newReplayStatus, replayStatus.PlayingSpeed);
                }
            }

            return handle >= 0;
        }

        public bool FastForwardReplay(IntPtr screenHandle)
        {
            RelayStatusModel replayStatus;
            if (!TryGetReplayStatusByScreenHandle(screenHandle, out replayStatus))
            {
                return false;
            }

            return SetReplaySpeed(replayStatus, replayStatus.PlayingSpeed * 2);
        }

        public bool SlowForwardReplay(IntPtr screenHandle)
        {
            RelayStatusModel replayStatus;
            if (!TryGetReplayStatusByScreenHandle(screenHandle, out replayStatus))
            {
                return false;
            }

            return SetReplaySpeed(replayStatus, replayStatus.PlayingSpeed / 2);
        }

        public bool SetReplaySpeed(IntPtr screenHandle, double replaySpeed)
        {
            RelayStatusModel replayStatus;
            if (!TryGetReplayStatusByScreenHandle(screenHandle, out replayStatus))
            {
                return false;
            }

            return SetReplaySpeed(replayStatus, replaySpeed);
        }

        public bool SetReplayPosition(IntPtr screenHandle, DateTime replayTime)
        {
            RelayStatusModel replayStatus;
            if (!TryGetReplayStatusByScreenHandle(screenHandle, out replayStatus))
            {
                return false;
            }

            if (!VideoRecordCache.HasTimeSpecificRecordFile(replayStatus.Camera.ChannelID, replayTime))
            {
                ReplayPositionChangedEvent?.Invoke(screenHandle, DateTime.MaxValue);
                return false;
            }

            if (replayStatus.StartPlayDateTime < replayTime)//跳放的时间点在录像播放起始时间点后，则直接跳放
            {
                return DirectlySetReplayPosition(replayStatus, replayTime);
            }
            else//跳放的时间点在录像播放起始点前，则需要重新播放
            {
                return RestartReplay(screenHandle, replayTime);
            }
        }

        public void Dispose()
        {
            _isQueryReplayPosition = false;
        }

        #endregion
    }
}

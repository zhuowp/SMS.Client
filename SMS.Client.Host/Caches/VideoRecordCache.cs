using SMS.Client.Host.Helpers.Replays;
using SMS.Client.Host.Interfaces.Replays;
using SMS.Client.Host.Models.Devices;
using SMS.Client.Host.Models.Replays;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SMS.Client.Host.Caches
{
    public class VideoRecordCache : IVideoRecordCache
    {
        #region Fields

        private static VideoRecordCache _instance = null;
        private static object _lock = new object();

        //录像文件缓存字典，key为通道id + 日期字符串
        private readonly ConcurrentDictionary<string, List<VideoRecordModel>> _videoRecordFileDict = new ConcurrentDictionary<string, List<VideoRecordModel>>();
        private IVideoRecordQueryHelper _videoRecordQueryHelper = null;

        #endregion

        #region Properties

        public static VideoRecordCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new VideoRecordCache();
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion

        #region Constructors

        private VideoRecordCache()
        {
            _videoRecordQueryHelper = VideoRecordQueryHelper.Instance;
        }

        #endregion

        #region Private Methods

        public static string GetRecordFileListKey(string channelId, DateTime date)
        {
            string recordFileListKey = string.Format("{0}-{1}", channelId, date.ToString("yyyy-MM-dd"));
            return recordFileListKey;
        }

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        public void SetVideoRecordQueryAdapter(IVideoRecordQueryHelper videoRecordQueryHelper)
        {
            _videoRecordQueryHelper = videoRecordQueryHelper;
        }

        /// <summary>
        /// 更新设备指定日期的录像文件
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<VideoRecordModel> UpdateRecordFilesByCameraAndDate(CameraModel camera, DateTime date)
        {
            string recordFileListKey = GetRecordFileListKey(camera.ChannelID, date);

            //查询录像文件
            List<VideoRecordModel> recordFileList = _videoRecordQueryHelper.QueryVideoRecordFilesInDay(camera, date);
            if (recordFileList != null)
            {
                _videoRecordFileDict.AddOrUpdate(recordFileListKey, recordFileList, (key, oldValue) => recordFileList);
                return recordFileList;
            }

            return new List<VideoRecordModel>();
        }

        /// <summary>
        /// 通过日期查找设备的录像文件列表
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<VideoRecordModel> GetRecordFileListByCameraAndDate(CameraModel camera, DateTime date)
        {
            string recordFileListKey = GetRecordFileListKey(camera.ChannelID, date);
            if (HasTimeSpecificRecordFile(camera.ChannelID, date))
            {
                return _videoRecordFileDict[recordFileListKey];
            }

            return new List<VideoRecordModel>();
        }

        /// <summary>
        /// 判断设备在指定时间点是否有录像文件
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public bool HasTimeSpecificRecordFile(string channelId, DateTime dateTime)
        {
            string recordFileListKey = GetRecordFileListKey(channelId, dateTime);

            //没有设备对应日期的录像文件列表
            if (!_videoRecordFileDict.ContainsKey(recordFileListKey) || _videoRecordFileDict[recordFileListKey].Count == 0)
            {
                return false;
            }

            //没有指定时间点的录像文件
            List<VideoRecordModel> recordFileList = _videoRecordFileDict[recordFileListKey];
            if (recordFileList.Find(p => (p.VideoRecordTimeRange.BeginTime <= dateTime && p.VideoRecordTimeRange.EndTime >= dateTime)) == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 判断当前播放时间是否已经超出录像文件的最大时间点
        /// </summary>
        /// <param name="replayStatus"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public bool IsTimeBeyondTimeRangeOfVideoFiles(string channelId, DateTime dateTime)
        {
            string recordFileListKey = GetRecordFileListKey(channelId, dateTime);
            if (!_videoRecordFileDict.ContainsKey(recordFileListKey))
            {
                return true;
            }

            List<VideoRecordModel> recordFileList = _videoRecordFileDict[recordFileListKey];
            if (recordFileList.Find(p => (p.VideoRecordTimeRange.EndTime > dateTime)) == null)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}

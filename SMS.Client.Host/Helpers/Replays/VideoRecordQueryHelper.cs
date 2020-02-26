﻿using SMS.Client.Host.Interfaces;
using SMS.Client.Host.Interfaces.Replays;
using SMS.Client.Host.Models;
using SMS.Client.Host.Models.Devices;
using SMS.Client.Host.Models.Replays;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Host.Helpers.Replays
{
    class VideoRecordQueryHelper : IVideoRecordQueryHelper
    {
        #region Fields

        private static VideoRecordQueryHelper _instance = null;
        private static object _lock = new object();

        #endregion

        #region Properties

        public static VideoRecordQueryHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new VideoRecordQueryHelper();
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion

        #region Constructors

        private VideoRecordQueryHelper()
        { }

        #endregion

        #region Private Methods

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        public List<DateTime> QueryDaysHavingVideoRecordByTimeRange(CameraModel camera, TimeRange timeRange)
        {
            throw new NotImplementedException();
        }

        public List<DateTime> QueryDaysHavingVideoRecordInMonth(CameraModel camera, DateTime monthDateTime)
        {
            throw new NotImplementedException();
        }

        public List<VideoRecordModel> QueryVideoRecordFilesByTimeRange(CameraModel camera, TimeRange timeRange)
        {
            throw new NotImplementedException();
        }

        public List<VideoRecordModel> QueryVideoRecordFilesInDay(CameraModel camera, DateTime dayDataTime)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

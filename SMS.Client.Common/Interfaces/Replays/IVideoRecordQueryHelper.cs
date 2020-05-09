using SMS.Client.Common.Models;
using SMS.Client.Common.Models.Devices;
using SMS.Client.Common.Models.Replays;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Common.Interfaces.Replays
{
    public interface IVideoRecordQueryHelper
    {
        List<DateTime> QueryDaysHavingVideoRecordByTimeRange(CameraModel camera, TimeRange timeRange);
        List<VideoRecordModel> QueryVideoRecordFilesByTimeRange(CameraModel camera, TimeRange timeRange);

        List<DateTime> QueryDaysHavingVideoRecordInMonth(CameraModel camera, DateTime monthDateTime);
        List<VideoRecordModel> QueryVideoRecordFilesInDay(CameraModel camera, DateTime dayDataTime);
    }
}

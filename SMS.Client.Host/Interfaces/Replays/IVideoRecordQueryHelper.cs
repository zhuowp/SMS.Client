using SMS.Client.Host.Models;
using SMS.Client.Host.Models.Devices;
using SMS.Client.Host.Models.Replays;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Host.Interfaces.Replays
{
    public interface IVideoRecordQueryHelper
    {
        List<DateTime> QueryDaysHavingVideoRecordByTimeRange(CameraModel camera, TimeRange timeRange);
        List<VideoRecordModel> QueryVideoRecordFilesByTimeRange(CameraModel camera, TimeRange timeRange);

        List<DateTime> QueryDaysHavingVideoRecordInMonth(CameraModel camera, DateTime monthDateTime);
        List<VideoRecordModel> QueryVideoRecordFilesInDay(CameraModel camera, DateTime dayDataTime);
    }
}

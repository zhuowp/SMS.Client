using SMS.Client.Host.Models.Devices;
using SMS.Client.Host.Models.Replays;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Host.Interfaces.Replays
{
    public interface IVideoRecordCache
    {
        List<VideoRecordModel> UpdateRecordFilesByCameraAndDate(CameraModel camera, DateTime date);
        List<VideoRecordModel> GetRecordFileListByCameraAndDate(CameraModel camera, DateTime date);
        bool HasTimeSpecificRecordFile(string channelId, DateTime dateTime);
        bool IsTimeBeyondTimeRangeOfVideoFiles(string channelId, DateTime dateTime);
    }
}

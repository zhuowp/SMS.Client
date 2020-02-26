using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Host.Models.Replays
{
    public class VideoRecordModel
    {
        public TimeRange VideoRecordTimeRange { get; set; }
        public int FileSize { get; set; }
        public string FileName { get; set; }
        public string Id { get; set; }
        public string ChannelId { get; set; }
        public object Data { get; set; }
    }
}

using SMS.Client.Common.Models.Devices;
using SMS.Client.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Common.Models.Replays
{
    public class ReplayModel : IReplayModel
    {
        public CameraModel ReplayCamera { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeRange ReplayTimeRange { get; set; }
        public IntPtr ScreenHandle { get; set; }
        public float PlaySpeed { get; set; } = 1;
        public PlayStatus ReplayStatus { get; set; }
        public bool IsQueryRecord { get; set; }
        public object ReplayData { get; set; }
    }
}

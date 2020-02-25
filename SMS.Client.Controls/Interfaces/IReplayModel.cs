using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Controls
{
    public interface IReplayModel
    {
        IntPtr ScreenHandle { get; set; }
        PlayStatus ReplayStatus { get; set; }
        DateTime BeginTime { get; set; }
        DateTime EndTime { get; set; }
        object ReplayData { get; set; }
    }
}

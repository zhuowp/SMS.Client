using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Controls
{
    public interface IReplayHelper
    {
        event Action<IntPtr, DateTime> ReplayPositionChangedEvent;

        long StartReplay(IReplayModel replayModel);
        bool StopReplay(IntPtr screenHandle);
        bool PauseReplay(IntPtr screenHandle);
        bool ResumeReplay(IntPtr screenHandle);
        bool SetReplaySpeed(IntPtr screenHandle, double replaySpeed);
        bool SetReplayPosition(IntPtr screenHandle, DateTime replayTime);
    }
}

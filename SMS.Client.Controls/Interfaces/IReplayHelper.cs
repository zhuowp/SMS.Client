using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Controls
{
    public interface IReplayHelper
    {
        event Action<IntPtr, DateTime> PlayPositionChangedEvent;

        int StartReplay(IReplayModel replayModel);
        bool StopReplay(int replayHandle);
        bool PauseReplay(int replayHandle);
        bool ContinueReplay(int replayHandle);
        bool SetReplaySpeed(int replayHandle, double replaySpeed);
        bool SetReplayPosition(int replayHandle, DateTime replayTime);
    }
}

using SMS.Client.Host.Models.Replays;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Host.Interfaces.Replays
{
    public interface IVideoReplayAdapter
    {
        long StartReplay(ReplayModel replayModel);
        bool PauseReplay(long replayHandle);
        bool ResumeReplay(long replayHandle);
        bool StopReplay(long replayHandle);

        DateTime GetReplayPositon(long replayHandle);
        bool SetReplayPositon(long replayHandle, DateTime replayTime);
        bool SetReplaySpeed(long replayHandle, double replaySpeed);
    }
}

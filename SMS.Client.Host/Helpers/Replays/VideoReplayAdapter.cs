using SMS.Client.Host.Interfaces.Replays;
using SMS.Client.Host.Models.Replays;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Host.Helpers.Replays
{
    public class VideoReplayAdapter : IVideoReplayAdapter
    {
        public DateTime GetReplayPositon(long replayHandle)
        {
            throw new NotImplementedException();
        }

        public bool PauseReplay(long replayHandle)
        {
            throw new NotImplementedException();
        }

        public bool ResumeReplay(long replayHandle)
        {
            throw new NotImplementedException();
        }

        public bool SetReplayPositon(long replayHandle, DateTime replayTime)
        {
            throw new NotImplementedException();
        }

        public bool SetReplaySpeed(long replayHandle, double replaySpeed)
        {
            throw new NotImplementedException();
        }

        public long StartReplay(ReplayModel replayModel)
        {
            throw new NotImplementedException();
        }

        public bool StopReplay(long replayHandle)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Controls
{
    public enum PlayStatus : int
    {
        Unknown = 0,
        Play = 1,
        Pause = 2,
        Stop = 3,
        FastForward = 4,
        SlowForward = 5,
        FastBackward = 6,
        SlowBackward = 7,
        Step = 8,
    }
}

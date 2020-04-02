using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Controls
{
    internal class TimelineHelper
    {
        internal static long GetScaleUnitTicks(TimeUnitType timeUnit)
        {
            long tick = 1;
            switch (timeUnit)
            {
                case TimeUnitType.Day:
                    tick = 864000000000;
                    break;
                case TimeUnitType.Hour:
                    tick = 36000000000;
                    break;
                case TimeUnitType.Minute:
                    tick = 600000000;
                    break;
                case TimeUnitType.Second:
                    tick = 10000000;
                    break;
            }

            return tick;
        }
    }
}

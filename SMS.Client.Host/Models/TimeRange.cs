using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Host.Models
{
    public struct TimeRange
    {
        #region Properties

        public DateTime BeginTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public TimeSpan TimeRangeSpan
        {
            get
            {
                return EndTime - BeginTime;
            }
        }

        #endregion

        #region Constructors

        public TimeRange(DateTime beginTime, DateTime endTime)
        {
            BeginTime = beginTime;
            EndTime = endTime;
        }

        #endregion
    }
}

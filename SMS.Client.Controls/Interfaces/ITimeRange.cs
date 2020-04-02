using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Controls
{
    public interface ITimeRange
    {
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}

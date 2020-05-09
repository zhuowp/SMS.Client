using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Common.Utilities
{
    public static class TimestampHelper
    {
        private static readonly DateTime _utcTimeStampStartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// DateTime转换为10位时间戳（单位：秒）
        /// </summary>
        /// <param name="dateTime"> DateTime</param>
        /// <returns>10位时间戳（单位：秒）</returns>
        public static long ToTimeStamp(this DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - _utcTimeStampStartTime).TotalSeconds;
        }

        /// <summary>
        /// DateTime转换为13位时间戳（单位：毫秒）
        /// </summary>
        /// <param name="dateTime"> DateTime</param>
        /// <returns>13位时间戳（单位：毫秒）</returns>
        public static long ToLongTimeStamp(this DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - _utcTimeStampStartTime).TotalMilliseconds;
        }

        /// <summary>
        /// 10位时间戳（单位：秒）转换为DateTime
        /// </summary>
        /// <param name="timeStamp">10位时间戳（单位：秒）</param>
        /// <returns>DateTime</returns>
        public static DateTime TimeStampToDateTime(this long timeStamp)
        {
            return _utcTimeStampStartTime.AddSeconds(timeStamp).ToLocalTime();
        }

        /// <summary>
        /// 13位时间戳（单位：毫秒）转换为DateTime
        /// </summary>
        /// <param name="longTimeStamp">13位时间戳（单位：毫秒）</param>
        /// <returns>DateTime</returns>
        public static DateTime LongTimeStampToDateTime(this long longTimeStamp)
        {
            return _utcTimeStampStartTime.AddMilliseconds(longTimeStamp).ToLocalTime();
        }
    }
}

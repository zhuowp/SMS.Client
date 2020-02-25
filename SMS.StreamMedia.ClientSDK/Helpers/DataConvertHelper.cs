using System;
using System.Collections.Generic;
using System.Text;
using static SMS.StreamMedia.ClientSDK.CHCNetSDK;

namespace SMS.StreamMedia.ClientSDK.Helpers
{
    public static class DataConvertHelper
    {
        public static DateTime HCDeviceTimeToDateTime(NET_DVR_TIME devTime)
        {
            return new DateTime((int)devTime.dwYear, (int)devTime.dwMonth, (int)devTime.dwDay, (int)devTime.dwHour, (int)devTime.dwMinute, (int)devTime.dwSecond);
        }

        public static DateTime ToDateTime(this NET_DVR_TIME devTime)
        {
            return HCDeviceTimeToDateTime(devTime);
        }

        public static NET_DVR_TIME DateTimeToHCDeviceTime(DateTime dateTime)
        {
            NET_DVR_TIME hcDevTime = new NET_DVR_TIME();

            hcDevTime.dwYear = (uint)dateTime.Year;
            hcDevTime.dwMonth = (uint)dateTime.Month;
            hcDevTime.dwDay = (uint)dateTime.Day;
            hcDevTime.dwHour = (uint)dateTime.Hour;
            hcDevTime.dwMinute = (uint)dateTime.Minute;
            hcDevTime.dwSecond = (uint)dateTime.Second;

            return hcDevTime;
        }

        public static NET_DVR_TIME ToHCDeviceTime(this DateTime devTime)
        {
            return DateTimeToHCDeviceTime(devTime);
        }
    }
}

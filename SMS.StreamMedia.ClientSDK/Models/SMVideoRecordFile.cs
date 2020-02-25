using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.StreamMedia.ClientSDK.Models
{
    public class SMVideoRecordFile
    {
        public string DevId { get; set; }
        public string FileName { get; set; }
        public int FileSize { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}

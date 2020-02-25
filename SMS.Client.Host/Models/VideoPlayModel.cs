using SMS.Client.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Host.Models
{
    public class VideoPlayModel : IPlayModel
    {
        public IntPtr ScreenHandle { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Channel { get; set; }
        public uint StreamType { get; set; }
    }
}

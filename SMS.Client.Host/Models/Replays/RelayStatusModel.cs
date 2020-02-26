using SMS.Client.Controls;
using SMS.Client.Host.Models.Devices;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Host.Models.Replays
{
    public class RelayStatusModel
    {
        public CameraModel Camera { get; set; }
        //当前播放的录像状态
        public PlayStatus PlayingStatus { get; set; }
        //当前播放的录像播放句柄
        public long PlayingHandle { get; set; }
        //录像起始播放时间点
        public DateTime StartPlayDateTime { get; set; }
        //录像当前播放时间点
        public DateTime PlayingDateTime { get; set; }
        //正在播放的播放速度
        public double PlayingSpeed { get; set; }
        //正在播放的控件句柄
        public IntPtr PlayingScreenHandle { get; set; }
    }
}

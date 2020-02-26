using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Controls
{
    public interface IRealtimePlayHelper
    {
        //开始预览
        long StartPlay(IPlayModel playModel);
        //停止预览
        bool StopPlay(long playHandle);
        //云台控制
        bool PTZControl(long playHandle, PTZControlType controlType, int stopFlag, int speed);        
        //抓图
        bool CapturePicture(long playHandle, string fileName);
    }
}

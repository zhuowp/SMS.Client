using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Controls
{
    public interface IRealtimePlayHelper
    {
        //开始预览
        int StartPlay(IPlayModel playModel);
        //停止预览
        bool StopPlay(int playHandle);
        //云台控制
        bool PTZControl(int playHandle, PTZControlType controlType, int stopFlag, int speed);        
        //抓图
        bool CapturePicture(int playHandle, string fileName);
    }
}

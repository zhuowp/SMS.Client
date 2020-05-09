using SMS.Client.Common.Models;
using SMS.Client.Controls;
using SMS.StreamMedia.ClientSDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Common.Utilities
{
    public class VideoPlayHelper : IRealtimePlayHelper
    {
        public bool CapturePicture(long playHandle, string fileName)
        {
            int result = SMClient.Instance.CapturePictureBMP((int)playHandle, fileName);
            return result == 0;
        }

        public bool PTZControl(long playHandle, PTZControlType controlType, int stopFlag, int speed)
        {
            SMPTZControlType smControlType = (SMPTZControlType)((int)controlType);

            return SMClient.Instance.PTZControl((int)playHandle, smControlType, stopFlag, speed);
        }

        public long StartPlay(IPlayModel playParam)
        {
            VideoPlayModel playModel = playParam as VideoPlayModel;

            return SMClient.Instance.StartPreview(playModel.ScreenHandle, playModel.Ip, playModel.Port, playModel.UserName, playModel.Password, playModel.Channel, playModel.StreamType);
        }

        public bool StopPlay(long playHandle)
        {
            return SMClient.Instance.StopPreview((int)playHandle) == 0;
        }
    }
}

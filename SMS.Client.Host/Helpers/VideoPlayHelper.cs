using SMS.Client.Controls;
using SMS.Client.Host.Models;
using SMS.StreamMedia.ClientSDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Host.Helpers
{
    public class VideoPlayHelper : IRealtimePlayHelper
    {
        public bool CapturePicture(int playHandle, string fileName)
        {
            int result = SMClient.Instance.CapturePictureBMP(playHandle, fileName);
            if (result == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool PTZControl(int playHandle, PTZControlType controlType, int stopFlag, int speed)
        {
            SMPTZControlType smControlType = (SMPTZControlType)((int)controlType);

            return SMClient.Instance.PTZControl(playHandle, smControlType, stopFlag, speed);
        }

        public int StartPlay(IPlayModel playParam)
        {
            VideoPlayModel playModel = playParam as VideoPlayModel;

            return SMClient.Instance.StartPreview(playModel.ScreenHandle, playModel.Ip, playModel.Port, playModel.UserName, playModel.Password, playModel.Channel, playModel.StreamType);
        }

        public bool StopPlay(int playHandle)
        {
            return SMClient.Instance.StopPreview(playHandle) == 0;
        }

        public void Dispose()
        {
        }
    }
}

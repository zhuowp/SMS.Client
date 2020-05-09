using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Common.Models.Devices
{
    public class CameraModel : NetDeviceBase
    {
        #region Fields

        private string _channelID;

        #endregion

        #region Properties

        public string ChannelID
        {
            get
            {
                return _channelID;
            }

            set
            {
                _channelID = value;
            }
        }

        #endregion
    }
}

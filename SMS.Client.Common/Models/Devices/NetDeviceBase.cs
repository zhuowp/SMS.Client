using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Common.Models.Devices
{
    public class NetDeviceBase : DeviceBase
    {
        #region Fields

        private string _ip = "";
        private int _port = 0;
        private string _userName = "";
        private string _password = "";

        #endregion

        #region Properties

        public string Ip
        {
            get
            {
                return _ip;
            }

            set
            {
                _ip = value;
            }
        }

        public int Port
        {
            get
            {
                return _port;
            }

            set
            {
                _port = value;
            }
        }

        public string UserName
        {
            get
            {
                return _userName;
            }

            set
            {
                _userName = value;
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }

            set
            {
                _password = value;
            }
        }

        #endregion
    }
}

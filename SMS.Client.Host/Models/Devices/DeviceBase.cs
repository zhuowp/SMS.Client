using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Host.Models.Devices
{
    public class DeviceBase 
    {
        #region Fields

        private string _name = "";
        private string _id = "";
        private string _regionId = "";
        private string _deviceType = "";

        #endregion

        #region Properties

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public string Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public string RegionId
        {
            get
            {
                return _regionId;
            }

            set
            {
                _regionId = value;
            }
        }

        public string DeviceType
        {
            get
            {
                return _deviceType;
            }

            set
            {
                _deviceType = value;
            }
        }

        #endregion
    }
}

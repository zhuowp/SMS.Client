using SMS.Client.Common.Models;
using SMS.StreamMedia.ClientSDK;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Common.Caches
{
    public class HolographicInfoCache
    {
        #region Fields

        private static HolographicInfoCache _instance = null;
        private static object _lock = new object();
        private ConcurrentDictionary<string, HolographicInfo> _holographicInfoDict = new ConcurrentDictionary<string, HolographicInfo>();

        #endregion

        #region Events

        public event Action<HolographicInfo> OnHolographicInfoChanged;

        #endregion

        #region Properties

        public static HolographicInfoCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new HolographicInfoCache();
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion

        #region Constructors

        private HolographicInfoCache()
        {
            SMClient.Instance.OnGisInfoChanged += Instance_OnGisInfoChanged;
        }

        private void Instance_OnGisInfoChanged(CHCNetSDK.NET_DVR_GIS_UPLOADINFO obj)
        {
            HolographicInfo holographicInfo = new HolographicInfo()
            {
                CameraParameter = new CameraParam()
                {
                    HorizontalFieldOfView = obj.fHorizontalValue,
                    VerticalFieldOfView = obj.fVerticalValue,
                    Pan = obj.struPtzPos.fPanPos,
                    Tilt = obj.struPtzPos.fTiltPos,
                    Zoom = obj.struPtzPos.fZoomPos,
                },
                GeoInfo = new GeographicInfo()
                {

                },
                Id = "test",
            };

            _holographicInfoDict.AddOrUpdate(holographicInfo.Id, holographicInfo, (oldKey, oldValue) => holographicInfo);
            OnHolographicInfoChanged?.Invoke(holographicInfo);
        }

        #endregion

        #region Private Methods

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        public HolographicInfo GetHolographicInfoByDeviceId(string devId)
        {
            if (_holographicInfoDict.TryGetValue(devId, out HolographicInfo holographicInfo))
            {
                return holographicInfo;
            }

            return default;
        }

        #endregion
    }
}

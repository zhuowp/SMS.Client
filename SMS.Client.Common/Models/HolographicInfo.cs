using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Common.Models
{
    public struct HolographicInfo
    {
        public string Id { get; set; }
        public CameraParam CameraParameter { get; set; }
        public GeographicInfo GeoInfo { get; set; }
    }
}

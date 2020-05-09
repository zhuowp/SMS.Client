using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Common.Models
{
    public struct CameraParam
    {
        public double HorizontalFieldOfView { get; set; }
        public double VerticalFieldOfView { get; set; }

        public double Pan { get; set; }
        public double Tilt { get; set; }
        public double Zoom { get; set; }
    }
}

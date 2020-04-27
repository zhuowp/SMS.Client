using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Common.Models
{
    public struct PTZ
    {
        public double Pan { get; set; }
        public double Tilt { get; set; }
        public double Zoom { get; set; }

        public PTZ(double pan, double tilt, double zoom)
        {
            Pan = pan;
            Tilt = tilt;
            Zoom = zoom;
        }
    }
}

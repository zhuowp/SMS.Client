using System;
using System.Collections.Generic;
using System.Text;

namespace SMS.Client.Controls
{
    public struct HeatPoint
    {
        public int EffectiveRadius { get; set; }
        public int Y { get; set; }
        public int X { get; set; }
        public double Weight { get; set; }
    }
}

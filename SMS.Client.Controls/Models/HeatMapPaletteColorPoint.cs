using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SMS.Client.Controls
{
    public class HeatMapPaletteColorPoint
    {
        public double Value { get; set; }
        public double Offset { get; set; }
        public Color Color { get; set; }

        public HeatMapPaletteColorPoint()
        {

        }

        public HeatMapPaletteColorPoint(double value, double offset, Color color)
        {
            Value = value;
            Offset = offset;
            Color = color;
        }

        public HeatMapPaletteColorPoint(double value, double offset, string color)
        {
            Value = value;
            Offset = offset;
            Color = (Color)ColorConverter.ConvertFromString(color);
        }
    }
}

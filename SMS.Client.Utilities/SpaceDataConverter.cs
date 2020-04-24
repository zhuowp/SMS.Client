using System;
using System.Collections.Concurrent;
using System.Windows;

namespace SMS.Client.Utilities
{
    public class SpaceDataConverter
    {
        #region Fields

        private readonly double _width = 1920;
        private readonly double _height = 1080;

        private readonly ConcurrentDictionary<double, double> _normalizedDistanceDict = new ConcurrentDictionary<double, double>();

        #endregion

        #region Properties

        #endregion

        #region Constructors

        public SpaceDataConverter(double width, double height)
        {
            _width = width;
            _height = height;
        }

        #endregion

        #region Private Methods

        private double NormalizedDisplacementToAngle(double viewField, double displacement)
        {
            double normalizedDistance = 0.5 / Math.Tan(viewField / 2);
            double angle = Math.Atan((0.5 - displacement) / normalizedDistance);
            return angle;
        }

        private double AngleToNormalizedDisplacement(double viewField, double angle)
        {
            double normalizedDistance = 0.5 / Math.Tan(viewField / 2);
            double displacement = 0.5 - normalizedDistance * Math.Tan(angle);

            return displacement;
        }

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        public Point AngleLocationToScreenLocation(double panAngle, double tiltAngle, double horizontalFieldOfView, double verticalFieldOfView)
        {
            double normalizedWidth = AngleToNormalizedDisplacement(panAngle, horizontalFieldOfView);
            double normalizedHeight = AngleToNormalizedDisplacement(tiltAngle, verticalFieldOfView);

            return new Point(normalizedWidth * _width, normalizedHeight * _height);
        }

        public Point ScreenLocationToAngleLocation(Point location, double horizontalFieldOfView, double verticalFieldOfView)
        {
            double panAngle = NormalizedDisplacementToAngle(horizontalFieldOfView, location.X);
            double tiltAngle = NormalizedDisplacementToAngle(verticalFieldOfView, location.Y);

            return new Point(panAngle, tiltAngle);
        }

        #endregion
    }
}

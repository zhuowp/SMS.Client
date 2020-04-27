using SMS.Client.Common.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SMS.Client.Common.Utilities
{
    public class SpaceTransformer
    {
        #region Consts

        private const double DOUBLE_PI = 6.2831853071795862;

        #endregion

        #region Fields

        private readonly double _width = 1920;
        private readonly double _height = 1080;

        private readonly ConcurrentDictionary<double, double> _normalizedDistanceDict = new ConcurrentDictionary<double, double>();

        #endregion

        #region Properties

        #endregion

        #region Constructors

        public SpaceTransformer(double width = 1920, double height = 1080)
        {
            _width = width;
            _height = height;
        }

        #endregion

        #region Private Methods

        private double GetNormalizedFocusLengthByViewField(double viewField)
        {
            return 0.5 / Math.Tan(viewField / 2);
        }

        private double NormalizedDisplacementToAngle(double viewField, double displacement)
        {
            double normalizedFocusLength = GetNormalizedFocusLengthByViewField(viewField);
            double angle = Math.Atan((displacement - 0.5) / normalizedFocusLength);

            return angle;
        }

        private double AngleToNormalizedDisplacement(double viewField, double angle)
        {
            double normalizedFocusLength = GetNormalizedFocusLengthByViewField(viewField);
            double displacement = 0.5 + normalizedFocusLength * Math.Tan(angle);

            return displacement;
        }

        private double AbsoluteAngleToCameraAngle(double absoluteAngle, double cameraRotateAngle)
        {
            return (absoluteAngle - cameraRotateAngle + DOUBLE_PI) % DOUBLE_PI;
        }

        private double CameraAngleToAbsoluteAngle(double cameraAngle, double cameraRotateAngle)
        {
            return (cameraRotateAngle + cameraAngle + DOUBLE_PI) % DOUBLE_PI;
        }

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        /// <summary>
        /// 传入的角均为角度制
        /// </summary>
        /// <param name="panAngle"></param>
        /// <param name="tiltAngle"></param>
        /// <param name="horizontalFieldOfView"></param>
        /// <param name="verticalFieldOfView"></param>
        /// <returns></returns>
        public Point AngleLocationToScreenLocation(double panAngle, double tiltAngle, double cameraPan, double cameraTilt, double horizontalFieldOfView, double verticalFieldOfView)
        {
            double radianPanAngle = panAngle.ToRadian();
            double radianTiltAngle = tiltAngle.ToRadian();

            double radianCameraPan = cameraPan.ToRadian();
            double radianCameraTilt = cameraTilt.ToRadian();

            double radianHorizontalFieldOfView = horizontalFieldOfView.ToRadian();
            double radianVerticalFieldOfView = verticalFieldOfView.ToRadian();

            double relativePanAngle = AbsoluteAngleToCameraAngle(radianPanAngle, radianCameraPan);
            double relativTiltAngle = AbsoluteAngleToCameraAngle(radianTiltAngle, radianCameraTilt);

            double normalizedWidth = AngleToNormalizedDisplacement(radianHorizontalFieldOfView, relativePanAngle);
            double normalizedHeight = AngleToNormalizedDisplacement(radianVerticalFieldOfView, relativTiltAngle);

            return new Point(normalizedWidth * _width, normalizedHeight * _height);
        }

        public Point AngleLocationToScreenLocation(PTZ ptz, PTZ cameraPtz, double horizontalFieldOfView, double verticalFieldOfView)
        {
            return AngleLocationToScreenLocation(ptz.Pan, ptz.Tilt, cameraPtz.Pan, cameraPtz.Tilt, horizontalFieldOfView, verticalFieldOfView);
        }

        public PTZ ScreenLocationToAngleLocation(Point location, double horizontalFieldOfView, double verticalFieldOfView)
        {
            double panAngle = NormalizedDisplacementToAngle(horizontalFieldOfView, location.X);
            double tiltAngle = NormalizedDisplacementToAngle(verticalFieldOfView, location.Y);

            return new PTZ(panAngle, tiltAngle, 1);
        }

        #endregion
    }
}

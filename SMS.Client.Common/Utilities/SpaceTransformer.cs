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

        private double CameraAngleToAbsoluteAngle(double angleToCamera, double cameraRotateAngle)
        {
            return (cameraRotateAngle + angleToCamera + DOUBLE_PI) % DOUBLE_PI;
        }

        private Point AngleLocationToScreenLocation(double panAngle, double tiltAngle, double cameraPan, double cameraTilt, double horizontalFieldOfView, double verticalFieldOfView)
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

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        public Point AngleLocationToScreenLocation(double panAngle, double tiltAngle, CameraParam cameraParam)
        {
            return AngleLocationToScreenLocation(panAngle, tiltAngle, cameraParam.Pan, cameraParam.Tilt, cameraParam.HorizontalFieldOfView, cameraParam.VerticalFieldOfView);
        }

        public PTZ ScreenLocationToAngleLocation(Point location, CameraParam cameraParam)
        {
            double radianCameraPan = cameraParam.Pan.ToRadian();
            double radianCameraTilt = cameraParam.Tilt.ToRadian();

            double radianHorizontalFieldOfView = cameraParam.HorizontalFieldOfView.ToRadian();
            double radianVerticalFieldOfView = cameraParam.VerticalFieldOfView.ToRadian();

            double horizontalNormalizedDisplacement = location.X / _width;
            double verticalNormalizedDisplacement = location.Y / _height;

            double panAngleToCamera = NormalizedDisplacementToAngle(radianHorizontalFieldOfView, horizontalNormalizedDisplacement);
            double tiltAngleToCamera = NormalizedDisplacementToAngle(radianVerticalFieldOfView, verticalNormalizedDisplacement);

            double absolutePanAngle = CameraAngleToAbsoluteAngle(panAngleToCamera, radianCameraPan);
            double absoluteTiltAngle = CameraAngleToAbsoluteAngle(tiltAngleToCamera, radianCameraTilt);

            return new PTZ(absolutePanAngle.ToDegree(), absoluteTiltAngle.ToDegree(), 1);
        }

        #endregion


        public void Pt2Xy(double hfov, double vfov, double cp, double ct, double tp, double tt, out double x, out double y)
        {
            x = 0.0;
            y = 0.0;

            double num = 0.0;
            int num2 = 1;
            int num3 = 1;
            if (cp >= tp)
            {
                num = cp - tp;
                num2 = -1;
            }
            else if (tp - cp > hfov)
            {
                num = 360.0 - tp + cp;
                num2 = -1;
            }
            else
            {
                num = tp - cp;
            }
            double num4 = Math.Cos((hfov / 2.0).ToRadian());
            double cor = Math.Acos(Math.Cos(ct.ToRadian()) * Math.Cos(tt.ToRadian()) * Math.Cos(num.ToRadian()) + Math.Sin(ct.ToRadian()) * Math.Sin(tt.ToRadian())).ToDegree();
            double num5 = num4 / Math.Cos(cor.ToRadian());
            double num6 = num5 * Math.Cos(tt.ToRadian());
            double num7 = num6 * Math.Sin(num.ToRadian());
            double num8 = num6 * Math.Cos(num.ToRadian());
            double num9 = Math.Atan(num5 * Math.Sin(tt.ToRadian()) / num8).ToDegree();

            if (num9 < ct)
            {
                num3 = -1;
            }
            double cor2 = Math.Abs(ct - num9);
            y = _height / 2.0 * Math.Tan(cor2.ToRadian()) / Math.Tan((vfov / 2.0).ToRadian());
            x = num7 * _width / 2.0 / Math.Sin((hfov / 2.0).ToRadian());
            y = _height / 2.0 + y * num3;
            x = _width / 2.0 + x * num2;
        }
    }
}

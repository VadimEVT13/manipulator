using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace InverseTest.GUI.Trackbol
{
    class _3DTrackboll:Trackball
    {
        public _3DTrackboll() : base()
        {
        }

        protected override void Rotate(Point currentPosition)
        {
            Vector3D currentPosition3D = ProjectToTrackball(
                           EventSource.ActualWidth, EventSource.ActualHeight, currentPosition);

            Vector3D axis = Vector3D.CrossProduct(_previousPosition3D, currentPosition3D);
            double angle = Vector3D.AngleBetween(_previousPosition3D, currentPosition3D);
            Quaternion delta = new Quaternion(axis, -angle);

            // Get the current orientantion from the RotateTransform3D
            AxisAngleRotation3D r = _rotation;
            Quaternion q = new Quaternion(_rotation.Axis, _rotation.Angle);

            // Compose the delta with the previous orientation
            q *= delta;

            // Write the new orientation back to the Rotation3D
            _rotation.Axis = q.Axis;
            _rotation.Angle = q.Angle;

            _previousPosition3D = currentPosition3D;
        }

        protected override void Track(Point currentPosition)
        {
            Point offsetTranslate = new Point(currentPosition.X - _previousPosition2D.X, currentPosition.Y - _previousPosition2D.Y);
            Console.WriteLine("Track: " + currentPosition.ToString());
            _translate.OffsetX -= offsetTranslate.X;
            _translate.OffsetY += offsetTranslate.Y;
        }

    }
}


using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Markup;

namespace _3DTools
{
    public class Trackball
    {
        private FrameworkElement _eventSource;
        private Point _previousPosition2D;
        private Vector3D _previousPosition3D = new Vector3D(0, 0, 1);

        private Transform3DGroup _transform;
        private ScaleTransform3D _scale = new ScaleTransform3D();
        private AxisAngleRotation3D _rotation = new AxisAngleRotation3D();

        public Trackball()
        {
            _transform = new Transform3DGroup();
            _transform.Children.Add(_scale);
            _transform.Children.Add(new RotateTransform3D(_rotation));
        }

        /// <summary>
        ///     A transform to move the camera or scene to the trackball's
        ///     current orientation and scale.
        /// </summary>
        public Transform3D Transform
        {
            get { return _transform; }
        }

        #region Event Handling

        /// <summary>
        ///     The FrameworkElement we listen to for mouse events.
        /// </summary>
        public FrameworkElement EventSource
        {
            get { return _eventSource; }
            
            set
            {
                if (_eventSource != null)
                {
                    _eventSource.MouseDown -= this.OnMouseDown;
                    _eventSource.MouseUp -= this.OnMouseUp;
                    _eventSource.MouseMove -= this.OnMouseMove;
                }

                _eventSource = value;

                _eventSource.MouseDown += this.OnMouseDown;
                _eventSource.MouseUp += this.OnMouseUp;
                _eventSource.MouseMove += this.OnMouseMove;
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            Mouse.Capture(EventSource, CaptureMode.Element);
            _previousPosition2D = e.GetPosition(EventSource);
            _previousPosition3D = ProjectToTrackball(
                EventSource.ActualWidth,
                EventSource.ActualHeight,
                _previousPosition2D);
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            Mouse.Capture(EventSource, CaptureMode.None);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Point currentPosition = e.GetPosition(EventSource);

            // Prefer tracking to zooming if both buttons are pressed.
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Track(currentPosition);
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                Zoom(currentPosition);
            }

            _previousPosition2D = currentPosition;
        }

        #endregion Event Handling

        private void Track(Point currentPosition)
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

        private Vector3D ProjectToTrackball(double width, double height, Point point)
        {
            double x = point.X / (width / 2);    // Scale so bounds map to [0,0] - [2,2]
            double y = point.Y / (height / 2);

            x = x - 1;                           // Translate 0,0 to the center
            y = 1 - y;                           // Flip so +Y is up instead of down

            double z2 = 1 - x * x - y * y;       // z^2 = 1 - x^2 - y^2
            double z = z2 > 0 ? Math.Sqrt(z2) : 0;

            return new Vector3D(x, y, z);
        }

        private void Zoom(Point currentPosition)
        {
            double yDelta = currentPosition.Y - _previousPosition2D.Y;
            
            double scale = Math.Exp(yDelta / 100);    // e^(yDelta/100) is fairly arbitrary.

            _scale.ScaleX *= scale;
            _scale.ScaleY *= scale;
            _scale.ScaleZ *= scale;
        }
    }
}

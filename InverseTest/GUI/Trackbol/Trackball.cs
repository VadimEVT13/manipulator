using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Markup;

namespace InverseTest.GUI
{
    public abstract class Trackball
    {
        private FrameworkElement _eventSource;
        protected Point _previousPosition2D;
        protected Vector3D _previousPosition3D = new Vector3D(0, 0, 1);

        private Transform3DGroup _transform;
        protected ScaleTransform3D _scale = new ScaleTransform3D();
        protected AxisAngleRotation3D _rotation = new AxisAngleRotation3D();
        protected TranslateTransform3D _translate = new TranslateTransform3D();

        public Trackball()
        {
            this._transform = new Transform3DGroup();
            this._transform.Children.Add(_scale);
            this._transform.Children.Add(new RotateTransform3D(_rotation));
            this._transform.Children.Add(_translate);
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
                    _eventSource.MouseWheel -= this.OnMouseWheel;
                    

                }

                _eventSource = value;

                _eventSource.MouseDown += this.OnMouseDown;
                _eventSource.MouseUp += this.OnMouseUp;
                _eventSource.MouseMove += this.OnMouseMove;
                _eventSource.MouseWheel += this.OnMouseWheel;
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
                Rotate(currentPosition);
            }


            _previousPosition2D = currentPosition;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Zoom(e.GetPosition(_eventSource), e.Delta);

        }

        #endregion Event Handling

        protected abstract void Track(Point currentPosition);

        protected virtual void Rotate(Point currentPosition)
        { }
        



        protected Vector3D ProjectToTrackball(double width, double height, Point point)
        {
            double x = point.X / (width / 2);    // Scale so bounds map to [0,0] - [2,2]
            double y = point.Y / (height / 2);

            x = x - 1;                           // Translate 0,0 to the center
            y = 1 - y;                           // Flip so +Y is up instead of down

            double z2 = 1 - x * x - y * y;       // z^2 = 1 - x^2 - y^2
            double z = z2 > 0 ? Math.Sqrt(z2) : 0;

            return new Vector3D(x, y, z);
        }

        protected void Zoom(Point point, int delta)
        {
            double scale = Math.Exp((double)delta / 1000);    // e^(delta/500) is fairly arbitrary.

            _scale.ScaleX *= scale;
            _scale.ScaleY *= scale;
            _scale.ScaleZ *= scale;
            _scale.CenterX = point.X;
            _scale.CenterY = point.Y;
        }


    }
}

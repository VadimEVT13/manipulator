using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using InverseTest.GUI;
using System.Windows.Media.Media3D;

namespace InverseTest.GUI
{
    class RightTrackboll : Trackball
    {

        public RightTrackboll(): base()
        {
        }

        protected override void Track(Point currentPosition)
        {   //Vector3D currentPosition3D = ProjectToTrackball(
              // EventSource.ActualWidth, EventSource.ActualHeight, currentPosition);

            Point offsetTranslate = new Point(currentPosition.X-_previousPosition2D.X, currentPosition.Y - _previousPosition2D.Y);
            Console.WriteLine("Track: " + currentPosition.ToString());
            _translate.OffsetX -= offsetTranslate.X;
            _translate.OffsetY += offsetTranslate.Y;

        }
        
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InverseTest.GUI.Trackbol
{
    class TopTrackboll:Trackball
    {
        public TopTrackboll() : base()
        { }

        protected override void Track(Point currentPosition)
        {
            Point offsetTranslate = new Point(currentPosition.X - _previousPosition2D.X, currentPosition.Y - _previousPosition2D.Y);
            Console.WriteLine("Track: " + currentPosition.ToString());
            _translate.OffsetZ -= offsetTranslate.X;
            _translate.OffsetX += offsetTranslate.Y;
        }
    }
}

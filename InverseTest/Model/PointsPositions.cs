using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest
{
    public class SystemPosition
    {
       public Point3D ManipPoint { get; }
        public Point3D TargetPoint { get; }
        public double DistanceManipulatorToScanPoint { get; }
        public double FocusEnlagment { get; }
       

        public SystemPosition(Point3D manipPoint, Point3D targetPoint, double distance, double focusEnlagment)
        {
            this.ManipPoint = manipPoint;
            this.TargetPoint = targetPoint;
            this.DistanceManipulatorToScanPoint = distance;
            this.FocusEnlagment = focusEnlagment;
        }
    }
}

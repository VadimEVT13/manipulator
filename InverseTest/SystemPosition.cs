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
       public Point3D manipPoint { get; }
        public Point3D targetPoint { get; }
       

        public SystemPosition(Point3D manipPoint, Point3D targetPoint)
        {
            this.manipPoint = manipPoint;
            this.targetPoint = targetPoint;
        }
    }
}

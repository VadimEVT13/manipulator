using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Model
{
    public class ScanPoint
    {
        public Point3D point { get; }

        public ScanPoint(Point3D point)
        {   this.point = point;
        }
    }
}

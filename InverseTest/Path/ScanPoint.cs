using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Path
{
    public class ScanPoint
    {
        private Guid id;
        public Point3D point { get; set; }

        public ScanPoint(Point3D point)
        {
            this.id = Guid.NewGuid();
            this.point = new Point3D(Math.Round(point.X, 2), Math.Round(point.Y, 2), Math.Round(point.Z, 2));
        }

        public void Transform(Transform3D trans)
        {
            this.point = trans.Transform(this.point);
        }

        public bool Equals(ScanPoint p)
        {
            if (p == null) return false;
            return id.Equals(p.id);
        }

        public override bool Equals(object obj)
        {
            if (obj is ScanPoint p)
            {
                return this.Equals(p);
            }
            return base.Equals(obj);
        }

    }
}

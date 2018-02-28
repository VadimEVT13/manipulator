using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Collision.Model
{
    public class PartShape
    {
        public Enum partType { get; }
        public Rect3D bounds { get; }
        public List<Point3D> points { get; }
        public Matrix3D transform { get; }

        public PartShape(Enum partName, Rect3D bounds, List<Point3D> points, Matrix3D transform)
        {
            this.partType = partName;
            this.bounds = bounds;
            this.points = points;
            this.transform = transform;
        }
    }
}

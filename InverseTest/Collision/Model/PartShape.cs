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
        public Enum PartType { get; }
        public Rect3D Bounds { get; }
        public List<Point3D> Points { get; }
        public Matrix3D Transform { get; }

        public PartShape(Enum partName, Rect3D bounds, List<Point3D> points, Matrix3D transform)
        {
            this.PartType = partName;
            this.Bounds = bounds;
            this.Points = points;
            this.Transform = transform;
        }
    }
}

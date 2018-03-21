using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Collision
{
    public class Plane
    {
        public Vector3D[] points;

        public Plane()
        {
            points = new Vector3D[3];
        }
        public Plane(Vector3D p1, Vector3D p2, Vector3D p3)
        {

            points = new Vector3D[3];
            points[0] = p1;
            points[1] = p2;
            points[2] = p3;
        }
        
    }

}

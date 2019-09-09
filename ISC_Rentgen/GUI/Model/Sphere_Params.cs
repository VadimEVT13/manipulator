using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ISC_Rentgen.GUI.Model
{
    public class Sphere_Params
    {
        public  Point3D Position { get { return position; } set { position = value; } }
        private Point3D position = new Point3D();

        public  double Radius { get { return radius; } set { if (value > 0) radius = value; } }
        private double radius = 1;

        public static string SphereName { get { return "Sphere"; } }
    }
}

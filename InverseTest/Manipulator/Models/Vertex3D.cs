using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Manipulator.Models
{
    public class Vertex3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public static double Distance(Vertex3D A, Vertex3D B)
        {
            return Math.Sqrt((A.X - B.X) * (A.X - B.X) +
                             (A.Y - B.Y) * (A.Y - B.Y) +
                             (A.Z - B.Z) * (A.Z - B.Z));
        }
    }
}

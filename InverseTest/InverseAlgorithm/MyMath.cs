using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.InverseAlgorithm
{
    public static class MyMath
    {
        public static double PowerOf(double x, int power)
        {
            double res = x;
            for (int i = 0; i < power-1; i++)
            {
                res = res * res;
            }
            return res;
        }

        public static double DistanceBetween(Point3D a, Point3D b)
        {
            Vector3D lengthVector = b - a;
            double length =
                Math.Sqrt(MyMath.PowerOf(lengthVector.X, 2) + MyMath.PowerOf(lengthVector.Y, 2) + MyMath.PowerOf(lengthVector.Z, 2));
            return length;
        }


        public static Point3D VectorProjectionOnPlane(Point3D targetPoint, Vector3D planeNormal)
        {
            Vector3D targetVector = new Vector3D(targetPoint.X, targetPoint.Y, targetPoint.Z);

            Vector3D unitNormal = planeNormal;
            unitNormal.Normalize();

            double dist = Vector3D.DotProduct(targetVector, unitNormal);

            Point3D resPoint  = targetPoint - dist * planeNormal;

            return resPoint;
        }

        public static Point3D GetPointOnSegment(Point3D a, Point3D b, double distanceFromA)
        {
            double distance = MyMath.DistanceBetween(a, b);
            double lambda = distanceFromA / (distance - distanceFromA);
            double x = (a.X + lambda * b.X) / (1 + lambda);

            double y = (a.Y + lambda * b.Y) / (1 + lambda);

            double z = (a.Z + lambda * b.Z) / (1 + lambda);
            return new Point3D(x, y, z);
        }

    }
}

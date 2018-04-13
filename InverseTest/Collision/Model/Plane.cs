using System;
using System.Windows.Media.Media3D;

namespace InverseTest.Collision
{
    public class Plane
    {
        Vector3D P0 = new Vector3D(0,0,0);
        public Vector3D P1 = new Vector3D();
        public Vector3D P2 = new Vector3D();
        public Vector3D P3 = new Vector3D();
        public Vector3D Hv = new Vector3D();

        public Plane()
        {
        }
        public Plane(Vector3D p1, Vector3D p2, Vector3D p3)
        {

             P1 = p1;
             P2 = p2;
             P3 = p3;


            // Уравнение плоскости
            /*a = y1 * (z2 - z3) + y2 * (z3 - z1) + y3 * (z1 - z2);
            b = z1 * (x2 - x3) + z2 * (x3 - x1) + z3 * (x1 - x2);
            c = x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2);
            d = -(x1 * (y2 * z3 - y3 * z2) + x2 * (y3 * z1 - y1 * z3) + x3 * (y1 * z2 - y2 * z1));*/
        }
        
        public Vector3D minPoint()
        {
            double A, B, C, D, H, t;
            //Vector3D Hv = new Vector3D();
            A = P1.Y * (P2.Z - P3.Z) + P2.Y * (P3.Z - P1.Z) + P3.Y * (P1.Z - P2.Z);
            B = P1.Z * (P2.X - P3.X) + P2.Z * (P3.X - P1.X) + P3.Z * (P1.X - P2.X);
            C = P1.X * (P2.Y - P3.Y) + P2.X * (P3.Y - P1.Y) + P3.X * (P1.Y - P2.Y);
            D = -(P1.X * (P2.Y * P3.Z - P3.Y * P2.Z) + P2.X * (P3.Y * P1.Z - P1.Y * P3.Z) + P3.X * (P1.Y * P2.Z - P2.Y * P1.Z));

            H = D / (Math.Sqrt(A * A + B * B + C * C));
            t = -D / (A * A + B * B + C * C);
            Hv.X = A * t;
            Hv.Y = B * t;
            Hv.Z = C * t;
            return Hv;
        }


        double triangle_square(double a, double b, double c)
        {
            double p = (a + b + c) / 2;
            return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
        }

        public bool inside_triangle()
        {
            bool inside = false;
            P0 = Hv;
            double AB = Math.Sqrt((P1.X - P2.X) * (P1.X - P2.X) + (P1.Y - P2.Y) * (P1.Y - P2.Y) + (P1.Z - P2.Z) * (P1.Z - P2.Z));
            double BC = Math.Sqrt((P2.X - P3.X) * (P2.X - P3.X) + (P2.Y - P3.Y) * (P2.Y - P3.Y) + (P2.Z - P3.Z) * (P2.Z - P3.Z));
            double CA = Math.Sqrt((P1.X - P3.X) * (P1.X - P3.X) + (P1.Y - P3.Y) * (P1.Y - P3.Y) + (P1.Z - P3.Z) * (P1.Z - P3.Z));

            double AP = Math.Sqrt((P0.X - P1.X) * (P0.X - P1.X) + (P0.Y - P1.Y) * (P0.Y - P1.Y) + (P0.Z - P1.Z) * (P0.Z - P1.Z));
            double BP = Math.Sqrt((P0.X - P2.X) * (P0.X - P2.X) + (P0.Y - P2.Y) * (P0.Y - P2.Y) + (P0.Z - P2.Z) * (P0.Z - P2.Z));
            double CP = Math.Sqrt((P0.X - P3.X) * (P0.X - P3.X) + (P0.Y - P3.Y) * (P0.Y - P3.Y) + (P0.Z - P3.Z) * (P0.Z - P3.Z));
            double diff = (triangle_square(AP, BP, AB) + triangle_square(AP, CP, CA) + triangle_square(BP, CP, BC)) - triangle_square(AB, BC, CA);
            if (Math.Abs(diff) < 0.0001) inside = true;
            return inside;
        }

    }

}

using System;
using System.Windows.Media.Media3D;

namespace InverseTest.Collision
{
    public class Plane
    {
        Vector3D P0 = new Vector3D(0, 0, 0); //начало координат
        public Vector3D P1,P2,P3, pointOnPlaneNormal = new Vector3D();
        public double[] equation = new double[4];

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

        private void Build_equation() //построение уравнения 0-A, 1-B, 2-C, 3-D
        {
            equation[0] = P1.Y * (P2.Z - P3.Z) + P2.Y * (P3.Z - P1.Z) + P3.Y * (P1.Z - P2.Z);
            equation[1] = P1.Z * (P2.X - P3.X) + P2.Z * (P3.X - P1.X) + P3.Z * (P1.X - P2.X);
            equation[2] = P1.X * (P2.Y - P3.Y) + P2.X * (P3.Y - P1.Y) + P3.X * (P1.Y - P2.Y);
            equation[3] = -(P1.X * (P2.Y * P3.Z - P3.Y * P2.Z) + P2.X * (P3.Y * P1.Z - P1.Y * P3.Z) + P3.X * (P1.Y * P2.Z - P2.Y * P1.Z));
        }

        public bool isPlane(Vector3D p) //проверка принадлежности точки к плоскости (не лежит ли весь симплекс в плоскости)
        {
            double E=0;
            Build_equation();
            //E = p.X * A + p.Y * B + p.Z * C + D;*/
            E = p.X * equation[0] + p.Y * equation[1] + p.Z * equation[2] + equation[3];
            if (Math.Abs(E)<0.001) return true; //иногда значение крайне близко к нулю, но им не является
            return false;
        }


        private void FindPointOnPlane() //поиск координат точки на плоскости треугольника 
        {
            double H, t;
            /*H = D / (Math.Sqrt(A * A + B * B + C * C));
            t = -D / (A * A + B * B + C * C);
            Hv.X = A * t;
            Hv.Y = B * t;
            Hv.Z = C * t;*/
            Build_equation();
            H = equation[3] / (Math.Sqrt(equation[0] * equation[0] + equation[1] * equation[1] + equation[2] * equation[2]));
            t = -equation[3] / (equation[0] * equation[0] + equation[1] * equation[1] + equation[2] * equation[2]);
            pointOnPlaneNormal.X = equation[0] * t;
            pointOnPlaneNormal.Y = equation[1] * t;
            pointOnPlaneNormal.Z = equation[2] * t;
        }


        double triangle_square(double a, double b, double c)
        {
            double p = (a + b + c) / 2;
            return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
        }

        public bool inside_triangle() 
        {
            //Vector3D Hv = minPoint();
            FindPointOnPlane();
            bool inside = false;
            P0 = pointOnPlaneNormal;
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

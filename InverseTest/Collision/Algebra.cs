using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Collision
{
    class Algebra
    {
        public static double proection(int axis, double centre, Point3D[] triangle) //определение к которой части принадлежит треугольник
        {
            double cen = 0;
            switch (axis)
            {
                case 1: cen = coor(triangle[0].X, triangle[1].X, triangle[2].X); if (cen <= centre) return 0; else return 1;
                case 2: cen = coor(triangle[0].Y, triangle[1].Y, triangle[2].Y); if (cen <= centre) return 0; else return 1;
                case 3: cen = coor(triangle[0].Z, triangle[1].Z, triangle[2].Z); if (cen <= centre) return 0; else return 1;
            }
            return 2;
        }

        private static double coor(double c1, double c2, double c3) //координата центра проекции треугольника на нормаль делящей плоскости
        {
            double max, min;
            List<double> t = new List<double>();
            t.Add(c1); t.Add(c2); t.Add(c3);
            t.Sort();
            min = t[0]; max = t[2];
            return min + (max - min)/2;
        }

        struct Plane
        {
            private double A, B, C, D;
            //Задание плоскости по трём точкам
            public Plane(Point3D p1, Point3D p2, Point3D p3)
            {
                //Line l = new Line(p1, p2);
                //if (l.HasPoint(p3)) throw new ArgumentException("Точки лежат на одной прямой");
                A = (p2.Y - p1.Y) * (p3.Z - p1.Z) - (p3.Y - p1.Y) * (p2.Z - p1.Z);
                B = (p3.X - p1.X) * (p2.Z - p1.Z) - (p2.X - p1.X) * (p3.Z - p1.Z);
                C = (p2.X - p1.X) * (p3.Y - p1.Y) - (p3.X - p1.X) * (p2.Y - p1.Y);
                D = -p1.X * A - p1.Y * B - p1.Z * C;
            }
            public Plane(double a, double b, double c, double d)
            {
                this.A = a;
                this.B = b;
                this.C = c;
                this.D = d;
            }

            //Точка пересечения прямой и данной плоскости
            public Point3D? CrossPoint(Line l)
            {
                //Если скалярное произведение нормалей прямой и плоскости равно нулю, значит они не пересекаются
                if (Vector3D.DotProduct(l.p, new Vector3D(A, B, C)) == 0)
                    return null;
                double t0 = -(A * l.M.X + B * l.M.Y + C * l.M.Z + D) / (A * l.p.X + B * l.p.Y + C * l.p.Z);
                return new Point3D(t0 * l.p.X + l.M.X, t0 * l.p.Y + l.M.Y, t0 * l.p.Z + l.M.Z);
            }
        }

        struct Line
        {
            //Точка, через которую проходит прямая
            public Point3D M;
            // Нормальный вектор
            public Vector3D p;
            public Line(Point3D m, Vector3D p)
            {
                M = m;
                this.p = p;
            }
            public Line(Point3D p1, Point3D p2)
            {
                M = p1;
                p = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            }
        }



        struct Triangle
        {
            public Point3D V1;
            public Point3D V2;
            public Point3D V3;

            public Triangle(Point3D p1, Point3D p2, Point3D p3)
            {
                this.V1 = p1;
                this.V2 = p2;
                this.V3 = p3;
            }

            public Point3D? IsIntersectWith(Triangle tr)
            {
                //Плоскость, в которой лежит текущий треугольник
                Plane pl = new Plane(V1, V2, V3);
                //Сторона между первой и второй вершинами данного треугольника
                Line l = new Line(tr.V1, tr.V2);
                //Точка пересечения стороны и плоскости
                Point3D? crosspt = pl.CrossPoint(l);
                //Если сторона и плоскость пересекаются
                if (crosspt != null)
                    //Проверям, чтобы точка находилась внутри треугольника
                    if (IsPointInside(tr, crosspt.Value))
                        return crosspt;
                //Сторона между второй и третьей вершинами треугольника
                l = new Line(tr.V2, tr.V3);
                crosspt = pl.CrossPoint(l);
                if (crosspt != null)
                    if (IsPointInside(tr, crosspt.Value))
                        return crosspt;
                //Сторона между третьей и первой вершинами
                l = new Line(tr.V3, tr.V1);
                crosspt = pl.CrossPoint(l);
                if (crosspt != null)
                    if (IsPointInside(tr, crosspt.Value))
                        return crosspt;
                return null;
            }


            private bool IsPointInside(Triangle tr, Point3D pt)
            {
                //Векторы сторон треугольника
                Vector3D a = new Vector3D(tr.V2.X - tr.V1.X, tr.V2.Y - tr.V1.Y, tr.V2.Z - tr.V1.Z);
                Vector3D b = new Vector3D(tr.V3.X - tr.V2.X, tr.V3.Y - tr.V2.Y, tr.V3.Z - tr.V2.Z);
                Vector3D c = new Vector3D(tr.V1.X - tr.V3.X, tr.V1.Y - tr.V3.Y, tr.V1.Z - tr.V3.Z);
                //Векторы вершин и точки
                Vector3D d1 = new Vector3D(pt.X - tr.V2.X, pt.Y - tr.V2.Y, pt.Z - tr.V2.Z);
                Vector3D d2 = new Vector3D(pt.X - tr.V3.X, pt.Y - tr.V3.Y, pt.Z - tr.V3.Z);
                Vector3D d3 = new Vector3D(pt.X - tr.V1.X, pt.Y - tr.V1.Y, pt.Z - tr.V1.Z);
                //Векторные произведения
                Vector3D d1b = Vector3D.CrossProduct(d1, b);//Vector3D.DotProduct(d1, b);
                Vector3D d2c = Vector3D.CrossProduct(d2, c);
                Vector3D d3a = Vector3D.CrossProduct(d3, a);
                //Вычисляем знаки скалярных произведений векторов. Если знаки совпадают, значит точка лежи внутри
                int sign1 = Math.Sign(Vector3D.DotProduct(d1b, d2c));
                int sign2 = Math.Sign(Vector3D.DotProduct(d2c, d3a));
                int sign3 = Math.Sign(Vector3D.DotProduct(d3a, d1b));
                //Знаки всех скалярных произведений должны быть положительны
                return ((sign1 == 1) && (sign2 == 1)) && (sign3 == 1);
            }
        }
    }
}

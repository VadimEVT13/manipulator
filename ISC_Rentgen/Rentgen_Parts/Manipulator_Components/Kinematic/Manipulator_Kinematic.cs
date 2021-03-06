﻿using ISC_Rentgen.Rentgen_Parts.Manipulator_Components.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ISC_Rentgen.Rentgen_Parts.Manipulator_Components.Kinematic
{
    public class Angle3D
    {
        public double O1 { get; set; }
        public double O2 { get; set; }
        public double O3 { get; set; }
        public double O4 { get; set; }
        public double O5 { get; set; }

        public Angle3D()
        {
            this.O1 = 0;
            this.O2 = 0;
            this.O3 = 0;
            this.O4 = 0;
            this.O5 = 0;
        }

        public Angle3D(Angle3D angle)
        {
            this.O1 = angle.O1;
            this.O2 = angle.O2;
            this.O3 = angle.O3;
            this.O4 = angle.O4;
            this.O5 = angle.O5;
        }

        public override string ToString()
        {
            return O1 + " " + O2 + " " + O3 + " " + O4 + " " + O5;
        }
        // Позможно нужно оставить базовую версию переопределения
        public override bool Equals(object obj)
        {
            bool flag =
                (O1 == ((Angle3D)obj).O1) &
                (O2 == ((Angle3D)obj).O2) &
                (O3 == ((Angle3D)obj).O3) &
                (O4 == ((Angle3D)obj).O4) &
                (O5 == ((Angle3D)obj).O5);
            return flag;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class LengthJoin
    {
        public double J1 { get; set; }
        public double J2 { get; set; }
        public double J3 { get; set; }
        public double J4 { get; set; }
        public double J5 { get; set; }
        public double Det { get; set; }
    }

    public class Matrix4D
    {
        public double K11 { get; set; }
        public double K12 { get; set; }
        public double K13 { get; set; }
        public double K14 { get; set; }
        public double K21 { get; set; }
        public double K22 { get; set; }
        public double K23 { get; set; }
        public double K24 { get; set; }
        public double K31 { get; set; }
        public double K32 { get; set; }
        public double K33 { get; set; }
        public double K34 { get; set; }
        public double K41 { get; set; }
        public double K42 { get; set; }
        public double K43 { get; set; }
        public double K44 { get; set; }

        public static Matrix4D M1(double O, double L1)
        {
            Matrix4D m = new Matrix4D();

            m.K11 = Math.Cos(O); m.K12 = -Math.Sin(O); m.K13 = 0; m.K14 = 0;
            m.K21 = Math.Sin(O); m.K22 = Math.Cos(O); m.K23 = 0; m.K24 = 0;
            m.K31 = 0; m.K32 = 0; m.K33 = 1; m.K34 = L1;
            m.K41 = 0; m.K42 = 0; m.K43 = 0; m.K44 = 1;

            return m;
        }

        public static Matrix4D M2(double O, double L2)
        {
            Matrix4D m = new Matrix4D();

            m.K11 = Math.Cos(O); m.K12 = 0; m.K13 = Math.Sin(O); m.K14 = L2 * Math.Sin(O);
            m.K21 = 0; m.K22 = 1; m.K23 = 0; m.K24 = 0;
            m.K31 = -Math.Sin(O); m.K32 = 0; m.K33 = Math.Cos(O); m.K34 = L2 * Math.Cos(O);
            m.K41 = 0; m.K42 = 0; m.K43 = 0; m.K44 = 1;

            return m;
        }

        public static Matrix4D M3(double O, double L3)
        {
            Matrix4D m = new Matrix4D();

            m.K11 = Math.Cos(O); m.K12 = 0; m.K13 = Math.Sin(O); m.K14 = L3 * Math.Cos(O);
            m.K21 = 0; m.K22 = 1; m.K23 = 0; m.K24 = 0;
            m.K31 = -Math.Sin(O); m.K32 = 0; m.K33 = Math.Cos(O); m.K34 = -L3 * Math.Sin(O);
            m.K41 = 0; m.K42 = 0; m.K43 = 0; m.K44 = 1;

            return m;
        }

        public static Matrix4D M4(double O)
        {
            Matrix4D m = new Matrix4D();

            m.K11 = 1; m.K12 = 0; m.K13 = 0; m.K14 = 0;
            m.K21 = 0; m.K22 = Math.Cos(O); m.K23 = -Math.Sin(O); m.K24 = 0;
            m.K31 = 0; m.K32 = Math.Sin(O); m.K33 = Math.Cos(O); m.K34 = 0;
            m.K41 = 0; m.K42 = 0; m.K43 = 0; m.K44 = 1;

            return m;
        }

        public static Matrix4D M5(double O)
        {
            Matrix4D m = new Matrix4D();

            m.K11 = Math.Cos(O); m.K12 = 0; m.K13 = Math.Sin(O); m.K14 = 0;
            m.K21 = 0; m.K22 = 1; m.K23 = 0; m.K24 = 0;
            m.K31 = -Math.Sin(O); m.K32 = 0; m.K33 = Math.Cos(O); m.K34 = 0;
            m.K41 = 0; m.K42 = 0; m.K43 = 0; m.K44 = 1;

            return m;
        }

        public static Matrix4D MX(double X)
        {
            Matrix4D m = new Matrix4D();

            m.K11 = 1; m.K12 = 0; m.K13 = 0; m.K14 = X;
            m.K21 = 0; m.K22 = 1; m.K23 = 0; m.K24 = 0;
            m.K31 = 0; m.K32 = 0; m.K33 = 1; m.K34 = 0;
            m.K41 = 0; m.K42 = 0; m.K43 = 0; m.K44 = 1;

            return m;
        }

        public static Matrix4D MZ(double Z)
        {
            Matrix4D m = new Matrix4D();

            m.K11 = 1; m.K12 = 0; m.K13 = 0; m.K14 = 0;
            m.K21 = 0; m.K22 = 1; m.K23 = 0; m.K24 = 0;
            m.K31 = 0; m.K32 = 0; m.K33 = 1; m.K34 = Z;
            m.K41 = 0; m.K42 = 0; m.K43 = 0; m.K44 = 1;

            return m;
        }

        public static Matrix4D T(double alfa, double beta, Vertex3D P)
        {
            Matrix4D m = new Matrix4D();

            m.K11 = Math.Cos(beta) * Math.Cos(alfa);
            m.K12 = Math.Sin(beta);
            m.K13 = Math.Cos(beta) * Math.Sin(alfa);
            m.K14 = P.X;

            m.K21 = Math.Sin(beta) * Math.Cos(alfa);
            m.K22 = Math.Cos(beta);
            m.K23 = Math.Sin(beta) * Math.Sin(alfa);
            m.K24 = P.Y;

            m.K31 = -Math.Sin(alfa);
            m.K32 = 0;
            m.K33 = Math.Cos(alfa);
            m.K34 = P.Z;

            m.K41 = 0;
            m.K42 = 0;
            m.K43 = 0;
            m.K44 = 1;

            return m;
        }

        public static Matrix4D T(Vertex3D Pn, Vertex3D P4)
        {
            Matrix4D m = new Matrix4D();

            double x = Pn.X - P4.X;
            double y = Pn.Y - P4.Y;
            double z = Pn.Z - P4.Z;

            double b = GetAngle(x, y);
            double a = -GetAngle(Math.Sqrt(x * x + y * y), z);

            m.K11 = Math.Cos(b) * Math.Cos(a);
            m.K12 = Math.Sin(b);
            m.K13 = Math.Cos(b) * Math.Sin(a);
            m.K14 = P4.X;

            m.K21 = Math.Sin(b) * Math.Cos(a);
            m.K22 = Math.Cos(b);
            m.K23 = Math.Sin(b) * Math.Sin(a);
            m.K24 = P4.Y;

            m.K31 = -Math.Sin(a);
            m.K32 = 0;
            m.K33 = Math.Cos(a);
            m.K34 = P4.Z;

            m.K41 = 0;
            m.K42 = 0;
            m.K43 = 0;
            m.K44 = 1;

            return m;
        }

        public static Matrix4D MB(bool flag = true)
        {
            if (flag)
            {
                return new Matrix4D
                {
                    K11 = 1,
                    K12 = 0,
                    K13 = 0,
                    K14 = 0,
                    K21 = 0,
                    K22 = 1,
                    K23 = 0,
                    K24 = 0,
                    K31 = 0,
                    K32 = 0,
                    K33 = 1,
                    K34 = 0,
                    K41 = 0,
                    K42 = 0,
                    K43 = 0,
                    K44 = 1
                };
            }
            else
            {
                return new Matrix4D
                {
                    K11 = -1,
                    K12 = 0,
                    K13 = 0,
                    K14 = 0,
                    K21 = 0,
                    K22 = -1,
                    K23 = 0,
                    K24 = 0,
                    K31 = 0,
                    K32 = 0,
                    K33 = 1,
                    K34 = 0,
                    K41 = 0,
                    K42 = 0,
                    K43 = 0,
                    K44 = 1
                };
            }

        }

        public static Matrix4D Multiply(Matrix4D M1, Matrix4D M2)
        {
            Matrix4D m = new Matrix4D();

            m.K11 = M1.K11 * M2.K11 + M1.K12 * M2.K21 + M1.K13 * M2.K31 + M1.K14 * M2.K41;
            m.K12 = M1.K11 * M2.K12 + M1.K12 * M2.K22 + M1.K13 * M2.K32 + M1.K14 * M2.K42;
            m.K13 = M1.K11 * M2.K13 + M1.K12 * M2.K23 + M1.K13 * M2.K33 + M1.K14 * M2.K43;
            m.K14 = M1.K11 * M2.K14 + M1.K12 * M2.K24 + M1.K13 * M2.K34 + M1.K14 * M2.K44;

            m.K21 = M1.K21 * M2.K11 + M1.K22 * M2.K21 + M1.K23 * M2.K31 + M1.K24 * M2.K41;
            m.K22 = M1.K21 * M2.K12 + M1.K22 * M2.K22 + M1.K23 * M2.K32 + M1.K24 * M2.K42;
            m.K23 = M1.K21 * M2.K13 + M1.K22 * M2.K23 + M1.K23 * M2.K33 + M1.K24 * M2.K43;
            m.K24 = M1.K21 * M2.K14 + M1.K22 * M2.K24 + M1.K23 * M2.K34 + M1.K24 * M2.K44;

            m.K31 = M1.K31 * M2.K11 + M1.K32 * M2.K21 + M1.K33 * M2.K31 + M1.K34 * M2.K41;
            m.K32 = M1.K31 * M2.K12 + M1.K32 * M2.K22 + M1.K33 * M2.K32 + M1.K34 * M2.K42;
            m.K33 = M1.K31 * M2.K13 + M1.K32 * M2.K23 + M1.K33 * M2.K33 + M1.K34 * M2.K43;
            m.K34 = M1.K31 * M2.K14 + M1.K32 * M2.K24 + M1.K33 * M2.K34 + M1.K34 * M2.K44;

            m.K41 = M1.K41 * M2.K11 + M1.K42 * M2.K21 + M1.K43 * M2.K31 + M1.K44 * M2.K41;
            m.K42 = M1.K41 * M2.K12 + M1.K42 * M2.K22 + M1.K43 * M2.K32 + M1.K44 * M2.K42;
            m.K43 = M1.K41 * M2.K13 + M1.K42 * M2.K23 + M1.K43 * M2.K33 + M1.K44 * M2.K43;
            m.K44 = M1.K41 * M2.K14 + M1.K42 * M2.K24 + M1.K43 * M2.K34 + M1.K44 * M2.K44;

            return m;
        }

        private static double GetAngle(double X, double Y)
        {
            if (X == 0 && Y == 0)
            {
                return 0;
            }
            if (X == 0)
            {
                if (Y > 0)
                {
                    return Math.PI / 2;
                }
                else
                {
                    return -Math.PI / 2;
                }
            }
            if (X > 0)
            {
                return Math.Atan(Y / X);
            }
            else
            {
                if (Y >= 0)
                {
                    return Math.Atan(Y / X) + Math.PI;
                }
                else
                {
                    return Math.Atan(Y / X) - Math.PI;
                }
            }
        }
    }

    public class Ogranichenie
    {
        public double O1min { get; set; }
        public double O1max { get; set; }
        public double O2min { get; set; }
        public double O2max { get; set; }
        public double O3min { get; set; }
        public double O3max { get; set; }
        public double O4min { get; set; }
        public double O4max { get; set; }
        public double O5min { get; set; }
        public double O5max { get; set; }

        public static bool IsOK(Ogranichenie ogr, Angle3D ang)
        {
            if (ang.O1 >= ogr.O1min & ang.O1 <= ogr.O1max &
                ang.O2 >= ogr.O2min & ang.O2 <= ogr.O2max &
                ang.O3 >= ogr.O3min & ang.O3 <= ogr.O3max &
                ang.O4 >= ogr.O4min & ang.O4 <= ogr.O4max &
                ang.O5 >= ogr.O5min & ang.O5 <= ogr.O5max)
            {
                return true;
            }

            return false;
        }
    }

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

    public class Node
    {
        public Angle3D Angle { get; set; }
        public double Delta { get; set; }
    }

    public static class Manipulator_Kinematic
    {
        // точка установки манипулятора
        public static Point3D basePoint = new Point3D();

        // длины портала
        private static LengthJoin length = new LengthJoin();

        // углы
        private static Angle3D angles = new Angle3D();

        // ограничение
        private static Ogranichenie ogranich = new Ogranichenie()
        {
            O1min = -Math.PI / 2.0,
            O1max = Math.PI / 2.0,
            O2min = -Math.PI / 2.0,
            O2max = Math.PI / 2.0,
            O3min = -Math.PI / 2.0,
            O3max = Math.PI / 2.0,
            O4min = -Math.PI,
            //O4max = Math.PI - Math.PI / 2,
            O4max = Math.PI,
            O5min = -10 * Math.PI / 180.0,
            O5max = Math.PI
        };

        public static bool SetLen(LengthJoin joins)
        {
            bool result = joins.J1 >= 0 && joins.J2 >= 0 && joins.J3 >= 0 && joins.J4 >= 0 && joins.J5 >= 0;
            if (result)
            {
                length = joins;
            }
            return result;
        }

        public static LengthJoin GetLen()
        {
            return length;
        }

        public static bool SetBase(Point3D Base)
        {
            basePoint = Base;
            return true;
        }

        public static bool SetOgranichenie(Ogranichenie Ogran)
        {
            ogranich = new Ogranichenie()
            {
                O1max = Ogran.O1max,
                O1min = Ogran.O1min,
                O2max = Ogran.O2max,
                O2min = Ogran.O2min,
                O3max = Ogran.O3max,
                O3min = Ogran.O3min,
                O4max = Ogran.O4max,
                O4min = Ogran.O4min,
                O5max = Ogran.O5max,
                O5min = Ogran.O5min
            };

            return true;
        }

        public static Point3D GetBase()
        {
            return basePoint;
        }

        /// <summary>
        /// Перемножение матриц.
        /// </summary>
        /// <param name="matrixA"></param>
        /// <param name="matrixB"></param>
        /// <returns></returns>
        private static double[][] Matrix(double[][] matrixA, double[][] matrixB)
        {
            double[][] rez = new double[4][];
            for (int i = 0; i < 4; i++)
            {
                rez[i] = new double[4];
                for (int j = 0; j < 4; j++)
                {
                    rez[i][j] = 0;
                    for (int k = 0; k < 4; k++)
                    {
                        rez[i][j] = rez[i][j] + matrixA[i][k] * matrixB[k][j];
                    }
                }
            }
            return rez;
        }

        /// <summary>
        /// Задаём матрицы поворота от угла и дистанции
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="r1"></param>
        /// <returns></returns>
        private static double[][] M1(double o1, double r1)
        {
            double[][] m = new double[4][] {
                new double[4] { Math.Cos(o1), -Math.Sin(o1), 0,  0 },
                new double[4] { Math.Sin(o1),  Math.Cos(o1), 0,  0 } ,
                new double[4] {            0,             0, 1, r1 } ,
                new double[4] {            0,             0, 0,  1 } };
            return m;
        }

        /// <summary>
        /// Задаём матрицы поворота от угла и дистанции
        /// </summary>
        /// <param name="o2"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        private static double[][] M2(double o2, double r2)
        {
            double[][] m = new double[4][] {
                new double[4] {  Math.Cos(o2), 0, Math.Sin(o2), r2 * Math.Sin(o2)},
                new double[4] {             0, 1,            0,                 0},
                new double[4] { -Math.Sin(o2), 0, Math.Cos(o2), r2 * Math.Cos(o2)},
                new double[4] {             0, 0,            0,                 1}};
            return m;
        }

        /// <summary>
        /// Задаём матрицы поворота от угла и дистанции
        /// </summary>
        /// <param name="o3"></param>
        /// <param name="r3"></param>
        /// <returns></returns>
        private static double[][] M3(double o3, double r3)
        {
            double[][] m = new double[4][] {
                new double[4] {  Math.Cos(o3), 0, Math.Sin(o3),  r3 * Math.Cos(o3)},
                new double[4] {             0, 1,            0,                  0},
                new double[4] { -Math.Sin(o3), 0, Math.Cos(o3), -r3 * Math.Sin(o3)},
                new double[4] {             0, 0,            0,                  1}};
            return m;
        }

        /// <summary>
        /// Задаём матрицы поворота от угла и дистанции
        /// </summary>
        /// <param name="o4"></param>
        /// <returns></returns>
        private static double[][] M4(double o4)
        {
            double[][] m = new double[4][] {
                new double[4] { 1,            0,             0, 0},
                new double[4] { 0, Math.Cos(o4), -Math.Sin(o4), 0},
                new double[4] { 0, Math.Sin(o4),  Math.Cos(o4), 0},
                new double[4] { 0,            0,             0, 1}};
            return m;
        }

        /// <summary>
        /// Задаём матрицы поворота от угла и дистанции
        /// </summary>
        /// <param name="o5"></param>
        /// <param name="r4"></param>
        /// <returns></returns>
        private static double[][] M5(double o5, double r4)
        {
            double[][] m = new double[4][] {
                new double[4] {  Math.Cos(o5), 0, Math.Sin(o5),  r4 * Math.Cos(o5)},
                new double[4] {             0, 1,            0,                  0},
                new double[4] { -Math.Sin(o5), 0, Math.Cos(o5), -r4 * Math.Sin(o5)},
                new double[4] {             0, 0,            0,                  1}};
            return m;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        static double[][] Mbase(bool flag = true)
        {
            if (flag)
            {
                return new double[4][] {
                    new double[4] { 1, 0, 0, 0 },
                    new double[4] { 0, 1, 0, 0 } ,
                    new double[4] { 0, 0, 1, 0 } ,
                    new double[4] { 0, 0, 0, 1 } };
            }
            else
            {
                return new double[4][] {
                    new double[4] {-1, 0, 0, 0 },
                    new double[4] { 0,-1, 0, 0 } ,
                    new double[4] { 0, 0, 1, 0 } ,
                    new double[4] { 0, 0, 0, 1 } };
            }
        }

        /// <summary>
        /// -пи до +пи
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        private static double GetAngle(double X, double Y)
        {
            if (X == 0 && Y == 0)
            {
                return 0;
            }
            if (X == 0)
            {
                if (Y > 0)
                {
                    return Math.PI / 2;
                }
                else
                {
                    return -Math.PI / 2;
                }
            }
            if (X > 0)
            {
                return Math.Atan(Y / X);
            }
            else
            {
                if (Y >= 0)
                {
                    return Math.Atan(Y / X) + Math.PI;
                }
                else
                {
                    return Math.Atan(Y / X) - Math.PI;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="P3"></param>
        /// <returns></returns>
        private static Vertex3D GP1(Vertex3D P3)
        {
            angles.O1 = GetAngle(P3.X, P3.Y);
            if (angles.O1 > Math.PI / 4)
            {
                angles.O1 = angles.O1 - Math.PI;
            }
            else
                if (angles.O1 < -Math.PI / 4)
            {
                angles.O1 = angles.O1 + Math.PI;
            }

            double[][] m1 = M1(angles.O1, length.J1);
            Vertex3D point = new Vertex3D
            {
                X = m1[0][3],
                Y = m1[1][3],
                Z = m1[2][3]
            };
            return point;
        }

        private static Vertex3D GP2(Vertex3D P3)
        {
            double x = Math.Sqrt(P3.X * P3.X + P3.Y * P3.Y);
            if (P3.X < 0)
            {
                x = -x;
            }
            double z = P3.Z - length.J1;

            double L = Math.Sqrt(P3.X * P3.X + P3.Y * P3.Y + (P3.Z - length.J1) * (P3.Z - length.J1));
            double L3 = Math.Sqrt(length.J3 * length.J3 + length.Det * length.Det);             // Так как появлиось det, то и длинны меняются

            //углы по трём сторонам
            if (L != 0)
            {
                double A = Math.Acos((length.J2 * length.J2 + L * L - L3 * L3) / (2 * length.J2 * L));
                double B = Math.Acos((L3 * L3 + L * L - length.J2 * length.J2) / (2 * L3 * L));
                double G = Math.PI - A - B;

                double o = GetAngle(x, z);
                angles.O2 = Math.PI / 2 - (o + A);

                double[][] R = Matrix(M1(angles.O1, length.J1), M2(angles.O2, length.J2));

                Vertex3D point = new Vertex3D
                {
                    X = R[0][3],
                    Y = R[1][3],
                    Z = R[2][3]
                };

                double t = GetAngle(length.J3, length.Det);
                angles.O3 = Math.PI / 2 - G + GetAngle(length.J3, length.Det);

                R = Matrix(R, M3(angles.O3, length.J3));
                R = Matrix(R, mmove_Z(length.Det));

                return point;
            }
            else
            {
                return null;
            }
        }

        private static void Go4(Vertex3D P4)
        {
            double[][] R2 = Matrix(M1(angles.O1, length.J1), M2(angles.O2, length.J2));
            R2 = Matrix(R2, M3(angles.O3, length.J3));
            R2 = Matrix(R2, mmove_Z(length.Det));

            double newX = R2[0][0] * (P4.X - R2[0][3]) +
                        R2[1][0] * (P4.Y - R2[1][3]) +
                        R2[2][0] * (P4.Z - R2[2][3]);

            double newY = R2[0][1] * (P4.X - R2[0][3]) +
                        R2[1][1] * (P4.Y - R2[1][3]) +
                        R2[2][1] * (P4.Z - R2[2][3]);

            double newZ = R2[0][2] * (P4.X - R2[0][3]) +
                        R2[1][2] * (P4.Y - R2[1][3]) +
                        R2[2][2] * (P4.Z - R2[2][3]);

            angles.O4 = -GetAngle(newZ, newY);                      // Было отрицательным

            R2 = Matrix(R2, M4(angles.O4));
        }

        private static void Go5(Vertex3D P4)
        {
            double[][] R2 = Matrix(M1(angles.O1, length.J1), M2(angles.O2, length.J2));
            R2 = Matrix(R2, M3(angles.O3, length.J3));
            R2 = Matrix(R2, mmove_Z(length.Det));
            R2 = Matrix(R2, M4(angles.O4));

            double newX = R2[0][0] * (P4.X - R2[0][3]) +
                        R2[1][0] * (P4.Y - R2[1][3]) +
                        R2[2][0] * (P4.Z - R2[2][3]);

            double newY = R2[0][1] * (P4.X - R2[0][3]) +
                        R2[1][1] * (P4.Y - R2[1][3]) +
                        R2[2][1] * (P4.Z - R2[2][3]);

            double newZ = R2[0][2] * (P4.X - R2[0][3]) +
                        R2[1][2] * (P4.Y - R2[1][3]) +
                        R2[2][2] * (P4.Z - R2[2][3]);

            angles.O5 = GetAngle(newX, newZ);
        }

        /// <summary>
        /// Геттер углов.
        /// </summary>
        /// <returns>углы</returns>
        public static Angle3D GetAngles()
        {
            return angles;
        }

        private static double[][] newm5(double o5)
        {
            double[][] m = new double[4][] {
                new double[4] { 0, 0, 0, 0 },
                new double[4] { 0, 0, 0, 0 } ,
                new double[4] { 0, 0, 0, 0 } ,
                new double[4] { 0, 0, 0, 0 } };

            m[0][0] = Math.Cos(o5);
            m[0][2] = Math.Sin(o5);
            m[1][1] = 1;
            m[2][0] = -Math.Sin(o5);
            m[2][2] = Math.Cos(o5);
            m[3][3] = 1;

            return m;
        }

        static double[][] mrotate_Z(double o)
        {
            double[][] m = new double[4][] {
                new double[4] { 0, 0, 0, 0 },
                new double[4] { 0, 0, 0, 0 } ,
                new double[4] { 0, 0, 0, 0 } ,
                new double[4] { 0, 0, 0, 0 } };


            m[0][0] = Math.Cos(o);
            m[0][1] = -Math.Sin(o);
            m[1][0] = Math.Sin(o);
            m[1][1] = Math.Cos(o);
            m[2][2] = 1;
            m[3][3] = 1;

            return m;
        }

        static double[][] mrotate_Y(double o)
        {
            double[][] m = new double[4][] {
                new double[4] { 0, 0, 0, 0 },
                new double[4] { 0, 0, 0, 0 } ,
                new double[4] { 0, 0, 0, 0 } ,
                new double[4] { 0, 0, 0, 0 } };

            m[0][0] = Math.Cos(o);
            m[0][2] = Math.Sin(o);
            m[1][1] = 1;
            m[2][0] = -Math.Sin(o);
            m[2][2] = Math.Cos(o);
            m[3][3] = 1;

            return m;
        }

        static double[][] mrotate_X(double o)
        {
            double[][] m = new double[4][] {
                new double[4] { 0, 0, 0, 0 },
                new double[4] { 0, 0, 0, 0 } ,
                new double[4] { 0, 0, 0, 0 } ,
                new double[4] { 0, 0, 0, 0 } };

            m[0][0] = 1;
            m[1][1] = Math.Cos(o);
            m[1][2] = -Math.Sin(o);
            m[2][1] = Math.Sin(o);
            m[2][2] = Math.Cos(o);
            m[3][3] = 1;

            return m;
        }

        static double[][] mmove_X(double x)
        {
            double[][] m = new double[4][] {
                new double[4] { 0, 0, 0, 0 },
                new double[4] { 0, 0, 0, 0 } ,
                new double[4] { 0, 0, 0, 0 } ,
                new double[4] { 0, 0, 0, 0 } };

            m[0][0] = 1;
            m[0][3] = x;
            m[1][1] = 1;
            m[2][2] = 1;
            m[3][3] = 1;

            return m;
        }

        static double[][] mmove_Z(double z)
        {
            double[][] m = new double[4][] {
                new double[4] { 0, 0, 0, 0 },
                new double[4] { 0, 0, 0, 0 } ,
                new double[4] { 0, 0, 0, 0 } ,
                new double[4] { 0, 0, 0, 0 } };

            m[0][0] = 1;
            m[1][1] = 1;
            m[2][2] = 1;
            m[2][3] = z;
            m[3][3] = 1;

            return m;
        }

        // Матрица T или матрица манипулятора
        static double[][] mt(double alfa, double beta, Vertex3D point)
        {
            double[][] m = new double[4][] {
                new double[4] { 0, 0, 0, 0 },
                new double[4] { 0, 0, 0, 0 } ,
                new double[4] { 0, 0, 0, 0 } ,
                new double[4] { 0, 0, 0, 0 } };

            m[0][0] = Math.Cos(beta) * Math.Cos(alfa);
            m[0][1] = Math.Sin(beta);
            m[0][2] = Math.Cos(beta) * Math.Sin(alfa);
            m[0][3] = point.X;

            m[1][0] = Math.Sin(beta) * Math.Cos(alfa);
            m[1][1] = Math.Cos(beta);
            m[1][2] = Math.Sin(beta) * Math.Sin(alfa);
            m[1][3] = point.Y;

            m[2][0] = -Math.Sin(alfa);
            m[2][2] = Math.Cos(alfa);
            m[2][3] = point.Z;

            m[3][3] = 1;

            return m;
        }

        static private double[] GetAandB(Vertex3D P4, Vertex3D Pnab)
        {
            double x = Pnab.X - P4.X;
            double y = Pnab.Y - P4.Y;
            double z = Pnab.Z - P4.Z;

            double b = GetAngle(x, y);
            double a = GetAngle(Math.Sqrt(x * x + y * y), z);

            return new double[2] { a, b };
        }

        static public double[][] DirectKinematic(Angle3D angles)
        {
            double[][] R = Matrix(M1(angles.O1, length.J1), M2(angles.O2, length.J2));
            R = Matrix(R, M3(angles.O3, length.J3));
            R = Matrix(R, mmove_Z(length.Det));
            R = Matrix(R, M4(angles.O4));
            R = Matrix(R, newm5(angles.O5));
            R = Matrix(R, mmove_Z(length.J4));
            R = Matrix(R, mmove_X(length.J5));
            return R;
        }

        static private Vertex3D GetP3(double i, Vertex3D P4, out Vertex3D P34, double alf, double bet)
        {
            double[][] R = Matrix(mrotate_Z(bet), mrotate_Y(alf));
            R[0][3] = P4.X;
            R[1][3] = P4.Y;
            R[2][3] = P4.Z;

            R = Matrix(R, mmove_X(-length.J5));
            P34 = new Vertex3D
            {
                X = R[0][3],
                Y = R[1][3],
                Z = R[2][3]
            };

            double[][] Rt = Matrix(R, mrotate_X(i));
            Rt = Matrix(Rt, mmove_Z(-length.J4));

            Vertex3D point = new Vertex3D
            {
                X = Rt[0][3],
                Y = Rt[1][3],
                Z = Rt[2][3]
            };
            return point;
        }

        static private Vertex3D Modiffy(Vertex3D Point, Point3D basePoint)
        {
            // Положение манипулятора при работе системы Манипулятор-Портал
            // В отрицательной зоне параметр true                           
            double[][] mat = Mbase();

            double x_ = mat[0][0] * (Point.X - basePoint.X) + mat[1][0] * (Point.Y - basePoint.Y) + mat[2][0] * (Point.Z - basePoint.Z);
            double y_ = mat[0][1] * (Point.X - basePoint.X) + mat[1][1] * (Point.Y - basePoint.Y) + mat[2][1] * (Point.Z - basePoint.Z);
            double z_ = mat[0][2] * (Point.X - basePoint.X) + mat[1][2] * (Point.Y - basePoint.Y) + mat[2][2] * (Point.Z - basePoint.Z);

            return new Vertex3D
            {
                X = x_,
                Y = y_,
                Z = z_
            };
        }

        /// <summary>
        /// Решение обратной кинематики для неого угла iter от -pi до pi
        /// </summary>
        /// <param name="iter"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        /// <param name="X2"></param>
        /// <param name="Y2"></param>
        /// <param name="Z2"></param>
        /// <returns></returns>
        static private Angle3D InverseNab(double iter, double X, double Y, double Z, double X2, double Y2, double Z2)
        {
            Stack<Angle3D> rezultAngles = new Stack<Angle3D>();

            // Точка в новой системе координат
            Vertex3D P4 = Modiffy(new Vertex3D { X = X, Y = Y, Z = Z }, basePoint);
            Vertex3D Pn = Modiffy(new Vertex3D { X = X2, Y = Y2, Z = Z2 }, basePoint);

            // Получение углов альфа и бета
            double[] aandb = GetAandB(P4, Pn);
            double alf = -aandb[0];
            double bet = aandb[1];

            double[][] T = mt(alf, bet, P4);                        // Определение матрицы манипулятора

            Vertex3D P34 = new Vertex3D();

            //----------------------------------------------------------------------------------------------------------------------
            Vertex3D P3 = GetP3(iter, P4, out P34, alf, bet); // Получение множества точек P3

            // Ниже идёт проверка на достижимость до точки P3 манипулятором                
            if (Math.Sqrt(P3.X * P3.X + P3.Y * P3.Y + (P3.Z - length.J1) * (P3.Z - length.J1)) <= length.J2 + length.J3)
            {
                Vertex3D P1 = GP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                Vertex3D P2 = GP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                // Ниже условие, если получили точку то продолжаем 
                if (P2 != null)
                {
                    Go4(P34);
                    Go5(P34);

                    double[] angle4 = { angles.O4, 0 };
                    double[] angle5 = { -angles.O5 + Math.PI / 2, 0 };
                    // ----------------------------------------------------------
                    if (angles.O4 > 0)
                        angles.O4 = angles.O4 - Math.PI;
                    else
                    {
                        angles.O4 = angles.O4 + Math.PI;
                    }
                    angle4[1] = angles.O4;
                    Go5(P34);
                    angle5[1] = -angles.O5 + Math.PI / 2;

                    // Ниже выполненна прямая кинематика манипулятора

                    angles.O4 = angle4[0];
                    angles.O5 = angle5[0];

                    double[][] R = DirectKinematic(angles);
                    double dist1 = Vertex3D.Distance(P4, new Vertex3D { X = R[0][3], Y = R[1][3], Z = R[2][3] }) + Vertex3D.Distance(new Vertex3D() { X = T[0][0], Y = T[1][0], Z = T[2][0] }, new Vertex3D() { X = R[0][0], Y = R[1][0], Z = R[2][0] }) * 100;

                    angles.O4 = angle4[1];
                    angles.O5 = angle5[1];

                    R = DirectKinematic(angles);
                    double dist2 = Vertex3D.Distance(P4, new Vertex3D { X = R[0][3], Y = R[1][3], Z = R[2][3] }) + Vertex3D.Distance(new Vertex3D() { X = T[0][0], Y = T[1][0], Z = T[2][0] }, new Vertex3D() { X = R[0][0], Y = R[1][0], Z = R[2][0] }) * 100;


                    if (dist1 < dist2)
                        return new Angle3D
                        {
                            O1 = angles.O1,
                            O2 = angles.O2,
                            O3 = angles.O3,
                            O4 = angle4[0],
                            O5 = angle5[0]
                        };
                    else
                        return new Angle3D
                        {
                            O1 = angles.O1,
                            O2 = angles.O2,
                            O3 = angles.O3,
                            O4 = angle4[1],
                            O5 = angle5[1]
                        };
                }
            }

            return new Angle3D();
        }

        static private Angle3D Golden_section_search(double A, double B, double eps, Vertex3D Pnab, Vertex3D Pman)
        {
            if (B < A)
                throw new Exception("0x0001, Грань B должна быть больше чем A");

            Vertex3D pEx = new Vertex3D();
            double[][] Mat;
            double x1 = 0;
            double f1 = 0;
            double x2 = 0;
            double f2 = 0;
            double a = A;
            double b = B;
            double phi = (1 + Math.Sqrt(5)) / 2.0;

            // ------- Начало кода
            x1 = b - (b - a) / phi;
            x2 = a + (b - a) / phi;

            while (true)
            {
                if (Math.Abs(b - a) < eps)
                {
                    double rez = (a + b) / 2.0;
                    return InverseNab(rez, Pman.X, Pman.Y, Pman.Z, Pnab.X, Pnab.Y, Pnab.Z);
                }

                x1 = b - (b - a) / phi;
                x2 = a + (b - a) / phi;

                Mat = DirectKinematic(InverseNab(x1, Pman.X, Pman.Y, Pman.Z, Pnab.X, Pnab.Y, Pnab.Z), basePoint);
                f1 = Vertex3D.Distance(Pman, new Vertex3D { X = Mat[0][3], Y = Mat[1][3], Z = Mat[2][3] });
                Mat = DirectKinematic(InverseNab(x2, Pman.X, Pman.Y, Pman.Z, Pnab.X, Pnab.Y, Pnab.Z), basePoint);
                f2 = Vertex3D.Distance(Pman, new Vertex3D { X = Mat[0][3], Y = Mat[1][3], Z = Mat[2][3] });

                if (f1 >= f2)
                    a = x1;
                else
                    b = x2;
            }
        }

        private static Angle3D Golden_section_search(int[] Command, double A, double B, Angle3D Angles, double[][] T)
        {
            Vertex3D Pman = new Vertex3D()
            {
                X = T[0][3],
                Y = T[1][3],
                Z = T[2][3]
            };
            double eps = 0.0000000000001;

            if (B < A)
                throw new Exception("0x0001, Грань B должна быть больше чем A");

            Vertex3D pEx = new Vertex3D();
            double[][] Mat;
            double x1 = 0;
            double f1 = 0;
            double x2 = 0;
            double f2 = 0;
            double a = A;
            double b = B;
            double phi = (1 + Math.Sqrt(5)) / 2.0;

            // ------- Начало кода
            x1 = b - (b - a) / phi;
            x2 = a + (b - a) / phi;

            while (true)
            {
                if (Math.Abs(b - a) < eps)
                {
                    double rez = (a + b) / 2.0;
                    return new Angle3D()
                    {
                        O1 = Angles.O1 * (1 - Command[0]) + Command[0] * rez,
                        O2 = Angles.O2 * (1 - Command[1]) + Command[1] * rez,
                        O3 = Angles.O3 * (1 - Command[2]) + Command[2] * rez,
                        O4 = Angles.O4 * (1 - Command[3]) + Command[3] * rez,
                        O5 = Angles.O5 * (1 - Command[4]) + Command[4] * rez
                    };
                }

                x1 = b - (b - a) / phi;
                x2 = a + (b - a) / phi;

                Mat = DirectKinematic(new Angle3D()
                {
                    O1 = Angles.O1 * (1 - Command[0]) + Command[0] * x1,
                    O2 = Angles.O2 * (1 - Command[1]) + Command[1] * x1,
                    O3 = Angles.O3 * (1 - Command[2]) + Command[2] * x1,
                    O4 = Angles.O4 * (1 - Command[3]) + Command[3] * x1,
                    O5 = Angles.O5 * (1 - Command[4]) + Command[4] * x1
                }, basePoint);
                if (Command[3] == 1 || Command[4] == 1)
                {
                    f1 = Vertex3D.Distance(new Vertex3D { X = T[0][0], Y = T[1][0], Z = T[2][0] },
                        new Vertex3D { X = Mat[0][0], Y = Mat[1][0], Z = Mat[2][0] });
                }
                else
                {
                    f1 = Vertex3D.Distance(Pman, new Vertex3D { X = Mat[0][3], Y = Mat[1][3], Z = Mat[2][3] });
                }
                Mat = DirectKinematic(new Angle3D()
                {
                    O1 = Angles.O1 * (1 - Command[0]) + Command[0] * x2,
                    O2 = Angles.O2 * (1 - Command[1]) + Command[1] * x2,
                    O3 = Angles.O3 * (1 - Command[2]) + Command[2] * x2,
                    O4 = Angles.O4 * (1 - Command[3]) + Command[3] * x2,
                    O5 = Angles.O5 * (1 - Command[4]) + Command[4] * x2
                }, basePoint);

                if (Command[3] == 1 || Command[4] == 1)
                {
                    f2 = Vertex3D.Distance(new Vertex3D { X = T[0][0], Y = T[1][0], Z = T[2][0] },
                        new Vertex3D { X = Mat[0][0], Y = Mat[1][0], Z = Mat[2][0] });
                }
                else
                {
                    f2 = Vertex3D.Distance(Pman, new Vertex3D { X = Mat[0][3], Y = Mat[1][3], Z = Mat[2][3] });
                }

                if (f1 >= f2)
                    a = x1;
                else
                    b = x2;
            }
        }

        public static Angle3D CCD(Vertex3D P4, Vertex3D Pn)
        {
            double eps = 0.000000001;
            double N = 50;
            Node rezult_node = null;
            double[][] T;
            double[][] R;

            double[] aandb = GetAandB(P4, Pn);
            double alf = -aandb[0];
            double bet = aandb[1];

            T = mt(alf, bet, P4);

            for (int i = 0; i < N; i++)
            {
                double delta = double.MaxValue / 2;
                Random r = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
                angles = new Angle3D()
                {
                    O1 = r.NextDouble() * (ogranich.O1max - ogranich.O1min) + ogranich.O1min,
                    O2 = r.NextDouble() * (ogranich.O2max - ogranich.O2min) + ogranich.O2min,
                    O3 = r.NextDouble() * (ogranich.O3max - ogranich.O3min) + ogranich.O3min,
                    O4 = r.NextDouble() * (ogranich.O4max - ogranich.O4min) + ogranich.O4min,
                    O5 = r.NextDouble() * (ogranich.O5max - ogranich.O5min) + ogranich.O5min
                };

                double delta_prev = double.MaxValue;

                while (delta > eps & delta_prev > delta)
                {
                    angles = Golden_section_search(new int[] { 1, 0, 0, 0, 0 }, ogranich.O1min, ogranich.O1max, angles, T);
                    angles = Golden_section_search(new int[] { 0, 1, 0, 0, 0 }, ogranich.O2min, ogranich.O2max, angles, T);
                    angles = Golden_section_search(new int[] { 0, 0, 1, 0, 0 }, ogranich.O3min, ogranich.O3max, angles, T);
                    angles = Golden_section_search(new int[] { 0, 0, 0, 1, 0 }, ogranich.O4min, ogranich.O4max, angles, T);
                    angles = Golden_section_search(new int[] { 0, 0, 0, 0, 1 }, ogranich.O5min, ogranich.O5max, angles, T);

                    R = DirectKinematic(angles, basePoint);
                    double d1 = Vertex3D.Distance(P4, new Vertex3D() { X = R[0][3], Y = R[1][3], Z = R[2][3] });

                    angles = Golden_section_search(new int[] { 1, 0, 0, 0, 0 }, ogranich.O1min, ogranich.O1max, angles, T);
                    angles = Golden_section_search(new int[] { 0, 1, 0, 0, 0 }, ogranich.O2min, ogranich.O2max, angles, T);
                    angles = Golden_section_search(new int[] { 0, 0, 1, 0, 0 }, ogranich.O3min, ogranich.O3max, angles, T);
                    angles = Golden_section_search(new int[] { 0, 0, 0, 1, 0 }, ogranich.O4min, ogranich.O4max, angles, T);
                    angles = Golden_section_search(new int[] { 0, 0, 0, 0, 1 }, ogranich.O5min, ogranich.O5max, angles, T);

                    R = DirectKinematic(angles, basePoint);
                    double d2 = Vertex3D.Distance(P4, new Vertex3D() { X = R[0][3], Y = R[1][3], Z = R[2][3] });

                    delta_prev = delta;
                    delta = Math.Abs(d1 - d2);
                }

                R = DirectKinematic(angles, basePoint);
                if (rezult_node == null)
                {
                    rezult_node = new Node() { Angle = angles, Delta = Vertex3D.Distance(P4, new Vertex3D() { X = R[0][3], Y = R[1][3], Z = R[2][3] }) };
                }
                else
                {
                    Node temp_node = new Node() { Angle = angles, Delta = Vertex3D.Distance(P4, new Vertex3D() { X = R[0][3], Y = R[1][3], Z = R[2][3] }) };

                    if (temp_node.Delta < rezult_node.Delta)
                    {
                        rezult_node = temp_node;
                    }
                }
                if (rezult_node.Delta < 0.1)
                    return rezult_node.Angle;
            }

            return rezult_node.Angle;
        }

        /*
        public Stack<Angle3D> InverseNab(Vertex3D Pnab, Vertex3D P4)
        {
            // Инициализация

            // Задачи поиска интервалов:
            Stack<double[]> angle_P34 = new Stack<double[]>();
            double[] container = new double[2];                     // Контейнер для значений интерала
            double step = (Math.PI * 2) / 20;                       // Шаг движения по траектории
            Matrix4D R;                                           // Матрица R
            double prevPoint;                                       // Предыдущая точка
            double prevDist;                                        // Значение предыдущей точки
            double curPoint;                                        // Текущая точка
            double curDist;                                         // Значение текущей точки
            bool flag = true;                                       // Флаг блокировки при спуске

            // Задача поиска на интервале 
            Stack<Angle3D> rez = new Stack<Angle3D>();              // Все решения
            double eps = 0.00000001;                                // Точность

            // Поиск интервалов с впадиной             

            container[0] = -Math.PI;

            for (double i = -Math.PI + step; i <= Math.PI; i += step)
            {
                prevPoint = i - step;
                R = Kinematic.DirectKinematic(InverseNab(prevPoint, P4.X, P4.Y, P4.Z, Pnab.X, Pnab.Y, Pnab.Z), length);
                prevDist = Vertex3D.Distance(P4, new Vertex3D { X = R.K13, Y = R.K23, Z = R.K33 });

                curPoint = i;
                R = Kinematic.DirectKinematic(InverseNab(prevPoint, P4.X, P4.Y, P4.Z, Pnab.X, Pnab.Y, Pnab.Z), length);
                curDist = Vertex3D.Distance(P4, new Vertex3D { X = R.K13, Y = R.K23, Z = R.K33 });
                
                if (prevDist > curDist & flag == true)
                {
                    // запись интервала
                    container[1] = curPoint;
                    angle_P34.Push(container);

                    // новый интервал
                    container[0] = curPoint;

                    flag = false;
                }

                if (prevDist < curDist)
                    flag = true;

                if (i == Math.PI)
                {
                    // запись интервала и выход
                    container[1] = curPoint;
                    angle_P34.Push(container);
                }
            }

            // Поиск методом золотого сечения на интервалах
            foreach (double[] one in angle_P34)
            {
                rez.Push(Golden_section_search(one[0], one[1], eps, Pnab, P4));                            
            }

            return rez;
        }

            /// <summary>
        /// Добавил для адаптации!!!
        /// Пока основная функция поиска!!!
        /// То есть поправки есть!!!
        /// </summary>
        /// <param name="Pnab"></param>
        /// <param name="P4"></param>
        /// <returns></returns>
        */
        /// <summary>
        /// Основная функция
        /// </summary>
        /// <param name="X1">Схват</param>
        /// <param name="Y1">Схват</param>
        /// <param name="Z1">Схват</param>
        /// <param name="X2">Объект</param>
        /// <param name="Y2">Объект</param>
        /// <param name="Z2">Объект</param>
        /// <returns></returns>
        public static Stack<Angle3D> InverseNab(double X1, double Y1, double Z1, double X2, double Y2, double Z2)
        {
            // Точка в новой системе координат
            Vertex3D P4 = new Vertex3D
            {
                X = X1,
                Y = Y1,
                Z = Z1
            };

            Vertex3D Pn = new Vertex3D
            {
                X = X2,
                Y = Y2,
                Z = Z2
            };

            // Инициализация

            // Задачи поиска интервалов:
            Stack<double[]> angle_P34 = new Stack<double[]>();
            double[] container = new double[2];                     // Контейнер для значений интерала
            double step = (Math.PI * 2) / 75.0;                     // Шаг движения по траектории
            double[][] R;                                           // Матрица R
            double prevPoint;                                       // Предыдущая точка
            double prevDist;                                        // Значение предыдущей точки
            double curPoint;                                        // Текущая точка
            double curDist;                                         // Значение текущей точки
            bool flag = true;                                       // Флаг блокировки при спуске

            // Задача поиска на интервале 
            Stack<Angle3D> rez = new Stack<Angle3D>();              // Все решения
            double eps = 0.000000000001;                                // Точность

            // Поиск интервалов с впадиной             

            container[0] = -Math.PI;

            for (double i = -Math.PI + step; i <= Math.PI; i += step)
            {
                prevPoint = i - step;
                R = DirectKinematic(InverseNab(prevPoint, P4.X, P4.Y, P4.Z, Pn.X, Pn.Y, Pn.Z), basePoint);
                prevDist = Vertex3D.Distance(P4, new Vertex3D { X = R[0][3], Y = R[1][3], Z = R[2][3] });

                curPoint = i;
                R = DirectKinematic(InverseNab(curPoint, P4.X, P4.Y, P4.Z, Pn.X, Pn.Y, Pn.Z), basePoint);
                curDist = Vertex3D.Distance(P4, new Vertex3D { X = R[0][3], Y = R[1][3], Z = R[2][3] });

                if (prevDist > curDist & flag == true)
                {
                    // запись интервала
                    container[1] = curPoint;
                    angle_P34.Push(new double[2] { container[0], container[1] });

                    // новый интервал
                    container[0] = curPoint;

                    flag = false;
                }

                if (prevDist < curDist)
                    flag = true;

                if (i + step > Math.PI)
                {
                    // запись интервала и выход
                    container[1] = Math.PI;
                    angle_P34.Push(container);
                }
            }

            // Поиск методом золотого сечения на интервалах
            foreach (double[] one in angle_P34)
            {
                //rez.Push(Golden_section_search(one[0], one[1], eps, Pn, P4));
                Angle3D ang_temp = Golden_section_search(one[0], one[1], eps, Pn, P4);
                R = DirectKinematic(ang_temp, basePoint);

                Matrix4D T = Matrix4D.T(Pn, P4);

                if (Vertex3D.Distance(P4, new Vertex3D { X = R[0][3], Y = R[1][3], Z = R[2][3] }) <= 0.01 &
                    Vertex3D.Distance(new Vertex3D { X = R[0][0], Y = R[1][0], Z = R[2][0] },
                    new Vertex3D { X = T.K11, Y = T.K21, Z = T.K31 }) <= 0.01)
                {
                    rez.Push(ang_temp);
                }
            }

            Stack<Angle3D> satisfied = new Stack<Angle3D>();

            // Проверка на ограничение
            foreach (Angle3D one in rez)
            {
                if (Ogranichenie.IsOK(ogranich, one))
                {
                    satisfied.Push(one);
                }
            }

            // Если нет решения
            if (satisfied.Count == 0)
            {
                rez = new Stack<Angle3D>();
                step = (Math.PI * 2) / 400;
                angle_P34 = new Stack<double[]>();

                // Поиск интервалов с впадиной             

                container[0] = -Math.PI;

                for (double i = -Math.PI + step; i <= Math.PI; i += step)
                {
                    prevPoint = i - step;
                    R = DirectKinematic(InverseNab(prevPoint, P4.X, P4.Y, P4.Z, Pn.X, Pn.Y, Pn.Z), basePoint);
                    prevDist = Vertex3D.Distance(P4, new Vertex3D { X = R[0][3], Y = R[1][3], Z = R[2][3] });

                    curPoint = i;
                    R = DirectKinematic(InverseNab(curPoint, P4.X, P4.Y, P4.Z, Pn.X, Pn.Y, Pn.Z), basePoint);
                    curDist = Vertex3D.Distance(P4, new Vertex3D { X = R[0][3], Y = R[1][3], Z = R[2][3] });

                    if (prevDist > curDist & flag == true)
                    {
                        // запись интервала
                        container[1] = curPoint;
                        angle_P34.Push(new double[2] { container[0], container[1] });

                        // новый интервал
                        container[0] = curPoint;

                        flag = false;
                    }

                    if (prevDist < curDist)
                        flag = true;

                    if (i + step > Math.PI)
                    {
                        // запись интервала и выход
                        container[1] = Math.PI;
                        angle_P34.Push(container);
                    }
                }

                // Поиск методом золотого сечения на интервалах
                foreach (double[] one in angle_P34)
                {
                    //rez.Push(Golden_section_search(one[0], one[1], eps, Pn, P4));
                    Angle3D ang_temp = Golden_section_search(one[0], one[1], eps, Pn, P4);
                    R = DirectKinematic(ang_temp, basePoint);

                    Matrix4D T = Matrix4D.T(Pn, P4);

                    if (Vertex3D.Distance(P4, new Vertex3D { X = R[0][3], Y = R[1][3], Z = R[2][3] }) <= 0.01 &
                        Vertex3D.Distance(new Vertex3D { X = R[0][0], Y = R[1][0], Z = R[2][0] },
                        new Vertex3D { X = T.K11, Y = T.K21, Z = T.K31 }) <= 0.01)
                    {
                        // Проверка на ограничение
                        if (Ogranichenie.IsOK(ogranich, ang_temp))
                        {
                            rez.Push(ang_temp);
                        }
                    }
                }
            }
            else
                return satisfied;

            return rez;
        }

        #region ForFindWay

        public static List<double[][]> GetNormalMatrix(Angle3D angles, Vertex3D basepoint, LengthJoin lj)
        {
            List<double[][]> rezult = new List<double[][]>();

            double[][] example = new double[][] {
                new double[] { },
                new double[] { },
                new double[] { },
                new double[] { }
            };

            double[][] Mat0_rotate = new double[][] {
                new double[] { 0, 1, 0, basepoint.X },
                new double[] { 1, 0, 0, basepoint.Y },
                new double[] { 0, 0, 1, basepoint.Z },
                new double[] { 0, 0, 0, 1}
            };

            double[][] Mat0 = new double[][] {
                new double[] { Math.Cos(angles.O1),     -Math.Sin(angles.O1),       0, 0 },
                new double[] { Math.Sin(angles.O1),     Math.Cos(angles.O1),        0, 0 },
                new double[] { 0,                       0,                          1, 0 },
                new double[] { 0,                       0,                          0, 1 } };

            double[][] Mat1_perem = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, 0 },
                new double[] { 0, 0, 1, lj.J1 },
                new double[] { 0, 0, 0, 1 }
            };

            double[][] Mat1_rotate = new double[][] {
                new double[] { 1,  0, 0,  0 },
                new double[] { 0,  1, 0,  0 },
                new double[] { 0,  0, 1,  0 },
                new double[] { 0,  0, 0,  1 }
            };

            double[][] Mat1 = new double[][] {
                new double[] { 1, 0,                        0,                      0 },
                new double[] { 0, Math.Cos(angles.O2),      -Math.Sin(angles.O2),   0 },
                new double[] { 0, Math.Sin(angles.O2),      Math.Cos(angles.O2),    0 },
                new double[] { 0, 0,                        0,                      1 }
            };

            double[][] Mat2_perem = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, 0 },
                new double[] { 0, 0, 1, lj.J2 },
                new double[] { 0, 0, 0, 1 }
            };

            double[][] Mat2_rotate = new double[][] {
                new double[] { 1,  0, 0, 0 },
                new double[] { 0,  0, 1, 0 },
                new double[] { 0, -1, 0, 0 },
                new double[] { 0,  0, 0, 1 }
            };

            double[][] Mat2 = new double[][] {
                new double[] { 1, 0,                        0,                      0 },
                new double[] { 0, Math.Cos(angles.O3),      -Math.Sin(angles.O3),    0 },
                new double[] { 0, Math.Sin(angles.O3),      Math.Cos(angles.O3),    0 },
                new double[] { 0, 0,                        0,                      1 }
            };


            double[][] Mat3_perem_y = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, -lj.Det },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };

            double[][] Mat3_perem_z = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, 0 },
                new double[] { 0, 0, 1, lj.J3 },
                new double[] { 0, 0, 0, 1 }
            };

            double[][] Mat3 = new double[][] {
                new double[] { Math.Cos(angles.O4), -Math.Sin(angles.O4),   0, 0 },
                new double[] { Math.Sin(angles.O4), Math.Cos(angles.O4),    0, 0 },
                new double[] { 0,                   0,                      1, 0 },
                new double[] { 0,                   0,                      0, 1 }
            };

            double[][] Mat4_rotate = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, 0 },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };

            double[][] Mat4 = new double[][] {
                new double[] { 1, 0,                        0,                      0 },
                new double[] { 0, Math.Cos(angles.O5),      -Math.Sin(angles.O5),   0 },
                new double[] { 0, Math.Sin(angles.O5),      Math.Cos(angles.O5),    0 },
                new double[] { 0, 0,                        0,                      1 }
            };

            // повернули матрицу 0 как надо для матрицы 1
            double[][] R = Matrix(Mat0_rotate, Mat0);

            rezult.Add(R);                      // добавили самую первую матрицу

            R = Matrix(R, Mat1_perem);          // переместили локальные координаты к 2 узлу
            R = Matrix(R, Mat1_rotate);         // изменили вектора на 2 узле
            R = Matrix(R, Mat1);                // повернули 2 узел
            rezult.Add(R);                      // добавили 2 матрицу

            R = Matrix(R, Mat2_perem);          // переместили локальные координаты к 3 узлу
            R = Matrix(R, Mat2_rotate);         // переместили локальные координаты к 3 узлу
            R = Matrix(R, Mat2);                // повернули 3 узел
            rezult.Add(R);                      // добавили 3 матрицу

            R = Matrix(R, Mat3_perem_y);        // переместили локальные координаты к 4 узлу
            R = Matrix(R, Mat3_perem_z);
            R = Matrix(R, Mat3);                // повернули 4 узел
            rezult.Add(R);                      // добавили 4 матрицу

            R = Matrix(R, Mat4_rotate);         // переместили локальные координаты к 5 узлу
            R = Matrix(R, Mat4);                // повернули 5 узел
            rezult.Add(R);                      // добавили 5 матрицу

            return rezult;
        }

        #endregion

        public static double[][] DirectKinematic(Angle3D angles, Point3D basepoint)
        {
            double[][] mat = Mbase();

            double[][] R = Matrix(M1(angles.O1, length.J1), M2(angles.O2, length.J2));
            R = Matrix(R, M3(angles.O3, length.J3));
            R = Matrix(R, mmove_Z(length.Det));
            R = Matrix(R, M4(angles.O4));
            R = Matrix(R, newm5(angles.O5));
            R = Matrix(R, mmove_Z(length.J4));
            R = Matrix(R, mmove_X(length.J5));

            R[0][3] = mat[0][0] * (R[0][3] + basepoint.X) +
                      mat[1][0] * (R[1][3] + basepoint.Y) +
                      mat[2][0] * (R[2][3] + basepoint.Z);

            R[1][3] = mat[0][1] * (R[0][3] + basepoint.X) +
                      mat[1][1] * (R[1][3] + basepoint.Y) +
                      mat[2][1] * (R[2][3] + basepoint.Z);

            R[2][3] = mat[0][2] * (R[0][3] + basepoint.X) +
                      mat[1][2] * (R[1][3] + basepoint.Y) +
                      mat[2][2] * (R[2][3] + basepoint.Z);

            return R;
        }

        public static Matrix4D DirectKinematic(Angle3D angles, LengthJoin length)
        {
            Matrix4D R = Matrix4D.Multiply(Matrix4D.M1(angles.O1, length.J1), Matrix4D.M2(angles.O2, length.J2));
            R = Matrix4D.Multiply(R, Matrix4D.M3(angles.O3, length.J3));
            R = Matrix4D.Multiply(R, Matrix4D.MZ(length.Det));
            R = Matrix4D.Multiply(R, Matrix4D.M4(angles.O4));
            R = Matrix4D.Multiply(R, Matrix4D.M5(angles.O5));
            R = Matrix4D.Multiply(R, Matrix4D.MZ(length.J4));
            R = Matrix4D.Multiply(R, Matrix4D.MX(length.J5));
            return R;
        }

        public static List<Matrix3D> Direct_Kinematic(Point3D Base_Point, Join_Length_Manipulator Length, Angles_Manipulator Angles)
        {
            List<Matrix3D> Rezult = new List<Matrix3D>();

            Matrix3D Base = new Matrix3D() { M14 = Base_Point.X, M24 = Base_Point.Y, M34 = Base_Point.Z };

            Matrix3D Rotate1 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), -Angles.O1)).Value;
            Matrix3D Rotate2 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), -Angles.O2)).Value;
            Matrix3D Rotate3 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), -Angles.O3)).Value;
            Matrix3D Rotate4 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), -Angles.O4)).Value;
            Matrix3D Rotate5 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), -Angles.O5)).Value;

            Matrix3D Join1 = Matrix3D.Multiply(Rotate1, new Matrix3D() { M34 = Length.L1 });
            Matrix3D Join2 = Matrix3D.Multiply(Rotate2, new Matrix3D() { M34 = Length.L2 });
            Matrix3D Join3 = Matrix3D.Multiply(Rotate3, new Matrix3D() { M14 = Length.L3, M34 = Length.Det });
            Matrix3D Join4 = Matrix3D.Multiply(Rotate5, new Matrix3D() { M14 = Length.L5, M34 = Length.L4 });

            Rezult.Add(Matrix3D.Multiply(Base,      Join1));
            Rezult.Add(Matrix3D.Multiply(Rezult[0], Join2));
            //Matrix3D t = Matrix3D.Multiply(Rezult[1], new Matrix3D() { M34 = Length.Det });
            Rezult.Add(Matrix3D.Multiply(Rezult[1], Join3));
            Matrix3D t2 = Matrix3D.Multiply(Rezult[2], Rotate4);
            Rezult.Add(Matrix3D.Multiply(t2, Join4));

            return Rezult;
        }

        public static void map(Vertex3D P4, Vertex3D Pn)
        {
            double step = (Math.PI * 2) / 5000.0;
            double[][] R;
            double prevDist;

            string[] m1 = new string[15];
            m1[0] = "i = [";
            m1[1] = "z = [";
            m1[2] = "i2 = [";
            m1[3] = "z2 = [";

            for (double i = -Math.PI; i <= Math.PI; i += step)
            {
                Angle3D tempAngle = InverseNab(i, P4.X, P4.Y, P4.Z, Pn.X, Pn.Y, Pn.Z);
                R = DirectKinematic(tempAngle, basePoint);
                prevDist = Vertex3D.Distance(P4, new Vertex3D { X = R[0][3], Y = R[1][3], Z = R[2][3] });

                if (Ogranichenie.IsOK(ogranich, tempAngle) == true)
                {
                    m1[0] += string.Format("{0} ", i);
                    m1[1] += string.Format("{0} ", prevDist);
                }
                else
                {
                    m1[2] += string.Format("{0} ", i);
                    m1[3] += string.Format("{0} ", prevDist);
                }
            }

            m1[0] += "];";
            m1[1] += "];";
            m1[2] += "];";
            m1[3] += "];";

            m1[0] = m1[0].Replace(',', '.');
            m1[1] = m1[1].Replace(',', '.');
            m1[2] = m1[2].Replace(',', '.');
            m1[3] = m1[3].Replace(',', '.');

            m1[6] += "plot(i, z, 'og'); xgrid();";
            m1[7] += "plot(i2, z2, 'or'); xgrid();";

            System.IO.File.WriteAllLines("map.txt", m1);
        }

        public static Ogranichenie GetOgranichenie()
        {
            return ogranich;
        }
    }
}

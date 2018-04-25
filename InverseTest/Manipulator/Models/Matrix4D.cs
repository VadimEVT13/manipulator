using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Manipulator.Models
{
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
}

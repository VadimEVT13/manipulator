using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Manipulator
{
    public class Kinematic
    {
        // точка установки портала
        double[] base_point = { 0, 0, 0 };

        // длины портала
        double l1 = 0;
        double l2 = 0;
        double l3 = 0;
        double l4 = 0;

        // углы
        double o1 = 0;
        double o2 = 0;
        double o3 = 0;
        double o4 = 0;
        double o5 = 0;
        double o6 = 0;
        double a = 0;
        double b = 0;
        double g = 0;


        public Kinematic(double X = 0, double Y = 0, double Z = 0)
        {
            base_point[0] = X;
            base_point[1] = Y;
            base_point[2] = Z;
        }

        public bool setLen(double L1, double L2, double L3, double L4)
        {
            if (L1 >= 0 & L2 >= 0 & L3 >= 0 & L4 >= 0)
            {
                l1 = L1;
                l2 = L2;
                l3 = L3;
                l4 = L4;
                return true;
            }
            return false;
        }

        public double[] getLen()
        {
            double[] rez = { l1, l2, l3, l4 };
            return rez;
        }

        public bool setBase(double X, double Y, double Z)
        {
            base_point[0] = X;
            base_point[1] = Y;
            base_point[2] = Z;

            return true;
        }

        public double[] getBase()
        {
            return base_point;
        }

        //перемножение матриц
        double[][] matrix(double[][] m1, double[][] m2)
        {
            double[][] rez = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                rez[i] = new double[4];
                for (int j = 0; j < 4; j++)
                {
                    rez[i][j] = 0;
                    for (int k = 0; k < 4; k++)
                        rez[i][j] = rez[i][j] + m1[i][k] * m2[k][j];
                }
            }

            return rez;
        }

        //Задаём матрицы поворота от угла и дистанции
        double[][] m1(double o1, double r1)
        {
            double[][] m = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                m[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    m[i][j] = 0;
            }

            m[0][0] = Math.Cos(o1);
            m[0][1] = -Math.Sin(o1);
            m[1][0] = Math.Sin(o1);
            m[1][1] = Math.Cos(o1);
            m[2][2] = 1;
            m[2][3] = r1;
            m[3][3] = 1;

            return m;
        }

        double[][] m2(double o2, double r2)
        {
            double[][] m = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                m[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    m[i][j] = 0;
            }

            m[0][0] = Math.Cos(o2);
            m[0][2] = Math.Sin(o2);
            m[0][3] = r2 * Math.Sin(o2);
            m[1][1] = 1;
            m[2][0] = -Math.Sin(o2);
            m[2][2] = Math.Cos(o2);
            m[2][3] = r2 * Math.Cos(o2);
            m[3][3] = 1;

            return m;
        }

        double[][] m3(double o3, double r3)
        {
            double[][] m = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                m[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    m[i][j] = 0;
            }

            m[0][0] = Math.Cos(o3);
            m[0][2] = Math.Sin(o3);
            m[0][3] = r3 * Math.Cos(o3);
            m[1][1] = 1;
            m[2][0] = -Math.Sin(o3);
            m[2][2] = Math.Cos(o3);
            m[2][3] = -r3 * Math.Sin(o3);
            m[3][3] = 1;

            return m;
        }

        double[][] m4(double o4)
        {
            double[][] m = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                m[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    m[i][j] = 0;
            }

            m[0][0] = 1;
            m[1][1] = Math.Cos(o4);
            m[1][2] = -Math.Sin(o4);
            m[2][1] = Math.Sin(o4);
            m[2][2] = Math.Cos(o4);
            m[3][3] = 1;

            return m;
        }

        double[][] m5(double o5, double r4)
        {
            double[][] m = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                m[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    m[i][j] = 0;
            }

            m[0][0] = Math.Cos(o5);
            m[0][2] = Math.Sin(o5);
            m[0][3] = r4 * Math.Cos(o5);
            m[1][1] = 1;
            m[2][0] = -Math.Sin(o5);
            m[2][2] = Math.Cos(o5);
            m[2][3] = -r4 * Math.Sin(o5);
            m[3][3] = 1;

            return m;
        }

        double[][] m6(double o6)
        {
            double[][] m = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                m[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    m[i][j] = 0;
            }

            m[0][0] = 1;
            m[1][1] = Math.Cos(o6);
            m[1][2] = -Math.Sin(o6);
            m[2][1] = Math.Sin(o6);
            m[2][2] = Math.Cos(o6);
            m[3][3] = 1;

            return m;
        }

        double[][] mbase(bool flag = true)
        {
            double[][] m = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                m[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    m[i][j] = 0;
            }

            if (flag)
            {
                m[0][0] = 1;
                m[1][1] = 1;
                m[2][2] = 1;
                m[3][3] = 1;
            }
            else
            {
                m[0][0] = -1;
                m[1][1] = -1;
                m[2][2] = 1;
                m[3][3] = 1;
            }

            return m;
        }

        // -пи до +пи
        double getAngle(double X, double Y)
        {
            if (X == 0 & Y == 0) { return 0; }

            if (X == 0)
            {
                if (Y > 0)
                    return Math.PI / 2;
                else
                    return -Math.PI / 2;
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

        double[] gP4(double X, double Y, double Z)
        {
            double[] rez = { X, Y, Z };
            return rez;
        }

        double[] gP3(double[] P4, double[] Pnab)
        {
            double x = Pnab[0] - P4[0];
            double y = Pnab[1] - P4[1];
            double z = Pnab[2] - P4[2];

            b = getAngle(x, y);
            a = getAngle(Math.Sqrt(x * x + y * y), z);

            b = -b;
            a = -a;

            double[] rez = { P4[0], P4[1], P4[2] };

            rez[0] = P4[0] - l4 * Math.Cos(a) * Math.Cos(b);
            rez[1] = P4[1] + l4 * Math.Cos(a) * Math.Sin(b);
            rez[2] = P4[2] + l4 * Math.Sin(a);

            return rez;
        }

        double[] gP3(double[] P4, double a, double b)
        {
            double[] rez = { P4[0], P4[1], P4[2] };
            rez[0] = P4[0] - l4 * Math.Cos(a) * Math.Cos(b);
            rez[1] = P4[1] + l4 * Math.Cos(a) * Math.Sin(b);
            rez[2] = P4[2] + l4 * Math.Sin(a);

            return rez;
        }

        double[] gP1(double[] P3)
        {
            o1 = getAngle(P3[0], P3[1]);
            if (o1 > Math.PI / 4)
                o1 = o1 - Math.PI;
            if (o1 < -Math.PI / 4)
                o1 = o1 + Math.PI;


            Matrix.m1 = m1(o1, l1);

            double[] rez = { Matrix.m1[0][3], Matrix.m1[1][3], Matrix.m1[2][3] };
            return rez;
        }

        double[] gP2(double[] P3)
        {
            double x = Math.Sqrt(P3[0] * P3[0] + P3[1] * P3[1]);
            if (P3[0] < 0)
                x = -x;
            double z = P3[2] - l1;

            double L = Math.Sqrt(P3[0] * P3[0] + P3[1] * P3[1] + (P3[2] - l1) * (P3[2] - l1));

            //углы по трём сторонам
            if (L != 0)
            {
                double A = Math.Acos((l2 * l2 + L * L - l3 * l3) / (2 * l2 * L));
                double B = Math.Acos((l3 * l3 + L * L - l2 * l2) / (2 * l3 * L));
                double G = Math.PI - A - B;

                double o = getAngle(x, z);
                o2 = Math.PI / 2 - (o + A);

                Matrix.m2 = m2(o2, l2);
                double[][] R = matrix(Matrix.m1, Matrix.m2);

                double[] rez = { R[0][3], R[1][3], R[2][3] };

                // А проверить не могу, но установка в нужную точку типо
                //Angle.o3 = Math.PI / 2 - g - Math.Asin(30 / Math.Sqrt(30 * 30 + Length.L3 * Length.L3));
                o3 = Math.PI / 2 - G;
                Matrix.m3 = m3(o3, l3);

                return rez;
            }
            else
                return null;
        }

        void go4(double[] P4)
        {
            double[][] R2 = matrix(Matrix.m1, Matrix.m2);
            R2 = matrix(R2, Matrix.m3);

            double newX = R2[0][0] * (P4[0] - R2[0][3]) +
                        R2[1][0] * (P4[1] - R2[1][3]) +
                        R2[2][0] * (P4[2] - R2[2][3]);

            double newY = R2[0][1] * (P4[0] - R2[0][3]) +
                        R2[1][1] * (P4[1] - R2[1][3]) +
                        R2[2][1] * (P4[2] - R2[2][3]);

            double newZ = R2[0][2] * (P4[0] - R2[0][3]) +
                        R2[1][2] * (P4[1] - R2[1][3]) +
                        R2[2][2] * (P4[2] - R2[2][3]);

            o4 = getAngle(-newZ, newY);
            //o4 = Math.PI - Math.Acos(newZ / Math.Sqrt(newY * newY + newZ * newZ));

            //if (newY < 0)
            //  o4 = -o4;

            if (o4 * 180 / Math.PI < -90)
                o4 = o4 + Math.PI;
            if (o4 * 180 / Math.PI > 90)
                o4 = o4 - Math.PI;

            Matrix.m4 = m4(o4);
        }

        void go5(double[] P4)
        {

            double[][] R2 = matrix(Matrix.m1, Matrix.m2);
            R2 = matrix(R2, Matrix.m3);
            R2 = matrix(R2, Matrix.m4);

            double newX = R2[0][0] * (P4[0] - R2[0][3]) +
                        R2[1][0] * (P4[1] - R2[1][3]) +
                        R2[2][0] * (P4[2] - R2[2][3]);

            double newY = R2[0][1] * (P4[0] - R2[0][3]) +
                        R2[1][1] * (P4[1] - R2[1][3]) +
                        R2[2][1] * (P4[2] - R2[2][3]);

            double newZ = R2[0][2] * (P4[0] - R2[0][3]) +
                        R2[1][2] * (P4[1] - R2[1][3]) +
                        R2[2][2] * (P4[2] - R2[2][3]);

            o5 = -getAngle(newX, newZ);
            Matrix.m5 = m5(o5, l4);
        }

        void go6(double g)
        {
            double[][] R = matrix(Matrix.m1, Matrix.m2);
            R = matrix(R, Matrix.m3);
            R = matrix(R, Matrix.m4);
            R = matrix(R, Matrix.m5);

            o6 = -(g + getAngle(R[2][2], R[2][1]));

            Matrix.m6 = m6(o6);
        }

        public bool Inverse(double X, double Y, double Z, double alf = 0, double bet = 0, double gam = 0)
        {
            //манипулятор в отрицательной зоне тогда true!
            double[][] mat = mbase();

            double x_ = mat[0][0] * (X - base_point[0]) + mat[1][0] * (Y - base_point[1]) + mat[2][0] * (Z - base_point[2]);
            double y_ = mat[0][1] * (X - base_point[0]) + mat[1][1] * (Y - base_point[1]) + mat[2][1] * (Z - base_point[2]);
            double z_ = mat[0][2] * (X - base_point[0]) + mat[1][2] * (Y - base_point[1]) + mat[2][2] * (Z - base_point[2]);

            double[] P4 = gP4(x_, y_, z_); //изменение для новой системы координат

            alf = alf * Math.PI / 180;
            bet = bet * Math.PI / 180;
            gam = gam * Math.PI / 180;

            a = alf;
            b = bet;
            g = gam;

            double[] P3 = gP3(P4, alf, bet);

            //проверка 
            if (Math.Sqrt(P3[0] * P3[0] + P3[1] * P3[1] + (P3[2] - l1) * (P3[2] - l1)) <= l2 + l3)
            {
                double[] P1 = gP1(P3);
                double[] P2 = gP2(P3);
                if (P2 != null)
                {
                    go4(P4);
                    go5(P4);
                    go6(gam);

                    return true;
                }
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        public bool InverseNab(double X, double Y, double Z, double X2, double Y2, double Z2)
        {
            double[][] mat = mbase();

            double x_ = mat[0][0] * (X - base_point[0]) + mat[1][0] * (Y - base_point[1]) + mat[2][0] * (Z - base_point[2]);
            double y_ = mat[0][1] * (X - base_point[0]) + mat[1][1] * (Y - base_point[1]) + mat[2][1] * (Z - base_point[2]);
            double z_ = mat[0][2] * (X - base_point[0]) + mat[1][2] * (Y - base_point[1]) + mat[2][2] * (Z - base_point[2]);

            double[] P4 = gP4(x_, y_, z_);

            x_ = mat[0][0] * (X2 - base_point[0]) + mat[1][0] * (Y2 - base_point[1]) + mat[2][0] * (Z2 - base_point[2]);
            y_ = mat[0][1] * (X2 - base_point[0]) + mat[1][1] * (Y2 - base_point[1]) + mat[2][1] * (Z2 - base_point[2]);
            z_ = mat[0][2] * (X2 - base_point[0]) + mat[1][2] * (Y2 - base_point[1]) + mat[2][2] * (Z2 - base_point[2]);

            double[] nab = { x_, y_, z_ };

            double[] P3 = gP3(P4, nab);
            if (P3 != null)
            {
                g = 0;

                //проверка 
                if (Math.Sqrt(P3[0] * P3[0] + P3[1] * P3[1] + (P3[2] - l1) * (P3[2] - l1)) <= l2 + l3)
                {
                    double[] P1 = gP1(P3);
                    double[] P2 = gP2(P3);
                    go4(P4);
                    go5(P4);
                    go6(g);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
                return false;
        }

        public double[] getAngles()
        {
            double[] rez = { o1, o2, o3, o4, o5, o6 };
            return rez;
        }
    }

    static class Matrix
    {
        public static double[][] m1;
        public static double[][] m2;
        public static double[][] m3;
        public static double[][] m4;
        public static double[][] m5;
        public static double[][] m6;
    }
}

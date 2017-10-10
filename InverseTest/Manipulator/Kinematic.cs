using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Manipulator
{
    public class Kinematic
    {
        private MyPoint center_point;

        public Kinematic(double X = 0, double Y = 0, double Z = 0)
        {
            center_point = new MyPoint(X, Y, Z);
        }

        /// <summary>
        /// Функция задает длины звеньев
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <param name="r3"></param>
        /// <param name="r4"></param>
        /// <returns></returns>
        public bool SetLength(double r1, double r2, double r3, double r4)
        {
            if ((r1 > 0) & (r2 > 0) & (r3 > 0) & (r4 > 0))
            {
                Length.L1 = r1;
                Length.L2 = r2;
                Length.L3 = r3;
                Length.L4 = r4;
                return true;
            }
            else
                return false;
        }

        public double[] GetLength()
        {
            double[] rez = { Length.L1, Length.L2, Length.L3, Length.L4 };
            return rez;
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
            //double O1 = o1 * pi / 180;
            double[][] m1 = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                m1[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    m1[i][j] = 0;
            }

            m1[0][0] = Math.Cos(o1);
            m1[0][1] = -Math.Sin(o1);
            m1[1][0] = Math.Sin(o1);
            m1[1][1] = Math.Cos(o1);
            m1[2][2] = 1;
            m1[2][3] = r1;
            m1[3][3] = 1;

            return m1;
        }

        double[][] m2(double o2, double r2)
        {
            double[][] m1 = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                m1[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    m1[i][j] = 0;
            }

            m1[0][0] = Math.Cos(o2);
            m1[0][2] = Math.Sin(o2);
            m1[0][3] = r2 * Math.Sin(o2);
            m1[1][1] = 1;
            m1[2][0] = -Math.Sin(o2);
            m1[2][2] = Math.Cos(o2);
            m1[2][3] = r2 * Math.Cos(o2);
            m1[3][3] = 1;

            return m1;
        }

        double[][] m3(double o3, double r3)
        {
            double[][] m1 = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                m1[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    m1[i][j] = 0;
            }

            m1[0][0] = Math.Cos(o3);
            m1[0][2] = Math.Sin(o3);
            m1[0][3] = r3 * Math.Cos(o3);
            m1[1][1] = 1;
            m1[2][0] = -Math.Sin(o3);
            m1[2][2] = Math.Cos(o3);
            m1[2][3] = -r3 * Math.Sin(o3);
            m1[3][3] = 1;

            return m1;
        }

        double[][] m4(double o4)
        {
            double[][] m1 = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                m1[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    m1[i][j] = 0;
            }

            m1[0][0] = 1;
            m1[1][1] = Math.Cos(o4);
            m1[1][2] = -Math.Sin(o4);
            m1[2][1] = Math.Sin(o4);
            m1[2][2] = Math.Cos(o4);
            m1[3][3] = 1;

            return m1;
        }

        double[][] m5(double o5, double r4)
        {
            double[][] m1 = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                m1[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    m1[i][j] = 0;
            }

            m1[0][0] = Math.Cos(o5);
            m1[0][2] = Math.Sin(o5);
            m1[0][3] = r4 * Math.Cos(o5);
            m1[1][1] = 1;
            m1[2][0] = -Math.Sin(o5);
            m1[2][2] = Math.Cos(o5);
            m1[2][3] = -r4 * Math.Sin(o5);
            m1[3][3] = 1;

            return m1;
        }

        double[][] m6(double o6)
        {
            double[][] m1 = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                m1[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    m1[i][j] = 0;
            }

            m1[0][0] = 1;
            m1[1][1] = Math.Cos(o6);
            m1[1][2] = -Math.Sin(o6);
            m1[2][1] = Math.Sin(o6);
            m1[2][2] = Math.Cos(o6);
            m1[3][3] = 1;

            return m1;
        }

        double[][] mbase(bool flag = true)
        {
            double[][] m1 = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                m1[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    m1[i][j] = 0;
            }

            if (flag)
            {
                m1[0][0] = 1;
                m1[1][1] = 1;
                m1[2][2] = 1;
                m1[3][3] = 1;
            }
            else
            {
                m1[0][0] = -1;
                m1[1][1] = -1;
                m1[2][2] = 1;
                m1[3][3] = 1;
            }   
             
            return m1;
        }

        MyPoint gP4(double x, double y, double z)
        {
            MyPoint rez = new MyPoint(x, y, z);
            return rez;
        }

        //добавить уникальное положение!!
        MyPoint gP3(MyPoint P4, MyPoint Pnab)
        {
            if (Pnab.x >= P4.x)
            {
                double newx = Pnab.x - P4.x;
                double newy = Pnab.y - P4.y;
                double newz = Pnab.z - P4.z;

                if (newy * newy + newx * newx != 0)
                    Angle.b = Math.Asin(newy / Math.Sqrt(newy * newy + newx * newx));
                else
                    Angle.b = 0;

                if (newz * newz + newx * newx + newy * newy != 0)
                    Angle.a = Math.Asin(newz / Math.Sqrt(newz * newz + newx * newx + newy * newy));
                else
                    Angle.a = 0;

                Angle.b = -Angle.b;
                Angle.a = -Angle.a;

                double x = Length.L4 * Math.Cos(Angle.a) * Math.Cos(Angle.b);
                double y = Length.L4 * Math.Cos(Angle.a) * Math.Sin(Angle.b);
                double z = Length.L4 * Math.Sin(Angle.a);

                MyPoint rez = new MyPoint(P4.x - x, P4.y + y, P4.z + z);
                return rez;
            }
            else
                return null;
        }

        MyPoint gP3(MyPoint P, double a, double b)
        {
            double x = Length.L4 * Math.Cos(a) * Math.Cos(b);
            double y = Length.L4 * Math.Cos(a) * Math.Sin(b);
            double z = Length.L4 * Math.Sin(a);

            MyPoint rez = new MyPoint(P.x - x, P.y + y, P.z + z);
            return rez;
        }

        MyPoint gP1(MyPoint P3)
        {
            double o1 = 0;
            if ((P3.x == 0) & (P3.y == 0))
                o1 = 0;
            else
            {
                o1 = Math.Acos(P3.x / Math.Sqrt(P3.x * P3.x + P3.y * P3.y));
                if (P3.y < 0)
                    o1 = -o1;
            }

            Angle.o1 = o1;

            if (o1 * 180 / Math.PI > 90)
                Angle.o1 = o1 - Math.PI;
            if (o1 * 180 / Math.PI < -90)
                Angle.o1 = o1 + Math.PI;

            Matrix.m1 = m1(Angle.o1, Length.L1);

            MyPoint rez = new MyPoint(Matrix.m1[0][3], Matrix.m1[1][3], Matrix.m1[2][3]);
            return rez;
        }

        MyPoint gP2(MyPoint P3)
        {
            double x = Math.Sqrt(P3.x * P3.x + P3.y * P3.y);
            double L = Math.Sqrt(P3.x * P3.x + P3.y * P3.y + (P3.z - Length.L1) * (P3.z - Length.L1));

            //углы по трём сторонам
            //L != 0      
            double a = Math.Acos((Length.L2 * Length.L2 + L * L - Length.L3 * Length.L3) / (2 * Length.L2 * L));
            double b = Math.Acos((Length.L3 * Length.L3 + L * L - Length.L2 * Length.L2) / (2 * Length.L3 * L));
            double g = Math.PI - a - b;

            double o = 0;
            if (P3.z - Length.L1 < 0)
            {
                o = -Math.Acos(x / L);
            }
            else
            {
                o = Math.Acos(x / L);
            }

            // А проверить не могу, но установка в нужную точку типо
            //double o2 = o + a + Math.Asin(30 / Math.Sqrt(30 * 30 + Length.L3 * Length.L3));
            double o2 = o + a;
            Angle.o2 = Math.PI / 2 - o2;

            Matrix.m2 = m2(Angle.o2, Length.L2);

            double[][] R = matrix(Matrix.m1, Matrix.m2);

            MyPoint rez = new MyPoint(R[0][3], R[1][3], R[2][3]);

            Angle.o3 = Math.PI / 2 - g;
            Matrix.m3 = m3(Angle.o3, Length.L3);

            return rez;
        }

        void go4(MyPoint P4)
        {
            double[][] R2 = matrix(Matrix.m1, Matrix.m2);
            R2 = matrix(R2, Matrix.m3);

            double newX = R2[0][0] * (P4.x - R2[0][3]) +
                        R2[1][0] * (P4.y - R2[1][3]) +
                        R2[2][0] * (P4.z - R2[2][3]);

            double newY = R2[0][1] * (P4.x - R2[0][3]) +
                        R2[1][1] * (P4.y - R2[1][3]) +
                        R2[2][1] * (P4.z - R2[2][3]);

            double newZ = R2[0][2] * (P4.x - R2[0][3]) +
                        R2[1][2] * (P4.y - R2[1][3]) +
                        R2[2][2] * (P4.z - R2[2][3]);

            double o4 = 0;
            if (newY * newY + newZ * newZ != 0)

                o4 = Math.PI - Math.Acos(newZ / Math.Sqrt(newY * newY + newZ * newZ));

            if (newY < 0)
                o4 = -o4;

            if (o4 * 180 / Math.PI < -90)
                o4 = o4 + Math.PI;
            if (o4 * 180 / Math.PI > 90)
                o4 = o4 - Math.PI;

            Angle.o4 = o4;
            Matrix.m4 = m4(o4);
        }

        void go5(MyPoint P4)
        {

            double[][] R2 = matrix(Matrix.m1, Matrix.m2);
            R2 = matrix(R2, Matrix.m3);
            R2 = matrix(R2, Matrix.m4);

            double newX = R2[0][0] * (P4.x - R2[0][3]) +
                        R2[1][0] * (P4.y - R2[1][3]) +
                        R2[2][0] * (P4.z - R2[2][3]);

            double newY = R2[0][1] * (P4.x - R2[0][3]) +
                        R2[1][1] * (P4.y - R2[1][3]) +
                        R2[2][1] * (P4.z - R2[2][3]);

            double newZ = R2[0][2] * (P4.x - R2[0][3]) +
                        R2[1][2] * (P4.y - R2[1][3]) +
                        R2[2][2] * (P4.z - R2[2][3]);

            double L = Math.Sqrt(newX * newX + newZ * newZ);
            double o = Math.Acos(newX / L);
            if (-newZ < 0)
                o = -o;

            Angle.o5 = o;
            Matrix.m5 = m5(Angle.o5, Length.L4);
        }

        void go6(double g)
        {
            double[][] R = matrix(Matrix.m1, Matrix.m2);
            R = matrix(R, Matrix.m3);
            R = matrix(R, Matrix.m4);
            R = matrix(R, Matrix.m5);

            if (R[2][1] > 0)
                Angle.o6 = g - Math.Acos(R[2][2] / Math.Sqrt(R[2][2] * R[2][2] + R[2][1] * R[2][1]));
            else
                Angle.o6 = g + Math.Acos(R[2][2] / Math.Sqrt(R[2][2] * R[2][2] + R[2][1] * R[2][1]));

            Matrix.m6 = m6(Angle.o6);
        }

        /// <summary>
        /// Функция решает обратную кинематику
        /// </summary>
        /// <param name="X">Куда встать по x</param>
        /// <param name="Y">Куда встать по y</param>
        /// <param name="Z">Куда встать по z</param>
        /// <param name="alf">Угол альфа</param>
        /// <param name="bet">Угол бета</param>
        /// <param name="gam">Угол гамма</param>
        /// <returns></returns>
        public bool Inverse(double X, double Y, double Z, double alf, double bet, double gam)
        {
            //манипулятор в отрицательной зоне тогда true!
            double[][] mat = mbase();

            double x_ = mat[0][0] * (X - center_point.x) + mat[1][0] * (Y - center_point.y) + mat[2][0] * (Z - center_point.z);
            double y_ = mat[0][1] * (X - center_point.x) + mat[1][1] * (Y - center_point.y) + mat[2][1] * (Z - center_point.z);
            double z_ = mat[0][2] * (X - center_point.x) + mat[1][2] * (Y - center_point.y) + mat[2][2] * (Z - center_point.z);

            MyPoint P4 = gP4(x_, y_, z_); //изменение для новой системы координат

            alf = alf * Math.PI / 180;
            bet = bet * Math.PI / 180;
            gam = gam * Math.PI / 180;

            Angle.a = alf;
            Angle.b = bet;
            Angle.g = gam;

            MyPoint P3 = gP3(P4, alf, bet);

            /*слежение
            Vector rrr = new Vector();
            rrr.x = 3;
            rrr.y = 3;
            rrr.z = 3;
            gP3(P4, rrr);*/

            //проверка 
            if (Math.Sqrt(P3.x * P3.x + P3.y * P3.y + (P3.z - Length.L1) * (P3.z - Length.L1)) <= Length.L2 + Length.L3)
            {
                MyPoint P1 = gP1(P3);
                MyPoint P2 = gP2(P3);
                go4(P4);
                go5(P4);
                go6(gam);

                return true;
            }
            else
            {
                return false;
                //MessageBox.Show("Не дотягивается");
            }
        }

        /// <summary>
        /// Функция решает обратную кинематику с точкой наблюдения
        /// </summary>
        /// <param name="X">Куда встать по x</param>
        /// <param name="Y">Куда встать по y</param>
        /// <param name="Z">Куда встать по z</param>
        /// <param name="X2">Куда смотреть по x</param>
        /// <param name="Y2">Куда смотреть по y</param>
        /// <param name="Z2">Куда смотреть по z</param>
        /// <returns></returns>
        public bool InverseNab(double X, double Y, double Z, double X2, double Y2, double Z2)
        {
            double[][] mat = mbase();

            double x_ = mat[0][0] * (X - center_point.x) + mat[1][0] * (Y - center_point.y) + mat[2][0] * (Z - center_point.z);
            double y_ = mat[0][1] * (X - center_point.x) + mat[1][1] * (Y - center_point.y) + mat[2][1] * (Z - center_point.z);
            double z_ = mat[0][2] * (X - center_point.x) + mat[1][2] * (Y - center_point.y) + mat[2][2] * (Z - center_point.z);

            MyPoint P4 = gP4(x_, y_, z_);

            x_ = mat[0][0] * (X2 - center_point.x) + mat[1][0] * (Y2 - center_point.y) + mat[2][0] * (Z2 - center_point.z);
            y_ = mat[0][1] * (X2 - center_point.x) + mat[1][1] * (Y2 - center_point.y) + mat[2][1] * (Z2 - center_point.z);
            z_ = mat[0][2] * (X2 - center_point.x) + mat[1][2] * (Y2 - center_point.y) + mat[2][2] * (Z2 - center_point.z);

            MyPoint nab = new MyPoint(x_, y_, z_);

            MyPoint P3 = gP3(P4, nab);
            if (P3 != null)
            {
                Angle.g = 0;

                //проверка 
                if (Math.Sqrt(P3.x * P3.x + P3.y * P3.y + (P3.z - Length.L1) * (P3.z - Length.L1)) <= Length.L2 + Length.L3)
                {
                    MyPoint P1 = gP1(P3);
                    MyPoint P2 = gP2(P3);
                    go4(P4);
                    go5(P4);
                    go6(Angle.g);

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
    }

    class MyPoint
    {
        public double x = 0;
        public double y = 0;
        public double z = 0;

        public MyPoint(double X, double Y, double Z)
        {
            x = X;
            y = Y;
            z = Z;
        }
    }

    public static class Angle
    {
        public static double o1 = 0;
        public static double o2 = 0;
        public static double o3 = 0;
        public static double o4 = 0;
        public static double o5 = 0;
        public static double o6 = 0;
        public static double a = 0;
        public static double b = 0;
        public static double g = 0;
    }

    static class Length
    {
        public static double L1 = 49;
        public static double L2 = 80;
        public static double L3 = 80;
        public static double L4 = 35;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Manipulator
{
    public class Kinematic
    {
        List<double[]> solutions = new List<double[]>();

        // точка установки манипулятора
        double[] base_point = { 0, 0, 0 };

        // длины портала
        double l1 = 0;
        double l2 = 0;
        double l3 = 0;
        double l4 = 0;
        double l5 = 0;

        public double det = 0;

        // ограничения по углам
        double[,] ogranich = new double[,] { { -90, 90 },
                                             { -90, 90 },
                                             { -90, 70 },
                                             { -270, 270},
                                             { 0, 180 } };

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

        public bool setLen(double L1, double L2, double L3, double L4, double L5)
        {
            if (L1 >= 0 & L2 >= 0 & L3 >= 0 & L4 >= 0 & L5 >= 0)
            {
                l1 = L1;
                l2 = L2;
                l3 = L3;
                l4 = L4;
                l5 = L5;
                return true;
            }
            return false;
        }

        public double[] getLen()
        {
            double[] rez = { l1, l2, l3, l4, l5 };
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

            double L = Math.Sqrt(P3[0] * P3[0] + P3[1] * P3[1] +
                (P3[2] - l1) * (P3[2] - l1));
            double L3 = Math.Sqrt(l3 * l3 + det * det);             // Так как появлиось det, то и длинны меняются

            //углы по трём сторонам
            if (L != 0)
            {
                //double A = Math.Acos((l2 * l2 + L * L - l3 * l3) / (2 * l2 * L));
                //double B = Math.Acos((l3 * l3 + L * L - l2 * l2) / (2 * l3 * L));
                double A = Math.Acos((l2 * l2 + L * L - L3 * L3) / (2 * l2 * L));
                double B = Math.Acos((L3 * L3 + L * L - l2 * l2) / (2 * L3 * L));
                double G = Math.PI - A - B;

                double o = getAngle(x, z);
                o2 = Math.PI / 2 - (o + A);

                Matrix.m2 = m2(o2, l2);

                double[][] R = matrix(Matrix.m1, Matrix.m2);
                double[] rez = { R[0][3], R[1][3], R[2][3] };

                double t = getAngle(l3, det);
                o3 = Math.PI / 2 - G + getAngle(l3, det);
                Matrix.m3 = matrix(m3(o3, l3), mmove_Z(det));

                R = matrix(R, Matrix.m3);

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

            o4 = -getAngle(newZ, newY);                      // Было отрицательным

            /*
            if (o4 * 180 / Math.PI < -90)
                o4 = o4 + Math.PI;
            if (o4 * 180 / Math.PI > 90)
                o4 = o4 - Math.PI;*/

            Matrix.m4 = m4(o4);
            R2 = matrix(R2, Matrix.m4);
        }

        void go5(double[] P4)
        {
            double[][] R2 = matrix(Matrix.m1, Matrix.m2);
            R2 = matrix(R2, Matrix.m3);
            R2 = matrix(R2, m4(o4));

            double newX = R2[0][0] * (P4[0] - R2[0][3]) +
                        R2[1][0] * (P4[1] - R2[1][3]) +
                        R2[2][0] * (P4[2] - R2[2][3]);

            double newY = R2[0][1] * (P4[0] - R2[0][3]) +
                        R2[1][1] * (P4[1] - R2[1][3]) +
                        R2[2][1] * (P4[2] - R2[2][3]);

            double newZ = R2[0][2] * (P4[0] - R2[0][3]) +
                        R2[1][2] * (P4[1] - R2[1][3]) +
                        R2[2][2] * (P4[2] - R2[2][3]);

            o5 = getAngle(newX, newZ);
            //o5 = getAngle(newZ, newX);
            Matrix.m5 = m5(o5, l5);
        }

        public double[] getAngles()
        {
            double[] rez = { o1, o2, o3, o4, o5, o6 };
            return rez;
        }

        double[][] newm5(double o5)
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
            m[1][1] = 1;
            m[2][0] = -Math.Sin(o5);
            m[2][2] = Math.Cos(o5);
            m[3][3] = 1;

            return m;
        }

        double[][] mrotate_Z(double o)
        {
            double[][] m = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                m[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    m[i][j] = 0;
            }


            m[0][0] = Math.Cos(o);
            m[0][1] = -Math.Sin(o);
            m[1][0] = Math.Sin(o);
            m[1][1] = Math.Cos(o);
            m[2][2] = 1;
            m[3][3] = 1;

            return m;
        }

        double[][] mrotate_Y(double o)
        {
            double[][] m = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                m[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    m[i][j] = 0;
            }

            m[0][0] = Math.Cos(o);
            m[0][2] = Math.Sin(o);
            m[1][1] = 1;
            m[2][0] = -Math.Sin(o);
            m[2][2] = Math.Cos(o);
            m[3][3] = 1;

            return m;
        }

        double[][] mrotate_X(double o)
        {
            double[][] m = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                m[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    m[i][j] = 0;
            }

            m[0][0] = 1;
            m[1][1] = Math.Cos(o);
            m[1][2] = -Math.Sin(o);
            m[2][1] = Math.Sin(o);
            m[2][2] = Math.Cos(o);
            m[3][3] = 1;

            return m;
        }

        double[][] mmove_X(double x)
        {
            double[][] m = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                m[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    m[i][j] = 0;
            }

            m[0][0] = 1;
            m[0][3] = x;
            m[1][1] = 1;
            m[2][2] = 1;
            m[3][3] = 1;

            return m;
        }

        double[][] mmove_Z(double z)
        {
            double[][] m = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                m[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    m[i][j] = 0;
            }

            m[0][0] = 1;
            m[1][1] = 1;
            m[2][2] = 1;
            m[2][3] = z;
            m[3][3] = 1;

            return m;
        }

        // Матрица T или матрица манипулятора
        double[][] mt(double alfa, double beta, double[] point)
        {
            double[][] m = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                m[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    m[i][j] = 0;
            }

            m[0][0] = Math.Cos(beta) * Math.Cos(alfa);
            m[0][1] = Math.Sin(beta);
            m[0][2] = Math.Cos(beta) * Math.Sin(alfa);
            m[0][3] = point[0];

            m[1][0] = Math.Sin(beta) * Math.Cos(alfa);
            m[1][1] = Math.Cos(beta);
            m[1][2] = Math.Sin(beta) * Math.Sin(alfa);
            m[1][3] = point[1];

            m[2][0] = -Math.Sin(alfa);
            m[2][2] = Math.Cos(alfa);
            m[2][3] = point[2];

            m[3][3] = 1;

            return m;
        }

        Stack<double[]> newgP3(double[] P4, out double[] P34, double a, double b)
        {
            double[] rez = { P4[0], P4[1], P4[2] };
            rez[0] = P4[0] - l5 * Math.Cos(a) * Math.Cos(b);
            rez[1] = P4[1] - l5 * Math.Cos(a) * Math.Sin(b);
            rez[2] = P4[2] + l5 * Math.Sin(a);

            P34 = new double[] { rez[0], rez[1], rez[2] };

            double[][] R = matrix(mrotate_Z(b), mrotate_Y(a));
            R[0][3] = P4[0];
            R[1][3] = P4[1];
            R[2][3] = P4[2];

            R = matrix(R, mmove_X(-l5));

            Stack<double[]> P3stack = new Stack<double[]>();

            for (double i = -180 / 180.0 * Math.PI; i < 180 / 180.0 * Math.PI; i += 0.1 / 180.0 * Math.PI)
            {
                double[][] Rt = matrix(R, mrotate_X(i));
                Rt = matrix(Rt, mmove_Z(-l4));
                P3stack.Push(new double[] { Rt[0][3], Rt[1][3], Rt[2][3] });
            }

            return P3stack;
        }

        double[] getAandB(double[] P4, double[] Pnab)
        {
            double x = Pnab[0] - P4[0];
            double y = Pnab[1] - P4[1];
            double z = Pnab[2] - P4[2];

            b = getAngle(x, y);
            a = getAngle(Math.Sqrt(x * x + y * y), z);

            return new double[2] { a, b };
        }

        public Stack<double[]> Inverse(double X, double Y, double Z, double alf = 0, double bet = 0)
        {
            // Начальные данные

            double[] P34 = null;                                    // Точка P34
            Stack<double[]> rezultAngles = new Stack<double[]>();   // Стек решений кинематики
            double[][] mat = mbase();                               /* Положение манипулятора при работе системы Манипулятор-Портал
                                                                       В отрицательной зоне параметр true                           */

            // Преобразование точек к локальной системе координат манипулятора
            double x_ = mat[0][0] * (X - base_point[0]) + mat[1][0] * (Y - base_point[1]) + mat[2][0] * (Z - base_point[2]);
            double y_ = mat[0][1] * (X - base_point[0]) + mat[1][1] * (Y - base_point[1]) + mat[2][1] * (Z - base_point[2]);
            double z_ = mat[0][2] * (X - base_point[0]) + mat[1][2] * (Y - base_point[1]) + mat[2][2] * (Z - base_point[2]);

            double[] P4 = gP4(x_, y_, z_);                          // Точка в новой системе координат

            // Перевод градусов в радианы
            alf = alf * Math.PI / 180;                              // Угол альфа, вращение вокруг Y 
            bet = bet * Math.PI / 180;                              // Угол бета, вращение вокруг Z

            double[][] T = mt(alf, bet, P4);                        // Определение матрицы манипулятора

            //----------------------------------------------------------------------------------------------------------------------
            Stack<double[]> P3mass = newgP3(P4, out P34, alf, bet); // Получение множества точек P3

            foreach (double[] point in P3mass)                      // Для каждой такой точки P3 ищем решение кинематики
            {
                // Ниже идёт проверка на достижимость до точки P3 манипулятором                
                if (Math.Sqrt(point[0] * point[0] + point[1] * point[1] + (point[2] - l1) * (point[2] - l1)) <= l2 + l3)
                {
                    double[] P1 = gP1(point);                       // Получение точки P1 и получение обобщенной координаты O1
                    double[] P2 = gP2(point);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                    // Ниже условие, если получили точку то продолжаем 
                    if (P2 != null)
                    {
                        go4(P34);
                        go5(P34);

                        double[] angle4 = { o4, 0 };
                        double[] angle5 = { -o5 + Math.PI / 2, 0 };

                        if (o4 > Math.PI)
                            o4 = o4 - Math.PI;
                        else
                        {
                            o4 = o4 + Math.PI;
                        }
                        angle4[1] = o4;
                        go5(P34);
                        angle5[1] = -o5 + Math.PI / 2;

                        // Ниже выполненна прямая кинематика манипулятора
                        double[][] R = matrix(m1(o1, l1), m2(o2, l2));
                        R = matrix(R, m3(o3, l3));

                        R = matrix(R, mmove_Z(det));

                        R = matrix(R, m4(angle4[0]));
                        R = matrix(R, newm5(angle5[0]));
                        R = matrix(R, mmove_Z(l4));
                        R = matrix(R, mmove_X(l5));

                        double pogr = 0.1;

                        o4 = angle4[0];
                        o5 = angle5[0];
                        // Если отклонения от матрицы манипулятора Т меньше нормы, то записываем в результат
                        if (Math.Abs(R[0][0] - T[0][0]) < pogr & Math.Abs(R[1][0] - T[1][0]) < pogr & Math.Abs(R[2][0] - T[2][0]) < pogr)
                            rezultAngles.Push(getAngles());

                        R = matrix(m1(o1, l1), m2(o2, l2));
                        R = matrix(R, m3(o3, l3));

                        R = matrix(R, mmove_Z(det));

                        R = matrix(R, m4(angle4[1]));
                        R = matrix(R, newm5(angle5[1]));
                        R = matrix(R, mmove_Z(l4));
                        R = matrix(R, mmove_X(l5));

                        o4 = angle4[1];
                        o5 = angle5[1];
                        if (Math.Abs(R[0][0] - T[0][0]) < pogr & Math.Abs(R[1][0] - T[1][0]) < pogr & Math.Abs(R[2][0] - T[2][0]) < pogr)
                            rezultAngles.Push(getAngles());
                    }
                }
            }

            return rezultAngles;
        }

        /*
        public Stack<double[]> InverseNab(double X, double Y, double Z, double X2, double Y2, double Z2)
        {
            string[] P5_position = new string[8];
            // Начальные данные

            double[] P34 = null;                                    // Точка P34
            Stack<double[]> rezultAngles = new Stack<double[]>();   // Стек решений кинематики
            double[][] mat = mbase();                               

            // Преобразование точек к локальной системе координат манипулятора
            double x_ = mat[0][0] * (X - base_point[0]) + mat[1][0] * (Y - base_point[1]) + mat[2][0] * (Z - base_point[2]);
            double y_ = mat[0][1] * (X - base_point[0]) + mat[1][1] * (Y - base_point[1]) + mat[2][1] * (Z - base_point[2]);
            double z_ = mat[0][2] * (X - base_point[0]) + mat[1][2] * (Y - base_point[1]) + mat[2][2] * (Z - base_point[2]);

            double[] P4 = gP4(x_, y_, z_);                          // Точка в новой системе координат

            x_ = mat[0][0] * (X2 - base_point[0]) + mat[1][0] * (Y2 - base_point[1]) + mat[2][0] * (Z2 - base_point[2]);
            y_ = mat[0][1] * (X2 - base_point[0]) + mat[1][1] * (Y2 - base_point[1]) + mat[2][1] * (Z2 - base_point[2]);
            z_ = mat[0][2] * (X2 - base_point[0]) + mat[1][2] * (Y2 - base_point[1]) + mat[2][2] * (Z2 - base_point[2]);
            double[] Pn = new double[3] { x_, y_, z_ };

            double[] aandb = getAandB(P4, Pn);                      // Получение углов альфа и бета
            double alf = -aandb[0];
            double bet = aandb[1];

            double[][] T = mt(alf, bet, P4);                        // Определение матрицы манипулятора

            //----------------------------------------------------------------------------------------------------------------------
            Stack<double[]> P3mass = newgP3(P4, out P34, alf, bet); // Получение множества точек P3

            foreach (double[] point in P3mass)                      // Для каждой такой точки P3 ищем решение кинематики
            {
                // Ниже идёт проверка на достижимость до точки P3 манипулятором                
                if (Math.Sqrt(point[0] * point[0] + point[1] * point[1] + (point[2] - l1) * (point[2] - l1)) <= l2 + l3)
                {
                    double[] P1 = gP1(point);                       // Получение точки P1 и получение обобщенной координаты O1
                    double[] P2 = gP2(point);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                    // Ниже условие, если получили точку то продолжаем 
                    if (P2 != null)
                    {
                        go4(P34);                                   // Получение обобщенной координаты O4
                        go5(P34);                                   // Получение обобщенной координаты O5

                        double[] angle4 = { o4, 0 };
                        double[] angle5 = { -o5 + Math.PI / 2, 0 };
                        if (o4 > Math.PI)
                            o4 = o4 - Math.PI;
                        else
                        {
                            o4 = o4 + Math.PI;
                        }
                        angle4[1] = o4;
                        go5(P34);
                        angle5[1] = -o5 + Math.PI / 2;
                        //-----------------------------

                        // Ниже выполненна прямая кинематика манипулятора
                        double[][] R = matrix(m1(o1, l1), m2(o2, l2));
                        R = matrix(R, Matrix.m3);
                        R = matrix(R, m4(angle4[0]));
                        R = matrix(R, newm5(angle5[0]));
                        R = matrix(R, mmove_Z(l4));
                        R = matrix(R, mmove_X(l5));

                        P5_position[0] += String.Format("{0} ", P4[0] - R[0][3]);
                        P5_position[1] += String.Format("{0} ", P4[1] - R[1][3]);
                        P5_position[2] += String.Format("{0} ", P4[2] - R[2][3]);
                        P5_position[3] += String.Format("{0} ", Math.Sqrt((P4[0] - R[0][3]) * (P4[0] - R[0][3]) +
                                                                           (P4[1] - R[1][3]) * (P4[1] - R[1][3]) +
                                                                           (P4[2] - R[2][3]) * (P4[2] - R[2][3])));
                        P5_position[4] += String.Format("{0} ", angle4[0]);
                        P5_position[5] += String.Format("{0} ", angle5[0]);
                        P5_position[6] += String.Format("{0} ", angle4[1]);
                        P5_position[7] += String.Format("{0} ", angle5[1]);

                        double pogr = 0.1;

                        o4 = angle4[0];
                        o5 = angle5[0];
                        // Если отклонения от матрицы манипулятора Т меньше нормы, то записываем в результат
                        //if (Math.Abs(R[0][0] - T[0][0]) < pogr & Math.Abs(R[1][0] - T[1][0]) < pogr & Math.Abs(R[2][0] - T[2][0]) < pogr)

                        if (Math.Abs((R[0][3] - T[0][3]) * (R[0][3] - T[0][3])
                                    + (R[1][3] - T[1][3]) * (R[1][3] - T[1][3])
                                    + (R[2][3] - T[2][3]) * (R[2][3] - T[2][3])) < pogr
                                    )
                            rezultAngles.Push(getAngles());

                        R = matrix(m1(o1, l1), m2(o2, l2));

                        R = matrix(R, Matrix.m3);
                        R = matrix(R, m4(angle4[1]));
                        R = matrix(R, newm5(angle5[1]));
                        R = matrix(R, mmove_Z(l4));
                        R = matrix(R, mmove_X(l5));

                        o4 = angle4[1];
                        o5 = angle5[1];
                        //if (Math.Abs(R[0][0] - T[0][0]) < pogr & Math.Abs(R[1][0] - T[1][0]) < pogr & Math.Abs(R[2][0] - T[2][0]) < pogr)
                        if (Math.Abs((R[0][3] - T[0][3]) * (R[0][3] - T[0][3])
                                   + (R[1][3] - T[1][3]) * (R[1][3] - T[1][3])
                                   + (R[2][3] - T[2][3]) * (R[2][3] - T[2][3])) < pogr
                                    )
                            rezultAngles.Push(getAngles());
                    }
                }
            }

            P5_position[0] += " ОТКЛОНЕНИЯ ПОЗИЦИИ ПО X";
            P5_position[1] += " ОТКЛОНЕНИЯ ПОЗИЦИИ ПО Y";
            P5_position[2] += " ОТКЛОНЕНИЯ ПОЗИЦИИ ПО Z";
            P5_position[3] += " СУММАРНОЕ ОТКЛОНЕНИЕ";
            P5_position[4] += " УГОЛ o4 1";
            P5_position[5] += " УГОЛ o5 1";
            P5_position[6] += " УГОЛ o4 2";
            P5_position[7] += " УГОЛ o5 2";


            System.IO.File.WriteAllLines("P5_position.txt", P5_position);
            return rezultAngles;
        }
    */

        public double[][] DirectKinematic(double[] angles)
        {
            double[][] R = matrix(m1(angles[0], l1), m2(angles[1], l2));
            R = matrix(R, m3(angles[2], l3));
            R = matrix(R, mmove_Z(det));
            R = matrix(R, m4(angles[3]));
            R = matrix(R, newm5(angles[4]));
            R = matrix(R, mmove_Z(l4));
            R = matrix(R, mmove_X(l5));

            return R;
        }

        Stack<double[]> Search(double[] P4, double alf, double bet, double pogr, double leftboard, double rightdoard)
        {
            //string[] strmass = new string[3];  

            Stack<double[]> rezult = new Stack<double[]>();         // Результаты

            decimal left = (decimal)leftboard;                   // левая грань
            decimal right = (decimal)rightdoard;                    // правая грань

            double deviation = 10000;                               // отклонение
            double prev_deviation = 10001;

            while (deviation > pogr)                                // пока отклонение больше погрешности
            {
                if (prev_deviation == deviation)
                    return new Stack<double[]>();
                decimal mid = (left + right) / (decimal)2;    // середина отрезка
                decimal leftmid = (left + mid) / (decimal)2;    // значение левее середины
                decimal rightmid = (right + mid) / (decimal)2;    // значение правее середины

                double[] P1 = { 0, 0, 0 };
                double[] P2 = { 0, 0, 0 };
                double[] P3 = { 0, 0, 0 };
                double[] P34 = { 0, 0, 0 };

                List<double[]> leftrez = new List<double[]>();
                List<double[]> rightrez = new List<double[]>();

                if (deviation == 10000)
                {
                    List<double[]> midonly = new List<double[]>();

                    P3 = getP3((double)leftmid, P4, out P34, alf, bet);
                    if (Math.Sqrt(P3[0] * P3[0] + P3[1] * P3[1] + (P3[2] - l1) * (P3[2] - l1)) <= l2 + l3)
                    {
                        P1 = gP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                        P2 = gP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                                                            // Ниже условие, если получили точку то продолжаем 
                        if (P2 != null)
                        {
                            go4(P34);                                   // Получение обобщенной координаты O4
                            go5(P34);                                   // Получение обобщенной координаты O5

                            double[] angle4 = { o4, 0 };
                            double[] angle5 = { -o5 + Math.PI / 2, 0 };
                            if (o4 > Math.PI)
                                o4 = o4 - Math.PI;
                            else
                            {
                                o4 = o4 + Math.PI;
                            }
                            angle4[1] = o4;
                            go5(P34);
                            angle5[1] = -o5 + Math.PI / 2;

                            o4 = angle4[0];
                            o5 = angle5[0];

                            midonly.Add(getAngles());

                            o4 = angle4[1];
                            o5 = angle5[1];

                            midonly.Add(getAngles());

                            double[][] MR1 = DirectKinematic(midonly[0]);
                            double[][] MR2 = DirectKinematic(midonly[1]);

                            double ml1_length = Math.Sqrt((MR1[0][3] - P4[0]) * (MR1[0][3] - P4[0])
                                                        + (MR1[1][3] - P4[1]) * (MR1[1][3] - P4[1])
                                                        + (MR1[2][3] - P4[2]) * (MR1[2][3] - P4[2]));
                            double ml2_length = Math.Sqrt((MR2[0][3] - P4[0]) * (MR2[0][3] - P4[0])
                                                        + (MR2[1][3] - P4[1]) * (MR2[1][3] - P4[1])
                                                        + (MR2[2][3] - P4[2]) * (MR2[2][3] - P4[2]));

                            if (ml1_length < ml2_length & ml1_length <= pogr)
                            {
                                rezult.Push(midonly[0]);
                                /*
                                if (mid < (decimal)Math.PI)
                                    mid = mid + (decimal)Math.PI;
                                else
                                    mid = mid - (decimal)Math.PI;

                                P3 = getP3((double)mid, P4, out P34, alf, bet);
                                if (Math.Sqrt(P3[0] * P3[0] + P3[1] * P3[1] + (P3[2] - l1) * (P3[2] - l1)) <= l2 + l3)
                                {
                                    P1 = gP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                                    P2 = gP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                                                                        // Ниже условие, если получили точку то продолжаем 
                                    if (P2 != null)
                                    {
                                        go4(P34);                                   // Получение обобщенной координаты O4
                                        go5(P34);                                   // Получение обобщенной координаты O5

                                        double[] angl4 = { o4, 0 };
                                        double[] angl5 = { -o5 + Math.PI / 2, 0 };
                                        if (o4 > Math.PI)
                                            o4 = o4 - Math.PI;
                                        else
                                        {
                                            o4 = o4 + Math.PI;
                                        }
                                        angl4[1] = o4;
                                        go5(P34);
                                        angl5[1] = -o5 + Math.PI / 2;

                                        o4 = angl4[0];
                                        o5 = angl5[0];

                                        double[][] R = DirectKinematic(getAngles());

                                        double l1 = Math.Abs((R[0][3] - P4[0]) * (R[0][3] - P4[0])
                                               + (R[1][3] - P4[1]) * (R[1][3] - P4[1])
                                               + (R[2][3] - P4[2]) * (R[2][3] - P4[2]));

                                        o4 = angl4[1];
                                        o5 = angl5[1];

                                        R = DirectKinematic(getAngles());

                                        double l2 = Math.Abs((R[0][3] - P4[0]) * (R[0][3] - P4[0])
                                               + (R[1][3] - P4[1]) * (R[1][3] - P4[1])
                                               + (R[2][3] - P4[2]) * (R[2][3] - P4[2]));
                                        if (l1 < l2)
                                        {
                                            o4 = angl4[0];
                                            o5 = angl5[0];
                                            rezult.Push(getAngles());
                                        }
                                        else
                                            rezult.Push(getAngles());
                                    }
                                }*/

                                return rezult;
                            }
                            if (ml1_length > ml2_length & ml2_length <= pogr)
                            {
                                rezult.Push(midonly[1]);/*
                                if (mid < (decimal)Math.PI)
                                    mid = mid + (decimal)Math.PI;
                                else
                                    mid = mid - (decimal)Math.PI;

                                P3 = getP3((double)mid, P4, out P34, alf, bet);
                                if (Math.Sqrt(P3[0] * P3[0] + P3[1] * P3[1] + (P3[2] - l1) * (P3[2] - l1)) <= l2 + l3)
                                {
                                    P1 = gP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                                    P2 = gP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                                                                        // Ниже условие, если получили точку то продолжаем 
                                    if (P2 != null)
                                    {
                                        go4(P34);                                   // Получение обобщенной координаты O4
                                        go5(P34);                                   // Получение обобщенной координаты O5

                                        double[] angl4 = { o4, 0 };
                                        double[] angl5 = { -o5 + Math.PI / 2, 0 };
                                        if (o4 > Math.PI)
                                            o4 = o4 - Math.PI;
                                        else
                                        {
                                            o4 = o4 + Math.PI;
                                        }
                                        angl4[1] = o4;
                                        go5(P34);
                                        angl5[1] = -o5 + Math.PI / 2;

                                        o4 = angl4[0];
                                        o5 = angl5[0];

                                        double[][] R = DirectKinematic(getAngles());

                                        double l1 = Math.Abs((R[0][3] - P4[0]) * (R[0][3] - P4[0])
                                               + (R[1][3] - P4[1]) * (R[1][3] - P4[1])
                                               + (R[2][3] - P4[2]) * (R[2][3] - P4[2]));

                                        o4 = angl4[1];
                                        o5 = angl5[1];

                                        R = DirectKinematic(getAngles());

                                        double l2 = Math.Abs((R[0][3] - P4[0]) * (R[0][3] - P4[0])
                                               + (R[1][3] - P4[1]) * (R[1][3] - P4[1])
                                               + (R[2][3] - P4[2]) * (R[2][3] - P4[2]));
                                        if (l1 < l2)
                                        {
                                            o4 = angl4[0];
                                            o5 = angl5[0];
                                            rezult.Push(getAngles());
                                        }
                                        else
                                            rezult.Push(getAngles());
                                    }
                                }
                                */
                                return rezult;
                            }

                        }
                    }
                }
                /*string[] strmass = new string[3];
                for (double ind = 0; ind < Math.PI * 2; ind += 1 * Math.PI / 180)
                {
                    P3 = getP3(ind, P4, out P34, alf, bet);
                    if (Math.Sqrt(P3[0] * P3[0] + P3[1] * P3[1] + (P3[2] - l1) * (P3[2] - l1)) <= l2 + l3)
                    {
                        P1 = gP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                        P2 = gP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                                                            // Ниже условие, если получили точку то продолжаем 
                        if (P2 != null)
                        {
                            go4(P34);                                   // Получение обобщенной координаты O4
                            go5(P34);                                   // Получение обобщенной координаты O5

                            double[] angle4 = { o4, 0 };
                            double[] angle5 = { -o5 + Math.PI / 2, 0 };
                            if (o4 > Math.PI)
                                o4 = o4 - Math.PI;
                            else
                            {
                                o4 = o4 + Math.PI;
                            }
                            angle4[1] = o4;
                            go5(P34);
                            angle5[1] = -o5 + Math.PI / 2;

                            o4 = angle4[0];
                            o5 = angle5[0];

                            double[][] R = this.DirectKinematic(getAngles());
                            double l = Math.Sqrt((R[0][3] - P4[0]) * (R[0][3] - P4[0]) +
                                                 (R[1][3] - P4[1]) * (R[1][3] - P4[1]) +
                                                 (R[2][3] - P4[2]) * (R[2][3] - P4[2]));

                            strmass[0] += String.Format("{0} ", ind);
                            strmass[1] += String.Format("{0} ", l);

                            o4 = angle4[1];
                            o5 = angle5[1];

                            R = this.DirectKinematic(getAngles());

                            l = Math.Sqrt((R[0][3] - P4[0]) * (R[0][3] - P4[0]) +
                                                 (R[1][3] - P4[1]) * (R[1][3] - P4[1]) +
                                                 (R[2][3] - P4[2]) * (R[2][3] - P4[2]));
                            strmass[2] += String.Format("{0} ", l);
                        }
                    }
                }
                strmass[0] += " ______________ИНДЕКС";
                strmass[1] += " ______________1";
                strmass[2] += " ______________2";
                System.IO.File.WriteAllLines("tmp.txt", strmass);*/

                // ДЛЯ ЛЕВОГО
                P3 = getP3((double)leftmid, P4, out P34, alf, bet);
                if (Math.Sqrt(P3[0] * P3[0] + P3[1] * P3[1] + (P3[2] - l1) * (P3[2] - l1)) <= l2 + l3)
                {
                    P1 = gP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                    P2 = gP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                    // Ниже условие, если получили точку то продолжаем 
                    if (P2 != null)
                    {
                        go4(P34);                                   // Получение обобщенной координаты O4
                        go5(P34);                                   // Получение обобщенной координаты O5

                        double[] angle4 = { o4, 0 };
                        double[] angle5 = { -o5 + Math.PI / 2, 0 };
                        if (o4 > Math.PI)
                            o4 = o4 - Math.PI;
                        else
                        {
                            o4 = o4 + Math.PI;
                        }
                        angle4[1] = o4;
                        go5(P34);
                        angle5[1] = -o5 + Math.PI / 2;

                        o4 = angle4[0];
                        o5 = angle5[0];

                        leftrez.Add(getAngles());

                        o4 = angle4[1];
                        o5 = angle5[1];

                        leftrez.Add(getAngles());
                    }
                }

                // ДЛЯ ПРАВОГО
                P3 = getP3((double)rightmid, P4, out P34, alf, bet);
                if (Math.Sqrt(P3[0] * P3[0] + P3[1] * P3[1] + (P3[2] - l1) * (P3[2] - l1)) <= l2 + l3)
                {
                    P1 = gP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                    P2 = gP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                    // Ниже условие, если получили точку то продолжаем 
                    if (P2 != null)
                    {
                        go4(P34);                                   // Получение обобщенной координаты O4
                        go5(P34);                                   // Получение обобщенной координаты O5

                        double[] angle4 = { o4, 0 };
                        double[] angle5 = { -o5 + Math.PI / 2, 0 };
                        if (o4 > Math.PI)
                            o4 = o4 - Math.PI;
                        else
                        {
                            o4 = o4 + Math.PI;
                        }
                        angle4[1] = o4;
                        go5(P34);
                        angle5[1] = -o5 + Math.PI / 2;

                        o4 = angle4[0];
                        o5 = angle5[0];

                        rightrez.Add(getAngles());

                        o4 = angle4[1];
                        o5 = angle5[1];

                        rightrez.Add(getAngles());
                    }
                }

                double[][] LR1 = DirectKinematic(leftrez[0]);
                double[][] LR2 = DirectKinematic(leftrez[1]);
                double[][] RR1 = DirectKinematic(rightrez[0]);
                double[][] RR2 = DirectKinematic(rightrez[1]);

                double l1_length = Math.Sqrt((LR1[0][3] - P4[0]) * (LR1[0][3] - P4[0])
                                           + (LR1[1][3] - P4[1]) * (LR1[1][3] - P4[1])
                                           + (LR1[2][3] - P4[2]) * (LR1[2][3] - P4[2]));
                double l2_length = Math.Sqrt((LR2[0][3] - P4[0]) * (LR2[0][3] - P4[0])
                                           + (LR2[1][3] - P4[1]) * (LR2[1][3] - P4[1])
                                           + (LR2[2][3] - P4[2]) * (LR2[2][3] - P4[2]));
                double r1_length = Math.Sqrt((RR1[0][3] - P4[0]) * (RR1[0][3] - P4[0])
                                           + (RR1[1][3] - P4[1]) * (RR1[1][3] - P4[1])
                                           + (RR1[2][3] - P4[2]) * (RR1[2][3] - P4[2]));
                double r2_length = Math.Sqrt((RR2[0][3] - P4[0]) * (RR2[0][3] - P4[0])
                                           + (RR2[1][3] - P4[1]) * (RR2[1][3] - P4[1])
                                           + (RR2[2][3] - P4[2]) * (RR2[2][3] - P4[2]));
                bool flag1 = false;
                bool flag2 = false;
                if (deviation == prev_deviation)
                {
                    return new Stack<double[]>();
                }
                if (l1_length <= pogr || flag1)
                {
                    rezult.Push(leftrez[0]);/*
                    if (leftmid < (decimal)Math.PI)
                        leftmid = leftmid + (decimal)Math.PI;
                    else
                        leftmid = leftmid - (decimal)Math.PI;

                    P3 = getP3((double)leftmid, P4, out P34, alf, bet);
                    if (Math.Sqrt(P3[0] * P3[0] + P3[1] * P3[1] + (P3[2] - l1) * (P3[2] - l1)) <= l2 + l3)
                    {
                        P1 = gP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                        P2 = gP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                                                            // Ниже условие, если получили точку то продолжаем 
                        if (P2 != null)
                        {
                            go4(P34);                                   // Получение обобщенной координаты O4
                            go5(P34);                                   // Получение обобщенной координаты O5

                            double[] angle4 = { o4, 0 };
                            double[] angle5 = { -o5 + Math.PI / 2, 0 };
                            if (o4 > Math.PI)
                                o4 = o4 - Math.PI;
                            else
                            {
                                o4 = o4 + Math.PI;
                            }
                            angle4[1] = o4;
                            go5(P34);
                            angle5[1] = -o5 + Math.PI / 2;

                            o4 = angle4[0];
                            o5 = angle5[0];

                            double[][] R = DirectKinematic(getAngles());

                            double l1 = Math.Abs((R[0][3] - P4[0]) * (R[0][3] - P4[0])
                                   + (R[1][3] - P4[1]) * (R[1][3] - P4[1])
                                   + (R[2][3] - P4[2]) * (R[2][3] - P4[2]));

                            o4 = angle4[1];
                            o5 = angle5[1];

                            R = DirectKinematic(getAngles());

                            double l2 = Math.Abs((R[0][3] - P4[0]) * (R[0][3] - P4[0])
                                   + (R[1][3] - P4[1]) * (R[1][3] - P4[1])
                                   + (R[2][3] - P4[2]) * (R[2][3] - P4[2]));
                            if (l1 < l2)
                            {
                                o4 = angle4[0];
                                o5 = angle5[0];
                                rezult.Push(getAngles());
                            }
                            else
                                rezult.Push(getAngles());
                        }
                    }
                    */
                    return rezult;
                }
                if (l2_length <= pogr || flag2)
                {
                    rezult.Push(leftrez[1]);/*
                    if (leftmid < (decimal)Math.PI)
                        leftmid = leftmid + (decimal)Math.PI;
                    else
                        leftmid = leftmid - (decimal)Math.PI;

                    P3 = getP3((double)leftmid, P4, out P34, alf, bet);
                    if (Math.Sqrt(P3[0] * P3[0] + P3[1] * P3[1] + (P3[2] - l1) * (P3[2] - l1)) <= l2 + l3)
                    {
                        P1 = gP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                        P2 = gP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                                                            // Ниже условие, если получили точку то продолжаем 
                        if (P2 != null)
                        {
                            go4(P34);                                   // Получение обобщенной координаты O4
                            go5(P34);                                   // Получение обобщенной координаты O5

                            double[] angle4 = { o4, 0 };
                            double[] angle5 = { -o5 + Math.PI / 2, 0 };
                            if (o4 > Math.PI)
                                o4 = o4 - Math.PI;
                            else
                            {
                                o4 = o4 + Math.PI;
                            }
                            angle4[1] = o4;
                            go5(P34);
                            angle5[1] = -o5 + Math.PI / 2;

                            o4 = angle4[0];
                            o5 = angle5[0];

                            double[][] R = DirectKinematic(getAngles());

                            double l1 = Math.Abs((R[0][3] - P4[0]) * (R[0][3] - P4[0])
                                   + (R[1][3] - P4[1]) * (R[1][3] - P4[1])
                                   + (R[2][3] - P4[2]) * (R[2][3] - P4[2]));

                            o4 = angle4[1];
                            o5 = angle5[1];

                            R = DirectKinematic(getAngles());

                            double l2 = Math.Abs((R[0][3] - P4[0]) * (R[0][3] - P4[0])
                                   + (R[1][3] - P4[1]) * (R[1][3] - P4[1])
                                   + (R[2][3] - P4[2]) * (R[2][3] - P4[2]));
                            if (l1 < l2)
                            {
                                o4 = angle4[0];
                                o5 = angle5[0];
                                rezult.Push(getAngles());
                            }
                            else
                                rezult.Push(getAngles());
                        }
                    }*/
                    return rezult;
                }
                if (r1_length <= pogr)
                {
                    rezult.Push(rightrez[0]);/*
                    if (rightmid < (decimal)Math.PI)
                        rightmid = rightmid + (decimal)Math.PI;
                    else
                        rightmid = rightmid - (decimal)Math.PI;

                    P3 = getP3((double)rightmid, P4, out P34, alf, bet);
                    if (Math.Sqrt(P3[0] * P3[0] + P3[1] * P3[1] + (P3[2] - l1) * (P3[2] - l1)) <= l2 + l3)
                    {
                        P1 = gP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                        P2 = gP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                                                            // Ниже условие, если получили точку то продолжаем 
                        if (P2 != null)
                        {
                            go4(P34);                                   // Получение обобщенной координаты O4
                            go5(P34);                                   // Получение обобщенной координаты O5

                            double[] angle4 = { o4, 0 };
                            double[] angle5 = { -o5 + Math.PI / 2, 0 };
                            if (o4 > Math.PI)
                                o4 = o4 - Math.PI;
                            else
                            {
                                o4 = o4 + Math.PI;
                            }
                            angle4[1] = o4;
                            go5(P34);
                            angle5[1] = -o5 + Math.PI / 2;

                            o4 = angle4[0];
                            o5 = angle5[0];

                            double[][] R = DirectKinematic(getAngles());

                            double l1 = Math.Abs((R[0][3] - P4[0]) * (R[0][3] - P4[0])
                                   + (R[1][3] - P4[1]) * (R[1][3] - P4[1])
                                   + (R[2][3] - P4[2]) * (R[2][3] - P4[2]));

                            o4 = angle4[1];
                            o5 = angle5[1];

                            R = DirectKinematic(getAngles());

                            double l2 = Math.Abs((R[0][3] - P4[0]) * (R[0][3] - P4[0])
                                   + (R[1][3] - P4[1]) * (R[1][3] - P4[1])
                                   + (R[2][3] - P4[2]) * (R[2][3] - P4[2]));
                            if (l1 < l2)
                            {
                                o4 = angle4[0];
                                o5 = angle5[0];
                                rezult.Push(getAngles());
                            }
                            else
                                rezult.Push(getAngles());
                        }
                    }*/
                    return rezult;
                }
                if (r2_length <= pogr)
                {
                    rezult.Push(rightrez[1]);/*
                    if (rightmid < (decimal)Math.PI)
                        rightmid = rightmid + (decimal)Math.PI;
                    else
                        rightmid = rightmid - (decimal)Math.PI;

                    P3 = getP3((double)rightmid, P4, out P34, alf, bet);
                    if (Math.Sqrt(P3[0] * P3[0] + P3[1] * P3[1] + (P3[2] - l1) * (P3[2] - l1)) <= l2 + l3)
                    {
                        P1 = gP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                        P2 = gP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                                                            // Ниже условие, если получили точку то продолжаем 
                        if (P2 != null)
                        {
                            go4(P34);                                   // Получение обобщенной координаты O4
                            go5(P34);                                   // Получение обобщенной координаты O5

                            double[] angle4 = { o4, 0 };
                            double[] angle5 = { -o5 + Math.PI / 2, 0 };
                            if (o4 > Math.PI)
                                o4 = o4 - Math.PI;
                            else
                            {
                                o4 = o4 + Math.PI;
                            }
                            angle4[1] = o4;
                            go5(P34);
                            angle5[1] = -o5 + Math.PI / 2;

                            o4 = angle4[0];
                            o5 = angle5[0];

                            double[][] R = DirectKinematic(getAngles());

                            double l1 = Math.Abs((R[0][3] - P4[0]) * (R[0][3] - P4[0])
                                   + (R[1][3] - P4[1]) * (R[1][3] - P4[1])
                                   + (R[2][3] - P4[2]) * (R[2][3] - P4[2]));

                            o4 = angle4[1];
                            o5 = angle5[1];

                            R = DirectKinematic(getAngles());

                            double l2 = Math.Abs((R[0][3] - P4[0]) * (R[0][3] - P4[0])
                                   + (R[1][3] - P4[1]) * (R[1][3] - P4[1])
                                   + (R[2][3] - P4[2]) * (R[2][3] - P4[2]));
                            if (l1 < l2)
                            {
                                o4 = angle4[0];
                                o5 = angle5[0];
                                rezult.Push(getAngles());
                            }
                            else
                                rezult.Push(getAngles());
                        }
                    }*/
                    return rezult;
                }

                if (Math.Min(l1_length, l2_length) <= Math.Min(r1_length, r2_length))
                {
                    right = mid;
                    prev_deviation = deviation;
                    deviation = Math.Min(l1_length, l2_length);
                }
                else
                {
                    left = mid;
                    prev_deviation = deviation;
                    deviation = Math.Min(r1_length, r2_length);
                }
                /*for (double ind = 0; ind < Math.PI; ind += 1 * Math.PI / 180)
                {
                    P3 = getP3(ind, P4, out P34, alf, bet);
                    if (Math.Sqrt(P3[0] * P3[0] + P3[1] * P3[1] + (P3[2] - l1) * (P3[2] - l1)) <= l2 + l3)
                    {
                        P1 = gP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                        P2 = gP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                                                            // Ниже условие, если получили точку то продолжаем 
                        if (P2 != null)
                        {
                            go4(P34);                                   // Получение обобщенной координаты O4
                            go5(P34);                                   // Получение обобщенной координаты O5

                            double[] angle4 = { o4, 0 };
                            double[] angle5 = { -o5 + Math.PI / 2, 0 };
                            if (o4 > Math.PI)
                                o4 = o4 - Math.PI;
                            else
                            {
                                o4 = o4 + Math.PI;
                            }
                            angle4[1] = o4;
                            go5(P34);
                            angle5[1] = -o5 + Math.PI / 2;

                            o4 = angle4[0];
                            o5 = angle5[0];

                            double[][] R = this.DirectKinematic(getAngles());
                            double l = Math.Sqrt((R[0][3] - P4[0]) * (R[0][3] - P4[0]) +
                                                 (R[1][3] - P4[1]) * (R[1][3] - P4[1]) +
                                                 (R[2][3] - P4[2]) * (R[2][3] - P4[2]));

                            strmass[0] += String.Format("{0} ", ind);
                            strmass[1] += String.Format("{0} ", l);

                            o4 = angle4[1];
                            o5 = angle5[1];

                            R = this.DirectKinematic(getAngles());

                            l = Math.Sqrt((R[0][3] - P4[0]) * (R[0][3] - P4[0]) +
                                                 (R[1][3] - P4[1]) * (R[1][3] - P4[1]) +
                                                 (R[2][3] - P4[2]) * (R[2][3] - P4[2]));
                            strmass[2] += String.Format("{0} ", l);
                        }
                    }
                }
                strmass[0] += " ______________ИНДЕКС";
                strmass[1] += " ______________1";
                strmass[2] += " ______________2";
                System.IO.File.WriteAllLines("tmp.txt", strmass);*/


            }   // конец while

            return new Stack<double[]>();
        }

        Stack<double[]> findOnInterval(int inter, double[] P4, double alf, double bet, double tochnost, double leftboard, double rightboard)
        {
            Stack<double[]> rez = new Stack<double[]>();
            for (int i = 1; i < inter; i++)
            {
                double delta = (rightboard - leftboard) / (double)i;
                for (int j = 0; j < i; j++)
                {
                    rez = Search(P4, alf, bet, tochnost, delta * j + leftboard, delta * (j + 1) + leftboard);
                    if (rez.Count != 0)
                        return rez;
                }
            }

            return new Stack<double[]>();
        }

        public Stack<double[]> InverseNab(double X, double Y, double Z, double X2, double Y2, double Z2)
        {
            double[] P34 = null;                                    // Точка P34
            Stack<double[]> rezultAngles = new Stack<double[]>();   // Стек решений кинематики
            double[][] mat = mbase();                               /* Положение манипулятора при работе системы Манипулятор-Портал
                                                                       В отрицательной зоне параметр true                           */

            // Преобразование точек к локальной системе координат манипулятора
            double x_ = mat[0][0] * (X - base_point[0]) + mat[1][0] * (Y - base_point[1]) + mat[2][0] * (Z - base_point[2]);
            double y_ = mat[0][1] * (X - base_point[0]) + mat[1][1] * (Y - base_point[1]) + mat[2][1] * (Z - base_point[2]);
            double z_ = mat[0][2] * (X - base_point[0]) + mat[1][2] * (Y - base_point[1]) + mat[2][2] * (Z - base_point[2]);

            double[] P4 = gP4(x_, y_, z_);                          // Точка в новой системе координат

            x_ = mat[0][0] * (X2 - base_point[0]) + mat[1][0] * (Y2 - base_point[1]) + mat[2][0] * (Z2 - base_point[2]);
            y_ = mat[0][1] * (X2 - base_point[0]) + mat[1][1] * (Y2 - base_point[1]) + mat[2][1] * (Z2 - base_point[2]);
            z_ = mat[0][2] * (X2 - base_point[0]) + mat[1][2] * (Y2 - base_point[1]) + mat[2][2] * (Z2 - base_point[2]);
            double[] Pn = new double[3] { x_, y_, z_ };

            double[] aandb = getAandB(P4, Pn);                      // Получение углов альфа и бета
            double alf = -aandb[0];
            double bet = aandb[1];

            double[][] T = mt(alf, bet, P4);                        // Определение матрицы манипулятора

            //double tochnost = 0.000000000001;                        // Точность вычисления
            double tochnost = 0.000000000001;


            //Stack<double[]> rez =  Search(P4, alf, bet, tochnost, 0, Math.PI);
            Stack<double[]> rez  = Search(P4, alf, bet, tochnost, 0, Math.PI / 2);
            Stack<double[]> rez2 = Search(P4, alf, bet, tochnost, Math.PI / 2, Math.PI);
            Stack<double[]> rez3 = Search(P4, alf, bet, tochnost, Math.PI, Math.PI * 3 / 2.0);
            Stack<double[]> rez4 = Search(P4, alf, bet, tochnost, Math.PI * 3 / 2.0, Math.PI * 2);
            if (rez2.Count >= 1)
                rez.Push(rez2.Pop());
            if (rez3.Count >= 1)
                rez.Push(rez3.Pop());
            if (rez4.Count >= 1)
                rez.Push(rez4.Pop());

            if (rez.Count >= 2)
                return rez;

            if (rez.Count <= 1)
            {
                rez = findOnInterval(16, P4, alf, bet, tochnost, 0, Math.PI);
                rez2 = findOnInterval(16, P4, alf, bet, tochnost, Math.PI, Math.PI * 2);

                if (rez2.Count >= 1)
                {
                    rez.Push(rez2.Pop());
                    return rez;
                }
                
                
                /*
                string[] line = new string[3];
                for (double i = 0; i < Math.PI * 2; i += 0.1 * Math.PI / 180)
                {
                    double[] P3 = new double[] { 0, 0, 0 };
                    double[] P2 = new double[] { 0, 0, 0 };
                    double[] P1 = new double[] { 0, 0, 0 };
                    P3 = this.getP3(i, P4, out P34, alf, bet);

                    if (Math.Sqrt(P3[0] * P3[0] + P3[1] * P3[1] + (P3[2] - l1) * (P3[2] - l1)) <= l2 + l3)
                    {
                        P1 = gP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                        P2 = gP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                                                            // Ниже условие, если получили точку то продолжаем 
                        if (P2 != null)
                        {
                            go4(P34);                                   // Получение обобщенной координаты O4
                            go5(P34);                                   // Получение обобщенной координаты O5

                            double[] angle4 = { o4, 0 };
                            double[] angle5 = { -o5 + Math.PI / 2, 0 };
                            if (o4 > Math.PI)
                                o4 = o4 - Math.PI;
                            else
                            {
                                o4 = o4 + Math.PI;
                            }
                            angle4[1] = o4;
                            go5(P34);
                            angle5[1] = -o5 + Math.PI / 2;

                            o4 = angle4[0];
                            o5 = angle5[0];

                            double[][] R = this.DirectKinematic(getAngles());
                            double l = Math.Sqrt((R[0][3] - P4[0]) * (R[0][3] - P4[0]) +
                                                 (R[1][3] - P4[1]) * (R[1][3] - P4[1]) +
                                                 (R[2][3] - P4[2]) * (R[2][3] - P4[2]));
                            line[0] += String.Format("{0} ", i);
                            line[1] += String.Format("{0} ", l);

                            o4 = angle4[1];
                            o5 = angle5[1];
                            R = this.DirectKinematic(getAngles());
                                   l = Math.Sqrt((R[0][3] - P4[0]) * (R[0][3] - P4[0]) +
                                                 (R[1][3] - P4[1]) * (R[1][3] - P4[1]) +
                                                 (R[2][3] - P4[2]) * (R[2][3] - P4[2]));
                            line[2] += String.Format("{0} ", l);                    
                        }
                    }
                }
                line[0] += " !!!ИНДЕКС!!!";
                line[1] += " !!!L1!!!";
                line[2] += " !!!L2!!!";
                System.IO.File.WriteAllLines("tmp.txt", line);*/
                // тут вот проблема
            }
            return rez;
        }

        // проверка ограничений
        bool isAngleOK(double[] angle)
        {
            if (
               angle[0] >= ogranich[0, 0] * Math.PI / 180.0 &
               angle[0] <= ogranich[0, 1] * Math.PI / 180.0 &
               angle[1] >= ogranich[1, 0] * Math.PI / 180.0 &
               angle[1] <= ogranich[1, 1] * Math.PI / 180.0 &
               angle[2] >= ogranich[2, 0] * Math.PI / 180.0 &
               angle[2] <= ogranich[2, 1] * Math.PI / 180.0 &
               angle[3] >= ogranich[3, 0] * Math.PI / 180.0 &
               angle[3] <= ogranich[3, 1] * Math.PI / 180.0 &
               angle[4] >= ogranich[4, 0] * Math.PI / 180.0 &
               angle[4] <= ogranich[4, 1] * Math.PI / 180.0
               )
                return true;
            else
                return false;
        }

        double[] getP3(double i, double[] P4, out double[] P34, double alf, double bet)
        {
            double[] rez = { P4[0], P4[1], P4[2] };
            rez[0] = P4[0] - l5 * Math.Cos(alf) * Math.Cos(bet);
            rez[1] = P4[1] - l5 * Math.Cos(alf) * Math.Sin(bet);
            rez[2] = P4[2] + l5 * Math.Sin(alf);

            P34 = new double[] { rez[0], rez[1], rez[2] };

            double[][] R = matrix(mrotate_Z(bet), mrotate_Y(alf));
            R[0][3] = P4[0];
            R[1][3] = P4[1];
            R[2][3] = P4[2];

            R = matrix(R, mmove_X(-l5));

            double[][] Rt = matrix(R, mrotate_X(i));
            Rt = matrix(Rt, mmove_Z(-l4));

            return new double[] { Rt[0][3], Rt[1][3], Rt[2][3] };
        }
    }

    static class Matrix
    {
        public static double[][] m1;
        public static double[][] m2;
        public static double[][] m3;
        public static double[][] m4;
        public static double[][] m5;
    }
}

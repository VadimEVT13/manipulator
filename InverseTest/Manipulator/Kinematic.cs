using InverseTest.Manipulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Manipulator
{
    public class Kinematic
    {
        // точка установки манипулятора
        private Vertex3D basePoint = new Vertex3D();

        // длины портала
        private double l1 = 0;
        private double l2 = 0;
        private double l3 = 0;
        private double l4 = 0;
        private double l5 = 0;

        public double det = 0;
        // углы
        double o1 = 0;
        double o2 = 0;
        double o3 = 0;
        double o4 = 0;
        double o5 = 0;

        public Kinematic(double X = 0, double Y = 0, double Z = 0)
        {
            basePoint.X = X;
            basePoint.Y = Y;
            basePoint.Z = Z;
        }

        public bool SetLen(double setL1, double setL2, double setL3, double setL4, double setL5)
        {
            bool result = setL1 >= 0 && setL2 >= 0 && setL3 >= 0 && setL4 >= 0 && setL5 >= 0;
            if (result)
            {
                l1 = setL1;
                l2 = setL2;
                l3 = setL3;
                l4 = setL4;
                l5 = setL5;
            }
            return result;
        }

        public double[] GetLen()
        {
            double[] rez = { l1, l2, l3, l4, l5 };
            return rez;
        }

        public bool SetBase(double X, double Y, double Z)
        {
            basePoint.X = X;
            basePoint.Y = Y;
            basePoint.Z = Z;
            return true;
        }

        public Vertex3D GetBase()
        {
            return basePoint;
        }

        /// <summary>
        /// Перемножение матриц.
        /// </summary>
        /// <param name="matrixA"></param>
        /// <param name="matrixB"></param>
        /// <returns></returns>
        private double[][] Matrix(double[][] matrixA, double[][] matrixB)
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
        private double[][] M1(double o1, double r1)
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
        private double[][] M2(double o2, double r2)
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
        private double[][] M3(double o3, double r3)
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
        private double[][] M4(double o4)
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
        private double[][] M5(double o5, double r4)
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
        double[][] Mbase(bool flag = true)
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
        private double GetAngle(double X, double Y)
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
        private Vertex3D GP1(Vertex3D P3)
        {
            o1 = GetAngle(P3.X, P3.Y);
            if (o1 > Math.PI / 4)
            {
                o1 = o1 - Math.PI;
            }
            else if (o1 < -Math.PI / 4)
            {
                o1 = o1 + Math.PI;
            }
            Manipulator.Matrix.m1 = M1(o1, l1);
            Vertex3D point = new Vertex3D
            {
                X = Manipulator.Matrix.m1[0][3],
                Y = Manipulator.Matrix.m1[1][3],
                Z = Manipulator.Matrix.m1[2][3]
            };
            return point;
        }

        private Vertex3D GP2(Vertex3D P3)
        {
            double x = Math.Sqrt(P3.X * P3.X + P3.Y * P3.Y);
            if (P3.X < 0)
            {
                x = -x;
            }
            double z = P3.Z - l1;

            double L = Math.Sqrt(P3.X * P3.X + P3.Y * P3.Y + (P3.Z - l1) * (P3.Z - l1));
            // Так как появлиось det, то и длинны меняются
            double L3 = Math.Sqrt(l3 * l3 + det * det);

            //углы по трём сторонам
            if (L != 0)
            {
                double A = Math.Acos((l2 * l2 + L * L - L3 * L3) / (2 * l2 * L));
                double B = Math.Acos((L3 * L3 + L * L - l2 * l2) / (2 * L3 * L));
                double G = Math.PI - A - B;

                double o = GetAngle(x, z);
                o2 = Math.PI / 2 - (o + A);

                Manipulator.Matrix.m2 = M2(o2, l2);

                double[][] R = Matrix(Manipulator.Matrix.m1, Manipulator.Matrix.m2);

                Vertex3D point = new Vertex3D
                {
                    X = R[0][3],
                    Y = R[1][3],
                    Z = R[2][3]
                };

                double t = GetAngle(l3, det);
                o3 = Math.PI / 2 - G + GetAngle(l3, det);
                Manipulator.Matrix.m3 = Matrix(M3(o3, l3), mmove_Z(det));

                R = Matrix(R, Manipulator.Matrix.m3);

                return point;
            }
            else
            {
                return null;
            }
        }

        private void Go4(Vertex3D P4)
        {
            double[][] R2 = Matrix(Manipulator.Matrix.m1, Manipulator.Matrix.m2);
            R2 = Matrix(R2, Manipulator.Matrix.m3);

            double newX = R2[0][0] * (P4.X - R2[0][3]) +
                        R2[1][0] * (P4.Y - R2[1][3]) +
                        R2[2][0] * (P4.Z - R2[2][3]);

            double newY = R2[0][1] * (P4.X - R2[0][3]) +
                        R2[1][1] * (P4.Y - R2[1][3]) +
                        R2[2][1] * (P4.Z - R2[2][3]);

            double newZ = R2[0][2] * (P4.X - R2[0][3]) +
                        R2[1][2] * (P4.Y - R2[1][3]) +
                        R2[2][2] * (P4.Z - R2[2][3]);
            // Было отрицательным
            o4 = -GetAngle(newZ, newY);
            Manipulator.Matrix.m4 = M4(o4);
            R2 = Matrix(R2, Manipulator.Matrix.m4);
        }

        private void Go5(Vertex3D P4)
        {
            double[][] R2 = Matrix(Manipulator.Matrix.m1, Manipulator.Matrix.m2);
            R2 = Matrix(R2, Manipulator.Matrix.m3);
            R2 = Matrix(R2, M4(o4));

            double newX = R2[0][0] * (P4.X - R2[0][3]) +
                        R2[1][0] * (P4.Y - R2[1][3]) +
                        R2[2][0] * (P4.Z - R2[2][3]);

            double newY = R2[0][1] * (P4.X - R2[0][3]) +
                        R2[1][1] * (P4.Y - R2[1][3]) +
                        R2[2][1] * (P4.Z - R2[2][3]);

            double newZ = R2[0][2] * (P4.X - R2[0][3]) +
                        R2[1][2] * (P4.Y - R2[1][3]) +
                        R2[2][2] * (P4.Z - R2[2][3]);

            o5 = GetAngle(newX, newZ);
            Manipulator.Matrix.m5 = M5(o5, l5);
        }

        /// <summary>
        /// Геттер углов.
        /// </summary>
        /// <returns>углы</returns>
        public Angle3D GetAngles()
        {
            Angle3D angle = new Angle3D
            {
                O1 = o1,
                O2 = o2,
                O3 = o3,
                O4 = o4,
                O5 = o5
            };
            return angle;
        }

        private double[][] Newm5(double o5)
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

        double[][] mrotate_Z(double o)
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

        double[][] mrotate_Y(double o)
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

        double[][] mrotate_X(double o)
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

        double[][] mmove_X(double x)
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

        double[][] mmove_Z(double z)
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
        double[][] mt(double alfa, double beta, Vertex3D point)
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

        private Stack<Vertex3D> NewgP3(Vertex3D P4, out Vertex3D P34, double a, double b)
        {
            Vertex3D value = new Vertex3D
            {
                X = P4.X - l5 * Math.Cos(a) * Math.Cos(b),
                Y = P4.Y - l5 * Math.Cos(a) * Math.Sin(b),
                Z = P4.Z + l5 * Math.Sin(a)
            };
            P34 = value;

            double[][] R = Matrix(mrotate_Z(b), mrotate_Y(a));
            R[0][3] = P4.X;
            R[1][3] = P4.Y;
            R[2][3] = P4.Z;

            R = Matrix(R, mmove_X(-l5));

            Stack<Vertex3D> P3stack = new Stack<Vertex3D>();

            for (double i = -180 / 180.0 * Math.PI; i < 180 / 180.0 * Math.PI; i += 0.1 / 180.0 * Math.PI)
            {
                double[][] Rt = Matrix(R, mrotate_X(i));
                Rt = Matrix(Rt, mmove_Z(-l4));
                Vertex3D point = new Vertex3D
                {
                    X = Rt[0][3],
                    Y = Rt[1][3],
                    Z = Rt[2][3]
                };

                P3stack.Push(point);
            }
            return P3stack;
        }

        private double[] GetAandB(Vertex3D P4, Vertex3D Pnab)
        {
            double x = Pnab.X - P4.X;
            double y = Pnab.Y - P4.Y;
            double z = Pnab.Z - P4.Z;

            double b = GetAngle(x, y);
            double a = GetAngle(Math.Sqrt(x * x + y * y), z);

            return new double[2] { a, b };
        }

        public Stack<Angle3D> Inverse(double X, double Y, double Z, double alf = 0, double bet = 0)
        {
            Stack<Angle3D> rezultAngles = new Stack<Angle3D>();   // Стек решений кинематики
            double[][] mat = Mbase();                               /* Положение манипулятора при работе системы Манипулятор-Портал
                                                                       В отрицательной зоне параметр true                           */

            // Преобразование точек к локальной системе координат манипулятора
            double x_ = mat[0][0] * (X - basePoint.X) + mat[1][0] * (Y - basePoint.Y) + mat[2][0] * (Z - basePoint.Z);
            double y_ = mat[0][1] * (X - basePoint.X) + mat[1][1] * (Y - basePoint.Y) + mat[2][1] * (Z - basePoint.Z);
            double z_ = mat[0][2] * (X - basePoint.X) + mat[1][2] * (Y - basePoint.Y) + mat[2][2] * (Z - basePoint.Z);

            // Точка в новой системе координат
            Vertex3D P4 = new Vertex3D
            {
                X = x_,
                Y = y_,
                Z = z_
            };

            // Перевод градусов в радианы
            alf = alf * Math.PI / 180;                             // Угол альфа, вращение вокруг Y 
            bet = bet * Math.PI / 180;                              // Угол бета, вращение вокруг Z

            double[][] T = mt(alf, bet, P4);                        // Определение матрицы манипулятора

            //----------------------------------------------------------------------------------------------------------------------
            Stack<Vertex3D> P3mass = NewgP3(P4, out Vertex3D P34, alf, bet); // Получение множества точек P3

            foreach (Vertex3D point in P3mass)                      // Для каждой такой точки P3 ищем решение кинематики
            {
                // Ниже идёт проверка на достижимость до точки P3 манипулятором                
                if (Math.Sqrt(point.X * point.X + point.Y * point.Y + (point.Z - l1) * (point.Z - l1)) <= l2 + l3)
                {
                    Vertex3D P1 = GP1(point);                       // Получение точки P1 и получение обобщенной координаты O1
                    Vertex3D P2 = GP2(point);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                    // Ниже условие, если получили точку то продолжаем 
                    if (P2 != null)
                    {
                        Go4(P34);
                        Go5(P34);

                        double[] angle4 = { o4, 0 };
                        double[] angle5 = { -o5 + Math.PI / 2, 0 };

                        if (o4 > Math.PI)
                        {
                            o4 = o4 - Math.PI;
                        }
                        else
                        {
                            o4 = o4 + Math.PI;
                        }
                        angle4[1] = o4;
                        Go5(P34);
                        angle5[1] = -o5 + Math.PI / 2;

                        // Ниже выполненна прямая кинематика манипулятора
                        double[][] R = Matrix(M1(o1, l1), M2(o2, l2));
                        R = Matrix(R, M3(o3, l3));

                        R = Matrix(R, mmove_Z(det));

                        R = Matrix(R, M4(angle4[0]));
                        R = Matrix(R, Newm5(angle5[0]));
                        R = Matrix(R, mmove_Z(l4));
                        R = Matrix(R, mmove_X(l5));

                        double pogr = 0.1;

                        o4 = angle4[0];
                        o5 = angle5[0];
                        // Если отклонения от матрицы манипулятора Т меньше нормы, то записываем в результат
                        if (Math.Abs(R[0][0] - T[0][0]) < pogr && Math.Abs(R[1][0] - T[1][0]) < pogr && Math.Abs(R[2][0] - T[2][0]) < pogr)
                        {
                            rezultAngles.Push(GetAngles());
                        }

                        R = Matrix(M1(o1, l1), M2(o2, l2));
                        R = Matrix(R, M3(o3, l3));

                        R = Matrix(R, mmove_Z(det));

                        R = Matrix(R, M4(angle4[1]));
                        R = Matrix(R, Newm5(angle5[1]));
                        R = Matrix(R, mmove_Z(l4));
                        R = Matrix(R, mmove_X(l5));

                        o4 = angle4[1];
                        o5 = angle5[1];
                        if (Math.Abs(R[0][0] - T[0][0]) < pogr && Math.Abs(R[1][0] - T[1][0]) < pogr && Math.Abs(R[2][0] - T[2][0]) < pogr)
                        {
                            rezultAngles.Push(GetAngles());
                        }
                    }
                }
            }
            return rezultAngles;
        }

        public double[][] DirectKinematic(Angle3D angles)
        {
            double[][] R = Matrix(M1(angles.O1, l1), M2(angles.O2, l2));
            R = Matrix(R, M3(angles.O3, l3));
            R = Matrix(R, mmove_Z(det));
            R = Matrix(R, M4(angles.O4));
            R = Matrix(R, Newm5(angles.O5));
            R = Matrix(R, mmove_Z(l4));
            R = Matrix(R, mmove_X(l5));
            return R;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="P4"></param>
        /// <param name="alf"></param>
        /// <param name="bet"></param>
        /// <param name="pogr"></param>
        /// <param name="leftboard"></param>
        /// <param name="rightdoard"></param>
        /// <returns></returns>
        private Stack<Angle3D> Search(Vertex3D P4, double alf, double bet, double pogr, double leftboard, double rightdoard)
        {
            Stack<Angle3D> rezult = new Stack<Angle3D>();         // Результаты

            decimal left = (decimal)leftboard;                   // левая грань
            decimal right = (decimal)rightdoard;                    // правая грань

            double deviation = 10000;                               // отклонение
            double prev_deviation = 10001;

            while (deviation > pogr)                                // пока отклонение больше погрешности
            {
                if (prev_deviation == deviation)
                {
                    return new Stack<Angle3D>();
                }
                decimal mid = (left + right) / (decimal)2;    // середина отрезка
                decimal leftmid = (left + mid) / (decimal)2;    // значение левее середины
                decimal rightmid = (right + mid) / (decimal)2;    // значение правее середины

                Vertex3D P1 = new Vertex3D();
                Vertex3D P2 = new Vertex3D();
                Vertex3D P3 = new Vertex3D();
                Vertex3D P34 = new Vertex3D();

                List<Angle3D> leftrez = new List<Angle3D>();
                List<Angle3D> rightrez = new List<Angle3D>();

                if (deviation == 10000)
                {
                    List<Angle3D> midonly = new List<Angle3D>();

                    P34 = GetP34(P4, alf, bet);
                    P3 = GetP3((double)leftmid, P4, alf, bet);
                    if (Math.Sqrt(P3.X * P3.X + P3.Y * P3.Y + (P3.Z - l1) * (P3.Z - l1)) <= l2 + l3)
                    {
                        P1 = GP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                        P2 = GP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                                                            // Ниже условие, если получили точку то продолжаем 
                        if (P2 != null)
                        {
                            Go4(P34);                                   // Получение обобщенной координаты O4
                            Go5(P34);                                   // Получение обобщенной координаты O5

                            double[] angle4 = { o4, 0 };
                            double[] angle5 = { -o5 + Math.PI / 2, 0 };
                            if (o4 > Math.PI)
                                o4 = o4 - Math.PI;
                            else
                            {
                                o4 = o4 + Math.PI;
                            }
                            angle4[1] = o4;
                            Go5(P34);
                            angle5[1] = -o5 + Math.PI / 2;

                            o4 = angle4[0];
                            o5 = angle5[0];

                            midonly.Add(GetAngles());

                            o4 = angle4[1];
                            o5 = angle5[1];

                            midonly.Add(GetAngles());

                            double[][] MR1 = DirectKinematic(midonly[0]);
                            double[][] MR2 = DirectKinematic(midonly[1]);

                            double ml1_length = Math.Sqrt((MR1[0][3] - P4.X) * (MR1[0][3] - P4.X)
                                                        + (MR1[1][3] - P4.Y) * (MR1[1][3] - P4.Y)
                                                        + (MR1[2][3] - P4.Z) * (MR1[2][3] - P4.Z));
                            double ml2_length = Math.Sqrt((MR2[0][3] - P4.X) * (MR2[0][3] - P4.X)
                                                        + (MR2[1][3] - P4.Y) * (MR2[1][3] - P4.Y)
                                                        + (MR2[2][3] - P4.Z) * (MR2[2][3] - P4.Z));

                            if (ml1_length < ml2_length && ml1_length <= pogr)
                            {
                                rezult.Push(midonly[0]);
                                return rezult;
                            }
                            if (ml1_length > ml2_length && ml2_length <= pogr)
                            {
                                rezult.Push(midonly[1]);
                                return rezult;
                            }

                        }
                    }
                }

                // ДЛЯ ЛЕВОГО
                P34 = GetP34(P4, alf, bet);
                P3 = GetP3((double)leftmid, P4, alf, bet);
                if (Math.Sqrt(P3.X * P3.X + P3.Y * P3.Y + (P3.Z - l1) * (P3.Z - l1)) <= l2 + l3)
                {
                    P1 = GP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                    P2 = GP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                    // Ниже условие, если получили точку то продолжаем 
                    if (P2 != null)
                    {
                        Go4(P34);                                   // Получение обобщенной координаты O4
                        Go5(P34);                                   // Получение обобщенной координаты O5

                        double[] angle4 = { o4, 0 };
                        double[] angle5 = { -o5 + Math.PI / 2, 0 };
                        if (o4 > Math.PI)
                            o4 = o4 - Math.PI;
                        else
                        {
                            o4 = o4 + Math.PI;
                        }
                        angle4[1] = o4;
                        Go5(P34);
                        angle5[1] = -o5 + Math.PI / 2;

                        o4 = angle4[0];
                        o5 = angle5[0];

                        leftrez.Add(GetAngles());

                        o4 = angle4[1];
                        o5 = angle5[1];

                        leftrez.Add(GetAngles());
                    }
                }

                // ДЛЯ ПРАВОГО
                P34 = GetP34(P4, alf, bet);
                P3 = GetP3((double)rightmid, P4, alf, bet);
                if (Math.Sqrt(P3.X * P3.X + P3.Y * P3.Y + (P3.Z - l1) * (P3.Z - l1)) <= l2 + l3)
                {
                    P1 = GP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                    P2 = GP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                    // Ниже условие, если получили точку то продолжаем 
                    if (P2 != null)
                    {
                        Go4(P34);                                   // Получение обобщенной координаты O4
                        Go5(P34);                                   // Получение обобщенной координаты O5

                        double[] angle4 = { o4, 0 };
                        double[] angle5 = { -o5 + Math.PI / 2, 0 };
                        if (o4 > Math.PI)
                            o4 = o4 - Math.PI;
                        else
                        {
                            o4 = o4 + Math.PI;
                        }
                        angle4[1] = o4;
                        Go5(P34);
                        angle5[1] = -o5 + Math.PI / 2;

                        o4 = angle4[0];
                        o5 = angle5[0];

                        rightrez.Add(GetAngles());

                        o4 = angle4[1];
                        o5 = angle5[1];

                        rightrez.Add(GetAngles());
                    }
                }

                double[][] LR1 = DirectKinematic(leftrez[0]);
                double[][] LR2 = DirectKinematic(leftrez[1]);
                double[][] RR1 = DirectKinematic(rightrez[0]);
                double[][] RR2 = DirectKinematic(rightrez[1]);

                double l1_length = Math.Sqrt((LR1[0][3] - P4.X) * (LR1[0][3] - P4.X)
                                           + (LR1[1][3] - P4.Y) * (LR1[1][3] - P4.Y)
                                           + (LR1[2][3] - P4.Z) * (LR1[2][3] - P4.Z));
                double l2_length = Math.Sqrt((LR2[0][3] - P4.X) * (LR2[0][3] - P4.X)
                                           + (LR2[1][3] - P4.Y) * (LR2[1][3] - P4.Y)
                                           + (LR2[2][3] - P4.Z) * (LR2[2][3] - P4.Z));
                double r1_length = Math.Sqrt((RR1[0][3] - P4.X) * (RR1[0][3] - P4.X)
                                           + (RR1[1][3] - P4.Y) * (RR1[1][3] - P4.Y)
                                           + (RR1[2][3] - P4.Z) * (RR1[2][3] - P4.Z));
                double r2_length = Math.Sqrt((RR2[0][3] - P4.X) * (RR2[0][3] - P4.X)
                                           + (RR2[1][3] - P4.Y) * (RR2[1][3] - P4.Y)
                                           + (RR2[2][3] - P4.Z) * (RR2[2][3] - P4.Z));
                bool flag1 = false;
                bool flag2 = false;
                if (deviation == prev_deviation)
                {
                    return new Stack<Angle3D>();
                }
                if (l1_length <= pogr || flag1)
                {
                    rezult.Push(leftrez[0]);
                    return rezult;
                }
                if (l2_length <= pogr || flag2)
                {
                    rezult.Push(leftrez[1]);
                    return rezult;
                }
                if (r1_length <= pogr)
                {
                    rezult.Push(rightrez[0]);
                    return rezult;
                }
                if (r2_length <= pogr)
                {
                    rezult.Push(rightrez[1]);
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
            }   // конец while

            return new Stack<Angle3D>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inter"></param>
        /// <param name="P4"></param>
        /// <param name="alf"></param>
        /// <param name="bet"></param>
        /// <param name="tochnost"></param>
        /// <param name="leftboard"></param>
        /// <param name="rightboard"></param>
        /// <returns></returns>
        private Stack<Angle3D> FindOnInterval(int inter, Vertex3D P4, double alf, double bet, double tochnost, double leftboard, double rightboard)
        {
            for (int i = 1; i < inter; i++)
            {
                double delta = (rightboard - leftboard) / (double)i;
                for (int j = 0; j < i; j++)
                {
                    Stack<Angle3D> rez = Search(P4, alf, bet, tochnost, delta * j + leftboard, delta * (j + 1) + leftboard);
                    if (rez.Count != 0)
                    {
                        return rez;
                    }
                }
            }
            return new Stack<Angle3D>();
        }

        public Stack<Angle3D> InverseNab(double X, double Y, double Z, double X2, double Y2, double Z2)
        {
            double[][] mat = Mbase();                               /* Положение манипулятора при работе системы Манипулятор-Портал
                                                                       В отрицательной зоне параметр true                           */

            // Преобразование точек к локальной системе координат манипулятора
            double x_ = mat[0][0] * (X - basePoint.X) + mat[1][0] * (Y - basePoint.Y) + mat[2][0] * (Z - basePoint.Z);
            double y_ = mat[0][1] * (X - basePoint.X) + mat[1][1] * (Y - basePoint.Y) + mat[2][1] * (Z - basePoint.Z);
            double z_ = mat[0][2] * (X - basePoint.X) + mat[1][2] * (Y - basePoint.Y) + mat[2][2] * (Z - basePoint.Z);

            // Точка в новой системе координат
            Vertex3D P4 = new Vertex3D
            {
                X = x_,
                Y = y_,
                Z = z_
            };

            x_ = mat[0][0] * (X2 - basePoint.X) + mat[1][0] * (Y2 - basePoint.Y) + mat[2][0] * (Z2 - basePoint.Z);
            y_ = mat[0][1] * (X2 - basePoint.X) + mat[1][1] * (Y2 - basePoint.Y) + mat[2][1] * (Z2 - basePoint.Z);
            z_ = mat[0][2] * (X2 - basePoint.X) + mat[1][2] * (Y2 - basePoint.Y) + mat[2][2] * (Z2 - basePoint.Z);

            Vertex3D Pn = new Vertex3D
            {
                X = x_,
                Y = y_,
                Z = z_
            };
            // Получение углов альфа и бета
            double[] aandb = GetAandB(P4, Pn);
            double alf = -aandb[0];
            double bet = aandb[1];
            //Точность вычисления
            double tochnost = 0.000000000001;

            Stack<Angle3D> rez  = Search(P4, alf, bet, tochnost, 0, Math.PI / 2);
            Stack<Angle3D> rez2 = Search(P4, alf, bet, tochnost, Math.PI / 2, Math.PI);
            Stack<Angle3D> rez3 = Search(P4, alf, bet, tochnost, Math.PI, Math.PI * 3 / 2.0);
            Stack<Angle3D> rez4 = Search(P4, alf, bet, tochnost, Math.PI * 3 / 2.0, Math.PI * 2);
            if (rez2.Count >= 1)
            {
                rez.Push(rez2.Pop());
            }
            if (rez3.Count >= 1)
            {
                rez.Push(rez3.Pop());
            }
            if (rez4.Count >= 1)
            {
                rez.Push(rez4.Pop());
            }
            if (rez.Count >= 2)
            {
                return rez;
            }
            if (rez.Count <= 1)
            {
                rez = FindOnInterval(16, P4, alf, bet, tochnost, 0, Math.PI);
                rez2 = FindOnInterval(16, P4, alf, bet, tochnost, Math.PI, Math.PI * 2);
                if (rez2.Count >= 1)
                {
                    rez.Push(rez2.Pop());
                    return rez;
                }
            }
            return rez;
        }

        private Vertex3D GetP34(Vertex3D P4, double alf, double bet)
        {
            return new Vertex3D
            {
                X = P4.X - l5 * Math.Cos(alf) * Math.Cos(bet),
                Y = P4.Y - l5 * Math.Cos(alf) * Math.Sin(bet),
                Z = P4.Z + l5 * Math.Sin(alf)
            };
        }

        private Vertex3D GetP3(double i, Vertex3D P4, double alf, double bet)
        {
            double[][] R = Matrix(mrotate_Z(bet), mrotate_Y(alf));
            R[0][3] = P4.X;
            R[1][3] = P4.Y;
            R[2][3] = P4.Z;

            R = Matrix(R, mmove_X(-l5));

            double[][] Rt = Matrix(R, mrotate_X(i));
            Rt = Matrix(Rt, mmove_Z(-l4));

            Vertex3D point = new Vertex3D
            {
                X = Rt[0][3],
                Y = Rt[1][3],
                Z = Rt[2][3]
            };
            return point;
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

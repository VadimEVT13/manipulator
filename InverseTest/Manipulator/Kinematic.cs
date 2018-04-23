using InverseTest.Manipulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
    Заметка: 
        поднять проверку досягаемости до заданной точки выше по уровням!
        то есть поднять с низкого уровня на более высокий, точка входа инверсной функции!
*/

namespace InverseTest.Manipulator
{
    public class Kinematic
    {
        // точка установки манипулятора
        public Vertex3D basePoint = new Vertex3D();

        // длины портала
        private LengthJoin length = new LengthJoin();

        // углы
        private Angle3D angles = new Angle3D();

        // ограничение
        private Ogranichenie ogranich = new Ogranichenie();

        public Kinematic(Vertex3D Base)
        {
            basePoint = Base;
            ogranich = new Ogranichenie()
            {
                O1min = -Math.PI / 2.0,
                O1max = Math.PI / 2.0,
                O2min = -Math.PI / 2.0,
                O2max = Math.PI / 2.0,
                O3min = -Math.PI / 2.0,
                O3max = Math.PI / 2.0,
                O4min = -Math.PI,
                O4max = Math.PI,
                O5min = -10 * Math.PI / 180.0,
                O5max = Math.PI
            };
        }

        public bool SetLen(LengthJoin joins)
        {
            bool result = joins.J1 >= 0 && joins.J2 >= 0 && joins.J3 >= 0 && joins.J4 >= 0 && joins.J5 >= 0;
            if (result)
            {
                length = joins;
            }
            return result;
        }

        public LengthJoin GetLen()
        {
            return length;
        }

        public bool SetBase(Vertex3D Base)
        {
            basePoint = Base;
            return true;
        }

        public bool SetOgranichenie(Ogranichenie Ogran)
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

        private Vertex3D GP2(Vertex3D P3)
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

        private void Go4(Vertex3D P4)
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

        private void Go5(Vertex3D P4)
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
        public Angle3D GetAngles()
        {
            return angles;
        }

        private double[][] newm5(double o5)
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
        /*
        private Stack<Vertex3D> NewgP3(Vertex3D P4, out Vertex3D P34, double a, double b)
        {
            Vertex3D value = new Vertex3D
            {
                X = P4.X - length.J5 * Math.Cos(a) * Math.Cos(b),
                Y = P4.Y - length.J5 * Math.Cos(a) * Math.Sin(b),
                Z = P4.Z + length.J5 * Math.Sin(a)
            };
            P34 = value;

            double[][] R = Matrix(mrotate_Z(b), mrotate_Y(a));
            R[0][3] = P4.X;
            R[1][3] = P4.Y;
            R[2][3] = P4.Z;

            R = Matrix(R, mmove_X(-length.J5));

            Stack<Vertex3D> P3stack = new Stack<Vertex3D>();

            for (double i = -180 / 180.0 * Math.PI; i < 180 / 180.0 * Math.PI; i += 0.1 / 180.0 * Math.PI)
            {
                double[][] Rt = Matrix(R, mrotate_X(i));
                Rt = Matrix(Rt, mmove_Z(-length.J4));
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
        */

        private double[] GetAandB(Vertex3D P4, Vertex3D Pnab)
        {
            double x = Pnab.X - P4.X;
            double y = Pnab.Y - P4.Y;
            double z = Pnab.Z - P4.Z;

            double b = GetAngle(x, y);
            double a = GetAngle(Math.Sqrt(x * x + y * y), z);

            return new double[2] { a, b };
        }
        /*
        public Stack<Angle3D> Inverse(double X, double Y, double Z, double alf = 0, double bet = 0)
        {
            Stack<Angle3D> rezultAngles = new Stack<Angle3D>();   // Стек решений кинематики
            double[][] mat = Mbase();                               

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
                if (Math.Sqrt(point.X * point.X + point.Y * point.Y + (point.Z - length.J1) * (point.Z - length.J1)) <= length.J2 + length.J3)
                {
                    Vertex3D P1 = GP1(point);                       // Получение точки P1 и получение обобщенной координаты O1
                    Vertex3D P2 = GP2(point);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                    // Ниже условие, если получили точку то продолжаем 
                    if (P2 != null)
                    {
                        Go4(P34);
                        Go5(P34);

                        double[] angle4 = { angles.O4, 0 };
                        double[] angle5 = { -angles.O5 + Math.PI / 2, 0 };

                        if (angles.O4 > Math.PI)
                            angles.O4 = angles.O4 - Math.PI;
                        else
                        {
                            angles.O4 = angles.O4 + Math.PI;
                        }
                        angle4[1] = angles.O4;
                        Go5(P34);
                        angle5[1] = -angles.O5 + Math.PI / 2;

                        // Ниже выполненна прямая кинематика манипулятора
                        double[][] R = Matrix(M1(angles.O1, length.J1), M2(angles.O2, length.J2));
                        R = Matrix(R, M3(angles.O3, length.J3));

                        R = Matrix(R, mmove_Z(length.Det));

                        R = Matrix(R, M4(angle4[0]));
                        R = Matrix(R, newm5(angle5[0]));
                        R = Matrix(R, mmove_Z(length.J4));
                        R = Matrix(R, mmove_X(length.J5));

                        double pogr = 0.1;

                        angles.O4 = angle4[0];
                        angles.O5 = angle5[0];
                        // Если отклонения от матрицы манипулятора Т меньше нормы, то записываем в результат
                        if (Math.Abs(R[0][0] - T[0][0]) < pogr && Math.Abs(R[1][0] - T[1][0]) < pogr && Math.Abs(R[2][0] - T[2][0]) < pogr)
                        {
                            rezultAngles.Push(GetAngles());
                        }

                        R = Matrix(M1(angles.O1, length.J1), M2(angles.O2, length.J2));
                        R = Matrix(R, M3(angles.O3, length.J3));

                        R = Matrix(R, mmove_Z(length.Det));

                        R = Matrix(R, M4(angle4[1]));
                        R = Matrix(R, newm5(angle5[1]));
                        R = Matrix(R, mmove_Z(length.J4));
                        R = Matrix(R, mmove_X(length.J5));

                        angles.O4 = angle4[1];
                        angles.O5 = angle5[1];
                        if (Math.Abs(R[0][0] - T[0][0]) < pogr && Math.Abs(R[1][0] - T[1][0]) < pogr && Math.Abs(R[2][0] - T[2][0]) < pogr)
                        {
                            rezultAngles.Push(GetAngles());
                        }
                    }
                }
            }
            return rezultAngles;
        }
        */
        public double[][] DirectKinematic(Angle3D angles)
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
        /*
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

                    P3 = GetP3((double)leftmid, P4, out P34, alf, bet);
                    if (Math.Sqrt(P3.X * P3.X + P3.Y * P3.Y + (P3.Z - length.J1) * (P3.Z - length.J1)) <= length.J2 + length.J3)
                    {
                        P1 = GP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                        P2 = GP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                                                            // Ниже условие, если получили точку то продолжаем 
                        if (P2 != null)
                        {
                            Go4(P34);                                   // Получение обобщенной координаты O4
                            Go5(P34);                                   // Получение обобщенной координаты O5

                            double[] angle4 = { angles.O4, 0 };
                            double[] angle5 = { -angles.O5 + Math.PI / 2, 0 };
                            if (angles.O4 > 0)
                                angles.O4 = angles.O4 - Math.PI;
                            else
                            {
                                angles.O4 = angles.O4 + Math.PI;
                            }
                            angle4[1] = angles.O4;
                            Go5(P34);
                            angle5[1] = -angles.O5 + Math.PI / 2;

                            angles.O4 = angle4[0];
                            angles.O5 = angle5[0];

                            midonly.Add(new Angle3D(GetAngles()));

                            angles.O4 = angle4[1];
                            angles.O5 = angle5[1];

                            midonly.Add(new Angle3D(GetAngles()));

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
                P3 = GetP3((double)leftmid, P4, out P34, alf, bet);
                if (Math.Sqrt(P3.X * P3.X + P3.Y * P3.Y + (P3.Z - length.J1) * (P3.Z - length.J1)) <= length.J2 + length.J3)
                {
                    P1 = GP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                    P2 = GP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                    // Ниже условие, если получили точку то продолжаем 
                    if (P2 != null)
                    {
                        Go4(P34);                                   // Получение обобщенной координаты O4
                        Go5(P34);                                   // Получение обобщенной координаты O5

                        double[] angle4 = { angles.O4, 0 };
                        double[] angle5 = { -angles.O5 + Math.PI / 2, 0 };
                        if (angles.O4 > 0)
                            angles.O4 = angles.O4 - Math.PI;
                        else
                        {
                            angles.O4 = angles.O4 + Math.PI;
                        }
                        angle4[1] = angles.O4;
                        Go5(P34);
                        angle5[1] = -angles.O5 + Math.PI / 2;

                        angles.O4 = angle4[0];
                        angles.O5 = angle5[0];

                        leftrez.Add(new Angle3D(GetAngles()));

                        angles.O4 = angle4[1];
                        angles.O5 = angle5[1];

                        leftrez.Add(new Angle3D(GetAngles()));
                    }
                }

                // ДЛЯ ПРАВОГО
                P3 = GetP3((double)rightmid, P4, out P34, alf, bet);
                if (Math.Sqrt(P3.X * P3.X + P3.Y * P3.Y + (P3.Z - length.J1) * (P3.Z - length.J1)) <= length.J2 + length.J3)
                {
                    P1 = GP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                    P2 = GP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                    // Ниже условие, если получили точку то продолжаем 
                    if (P2 != null)
                    {
                        Go4(P34);                                   // Получение обобщенной координаты O4
                        Go5(P34);                                   // Получение обобщенной координаты O5

                        double[] angle4 = { angles.O4, 0 };
                        double[] angle5 = { -angles.O5 + Math.PI / 2, 0 };
                        if (angles.O4 > 0)
                            angles.O4 = angles.O4 - Math.PI;
                        else
                        {
                            angles.O4 = angles.O4 + Math.PI;
                        }
                        angle4[1] = angles.O4;
                        Go5(P34);
                        angle5[1] = -angles.O5 + Math.PI / 2;

                        angles.O4 = angle4[0];
                        angles.O5 = angle5[0];

                        rightrez.Add(new Angle3D(GetAngles()));

                        angles.O4 = angle4[1];
                        angles.O5 = angle5[1];

                        rightrez.Add(new Angle3D(GetAngles()));
                    }
                }

                double l1_length = double.MaxValue;
                double l2_length = double.MaxValue;
                double r1_length = double.MaxValue;
                double r2_length = double.MaxValue;

                if (leftrez.Count >= 1)
                {
                    double[][] LR1 = DirectKinematic(leftrez[0]);
                    l1_length = Math.Sqrt((LR1[0][3] - P4.X) * (LR1[0][3] - P4.X)
                                           + (LR1[1][3] - P4.Y) * (LR1[1][3] - P4.Y)
                                           + (LR1[2][3] - P4.Z) * (LR1[2][3] - P4.Z));
                }
                if (leftrez.Count == 2)
                {
                    double[][] LR2 = DirectKinematic(leftrez[1]);
                    l2_length = Math.Sqrt((LR2[0][3] - P4.X) * (LR2[0][3] - P4.X)
                                           + (LR2[1][3] - P4.Y) * (LR2[1][3] - P4.Y)
                                           + (LR2[2][3] - P4.Z) * (LR2[2][3] - P4.Z));
                }
                if (rightrez.Count >= 1)
                {
                    double[][] RR1 = DirectKinematic(rightrez[0]);
                    r1_length = Math.Sqrt((RR1[0][3] - P4.X) * (RR1[0][3] - P4.X)
                                               + (RR1[1][3] - P4.Y) * (RR1[1][3] - P4.Y)
                                               + (RR1[2][3] - P4.Z) * (RR1[2][3] - P4.Z));
                }
                if (rightrez.Count >= 2)
                {
                    double[][] RR2 = DirectKinematic(rightrez[1]);
                    r2_length = Math.Sqrt((RR2[0][3] - P4.X) * (RR2[0][3] - P4.X)
                                           + (RR2[1][3] - P4.Y) * (RR2[1][3] - P4.Y)
                                           + (RR2[2][3] - P4.Z) * (RR2[2][3] - P4.Z));
                }

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
        */
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
        /*
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
        */
        /*
        public Stack<Angle3D> InverseNab(double X, double Y, double Z, double X2, double Y2, double Z2)
        {
            double[][] mat = Mbase();                               // Положение манипулятора при работе системы Манипулятор-Портал
                                                                    // В отрицательной зоне параметр true                           

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

            Stack<Angle3D> rez = Search(P4, alf, bet, tochnost, 0, Math.PI / 2);
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

            foreach (Angle3D one in rez)
            {
                if (one.O5 < 0)
                    one.O5 += Math.PI;
            }

            return rez;
        }
        */

        private Vertex3D GetP3(double i, Vertex3D P4, out Vertex3D P34, double alf, double bet)
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

        private Vertex3D Modiffy(Vertex3D Point, Vertex3D basePoint)
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
        private Angle3D InverseNab(double iter, double X, double Y, double Z, double X2, double Y2, double Z2)
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

            //----------------------------------------------------------------------------------------------------------------------
            Vertex3D P3 = GetP3(iter, P4, out Vertex3D P34, alf, bet); // Получение множества точек P3

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

        private Angle3D Golden_section_search(double A, double B, double eps, Vertex3D Pnab, Vertex3D Pman)
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

        private Angle3D Golden_section_search(int[] Command, double A, double B, Angle3D Angles, double[][] T)
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

        public Angle3D CCD(Vertex3D P4, Vertex3D Pn)
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

        //public Stack<Angle3D> Search_with_gold(Vertex3D Pnab, Vertex3D P4, double eps)
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
        */
        /// <summary>
        /// Добавил для адаптации!!!
        /// Пока основная функция поиска!!!
        /// То есть поправки есть!!!
        /// </summary>
        /// <param name="Pnab"></param>
        /// <param name="P4"></param>
        /// <returns></returns>
        public Stack<Angle3D> InverseNab(double X1, double Y1, double Z1, double X2, double Y2, double Z2)
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

        public double[][] DirectKinematic(Angle3D angles, Vertex3D basepoint)
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

        public void map(Vertex3D P4, Vertex3D Pn)
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
        /*   
        private Matrix4D DirectKinematic(Angle3D angles, Vertex3D basepoint)
        {
            Matrix4D mat = Matrix4D.MB();

            Matrix4D R = Matrix4D.Multiply(Matrix4D.M1(angles.O1, length.J1), Matrix4D.M2(angles.O2, length.J2));
            R = Matrix4D.Multiply(R, Matrix4D.M3(angles.O3, length.J3));
            R = Matrix4D.Multiply(R, Matrix4D.MZ(length.Det));
            R = Matrix4D.Multiply(R, Matrix4D.M4(angles.O4));
            R = Matrix4D.Multiply(R, Matrix4D.M5(angles.O5));
            R = Matrix4D.Multiply(R, Matrix4D.MZ(length.J4));
            R = Matrix4D.Multiply(R, Matrix4D.MX(length.J5));
            
            R.K13   = mat.K11 * (R.K14 + basepoint.X) +
                      mat.K21 * (R.K24 + basepoint.Y) +
                      mat.K31 * (R.K34 + basepoint.Z);

            R.K23   = mat.K12 * (R.K14 + basepoint.X) +
                      mat.K22 * (R.K24 + basepoint.Y) +
                      mat.K32 * (R.K34 + basepoint.Z);

            R.K33   = mat.K13 * (R.K14 + basepoint.X) +
                      mat.K23 * (R.K24 + basepoint.Y) +
                      mat.K33 * (R.K34 + basepoint.Z);

            return R;
        }
        */
    }
}
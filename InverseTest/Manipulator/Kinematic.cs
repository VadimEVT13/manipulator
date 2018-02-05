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
        private Vector5D len = new Vector5D();

        public double det = 0;
        // углы
        private Vector5D ang = new Vector5D();
        /*double o1 = 0;
        double o2 = 0;
        double o3 = 0;
        double o4 = 0;
        double o5 = 0;//*/

        public Kinematic(double X = 0, double Y = 0, double Z = 0)
        {
            basePoint.X = X;
            basePoint.Y = Y;
            basePoint.Z = Z;
        }

        public void SetLen(Vector5D setLen)
        {
            if (setLen.K1 >= 0 && setLen.K2 >= 0 && setLen.K3 >= 0 && setLen.K4 >= 0 && setLen.K5 >= 0)
            {
                len = setLen;
            }
        }

        public Vector5D GetLen()
        {
            return len;
        }

        public void SetBase(Vertex3D setBase)
        {
            basePoint = setBase;
        }

        public Vertex3D GetBase()
        {
            return basePoint;
        }

        /// <summary>
        /// Геттер углов.
        /// </summary>
        /// <returns>углы</returns>
        public Vector5D GetAngles()
        {
            return new Vector5D
            {
                K1 = ang.K1,
                K2 = ang.K2,
                K3 = ang.K3,
                K4 = ang.K4,
                K5 = ang.K5
            };
        }



        /// <summary>
        /// Задаём матрицы поворота от угла и дистанции
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="r1"></param>
        /// <returns></returns>
        private Matrix4D M1(double o1, double r1)
        {
            return new Matrix4D
            {
                K00 = Math.Cos(o1),
                K01 = -Math.Sin(o1),
                K10 = Math.Sin(o1),
                K11 = Math.Cos(o1),
                K22 = 1,
                K23 = r1,
                K33 = 1
            };
        }

        /// <summary>
        /// Задаём матрицы поворота от угла и дистанции
        /// </summary>
        /// <param name="o2"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        private Matrix4D M2(double o2, double r2)
        {
            return new Matrix4D
            {
                K00 = Math.Cos(o2),
                K02 = Math.Sin(o2),
                K03 = r2 * Math.Sin(o2),
                K11 = 1,
                K20 = -Math.Sin(o2),
                K22 = Math.Cos(o2),
                K23 = r2 * Math.Cos(o2),
                K33 = 1
            };
        }

        /// <summary>
        /// Задаём матрицы поворота от угла и дистанции
        /// </summary>
        /// <param name="o3"></param>
        /// <param name="r3"></param>
        /// <returns></returns>
        private Matrix4D M3(double o3, double r3)
        {
            return new Matrix4D
            {
                K00 = Math.Cos(o3),
                K02 = Math.Sin(o3),
                K03 = r3 * Math.Cos(o3),
                K11 = 1,
                K20 = -Math.Sin(o3),
                K22 = Math.Cos(o3),
                K23 = -r3 * Math.Sin(o3),
                K33 = 1
            };
        }

        /// <summary>
        /// Задаём матрицы поворота от угла и дистанции
        /// </summary>
        /// <param name="o4"></param>
        /// <returns></returns>
        private Matrix4D M4(double o4)
        {
            return new Matrix4D
            {
                K00 = 1,
                K11 = Math.Cos(o4),
                K12 = -Math.Sin(o4),
                K21 = Math.Sin(o4),
                K22 = Math.Cos(o4),
                K33 = 1
            };
        }

        /// <summary>
        /// Задаём матрицы поворота от угла и дистанции
        /// </summary>
        /// <param name="o5"></param>
        /// <param name="r4"></param>
        /// <returns></returns>
        private Matrix4D M5(double o5, double r4)
        {
            return new Matrix4D
            {
                K00 = Math.Cos(o5),
                K02 = Math.Sin(o5),
                K03 = r4 * Math.Cos(o5),
                K11 = 1,
                K20 = -Math.Sin(o5),
                K22 = Math.Cos(o5),
                K23 = -r4 * Math.Sin(o5),
                K33 = 1
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        private Matrix4D Mbase(bool flag = true)
        {
            double value;
            if (flag)
            {
                value = 1;
            }
            else
            {
                value = -1;
            }
            return new Matrix4D()
            {
                K00 = value,
                K11 = value,
                K22 = 1,
                K33 = 1
            };
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
            ang.K1 = GetAngle(P3.X, P3.Y);
            if (ang.K1 > Math.PI / 4)
            {
                ang.K1 = ang.K1 - Math.PI;
            }
            else if (ang.K1 < -Math.PI / 4)
            {
                ang.K1 = ang.K1 + Math.PI;
            }
            Manipulator.Matrix.m1 = M1(ang.K1, len.K1);
            Vertex3D point = new Vertex3D
            {
                X = Manipulator.Matrix.m1.K03,
                Y = Manipulator.Matrix.m1.K13,
                Z = Manipulator.Matrix.m1.K23
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
            double z = P3.Z - len.K1;

            double L = Math.Sqrt(P3.X * P3.X + P3.Y * P3.Y + (P3.Z - len.K1) * (P3.Z - len.K1));
            // Так как появлиось det, то и длинны меняются
            double L3 = Math.Sqrt(len.K3 * len.K3 + det * det);

            //углы по трём сторонам
            if (L != 0)
            {
                double A = Math.Acos((len.K2 * len.K2 + L * L - L3 * L3) / (2 * len.K2 * L));
                double B = Math.Acos((L3 * L3 + L * L - len.K2 * len.K2) / (2 * L3 * L));
                double G = Math.PI - A - B;

                double o = GetAngle(x, z);
                ang.K2 = Math.PI / 2 - (o + A);

                Manipulator.Matrix.m2 = M2(ang.K2, len.K2);

                Matrix4D R = Matrix4D.Multiply(Manipulator.Matrix.m1, Manipulator.Matrix.m2);

                Vertex3D point = new Vertex3D
                {
                    X = R.K03,
                    Y = R.K13,
                    Z = R.K23
                };
                double t = GetAngle(len.K3, det);
                ang.K3 = Math.PI / 2 - G + GetAngle(len.K3, det);
                Manipulator.Matrix.m3 = Matrix4D.Multiply(M3(ang.K3, len.K3), Mmove_Z(det));
                R = Matrix4D.Multiply(R, Manipulator.Matrix.m3);
                return point;
            }
            else
            {
                return null;
            }
        }

        private void Go4(Vertex3D P4)
        {
            Matrix4D R2 = Matrix4D.Multiply(Manipulator.Matrix.m1, Manipulator.Matrix.m2);
            R2 = Matrix4D.Multiply(R2, Manipulator.Matrix.m3);
            double newY = R2.K01 * (P4.X - R2.K03) + R2.K11 * (P4.Y - R2.K13) + R2.K21 * (P4.Z - R2.K23);
            double newZ = R2.K02 * (P4.X - R2.K03) + R2.K12 * (P4.Y - R2.K13) + R2.K22 * (P4.Z - R2.K23);
            // Было отрицательным
            ang.K4 = -GetAngle(newZ, newY);
            Manipulator.Matrix.m4 = M4(ang.K4);
            R2 = Matrix4D.Multiply(R2, Manipulator.Matrix.m4);
        }

        private void Go5(Vertex3D P4)
        {
            Matrix4D R2 = Matrix4D.Multiply(Manipulator.Matrix.m1, Manipulator.Matrix.m2);
            R2 = Matrix4D.Multiply(R2, Manipulator.Matrix.m3);
            R2 = Matrix4D.Multiply(R2, M4(ang.K4));
            double newX = R2.K00 * (P4.X - R2.K03) + R2.K10 * (P4.Y - R2.K13) + R2.K20 * (P4.Z - R2.K23);
            double newZ = R2.K02 * (P4.X - R2.K03) + R2.K12 * (P4.Y - R2.K13) + R2.K22 * (P4.Z - R2.K23);
            ang.K5 = GetAngle(newX, newZ);
            Manipulator.Matrix.m5 = M5(ang.K5, len.K5);
        }

        private Matrix4D Newm5(double o5)
        {
            return new Matrix4D
            {
                K00 = Math.Cos(o5),
                K02 = Math.Sin(o5),
                K11 = 1,
                K20 = -Math.Sin(o5),
                K22 = Math.Cos(o5),
                K33 = 1
            };
        }

        private Matrix4D Mrotate_Z(double o)
        {
            return new Matrix4D
            {
                K00 = Math.Cos(o),
                K01 = -Math.Sin(o),
                K10 = Math.Sin(o),
                K11 = Math.Cos(o),
                K22 = 1,
                K33 = 1
            };
        }

        private Matrix4D Mrotate_Y(double o)
        {
            return new Matrix4D
            {
                K00 = Math.Cos(o),
                K02 = Math.Sin(o),
                K11 = 1,
                K20 = -Math.Sin(o),
                K22 = Math.Cos(o),
                K33 = 1
            };
        }

        private Matrix4D Mrotate_X(double o)
        {
            return new Matrix4D
            {
                K00 = 1,
                K11 = Math.Cos(o),
                K12 = -Math.Sin(o),
                K21 = Math.Sin(o),
                K22 = Math.Cos(o),
                K33 = 1
            };
        }

        private Matrix4D Mmove_X(double x)
        {
            return new Matrix4D
            {
                K00 = 1,
                K03 = x,
                K11 = 1,
                K22 = 1,
                K33 = 1
            };
        }

        private Matrix4D Mmove_Z(double z)
        {
            return new Matrix4D
            {
                K00 = 1,
                K11 = 1,
                K22 = 1,
                K23 = z,
                K33 = 1
            };
        }

        // Матрица T или матрица манипулятора
        private Matrix4D Mt(double alfa, double beta, Vertex3D point)
        {
            return new Matrix4D
            {
                K00 = Math.Cos(beta) * Math.Cos(alfa),
                K01 = Math.Sin(beta),
                K02 = Math.Cos(beta) * Math.Sin(alfa),
                K03 = point.X,
                K10 = Math.Sin(beta) * Math.Cos(alfa),
                K11 = Math.Cos(beta),
                K12 = Math.Sin(beta) * Math.Sin(alfa),
                K13 = point.Y,
                K20 = -Math.Sin(alfa),
                K22 = Math.Cos(alfa),
                K23 = point.Z,
                K33 = 1
            };
        }

        private Stack<Vertex3D> NewgP3(Vertex3D P4, out Vertex3D P34, double a, double b)
        {
            Vertex3D value = new Vertex3D
            {
                X = P4.X - len.K5 * Math.Cos(a) * Math.Cos(b),
                Y = P4.Y - len.K5 * Math.Cos(a) * Math.Sin(b),
                Z = P4.Z + len.K5 * Math.Sin(a)
            };
            P34 = value;
            Matrix4D R = Matrix4D.Multiply(Mrotate_Z(b), Mrotate_Y(a));
            R.K03 = P4.X;
            R.K13 = P4.Y;
            R.K23 = P4.Z;
            R = Matrix4D.Multiply(R, Mmove_X(-len.K5));
            Stack<Vertex3D> P3stack = new Stack<Vertex3D>();
            for (double i = -180 / 180.0 * Math.PI; i < 180 / 180.0 * Math.PI; i += 0.1 / 180.0 * Math.PI)
            {
                Matrix4D Rt = Matrix4D.Multiply(R, Mrotate_X(i));
                Rt = Matrix4D.Multiply(Rt, Mmove_Z(-len.K4));
                Vertex3D point = new Vertex3D
                {
                    X = Rt.K03,
                    Y = Rt.K13,
                    Z = Rt.K23
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

        public Stack<Vector5D> Inverse(double X, double Y, double Z, double alf = 0, double bet = 0)
        {
            Stack<Vector5D> rezultAngles = new Stack<Vector5D>();   // Стек решений кинематики
            Matrix4D mat = Mbase();                               /* Положение манипулятора при работе системы Манипулятор-Портал
                                                                       В отрицательной зоне параметр true                           */

            // Преобразование точек к локальной системе координат манипулятора
            double x_ = mat.K00 * (X - basePoint.X) + mat.K10 * (Y - basePoint.Y) + mat.K20 * (Z - basePoint.Z);
            double y_ = mat.K01 * (X - basePoint.X) + mat.K11 * (Y - basePoint.Y) + mat.K21 * (Z - basePoint.Z);
            double z_ = mat.K02 * (X - basePoint.X) + mat.K12 * (Y - basePoint.Y) + mat.K22 * (Z - basePoint.Z);

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

            Matrix4D T = Mt(alf, bet, P4);                        // Определение матрицы манипулятора

            //----------------------------------------------------------------------------------------------------------------------
            Stack<Vertex3D> P3mass = NewgP3(P4, out Vertex3D P34, alf, bet); // Получение множества точек P3

            foreach (Vertex3D point in P3mass)                      // Для каждой такой точки P3 ищем решение кинематики
            {
                // Ниже идёт проверка на достижимость до точки P3 манипулятором                
                if (Math.Sqrt(point.X * point.X + point.Y * point.Y + (point.Z - len.K1) * (point.Z - len.K1)) <= len.K2 + len.K3)
                {
                    Vertex3D P1 = GP1(point);                       // Получение точки P1 и получение обобщенной координаты O1
                    Vertex3D P2 = GP2(point);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                    // Ниже условие, если получили точку то продолжаем 
                    if (P2 != null)
                    {
                        Go4(P34);
                        Go5(P34);

                        double[] angle4 = { ang.K4, 0 };
                        double[] angle5 = { -ang.K5 + Math.PI / 2, 0 };

                        if (ang.K4 > Math.PI)
                        {
                            ang.K4 = ang.K4 - Math.PI;
                        }
                        else
                        {
                            ang.K4 = ang.K4 + Math.PI;
                        }
                        angle4[1] = ang.K4;
                        Go5(P34);
                        angle5[1] = -ang.K5 + Math.PI / 2;

                        // Ниже выполненна прямая кинематика манипулятора
                        Matrix4D R = Matrix4D.Multiply(M1(ang.K1, len.K1), M2(ang.K2, len.K2));
                        R = Matrix4D.Multiply(R, M3(ang.K3, len.K3));
                        R = Matrix4D.Multiply(R, Mmove_Z(det));
                        R = Matrix4D.Multiply(R, M4(angle4[0]));
                        R = Matrix4D.Multiply(R, Newm5(angle5[0]));
                        R = Matrix4D.Multiply(R, Mmove_Z(len.K4));
                        R = Matrix4D.Multiply(R, Mmove_X(len.K5));

                        double pogr = 0.1;

                        ang.K4 = angle4[0];
                        ang.K5 = angle5[0];
                        // Если отклонения от матрицы манипулятора Т меньше нормы, то записываем в результат
                        if (Math.Abs(R.K00 - T.K00) < pogr && Math.Abs(R.K10 - T.K10) < pogr && Math.Abs(R.K20 - T.K20) < pogr)
                        {
                            rezultAngles.Push(GetAngles());
                        }

                        R = Matrix4D.Multiply(M1(ang.K1, len.K1), M2(ang.K2, len.K2));
                        R = Matrix4D.Multiply(R, M3(ang.K3, len.K3));
                        R = Matrix4D.Multiply(R, M4(angle4[1]));
                        R = Matrix4D.Multiply(R, Newm5(angle5[1]));
                        R = Matrix4D.Multiply(R, Mmove_Z(len.K4));
                        R = Matrix4D.Multiply(R, Mmove_X(len.K5));
                        ang.K4 = angle4[1];
                        ang.K5 = angle5[1];
                        if (Math.Abs(R.K00 - T.K00) < pogr && Math.Abs(R.K10 - T.K10) < pogr && Math.Abs(R.K20 - T.K20) < pogr)
                        {
                            rezultAngles.Push(GetAngles());
                        }
                    }
                }
            }
            return rezultAngles;
        }

        public Matrix4D DirectKinematic(Vector5D angles)
        {
            Matrix4D R = Matrix4D.Multiply(M1(angles.K1, len.K1), M2(angles.K2, len.K2));
            R = Matrix4D.Multiply(R, M3(angles.K3, len.K3));
            R = Matrix4D.Multiply(R, Mmove_Z(det));
            R = Matrix4D.Multiply(R, M4(angles.K4));
            R = Matrix4D.Multiply(R, Newm5(angles.K5));
            R = Matrix4D.Multiply(R, Mmove_Z(len.K4));
            R = Matrix4D.Multiply(R, Mmove_X(len.K5));
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
        private Stack<Vector5D> Search(Vertex3D P4, double alf, double bet, double pogr, double leftboard, double rightdoard)
        {
            Stack<Vector5D> rezult = new Stack<Vector5D>();         // Результаты

            decimal left = (decimal)leftboard;                   // левая грань
            decimal right = (decimal)rightdoard;                    // правая грань

            double deviation = 10000;                               // отклонение
            double prev_deviation = 10001;

            while (deviation > pogr)                                // пока отклонение больше погрешности
            {
                if (prev_deviation == deviation)
                {
                    return new Stack<Vector5D>();
                }
                decimal mid = (left + right) / (decimal)2;    // середина отрезка
                decimal leftmid = (left + mid) / (decimal)2;    // значение левее середины
                decimal rightmid = (right + mid) / (decimal)2;    // значение правее середины

                Vertex3D P1 = new Vertex3D();
                Vertex3D P2 = new Vertex3D();
                Vertex3D P3 = new Vertex3D();
                Vertex3D P34 = new Vertex3D();

                List<Vector5D> leftrez = new List<Vector5D>();
                List<Vector5D> rightrez = new List<Vector5D>();

                if (deviation == 10000)
                {
                    List<Vector5D> midonly = new List<Vector5D>();

                    P34 = GetP34(P4, alf, bet);
                    P3 = GetP3((double)leftmid, P4, alf, bet);
                    if (Math.Sqrt(P3.X * P3.X + P3.Y * P3.Y + (P3.Z - len.K1) * (P3.Z - len.K1)) <= len.K2 + len.K3)
                    {
                        P1 = GP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                        P2 = GP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                                                            // Ниже условие, если получили точку то продолжаем 
                        if (P2 != null)
                        {
                            Go4(P34);                                   // Получение обобщенной координаты O4
                            Go5(P34);                                   // Получение обобщенной координаты O5

                            double[] angle4 = { ang.K4, 0 };
                            double[] angle5 = { -ang.K5 + Math.PI / 2, 0 };
                            if (ang.K4 > Math.PI)
                                ang.K4 = ang.K4 - Math.PI;
                            else
                            {
                                ang.K4 = ang.K4 + Math.PI;
                            }
                            angle4[1] = ang.K4;
                            Go5(P34);
                            angle5[1] = -ang.K5 + Math.PI / 2;

                            ang.K4 = angle4[0];
                            ang.K5 = angle5[0];

                            midonly.Add(GetAngles());

                            ang.K4 = angle4[1];
                            ang.K5 = angle5[1];

                            midonly.Add(GetAngles());

                            Matrix4D MR1 = DirectKinematic(midonly[0]);
                            Matrix4D MR2 = DirectKinematic(midonly[1]);

                            double ml1_length = Math.Sqrt((MR1.K03 - P4.X) * (MR1.K03 - P4.X)
                                                        + (MR1.K13 - P4.Y) * (MR1.K13 - P4.Y)
                                                        + (MR1.K23 - P4.Z) * (MR1.K23 - P4.Z));
                            double ml2_length = Math.Sqrt((MR2.K03 - P4.X) * (MR2.K03 - P4.X)
                                                        + (MR2.K13 - P4.Y) * (MR2.K13 - P4.Y)
                                                        + (MR2.K23 - P4.Z) * (MR2.K23 - P4.Z));

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
                if (Math.Sqrt(P3.X * P3.X + P3.Y * P3.Y + (P3.Z - len.K1) * (P3.Z - len.K1)) <= len.K2 + len.K3)
                {
                    P1 = GP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                    P2 = GP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                    // Ниже условие, если получили точку то продолжаем 
                    if (P2 != null)
                    {
                        Go4(P34);                                   // Получение обобщенной координаты O4
                        Go5(P34);                                   // Получение обобщенной координаты O5

                        double[] angle4 = { ang.K4, 0 };
                        double[] angle5 = { -ang.K5 + Math.PI / 2, 0 };
                        if (ang.K4 > Math.PI)
                            ang.K4 = ang.K4 - Math.PI;
                        else
                        {
                            ang.K4 = ang.K4 + Math.PI;
                        }
                        angle4[1] = ang.K4;
                        Go5(P34);
                        angle5[1] = -ang.K5 + Math.PI / 2;

                        ang.K4 = angle4[0];
                        ang.K5 = angle5[0];

                        leftrez.Add(GetAngles());

                        ang.K4 = angle4[1];
                        ang.K5 = angle5[1];

                        leftrez.Add(GetAngles());
                    }
                }

                // ДЛЯ ПРАВОГО
                P34 = GetP34(P4, alf, bet);
                P3 = GetP3((double)rightmid, P4, alf, bet);
                if (Math.Sqrt(P3.X * P3.X + P3.Y * P3.Y + (P3.Z - len.K1) * (P3.Z - len.K1)) <= len.K2 + len.K3)
                {
                    P1 = GP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                    P2 = GP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                    // Ниже условие, если получили точку то продолжаем 
                    if (P2 != null)
                    {
                        Go4(P34);                                   // Получение обобщенной координаты O4
                        Go5(P34);                                   // Получение обобщенной координаты O5

                        double[] angle4 = { ang.K4, 0 };
                        double[] angle5 = { -ang.K5 + Math.PI / 2, 0 };
                        if (ang.K4 > Math.PI)
                            ang.K4 = ang.K4 - Math.PI;
                        else
                        {
                            ang.K4 = ang.K4 + Math.PI;
                        }
                        angle4[1] = ang.K4;
                        Go5(P34);
                        angle5[1] = -ang.K5 + Math.PI / 2;

                        ang.K4 = angle4[0];
                        ang.K5 = angle5[0];

                        rightrez.Add(GetAngles());

                        ang.K4 = angle4[1];
                        ang.K5 = angle5[1];

                        rightrez.Add(GetAngles());
                    }
                }

                Matrix4D LR1 = DirectKinematic(leftrez[0]);
                Matrix4D LR2 = DirectKinematic(leftrez[1]);
                Matrix4D RR1 = DirectKinematic(rightrez[0]);
                Matrix4D RR2 = DirectKinematic(rightrez[1]);

                double l1_length = Math.Sqrt((LR1.K03 - P4.X) * (LR1.K03 - P4.X)
                                           + (LR1.K13 - P4.Y) * (LR1.K13 - P4.Y)
                                           + (LR1.K23 - P4.Z) * (LR1.K23 - P4.Z));
                double l2_length = Math.Sqrt((LR2.K03 - P4.X) * (LR2.K03 - P4.X)
                                           + (LR2.K13 - P4.Y) * (LR2.K13 - P4.Y)
                                           + (LR2.K23 - P4.Z) * (LR2.K23 - P4.Z));
                double r1_length = Math.Sqrt((RR1.K03 - P4.X) * (RR1.K03 - P4.X)
                                           + (RR1.K13 - P4.Y) * (RR1.K13 - P4.Y)
                                           + (RR1.K23 - P4.Z) * (RR1.K23 - P4.Z));
                double r2_length = Math.Sqrt((RR2.K03 - P4.X) * (RR2.K03 - P4.X)
                                           + (RR2.K13 - P4.Y) * (RR2.K13 - P4.Y)
                                           + (RR2.K23 - P4.Z) * (RR2.K23 - P4.Z));
                bool flag1 = false;
                bool flag2 = false;
                if (deviation == prev_deviation)
                {
                    return new Stack<Vector5D>();
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

            return new Stack<Vector5D>();
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
        private Stack<Vector5D> FindOnInterval(int inter, Vertex3D P4, double alf, double bet, double tochnost, double leftboard, double rightboard)
        {
            for (int i = 1; i < inter; i++)
            {
                double delta = (rightboard - leftboard) / (double)i;
                for (int j = 0; j < i; j++)
                {
                    Stack<Vector5D> rez = Search(P4, alf, bet, tochnost, delta * j + leftboard, delta * (j + 1) + leftboard);
                    if (rez.Count != 0)
                    {
                        return rez;
                    }
                }
            }
            return new Stack<Vector5D>();
        }

        public Stack<Vector5D> InverseNab(double X, double Y, double Z, double X2, double Y2, double Z2)
        {
            Matrix4D mat = Mbase();                               /* Положение манипулятора при работе системы Манипулятор-Портал
                                                                       В отрицательной зоне параметр true                           */

            // Преобразование точек к локальной системе координат манипулятора
            double x_ = mat.K00 * (X - basePoint.X) + mat.K10 * (Y - basePoint.Y) + mat.K20 * (Z - basePoint.Z);
            double y_ = mat.K01 * (X - basePoint.X) + mat.K11 * (Y - basePoint.Y) + mat.K21 * (Z - basePoint.Z);
            double z_ = mat.K02 * (X - basePoint.X) + mat.K12 * (Y - basePoint.Y) + mat.K22 * (Z - basePoint.Z);

            // Точка в новой системе координат
            Vertex3D P4 = new Vertex3D
            {
                X = x_,
                Y = y_,
                Z = z_
            };

            x_ = mat.K00 * (X2 - basePoint.X) + mat.K10 * (Y2 - basePoint.Y) + mat.K20 * (Z2 - basePoint.Z);
            y_ = mat.K01 * (X2 - basePoint.X) + mat.K11 * (Y2 - basePoint.Y) + mat.K21 * (Z2 - basePoint.Z);
            z_ = mat.K02 * (X2 - basePoint.X) + mat.K12 * (Y2 - basePoint.Y) + mat.K22 * (Z2 - basePoint.Z);

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

            Stack<Vector5D> rez  = Search(P4, alf, bet, tochnost, 0, Math.PI / 2);
            Stack<Vector5D> rez2 = Search(P4, alf, bet, tochnost, Math.PI / 2, Math.PI);
            Stack<Vector5D> rez3 = Search(P4, alf, bet, tochnost, Math.PI, Math.PI * 3 / 2.0);
            Stack<Vector5D> rez4 = Search(P4, alf, bet, tochnost, Math.PI * 3 / 2.0, Math.PI * 2);
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
                X = P4.X - len.K5 * Math.Cos(alf) * Math.Cos(bet),
                Y = P4.Y - len.K5 * Math.Cos(alf) * Math.Sin(bet),
                Z = P4.Z + len.K5 * Math.Sin(alf)
            };
        }

        private Vertex3D GetP3(double i, Vertex3D P4, double alf, double bet)
        {
            Matrix4D R = Matrix4D.Multiply(Mrotate_Z(bet), Mrotate_Y(alf));
            R.K03 = P4.X;
            R.K13 = P4.Y;
            R.K23 = P4.Z;
            R = Matrix4D.Multiply(R, Mmove_X(-len.K5));
            Matrix4D Rt = Matrix4D.Multiply(R, Mrotate_X(i));
            Rt = Matrix4D.Multiply(Rt, Mmove_Z(-len.K4));
            Vertex3D point = new Vertex3D
            {
                X = Rt.K03,
                Y = Rt.K13,
                Z = Rt.K23
            };
            return point;
        }
    }

    static class Matrix
    {
        public static Matrix4D m1;
        public static Matrix4D m2;
        public static Matrix4D m3;
        public static Matrix4D m4;
        public static Matrix4D m5;
    }
}

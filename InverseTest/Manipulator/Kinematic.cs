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
        /// <summary>
        /// Точка установки манипулятора.
        /// </summary>
        public Vector3D Base { get; set; }
        /// <summary>
        /// Длины портала.
        /// </summary>
        public Vector5D Len { get; set; }
        /// <summary>
        /// Углы портала.
        /// </summary>
        public Vector5D Angles { get; set; }
        public double Det { get; set; }
        private Matrix4D m1;
        private Matrix4D m2;
        private Matrix4D m3;

        public Kinematic(double setX = 0, double setY = 0, double setZ = 0)
        {
            Base = new Vector3D()
            {
                X = setX,
                Y = setY,
                Z = setZ
            };
            Len = new Vector5D();
            Angles = new Vector5D();
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
        private Vector3D GP1(Vector3D P3)
        {
            Angles.K1 = GetAngle(P3.X, P3.Y);
            if (Angles.K1 > Math.PI / 4)
            {
                Angles.K1 = Angles.K1 - Math.PI;
            }
            else if (Angles.K1 < -Math.PI / 4)
            {
                Angles.K1 = Angles.K1 + Math.PI;
            }
            m1 = Matrix4D.M1(Angles.K1, Len.K1);
            return new Vector3D
            {
                X = m1.K03,
                Y = m1.K13,
                Z = m1.K23
            };
        }

        private Vector3D GP2(Vector3D P3)
        {
            double x = Math.Sqrt(P3.X * P3.X + P3.Y * P3.Y);
            if (P3.X < 0)
            {
                x = -x;
            }
            double z = P3.Z - Len.K1;

            double L = Math.Sqrt(P3.X * P3.X + P3.Y * P3.Y + (P3.Z - Len.K1) * (P3.Z - Len.K1));
            // Так как появлиось det, то и длинны меняются
            double L3 = Math.Sqrt(Len.K3 * Len.K3 + Det * Det);

            //углы по трём сторонам
            if (L != 0)
            {
                double A = Math.Acos((Len.K2 * Len.K2 + L * L - L3 * L3) / (2 * Len.K2 * L));
                double B = Math.Acos((L3 * L3 + L * L - Len.K2 * Len.K2) / (2 * L3 * L));
                double G = Math.PI - A - B;

                double o = GetAngle(x, z);
                Angles.K2 = Math.PI / 2 - (o + A);

                m2 = Matrix4D.M2(Angles.K2, Len.K2);

                Matrix4D R = Matrix4D.Multiply(m1, m2);
                Angles.K3 = Math.PI / 2 - G + GetAngle(Len.K3, Det);
                m3 = Matrix4D.Multiply(Matrix4D.M3(Angles.K3, Len.K3), Matrix4D.MoveZ(Det));
                return new Vector3D
                {
                    X = R.K03,
                    Y = R.K13,
                    Z = R.K23
                };
            }
            else
            {
                return null;
            }
        }

        private void Go4(Vector3D P4)
        {
            Matrix4D R2 = Matrix4D.Multiply(Matrix4D.Multiply(m1, m2), m3);
            double newY = R2.K01 * (P4.X - R2.K03) + R2.K11 * (P4.Y - R2.K13) + R2.K21 * (P4.Z - R2.K23);
            double newZ = R2.K02 * (P4.X - R2.K03) + R2.K12 * (P4.Y - R2.K13) + R2.K22 * (P4.Z - R2.K23);
            Angles.K4 = -GetAngle(newZ, newY);
        }

        private void Go5(Vector3D P4)
        {
            Matrix4D R2 = Matrix4D.Multiply(Matrix4D.Multiply(m1, m2), m3);
            R2 = Matrix4D.Multiply(R2, Matrix4D.M4(Angles.K4));
            double newX = R2.K00 * (P4.X - R2.K03) + R2.K10 * (P4.Y - R2.K13) + R2.K20 * (P4.Z - R2.K23);
            double newZ = R2.K02 * (P4.X - R2.K03) + R2.K12 * (P4.Y - R2.K13) + R2.K22 * (P4.Z - R2.K23);
            Angles.K5 = GetAngle(newX, newZ);
        }

        private Stack<Vector3D> NewgP3(Vector3D P4, out Vector3D P34, double a, double b)
        {
            Vector3D value = new Vector3D
            {
                X = P4.X - Len.K5 * Math.Cos(a) * Math.Cos(b),
                Y = P4.Y - Len.K5 * Math.Cos(a) * Math.Sin(b),
                Z = P4.Z + Len.K5 * Math.Sin(a)
            };
            P34 = value;
            Matrix4D R = Matrix4D.Multiply(Matrix4D.RotateZ(b), Matrix4D.RotateY(a));
            R.K03 = P4.X;
            R.K13 = P4.Y;
            R.K23 = P4.Z;
            R = Matrix4D.Multiply(R, Matrix4D.MoveX(-Len.K5));
            Stack<Vector3D> P3stack = new Stack<Vector3D>();
            for (double i = -180 / 180.0 * Math.PI; i < 180 / 180.0 * Math.PI; i += 0.1 / 180.0 * Math.PI)
            {
                Matrix4D Rt = Matrix4D.Multiply(R, Matrix4D.RotateX(i));
                Rt = Matrix4D.Multiply(Rt, Matrix4D.MoveZ(-Len.K4));
                Vector3D point = new Vector3D
                {
                    X = Rt.K03,
                    Y = Rt.K13,
                    Z = Rt.K23
                };

                P3stack.Push(point);
            }
            return P3stack;
        }

        private double[] GetAandB(Vector3D P4, Vector3D Pnab)
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
            Matrix4D mat = Matrix4D.Mbase();                               /* Положение манипулятора при работе системы Манипулятор-Портал
                                                                       В отрицательной зоне параметр true                           */

            // Преобразование точек к локальной системе координат манипулятора
            double x_ = mat.K00 * (X - Base.X) + mat.K10 * (Y - Base.Y) + mat.K20 * (Z - Base.Z);
            double y_ = mat.K01 * (X - Base.X) + mat.K11 * (Y - Base.Y) + mat.K21 * (Z - Base.Z);
            double z_ = mat.K02 * (X - Base.X) + mat.K12 * (Y - Base.Y) + mat.K22 * (Z - Base.Z);

            // Точка в новой системе координат
            Vector3D P4 = new Vector3D
            {
                X = x_,
                Y = y_,
                Z = z_
            };

            // Перевод градусов в радианы
            alf = alf * Math.PI / 180;                             // Угол альфа, вращение вокруг Y 
            bet = bet * Math.PI / 180;                              // Угол бета, вращение вокруг Z

            Matrix4D T = Matrix4D.Mt(alf, bet, P4);                        // Определение матрицы манипулятора

            //----------------------------------------------------------------------------------------------------------------------
            Stack<Vector3D> P3mass = NewgP3(P4, out Vector3D P34, alf, bet); // Получение множества точек P3

            foreach (Vector3D point in P3mass)                      // Для каждой такой точки P3 ищем решение кинематики
            {
                // Ниже идёт проверка на достижимость до точки P3 манипулятором                
                if (Math.Sqrt(point.X * point.X + point.Y * point.Y + (point.Z - Len.K1) * (point.Z - Len.K1)) <= Len.K2 + Len.K3)
                {
                    Vector3D P1 = GP1(point);                       // Получение точки P1 и получение обобщенной координаты O1
                    Vector3D P2 = GP2(point);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                    // Ниже условие, если получили точку то продолжаем 
                    if (P2 != null)
                    {
                        Go4(P34);
                        Go5(P34);

                        double[] angle4 = { Angles.K4, 0 };
                        double[] angle5 = { -Angles.K5 + Math.PI / 2, 0 };

                        if (Angles.K4 > Math.PI)
                        {
                            Angles.K4 = Angles.K4 - Math.PI;
                        }
                        else
                        {
                            Angles.K4 = Angles.K4 + Math.PI;
                        }
                        angle4[1] = Angles.K4;
                        Go5(P34);
                        angle5[1] = -Angles.K5 + Math.PI / 2;

                        // Ниже выполненна прямая кинематика манипулятора
                        Matrix4D R = Matrix4D.Multiply(Matrix4D.M1(Angles.K1, Len.K1), Matrix4D.M2(Angles.K2, Len.K2));
                        R = Matrix4D.Multiply(R, Matrix4D.M3(Angles.K3, Len.K3));
                        R = Matrix4D.Multiply(R, Matrix4D.MoveZ(Det));
                        R = Matrix4D.Multiply(R, Matrix4D.M4(angle4[0]));
                        R = Matrix4D.Multiply(R, Matrix4D.Newm5(angle5[0]));
                        R = Matrix4D.Multiply(R, Matrix4D.MoveZ(Len.K4));
                        R = Matrix4D.Multiply(R, Matrix4D.MoveX(Len.K5));

                        double pogr = 0.1;

                        Angles.K4 = angle4[0];
                        Angles.K5 = angle5[0];
                        // Если отклонения от матрицы манипулятора Т меньше нормы, то записываем в результат
                        if (Math.Abs(R.K00 - T.K00) < pogr && Math.Abs(R.K10 - T.K10) < pogr && Math.Abs(R.K20 - T.K20) < pogr)
                        {
                            rezultAngles.Push(Angles.Copy());
                        }

                        R = Matrix4D.Multiply(Matrix4D.M1(Angles.K1, Len.K1), Matrix4D.M2(Angles.K2, Len.K2));
                        R = Matrix4D.Multiply(R, Matrix4D.M3(Angles.K3, Len.K3));
                        R = Matrix4D.Multiply(R, Matrix4D.M4(angle4[1]));
                        R = Matrix4D.Multiply(R, Matrix4D.Newm5(angle5[1]));
                        R = Matrix4D.Multiply(R, Matrix4D.MoveZ(Len.K4));
                        R = Matrix4D.Multiply(R, Matrix4D.MoveX(Len.K5));
                        Angles.K4 = angle4[1];
                        Angles.K5 = angle5[1];
                        if (Math.Abs(R.K00 - T.K00) < pogr && Math.Abs(R.K10 - T.K10) < pogr && Math.Abs(R.K20 - T.K20) < pogr)
                        {
                            rezultAngles.Push(Angles.Copy());
                        }
                    }
                }
            }
            return rezultAngles;
        }

        public Matrix4D DirectKinematic(Vector5D angles)
        {
            Matrix4D R = Matrix4D.Multiply(Matrix4D.M1(angles.K1, Len.K1), Matrix4D.M2(angles.K2, Len.K2));
            R = Matrix4D.Multiply(R, Matrix4D.M3(angles.K3, Len.K3));
            R = Matrix4D.Multiply(R, Matrix4D.MoveZ(Det));
            R = Matrix4D.Multiply(R, Matrix4D.M4(angles.K4));
            R = Matrix4D.Multiply(R, Matrix4D.Newm5(angles.K5));
            R = Matrix4D.Multiply(R, Matrix4D.MoveZ(Len.K4));
            R = Matrix4D.Multiply(R, Matrix4D.MoveX(Len.K5));
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
        private Stack<Vector5D> Search(Vector3D P4, double alf, double bet, double pogr, double leftboard, double rightdoard)
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

                Vector3D P1;
                Vector3D P2;
                Vector3D P3;
                Vector3D P34;

                List<Vector5D> leftrez = new List<Vector5D>();
                List<Vector5D> rightrez = new List<Vector5D>();

                if (deviation == 10000)
                {
                    List<Vector5D> midonly = new List<Vector5D>();

                    P34 = GetP34(P4, alf, bet);
                    P3 = GetP3((double)leftmid, P4, alf, bet);
                    if (Math.Sqrt(P3.X * P3.X + P3.Y * P3.Y + (P3.Z - Len.K1) * (P3.Z - Len.K1)) <= Len.K2 + Len.K3)
                    {
                        P1 = GP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                        P2 = GP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                                                            // Ниже условие, если получили точку то продолжаем 
                        if (P2 != null)
                        {
                            Go4(P34);                                   // Получение обобщенной координаты O4
                            Go5(P34);                                   // Получение обобщенной координаты O5

                            double[] angle4 = { Angles.K4, 0 };
                            double[] angle5 = { -Angles.K5 + Math.PI / 2, 0 };
                            if (Angles.K4 > Math.PI)
                                Angles.K4 = Angles.K4 - Math.PI;
                            else
                            {
                                Angles.K4 = Angles.K4 + Math.PI;
                            }
                            angle4[1] = Angles.K4;
                            Go5(P34);
                            angle5[1] = -Angles.K5 + Math.PI / 2;

                            Angles.K4 = angle4[0];
                            Angles.K5 = angle5[0];

                            midonly.Add(Angles.Copy());

                            Angles.K4 = angle4[1];
                            Angles.K5 = angle5[1];

                            midonly.Add(Angles.Copy());

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
                if (Math.Sqrt(P3.X * P3.X + P3.Y * P3.Y + (P3.Z - Len.K1) * (P3.Z - Len.K1)) <= Len.K2 + Len.K3)
                {
                    P1 = GP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                    P2 = GP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                    // Ниже условие, если получили точку то продолжаем 
                    if (P2 != null)
                    {
                        Go4(P34);                                   // Получение обобщенной координаты O4
                        Go5(P34);                                   // Получение обобщенной координаты O5

                        double[] angle4 = { Angles.K4, 0 };
                        double[] angle5 = { -Angles.K5 + Math.PI / 2, 0 };
                        if (Angles.K4 > Math.PI)
                            Angles.K4 = Angles.K4 - Math.PI;
                        else
                        {
                            Angles.K4 = Angles.K4 + Math.PI;
                        }
                        angle4[1] = Angles.K4;
                        Go5(P34);
                        angle5[1] = -Angles.K5 + Math.PI / 2;

                        Angles.K4 = angle4[0];
                        Angles.K5 = angle5[0];

                        leftrez.Add(Angles.Copy());

                        Angles.K4 = angle4[1];
                        Angles.K5 = angle5[1];

                        leftrez.Add(Angles.Copy());
                    }
                }

                // ДЛЯ ПРАВОГО
                P34 = GetP34(P4, alf, bet);
                P3 = GetP3((double)rightmid, P4, alf, bet);
                if (Math.Sqrt(P3.X * P3.X + P3.Y * P3.Y + (P3.Z - Len.K1) * (P3.Z - Len.K1)) <= Len.K2 + Len.K3)
                {
                    P1 = GP1(P3);                       // Получение точки P1 и получение обобщенной координаты O1
                    P2 = GP2(P3);                       // Получение точки P2 и получение обобщенной координаты O2 и O3
                    // Ниже условие, если получили точку то продолжаем 
                    if (P2 != null)
                    {
                        Go4(P34);                                   // Получение обобщенной координаты O4
                        Go5(P34);                                   // Получение обобщенной координаты O5

                        double[] angle4 = { Angles.K4, 0 };
                        double[] angle5 = { -Angles.K5 + Math.PI / 2, 0 };
                        if (Angles.K4 > Math.PI)
                            Angles.K4 = Angles.K4 - Math.PI;
                        else
                        {
                            Angles.K4 = Angles.K4 + Math.PI;
                        }
                        angle4[1] = Angles.K4;
                        Go5(P34);
                        angle5[1] = -Angles.K5 + Math.PI / 2;

                        Angles.K4 = angle4[0];
                        Angles.K5 = angle5[0];

                        rightrez.Add(Angles.Copy());

                        Angles.K4 = angle4[1];
                        Angles.K5 = angle5[1];

                        rightrez.Add(Angles.Copy());
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
        private Stack<Vector5D> FindOnInterval(int inter, Vector3D P4, double alf, double bet, double tochnost, double leftboard, double rightboard)
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
            Matrix4D mat = Matrix4D.Mbase();                               /* Положение манипулятора при работе системы Манипулятор-Портал
                                                                       В отрицательной зоне параметр true                           */

            // Преобразование точек к локальной системе координат манипулятора
            double x_ = mat.K00 * (X - Base.X) + mat.K10 * (Y - Base.Y) + mat.K20 * (Z - Base.Z);
            double y_ = mat.K01 * (X - Base.X) + mat.K11 * (Y - Base.Y) + mat.K21 * (Z - Base.Z);
            double z_ = mat.K02 * (X - Base.X) + mat.K12 * (Y - Base.Y) + mat.K22 * (Z - Base.Z);

            // Точка в новой системе координат
            Vector3D P4 = new Vector3D
            {
                X = x_,
                Y = y_,
                Z = z_
            };

            x_ = mat.K00 * (X2 - Base.X) + mat.K10 * (Y2 - Base.Y) + mat.K20 * (Z2 - Base.Z);
            y_ = mat.K01 * (X2 - Base.X) + mat.K11 * (Y2 - Base.Y) + mat.K21 * (Z2 - Base.Z);
            z_ = mat.K02 * (X2 - Base.X) + mat.K12 * (Y2 - Base.Y) + mat.K22 * (Z2 - Base.Z);

            Vector3D Pn = new Vector3D
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

        private Vector3D GetP34(Vector3D P4, double alf, double bet)
        {
            return new Vector3D
            {
                X = P4.X - Len.K5 * Math.Cos(alf) * Math.Cos(bet),
                Y = P4.Y - Len.K5 * Math.Cos(alf) * Math.Sin(bet),
                Z = P4.Z + Len.K5 * Math.Sin(alf)
            };
        }

        private Vector3D GetP3(double i, Vector3D P4, double alf, double bet)
        {
            Matrix4D R = Matrix4D.Multiply(Matrix4D.RotateZ(bet), Matrix4D.RotateY(alf));
            R.K03 = P4.X;
            R.K13 = P4.Y;
            R.K23 = P4.Z;
            R = Matrix4D.Multiply(R, Matrix4D.MoveX(-Len.K5));
            Matrix4D Rt = Matrix4D.Multiply(R, Matrix4D.RotateX(i));
            Rt = Matrix4D.Multiply(Rt, Matrix4D.MoveZ(-Len.K4));
            return new Vector3D
            {
                X = Rt.K03,
                Y = Rt.K13,
                Z = Rt.K23
            };
        }
    }
}

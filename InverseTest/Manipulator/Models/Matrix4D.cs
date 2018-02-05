using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Manipulator.Models
{
    public class Matrix4D
    {
        public double K00 { get; set; }
        public double K01 { get; set; }
        public double K02 { get; set; }
        public double K03 { get; set; }

        public double K10 { get; set; }
        public double K11 { get; set; }
        public double K12 { get; set; }
        public double K13 { get; set; }

        public double K20 { get; set; }
        public double K21 { get; set; }
        public double K22 { get; set; }
        public double K23 { get; set; }

        public double K30 { get; set; }
        public double K31 { get; set; }
        public double K32 { get; set; }
        public double K33 { get; set; }

        public static Matrix4D Multiply(Matrix4D mA, Matrix4D mB)
        {
            return new Matrix4D()
            {
                K00 = mA.K00 * mB.K00 + mA.K01 * mB.K10 + mA.K02 * mB.K20 + mA.K03 * mB.K30,
                K01 = mA.K00 * mB.K01 + mA.K01 * mB.K11 + mA.K02 * mB.K21 + mA.K03 * mB.K31,
                K02 = mA.K00 * mB.K02 + mA.K01 * mB.K12 + mA.K02 * mB.K22 + mA.K03 * mB.K32,
                K03 = mA.K00 * mB.K03 + mA.K01 * mB.K13 + mA.K02 * mB.K23 + mA.K03 * mB.K33,

                K10 = mA.K10 * mB.K00 + mA.K11 * mB.K10 + mA.K12 * mB.K20 + mA.K13 * mB.K30,
                K11 = mA.K10 * mB.K01 + mA.K11 * mB.K11 + mA.K12 * mB.K21 + mA.K13 * mB.K31,
                K12 = mA.K10 * mB.K02 + mA.K11 * mB.K12 + mA.K12 * mB.K22 + mA.K13 * mB.K32,
                K13 = mA.K10 * mB.K03 + mA.K11 * mB.K13 + mA.K12 * mB.K23 + mA.K13 * mB.K33,

                K20 = mA.K20 * mB.K00 + mA.K21 * mB.K10 + mA.K22 * mB.K20 + mA.K23 * mB.K30,
                K21 = mA.K20 * mB.K01 + mA.K21 * mB.K11 + mA.K22 * mB.K21 + mA.K23 * mB.K31,
                K22 = mA.K20 * mB.K02 + mA.K21 * mB.K12 + mA.K22 * mB.K22 + mA.K23 * mB.K32,
                K23 = mA.K20 * mB.K03 + mA.K21 * mB.K13 + mA.K22 * mB.K23 + mA.K23 * mB.K33,

                K30 = mA.K30 * mB.K00 + mA.K31 * mB.K10 + mA.K32 * mB.K20 + mA.K33 * mB.K30,
                K31 = mA.K30 * mB.K01 + mA.K31 * mB.K11 + mA.K32 * mB.K21 + mA.K33 * mB.K31,
                K32 = mA.K30 * mB.K02 + mA.K31 * mB.K12 + mA.K32 * mB.K22 + mA.K33 * mB.K32,
                K33 = mA.K30 * mB.K03 + mA.K31 * mB.K13 + mA.K32 * mB.K23 + mA.K33 * mB.K33,
            };
        }

        public static Matrix4D Newm5(double o5)
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

        public static Matrix4D RotateZ(double o)
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

        public static Matrix4D RotateY(double o)
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

        public static Matrix4D RotateX(double o)
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

        public static Matrix4D MoveX(double x)
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

        public static Matrix4D MoveZ(double z)
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
        public static Matrix4D Mt(double alfa, double beta, Vector3D point)
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


        /// <summary>
        /// Задаём матрицы поворота от угла и дистанции
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="r1"></param>
        /// <returns></returns>
        public static Matrix4D M1(double o1, double r1)
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
        public static Matrix4D M2(double o2, double r2)
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
        public static Matrix4D M3(double o3, double r3)
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
        public static Matrix4D M4(double o4)
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
        public static Matrix4D M5(double o5, double r4)
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
        public static Matrix4D Mbase(bool flag = true)
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Manipulator
{
    class MathUtils
    {
        public static double AngleToRadians(double angle)
        {
            return (angle * Math.PI) / 180;
        }

        public static double RadiansToAngle(double radians)
        {
            return (radians * 180) / Math.PI;
        }

        public static double DistanceTwoPoint(Point3D p1, Point3D p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2) + Math.Pow(p2.Z - p1.Z, 2));
        }

        public static Point3D GetRectCenter(Rect3D rect)
        {
            Point3D point = rect.Location;
            Size3D size = rect.Size;
            return new Point3D(point.X + size.X/2, point.Y + size.Y / 2, point.Z + size.Z / 2);
        }

        /// <summary>
        /// Перемножение матриц 4x4
        /// </summary>
        /// <param name="matrixA">Левая матрица</param>
        /// <param name="matrixB">Правая матрица</param>
        /// <returns></returns>
        public static double[][] Mul_Matrix(double[][] matrixA, double[][] matrixB)
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
    }
}

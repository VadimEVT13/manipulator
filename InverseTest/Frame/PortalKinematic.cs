using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Frame
{
    class PortalKinematic
    {
        //рамки
        double x_max = 0;
        double y_max = 0;
        double z_max = 0;

        //углы
        double alf = 0;
        double bet = 0;

        //портал
        double x_p = 0;
        double y_p = 0;
        double z_p = 0;

        //точка схвата
        double x_m = 0;
        double y_m = 0;
        double z_m = 0;

        //точка наблюдения
        double x_n = 0;
        double y_n = 0;
        double z_n = 0;

        //точка начала координат портала
        double x_portal = 0;
        double y_portal = 0;
        double z_portal = 0;

        //длины портала
        double l1 = 0;
        double l2 = 0;
        double l3 = 0;

        /*для присоединения модуля к внешней оболочке 
        прийдётся менять устои компьютерной графики 
        тем самым поменять y и z местами*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="X_max">Максимально возможное положение портала по оси x</param>
        /// <param name="Y_max">Максимально возможное положение портала по оси y</param>
        /// <param name="Z_max">Максимально возможное положение портала по оси z</param>
        /// <param name="Xp">Положение начала портала в абсолютных координатах по оси x</param>
        /// <param name="Yp">Положение начала портала в абсолютных координатах по оси y</param>
        /// <param name="Zp">Положение начала портала в абсолютных координатах по оси z</param>
        /// <param name="L1">Длинна ближайщего к площадке портала звена</param>
        /// <param name="L2">Длинна второго звена портала</param>
        /// <param name="L3">Длинна ближайщего к начальной точке портала звена</param>
        public PortalKinematic(double X_max, double Y_max, double Z_max, double Xp = 0, double Yp = 0, double Zp = 0, double L1 = 0, double L2 = 0, double L3 = 0)
        {
            if (X_max >= 0 & Y_max >= 0 & Z_max >= 0 & L1 >= 0 & L2 >= 0 & L3 >= 0)
            {
                x_max = X_max;
                y_max = Y_max;
                z_max = Z_max;

                x_portal = Xp;
                y_portal = Yp;
                z_portal = Zp;

                l1 = L1;
                l2 = L2;
                l3 = L3;
            }
        }

        //Перемножение матриц
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
        double[][] mb(double o1)
        {
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
            m1[3][3] = 1;

            return m1;
        }

        double[][] ma(double o1)
        {
            double[][] m1 = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                m1[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    m1[i][j] = 0;
            }

            m1[0][0] = Math.Cos(o1);
            m1[0][2] = Math.Sin(o1);
            m1[1][1] = 1;
            m1[2][0] = -Math.Sin(o1);
            m1[2][2] = Math.Cos(o1);
            m1[3][3] = 1;

            return m1;
        }

        double[][] ml(double l)
        {
            double[][] m1 = new double[4][];

            for (int i = 0; i < 4; i++)
            {
                m1[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    m1[i][j] = 0;
            }

            m1[0][0] = 1;
            m1[0][3] = l;
            m1[1][1] = 1;
            m1[2][2] = 1;
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

        /// <summary>
        /// Задание точки схвата манипулятора и точки наблюдения манипулятором
        /// </summary>
        /// <param name="Xm">Положение схвата манипулятора в абсолютных координатах по оси x</param>
        /// <param name="Ym">Положение схвата манипулятора в абсолютных координатах по оси y</param>
        /// <param name="Zm">Положение схвата манипулятора в абсолютных координатах по оси z</param>
        /// <param name="Xn">Положение точки наблюдения манипулятора в абсолютных координатах по оси x</param>
        /// <param name="Yn">Положение точки наблюдения манипулятора в абсолютных координатах по оси y</param>
        /// <param name="Zn">Положение точки наблюдения манипулятора в абсолютных координатах по оси z</param>
        /// <returns></returns>
        public bool setPointManipAndNab(double Xm, double Ym, double Zm, double Xn, double Yn, double Zn)
        {
            double[][] m = mbase(false);

            x_m = m[0][0] * (Xm - x_portal) + m[1][0] * (Ym - y_portal) + m[2][0] * (Zm - z_portal);
            y_m = m[0][1] * (Xm - x_portal) + m[1][1] * (Ym - y_portal) + m[2][1] * (Zm - z_portal);
            z_m = m[0][2] * (Xm - x_portal) + m[1][2] * (Ym - y_portal) + m[2][2] * (Zm - z_portal);

            x_n = m[0][0] * (Xn - x_portal) + m[1][0] * (Yn - y_portal) + m[2][0] * (Zn - z_portal);
            y_n = m[0][1] * (Xn - x_portal) + m[1][1] * (Yn - y_portal) + m[2][1] * (Zn - z_portal);
            z_n = m[0][2] * (Xn - x_portal) + m[1][2] * (Yn - y_portal) + m[2][2] * (Zn - z_portal);

            if (x_n <= x_m)
            {
                return true;
            }
            return false;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale">Параметр задает расстояние от точки наблюдения до площадки портала в разах от расстояния схвата манипулятора до точки наблюдения</param>
        /// <returns>Возвращает массив double. Первые 3 числа задают (x,y,z) точку портала, где крепится звенья портала. 4 и 5 число углы поворота звеньев alf и bet</returns>
        public double[] portalPoint(double dist = 10, double scale = 1)
        {
            //местоположение точки наблюдения относительно схвата манипулятора
            double x = x_m - x_n;
            double y = y_m - y_n;
            double z = z_m - z_n;

            //расстояние от схвата манипулятора до точки наблюдения
            double L = Math.Sqrt(x * x + y * y + z * z);
            
            L = dist * scale;                                       // Расстояние от точки наблюдения до площадки портала

            bet = 0;
            alf = 0;

            if (x >= 0)                                             // Вычисление углов наблюдения за точкой (перпендикулярность схвату)
            {
                alf = getAngle(Math.Sqrt(x * x + y * y), z);
                bet = getAngle(x, y);
            }
            else
            {
                alf = getAngle(-Math.Sqrt(x * x + y * y), z);
                bet = getAngle(x, y);
            }
            
            double L2 = L * scale;                                  // Расстояние только до площадки 
            L = L + l1;                                             // Расстояние до первого узла портала

            // Вычисление координат для сдвига
            double newx = L * Math.Cos(alf) * Math.Cos(bet);
            double newy = L * Math.Cos(alf) * Math.Sin(bet);
            double newz = L * Math.Sin(alf);
            
            double newx2 = L2 * Math.Cos(alf) * Math.Cos(bet);
            double newy2 = L2 * Math.Cos(alf) * Math.Sin(bet);
            double newz2 = L2 * Math.Sin(alf);

            // Точка первого узла портала
            x_p = x_n - newx;
            y_p = y_n - newy;
            z_p = z_n - newz;

            // Точка центра детектора
            double x_p2 = x_n - newx2;
            double y_p2 = y_n - newy2;
            double z_p2 = z_n - newz2;

            // Точка второго узла портала
            double[][] R = matrix(ma(-alf), ml(l2));
            double tx = x_p - R[0][3];
            double ty = y_p - R[1][3];
            double tz = z_p - R[2][3];

            // Точка третьего узла портала
            R = matrix(R, ma(alf));
            R = matrix(R, ml(l3));
            tx = x_p - R[0][3];
            ty = y_p - R[1][3];
            tz = z_p - R[2][3];

            // передвижение портала, углы, нахожение площадки после передвижения
            //double[] m = { tx, ty, tz, alf, bet, x_portal - x_p2, y_portal - y_p2, z_portal + z_p2 };

            // 0-2 координаты   - точка третьего узла портала
            // 3-4              - угол альфа и бета
            // 5-7              - 

            double[] m = { tx, ty, tz, alf, bet, x_portal - tx, y_portal - ty, z_portal + tz };
            if (tx <= x_max & ty <= y_max & tz <= z_max)
                return m;
            else
                return null;
        }
    }
}

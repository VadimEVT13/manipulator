using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest
{
    public class PortalKinematic
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
        double L1 = 0;
        double L2 = 0;

        /*для присоединения модуля к внешней оболочке 
        прийдётся менять устои компьютерной графики 
        тем самым поменять y и z местами*/

        /// <summary>
        /// Функция задает максимальные отклонения от начальной точки портала в миллиметрах
        /// </summary>
        public PortalKinematic(double X_max, double Y_max, double Z_max)
        {
            if (X_max >= 0 & Y_max >= 0 & Z_max >= 0)
            {
                x_max = X_max;
                y_max = Z_max;
                z_max = Y_max;
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
            m1[2][2] = 1;
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
        
        /// <summary>
        /// Функция задает начало координат портала в абсолютных координатах
        /// </summary>
        public void setPortal(double x, double y, double z)
        {
            x_portal = x;
            y_portal = z;
            z_portal = y;
        }
        
        /// <summary>
        /// Функция задает длину звеньев портала в миллиметрах
        /// </summary>
        /// <param name="l1">Первая от "схвата" манипулятора</param>
        /// <param name="l2"></param>
        /// <returns>Возвращает true если правильно задано</returns>
        public bool setPortalLen(double l1, double l2)
        {
            if (l1 >= 0 & l2 >= 0)
            {
                L1 = l1;
                L2 = l2;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Функция задает точку в которую встал "схват" манипулятора
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void setPointManip(double x, double y, double z)
        {
            x_m = x;
            y_m = z;
            z_m = y;
        }

        /// <summary>
        /// Функция задает точку на которую смотрит "схват" манипулятора, координаты точки наблюдения больше или равны координатам "схвата", возвращает ложь если не может установить
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public bool setPointNab(double x, double y, double z)
        {
            if (x_m <= x)
            {
                x_n = x;
                y_n = z;
                z_n = y;

                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Функция возвращает углы Альфа и Бета портала в виде массива из двух чисел в последовательности: альфа, бета. Альфа угол наклона по вертикали, а бета по горизонтали.
        /// </summary>
        /// <returns></returns>
        public double[] getAlfAndBet()
        {            
            double x = x_n - x_m;
            double y = y_n - y_m;
            double z = z_n - z_m;

            bet = 0;
            alf = 0;
            
            if (!(y == 0 & x == 0 & z == 0))
                alf = Math.Asin(z / Math.Sqrt(y * y + x * x + z * z));
            else
                alf = 0;

            if (!(y == 0 & x == 0 & z == 0))
                bet = Math.Asin(y / (Math.Sqrt(y * y + x * x + z * z) * Math.Cos(alf)));
            else
                bet = 0;

            double[] m = { alf, bet};

            return m;
        }

        /// <summary>
        /// Функция возвращает точку установки портала, возвращает массив из 3 чисел (x, y, z) или возвращает null, если точка вне досягаемости портала.
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        public double[] portalPoint(double scale)
        {
            //местоположение точки наблюдения относительно схвата манипулятора
            double x = x_n - x_m;
            double y = y_n - y_m;
            double z = z_n - z_m;

            //расстояние от схвата манипулятора до точки наблюдения
            double L = Math.Sqrt(x * x + y * y + z * z);

            //глупый код
            double L2 = L + L * scale; 
                       
            L = L + L * scale + L1; //вот тут будет + L1
            
            //изменение координат если повернуть линию с дистанцией
            double newx = L * Math.Cos(alf) * Math.Cos(bet);
            double newy = L * Math.Cos(alf) * Math.Sin(bet);
            double newz = L * Math.Sin(alf);

            //глупый код(2)
            double newx2 = L2 * Math.Cos(alf) * Math.Cos(bet);
            double newy2 = L2 * Math.Cos(alf) * Math.Sin(bet);
            double newz2 = L2 * Math.Sin(alf);

            //то где стоит промежуточная точка портала
            x_p = x_m + newx;
            y_p = y_m + newy;
            z_p = z_m + newz;

            //глупый код(3)
            double x_p2 = x_m + newx2;
            double y_p2 = y_m + newy2;
            double z_p2 = z_m + newz2;

            //если расстояние получилось расстояние меньше предела то всё хорошо
            if (x_p - x_portal <= x_max & y_p - y_portal <= y_max & z_p - z_portal <= z_max)
            {
                /*было
                double[] m = { x_p, z_p, y_p };*/

                //стало
                double[] m = { x_p, z_p, y_p, x_p2, z_p2, y_p2};
                return m;
            }

            return null;
        }

        /// <summary>
        /// Функция возвращает массив (x, y, z) как точку, в которую нужно установить основание портала (то место, которое двигают 3 первых двигателя) в абсолютных координатах. Возвращает null, если вне досягаемости.
        /// </summary>
        /// <returns></returns>        
        public double[] ustPoint()
        {
            double[][] R = mb(bet); //вместо 10 вставить переменную и где-то её вводить (l1)
            R = matrix(R, ml(L2)); //плечо 2
            R = matrix(R, mb(-bet));
            
            double[] m = { x_p + R[0][3], y_p + R[1][3], z_p + R[2][3]};

            if (m[0] <= x_max & m[1] <= y_max & m[2] <= z_max)
            {
                double[] newm = { x_p + R[0][3], z_p + R[2][3], y_p + R[1][3] };
                return newm;
            }

            return null;
        }


        /// <summary>
        /// Функция возвращает массив (x, y, z) как точку, в которую нужно установить основание портала (то место, которое двигают 3 первых двигателя) в координатах относительно портала. Возвращает null, если вне досягаемости.
        /// </summary>
        /// <returns></returns> 
        public double[] ustPointForPortal()
        {
            double[][] R = mb(bet); //вместо 10 вставить переменную и где-то её вводить (l1)
            R = matrix(R, ml(L2)); //плечо 2
            R = matrix(R, mb(-bet));

            double[] m = { x_p + R[0][3], y_p + R[1][3], z_p + R[2][3] };

            if (m[0] <= x_max & m[1] <= y_max & m[2] <= z_max)
            {
                m[0] = x_portal - m[0];
                m[1] = y_portal - m[1];
                m[2] = z_portal - m[2];

                double[] newm = { m[0], m[2], m[1]};
                return newm;
            }

            return null;
        }
    }
}

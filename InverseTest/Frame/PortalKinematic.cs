using InverseTest.Frame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Roentgen.Path;

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
            if (X_max >= 0 && Y_max >= 0 && Z_max >= 0 && L1 >= 0 && L2 >= 0 && L3 >= 0)
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

        private Matrix4D MA(double o1)
        {
            return new Matrix4D
            {
                K00 = Math.Cos(o1),
                K02 = Math.Sin(o1),
                K11 = 1,
                K20 = -Math.Sin(o1),
                K22 = Math.Cos(o1),
                K33 = 1
            };
        }

        private Matrix4D ML(double l)
        {
            return new Matrix4D
            {
                K00 = 1,
                K03 = l,
                K11 = 1,
                K22 = 1,
                K33 = 1,
            };
        }

        private Matrix4D MBase(bool flag = true)
        {
            if (flag)
            {
                return new Matrix4D
                {
                    K00 = 1,
                    K11 = 1,
                    K22 = 1,
                    K33 = 1,
                };
            }
            else
            {
                return new Matrix4D
                {
                    K00 = -1,
                    K11 = -1,
                    K22 = 1,
                    K33 = 1,
                };
            }
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
        public bool SetPointManipAndNab(double Xm, double Ym, double Zm, double Xn, double Yn, double Zn)
        {
            Matrix4D m = MBase(false);

            x_m = m.K00 * (Xm - x_portal) + m.K10 * (Ym - y_portal) + m.K20 * (Zm - z_portal);
            y_m = m.K01 * (Xm - x_portal) + m.K11 * (Ym - y_portal) + m.K21 * (Zm - z_portal);
            z_m = m.K02 * (Xm - x_portal) + m.K12 * (Ym - y_portal) + m.K22 * (Zm - z_portal);

            x_n = m.K00 * (Xn - x_portal) + m.K10 * (Yn - y_portal) + m.K20 * (Zn - z_portal);
            y_n = m.K01 * (Xn - x_portal) + m.K11 * (Yn - y_portal) + m.K21 * (Zn - z_portal);
            z_n = m.K20 * (Xn - x_portal) + m.K12 * (Yn - y_portal) + m.K22 * (Zn - z_portal);

            if (x_n <= x_m)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // -пи до +пи
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
        /// <param name="scale">Параметр задает расстояние от точки наблюдения до площадки портала в разах от расстояния схвата манипулятора до точки наблюдения</param>
        /// <returns>Возвращает массив double. Первые 3 числа задают (x,y,z) точку портала, где крепится звенья портала. 4 и 5 число углы поворота звеньев alf и bet</returns>
        public double[] PortalPoint(double dist = 10, double scale = 1)
        {
            //местоположение точки наблюдения относительно схвата манипулятора
            double x = x_m - x_n;
            double y = y_m - y_n;
            double z = z_m - z_n;

            //расстояние от схвата манипулятора до точки наблюдения
            double L = Math.Sqrt(x * x + y * y + z * z);
            
            //L = dist * scale;                                       // Расстояние от точки наблюдения до площадки портала

            bet = 0;
            alf = 0;
            alf = GetAngle(Math.Sqrt(x * x + y * y), z);            // Вычисление углов наблюдения за точкой (перпендикулярность схвату)
            bet = GetAngle(x, y);
            
            //double L2 = L * scale;                                  // Расстояние только до площадки 
            L = L + l1;                                             // Расстояние до первого узла портала

            // Вычисление координат для сдвига
            double newx = L * Math.Cos(alf) * Math.Cos(bet);
            double newy = L * Math.Cos(alf) * Math.Sin(bet);
            double newz = L * Math.Sin(alf);
            
            //double newx2 = L2 * Math.Cos(alf) * Math.Cos(bet);
            //double newy2 = L2 * Math.Cos(alf) * Math.Sin(bet);
            //double newz2 = L2 * Math.Sin(alf);

            // Точка первого узла портала
            x_p = x_n - newx;
            y_p = y_n - newy;
            z_p = z_n - newz;

            //// Точка центра детектора
            //double x_p2 = x_n - newx2;
            //double y_p2 = y_n - newy2;
            //double z_p2 = z_n - newz2;

            // Точка второго узла портала
            Matrix4D R = Matrix4D.Multiply(MA(-alf), ML(l2));
            double tx = x_p - R.K03;
            double ty = y_p - R.K13;
            double tz = z_p - R.K23;

            // Точка третьего узла портала
            R = Matrix4D.Multiply(R, MA(alf));
            R = Matrix4D.Multiply(R, ML(l3));
            tx = x_p - R.K03;
            ty = y_p - R.K13;
            tz = z_p - R.K23;

            // передвижение портала, углы, нахожение площадки после передвижения
            //double[] m = { tx, ty, tz, alf, bet, x_portal - x_p2, y_portal - y_p2, z_portal + z_p2 };

            // 0-2 координаты   - точка третьего узла портала
            // 3-4              - угол альфа и бета
            // 5-7              - 
            
            if (tx <= x_max && ty <= y_max && tz <= z_max)
            {
                //double[] m = { tx, ty, tz, alf, bet, x_portal - tx, y_portal - ty, z_portal + tz };
                double[] m = { tx, ty, tz, alf, bet, x_portal - tx, y_portal - ty, z_portal + tz };
                return m;
            }
            else
            {
                return null;
            }
        }

        public PortalAngle PortalPoint(Point3D manip_point, Point3D scan_point)
        {
            PortalAngle rezult_angle = new PortalAngle();

            Point3D differ = new Point3D() {
                X = scan_point.X - manip_point.X,
                Y = scan_point.Y - manip_point.Y,
                Z = scan_point.Z - manip_point.Z
                };

            // длинна от точки наблюдения до детектора
            double L = Math.Sqrt(differ.X * differ.X + differ.Y * differ.Y + differ.Z * differ.Z);

            rezult_angle.O1 = GetAngle(Math.Sqrt(differ.X * differ.X + differ.Y * differ.Y), differ.Z);
            rezult_angle.O2 = GetAngle(differ.X, differ.Y);

            Matrix3D rotateO1           = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), -rezult_angle.O1 * 180 / Math.PI)).Value;
            Matrix3D rotateO2           = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), -rezult_angle.O2 * 180 / Math.PI)).Value;
            Matrix3D reverse_rotateO1   = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), +rezult_angle.O1 * 180 / Math.PI)).Value;
            Matrix3D reverse_rotateO2   = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), +rezult_angle.O2 * 180 / Math.PI)).Value;
            Matrix3D detector = new Matrix3D() {
                M11 = 1, M12 = 0, M13 = 0, M14 = L,
                M21 = 0, M22 = 1, M23 = 0, M24 = 0,
                M31 = 0, M32 = 0, M33 = 1, M34 = 0,
                M44 = 1
            };
            Matrix3D m0 = new Matrix3D() { M14 = scan_point.X, M24 = scan_point.Y, M34 = scan_point.Z };
            Matrix3D R = m0;
            R = Matrix3D.Multiply(R, rotateO1);
            R = Matrix3D.Multiply(R, rotateO2);
            R = Matrix3D.Multiply(R, detector); // получили цент площадки детектора

            Matrix3D ml3 = new Matrix3D() {
                M11 = 1, M12 = 0, M13 = 0, M14 = l3,
                M21 = 0, M22 = 1, M23 = 0, M24 = 0,
                M31 = 0, M32 = 0, M33 = 1, M34 = 0,
                M44 = 1
            };
            R = Matrix3D.Multiply(R, ml3); // получили первый поворотный узел

            Matrix3D ml1 = new Matrix3D() {
                M11 = 1, M12 = 0, M13 = 0, M14 = l1,
                M21 = 0, M22 = 1, M23 = 0, M24 = 0,
                M31 = 0, M32 = 0, M33 = 1, M34 = 0,
                M44 = 1
            };

            R = Matrix3D.Multiply(R, reverse_rotateO2);
            R = Matrix3D.Multiply(R, reverse_rotateO1);
            R = Matrix3D.Multiply(R, ml1); // получили крепление к вертикальной раме

            rezult_angle.X = x_portal - R.M14;
            rezult_angle.Y = y_portal - R.M24;
            rezult_angle.Z = z_portal - R.M34;

            return rezult_angle;
        }

        // Работает правильно
        public PortalAngle InverseKinematic(Point3D ustanov_point, Point3D scan_point)
        {
            PortalAngle rezult = new PortalAngle();

            Point3D differ = new Point3D() {
                X =  ustanov_point.X - scan_point.X,
                Y =  ustanov_point.Y - scan_point.Y,
                Z =  ustanov_point.Z - scan_point.Z
            };

            rezult.O1 = GetAngle(Math.Sqrt(differ.X * differ.X + differ.Y * differ.Y), differ.Z);
            rezult.O2 = GetAngle(differ.X, differ.Y);

            Matrix3D rotateO1           = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), -rezult.O1 * 180 / Math.PI)).Value;
            Matrix3D rotateO2           = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), -rezult.O2 * 180 / Math.PI)).Value;
            Matrix3D reverse_rotateO1   = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), +rezult.O1 * 180 / Math.PI)).Value;
            Matrix3D reverse_rotateO2   = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), +rezult.O2 * 180 / Math.PI)).Value;
            Matrix3D detector = new Matrix3D() {
                M11 = 1, M12 = 0, M13 = 0, M14 = Math.Sqrt(differ.X * differ.X + differ.Y * differ.Y + differ.Z * differ.Z),
                M21 = 0, M22 = 1, M23 = 0, M24 = 0,
                M31 = 0, M32 = 0, M33 = 1, M34 = 0,
                M44 = 1
            };
            Matrix3D m0 = new Matrix3D() { M14 = scan_point.X, M24 = scan_point.Y, M34 = scan_point.Z };
            Matrix3D R = m0;
            R = Matrix3D.Multiply(R, rotateO1);
            R = Matrix3D.Multiply(R, rotateO2);
            R = Matrix3D.Multiply(R, detector); // получили цент площадки детектора
                        
            Matrix3D ml3 = new Matrix3D() {
                M11 = 1, M12 = 0, M13 = 0, M14 = l3,
                M21 = 0, M22 = 1, M23 = 0, M24 = 0,
                M31 = 0, M32 = 0, M33 = 1, M34 = 0,
                M44 = 1
            };
            R = Matrix3D.Multiply(R, ml3); // получили первый поворотный узел

            Matrix3D ml1 = new Matrix3D() {
                M11 = 1, M12 = 0, M13 = 0, M14 = l1,
                M21 = 0, M22 = 1, M23 = 0, M24 = 0,
                M31 = 0, M32 = 0, M33 = 1, M34 = 0,
                M44 = 1
            };

            R = Matrix3D.Multiply(R, reverse_rotateO2);
            R = Matrix3D.Multiply(R, reverse_rotateO1);
            R = Matrix3D.Multiply(R, ml1); // получили крепление к вертикальной раме

            rezult.X = R.M14;
            rezult.Y = R.M24;
            rezult.Z = R.M34;

            return rezult;
        }

        #region Оболочка
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Basepoint">угловая (базовая) точка портальной установки</param>
        /// <param name="Position">прирост от базовой точки</param>
        /// <param name="a">в градусах вокруг X</param>
        /// <param name="b">в градусах</param>
        /// <returns></returns>
        public List<Matrix3D> GetMatrix(Point3D Basepoint, PortalAngle Position)
        {                                    
            List<Matrix3D> rezult_list = new List<Matrix3D>();

            Matrix3D m0 = new Matrix3D() {
                M11 = 1, M12 = 0, M13 = 0, M14 = Basepoint.X,
                M21 = 0, M22 = 1, M23 = 0, M24 = Basepoint.Y,
                M31 = 0, M32 = 0, M33 = 1, M34 = Basepoint.Z,
                M44 = 1,
            };
            Matrix3D m1 = new Matrix3D() {
                M11 = 1, M12 = 0, M13 = 0, M14 = Position.X,
                M21 = 0, M22 = 1, M23 = 0, M24 = Position.Y,
                M31 = 0, M32 = 0, M33 = 1, M34 = Position.Z,
                M44 = 1,
            };

            Matrix3D ml1 = new Matrix3D() {
                M11 = 1, M12 = 0, M13 = 0, M14 = -l1,
                M21 = 0, M22 = 1, M23 = 0, M24 = 0,
                M31 = 0, M32 = 0, M33 = 1, M34 = 0,
                M44 = 1,
            };

            Matrix3D m2 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), -Position.O1 * 180 / Math.PI)).Value;
            Matrix3D m3 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), -Position.O2 * 180 / Math.PI)).Value;

            Matrix3D R = m0;
            rezult_list.Add(R);
            R = m1;
            rezult_list.Add(R);
            R = Matrix3D.Multiply(R, Matrix3D.Multiply(ml1, m2));
            rezult_list.Add(R);
            R = Matrix3D.Multiply(R, m3);
            rezult_list.Add(R);
            
            return rezult_list;
        }
        #endregion

        public Matrix3D DirectKinematic(PortalAngle Position)
        {
            Matrix3D m1 = new Matrix3D() {
                M11 = 1, M12 = 0, M13 = 0, M14 = Position.X,
                M21 = 0, M22 = 1, M23 = 0, M24 = Position.Y,
                M31 = 0, M32 = 0, M33 = 1, M34 = Position.Z,
                M44 = 1,
            };

            Matrix3D ml1 = new Matrix3D() {
                M11 = 1, M12 = 0, M13 = 0, M14 = -l1,
                M21 = 0, M22 = 1, M23 = 0, M24 = 0,
                M31 = 0, M32 = 0, M33 = 1, M34 = 0,
                M44 = 1,
            };

            Matrix3D m2 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), -Position.O1 * 180 / Math.PI)).Value;
            Matrix3D m3 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), -Position.O2 * 180 / Math.PI)).Value;

            Matrix3D ml3 = new Matrix3D() {
                M11 = 1, M12 = 0, M13 = 0, M14 = -l3,
                M21 = 0, M22 = 1, M23 = 0, M24 = 0,
                M31 = 0, M32 = 0, M33 = 1, M34 = 0,
                M44 = 1,
            };

            Matrix3D R = m1;
            R = Matrix3D.Multiply(R, Matrix3D.Multiply(ml1, m2));
            R = Matrix3D.Multiply(R, m3);
            R = Matrix3D.Multiply(R, ml3);

            return R;
        }
    }
}

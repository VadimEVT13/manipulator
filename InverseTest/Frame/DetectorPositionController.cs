using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Frame
{
    /// <summary>
    /// Контроллер преобразования глобальных координат в локальные координаты детектора.
    /// </summary>
    public static class DetectorPositionController
    {
        /// <summary>
        /// Начальное положение по оси X.
        /// </summary>
        private static double X_POSITION = 0;
        /// <summary>
        /// Начальное положение по оси Y.
        /// </summary>
        private static double Y_POSITION = 0;
        /// <summary>
        /// Начальное положение по оси Z.
        /// </summary>
        private static double Z_POSITION = 380;
        /// <summary>
        /// Начальное положение по оси A.
        /// </summary>
        private static double A_POSITION = 87;
        /// <summary>
        /// Начальное положение по оси B.
        /// </summary>
        private static double B_POSITION = 40;

        /// <summary>
        /// Преобразование мм в см.
        /// </summary>
        /// <param name="mmValue">значение в мм</param>
        /// <returns>значение в см</returns>
        public static double MmToSm(double mmValue)
        {
            return mmValue / 10;
        }

        /// <summary>
        /// Преобразование см в мм.
        /// </summary>
        /// <param name="smValue">значаение в см</param>
        /// <returns>значение в мм</returns>
        public static double SmToMm(double smValue)
        {
            return smValue * 10;
        }

        /// <summary>
        /// Преобразование X из глобальной в локальную.
        /// </summary>
        /// <param name="global">глобальная X</param>
        /// <returns>локальная X</returns>
        public static double XGlobalToLocal(double globalX)
        {
            return -SmToMm(globalX) + X_POSITION;
        }

        /// <summary>
        /// Преобразование X из локальной в глобальную.
        /// </summary>
        /// <param name="localX">локальная X</param>
        /// <returns>глобальная X</returns>
        public static double XLocalToGlobal(double localX)
        {
            return -MmToSm(localX - X_POSITION);
        }

        /// <summary>
        /// Преобразование Y из глобальной в локальную.
        /// </summary>
        /// <param name="globalY">глобальная Y</param>
        /// <returns>локальная Y</returns>
        public static double YGlobalToLocal(double globalY)
        {
            return SmToMm(globalY) + Y_POSITION;
        }

        /// <summary>
        /// Преобразование Y из локальной в глобальную.
        /// </summary>
        /// <param name="localY">локальная Y</param>
        /// <returns>глобальная Y</returns>
        public static double YLocalToGlobal(double localY)
        {
            return MmToSm(localY - Y_POSITION);
        }

        /// <summary>
        /// Преобразование Z из глобальной в локальную.
        /// </summary>
        /// <param name="global">глобальная Z</param>
        /// <returns>локальная Z</returns>
        public static double ZGlobalToLocal(double globalZ)
        {
            return -SmToMm(globalZ) + Z_POSITION;
        }

        /// <summary>
        /// Преобразование Z из локальной в глобальную.
        /// </summary>
        /// <param name="localZ">локальная Z</param>
        /// <returns>глобальная Z</returns>
        public static double ZLocalToGlobal(double localZ)
        {
            return -MmToSm(localZ - Z_POSITION);
        }

        /// <summary>
        /// Преобразование A из глобальной в локальную.
        /// </summary>
        /// <param name="globalA">глобальная A</param>
        /// <returns>локальная A</returns>
        public static double AGlobalToLocal(double globalA)
        {
            return -globalA + A_POSITION;
        }

        /// <summary>
        /// Преобразование A из локальной в глобальную.
        /// </summary>
        /// <param name="localA">локальная A</param>
        /// <returns>глобальная A</returns>
        public static double ALocalToGlobal(double localA)
        {
            return -(localA - A_POSITION);
        }

        /// <summary>
        /// Преобразование B из глобальной в локальную.
        /// </summary>
        /// <param name="globalB">глобальная B</param>
        /// <returns>локальная B</returns>
        public static double BGlobalToLocal(double globalB)
        {
            return -globalB + B_POSITION;
        }

        /// <summary>
        /// Преобразование B из локальной в глобальную.
        /// </summary>
        /// <param name="localB">локальная B</param>
        /// <returns>глобальная B</returns>
        public static double BLocalToGlobal(double localB)
        {
            return -(localB - B_POSITION);
        }
    }
}

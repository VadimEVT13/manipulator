using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Grbl.Models
{
    public class GDetector
    {
        /// <summary>
        /// Минимальное значение по оси X.
        /// </summary>
        private static int LIMIT_X_MIN = 1;
        /// <summary>
        /// Максимальное значение по оси X.
        /// </summary>
        private static int LIMIT_X_MAX = 450;
        /// <summary>
        /// Минимальное значение по оси Y.
        /// </summary>
        private static int LIMIT_Y_MIN = 1;
        /// <summary>
        /// Максимальное значение по оси Y.
        /// </summary>
        private static int LIMIT_Y_MAX = 760;
        /// <summary>
        /// Минимальное значение по оси Z.
        /// </summary>
        private static int LIMIT_Z_MIN = 1;
        /// <summary>
        /// Максимальное значение по оси Z.
        /// </summary>
        private static int LIMIT_Z_MAX = 760;
        /// <summary>
        /// Минимальное значение по оси A.
        /// </summary>
        private static int LIMIT_A_MIN = 1;
        /// <summary>
        /// Максимальное значение по оси A.
        /// </summary>
        private static int LIMIT_A_MAX = 70;
        /// <summary>
        /// Минимальное значение по оси B.
        /// </summary>
        private static int LIMIT_B_MIN = 1;
        /// <summary>
        /// Максимальное значение по оси B.
        /// </summary>
        private static int LIMIT_B_MAX = 180;

        public static double GetLimitValue(double value, double state, double minValue, double maxValue)
        {
            if (state + value < minValue)
            {
                return minValue - state;
            }
            else if (state + value > maxValue)
            {
                return maxValue - state;
            }
            else
            {
                return value;
            }
        }

        public static GPoint GlobalLimits(GPoint target)
        {
            return new GPoint
            {
                X = GetLimitValue(0, target.X, LIMIT_X_MIN, LIMIT_X_MAX),
                Y = GetLimitValue(0, target.Y, LIMIT_Y_MIN, LIMIT_Y_MAX),
                Z = GetLimitValue(0, target.Z, LIMIT_Z_MIN, LIMIT_Z_MAX),
                A = GetLimitValue(0, target.A, LIMIT_A_MIN, LIMIT_A_MAX),
                B = GetLimitValue(0, target.B, LIMIT_B_MIN, LIMIT_B_MAX)
            };
        }

        public static GPoint LocalLimits(GPoint position, GPoint target)
        {
            return new GPoint
            {
                X = GetLimitValue(position.X, target.X, LIMIT_X_MIN, LIMIT_X_MAX),
                Y = GetLimitValue(position.Y, target.Y, LIMIT_Y_MIN, LIMIT_Y_MAX),
                Z = GetLimitValue(position.Z, target.Z, LIMIT_Z_MIN, LIMIT_Z_MAX),
                A = GetLimitValue(position.A, target.A, LIMIT_A_MIN, LIMIT_A_MAX),
                B = GetLimitValue(position.B, target.B, LIMIT_B_MIN, LIMIT_B_MAX)
            };
        }
    }
}

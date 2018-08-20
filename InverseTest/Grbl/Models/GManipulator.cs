using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Grbl.Models
{
    public class GManipulator
    {
        /// <summary>
        /// Последовательный порт
        /// </summary>
        public GPort Port { get; set; }

        private static GManipulator instance;

        private GManipulator()
        {
            Port = new GPort(settings);
        }

        public static GManipulator getInstance()
        {
            if (instance == null)
                instance = new GManipulator();
            return instance;
        }

        private static GDevice settings = new GDevice
        {
            Name = "XManipulator",
            IsX = false,
            IsY = true,
            IsZ = false,
            IsA = false,
            IsB = true,
            IsC = true,
            IsD = false,
            IsE = false
        };

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

        public static double GetLimitValue(double value, double minValue, double maxValue)
        {
            if (value < minValue)
            {
                return minValue;
            }
            else if (value > maxValue)
            {
                return maxValue;
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
                X = GetLimitValue(target.X, 10, 170),
                Y = GetLimitValue(target.Y, 0, 180) + 115,
                Z = GetLimitValue(target.Z, 0, 180),
                B = -GetLimitValue(target.A, -180, 180) + 180,
                C = -GetLimitValue(target.B, -90, 90) + 185
            };
        }

        public static GPoint LocalLimits(GPoint position, GPoint target)
        {
            return new GPoint
            {
                //X = GetLimitValue(position.X, target.X, LIMIT_X_MIN, LIMIT_X_MAX),
                //Y = GetLimitValue(position.Y, target.Y, LIMIT_Y_MIN, LIMIT_Y_MAX),
                //Z = GetLimitValue(position.Z, target.Z, LIMIT_Z_MIN, LIMIT_Z_MAX),
                //A = GetLimitValue(position.A, target.A, LIMIT_A_MIN, LIMIT_A_MAX),
                //B = GetLimitValue(position.B, target.B, LIMIT_B_MIN, LIMIT_B_MAX)
            };
        }
    }
}

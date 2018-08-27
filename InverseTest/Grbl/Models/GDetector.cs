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
        /// Последовательный порт
        /// </summary>
        public GPort Port { get; set; }

        private static GDetector instance;

        private GDetector()
        {
            Port = new GPort(settings);
        }

        public static GDetector getInstance()
        {
            if (instance == null)
                instance = new GDetector();
            return instance;
        }

        private static GDevice settings = new GDevice
        {
            Name = "XDetector",
            IsC = false,
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
                X = GetLimitValue(target.X, 10, 445),
                Y = GetLimitValue(target.Y, 10, 760),
                Z = GetLimitValue(target.Z, 10, 760),
                A = GetLimitValue(target.A, 10, 110),
                B = GetLimitValue(target.B, 10, 175)
            };
        }

        public static GPoint LocalLimits(GPoint position, GPoint target)
        {
            return new GPoint
            {
                /*X = GetLimitValue(position.X, target.X, LIMIT_X_MIN, LIMIT_X_MAX),
                Y = GetLimitValue(position.Y, target.Y, LIMIT_Y_MIN, LIMIT_Y_MAX),
                Z = GetLimitValue(position.Z, target.Z, LIMIT_Z_MIN, LIMIT_Z_MAX),
                A = GetLimitValue(position.A, target.A, LIMIT_A_MIN, LIMIT_A_MAX),
                B = GetLimitValue(position.B, target.B, LIMIT_B_MIN, LIMIT_B_MAX)//*/
            };
        }
    }
}

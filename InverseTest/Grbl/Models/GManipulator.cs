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
            IsX = true,
            IsY = true,
            IsZ = false,
            IsA = true,
            IsB = true,
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
                X = GetLimitValue(target.X, 5, 150) + (234.250 - 90),
                Y = GetLimitValue(target.Y, 0, 150) + (263.870 - 90), //263.870
                Z = GetLimitValue(target.Z, 0, 180),
                A = GetLimitValue(target.A, -179, 179) + 32.959,
                B = -GetLimitValue(target.B, -90, 85) + (185.493)
            };
        }
    }
}

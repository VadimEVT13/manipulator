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
    }
}

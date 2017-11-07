using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}

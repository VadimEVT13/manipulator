using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest
{
    public static class MathUtils
    {
        public static double ToRadians(double angle)
        {
            return (Math.PI / 180) * angle; 
        }
    }
}

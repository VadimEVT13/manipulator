using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Bound
{
    public class Bound
    {
        public double BottomBound { get; }
        public double UpperBound { get; }

        public Bound(double bottom = int.MinValue, double upper = int.MaxValue)
        {
            this.BottomBound = bottom;
            this.UpperBound = upper;
        }

        public bool Check(double value)
        {
            return value <= this.UpperBound && value >= this.BottomBound;
        }

        /// <summary>
        /// Возвращает граничное значение ближайшее к входному если оно не входит в промежуток
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public double GetBoundary(double value)
        {
            if (value > this.UpperBound)
            {
                return this.UpperBound;
            }
            else if (value < this.BottomBound)
            {
                return this.BottomBound;
            }
            else return value;
        }
    }
}

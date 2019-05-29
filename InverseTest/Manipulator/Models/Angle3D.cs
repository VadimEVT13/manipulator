using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Manipulator.Models
{
    public class Angle3D
    {
        public double O1 { get; set; }
        public double O2 { get; set; }
        public double O3 { get; set; }
        public double O4 { get; set; }
        public double O5 { get; set; }

        public Angle3D()
        {
            this.O1 = 0;
            this.O2 = 0;
            this.O3 = 0;
            this.O4 = 0;
            this.O5 = 0;
        }

        public Angle3D(Angle3D angle)
        {
            this.O1 = angle.O1;
            this.O2 = angle.O2;
            this.O3 = angle.O3;
            this.O4 = angle.O4;
            this.O5 = angle.O5;
        }

        public override string ToString()
        {
            return O1 + " " + O2 + " " + O3 + " " + O4 + " " + O5;
        }
        // Позможно нужно оставить базовую версию переопределения
        public override bool Equals(object obj)
        {
            bool flag =
                (O1 == ((Angle3D)obj).O1) &
                (O2 == ((Angle3D)obj).O2) &
                (O3 == ((Angle3D)obj).O3) &
                (O4 == ((Angle3D)obj).O4) &
                (O5 == ((Angle3D)obj).O5);
            return flag;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

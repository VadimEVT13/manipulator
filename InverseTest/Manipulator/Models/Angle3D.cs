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


        public override string ToString()
        {
            return O1 + " " + O2 + " " + O3 + " " + O4 + " " + O5;
        }

    }



}

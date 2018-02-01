using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Manipulator.Models
{
    class Matrix4D
    {
        public double K11 { get; set; }
        public double K12 { get; set; }
        public double K13 { get; set; }
        public double K14 { get; set; }

        public double K21 { get; set; }
        public double K22 { get; set; }
        public double K23 { get; set; }
        public double K24 { get; set; }

        public double K31 { get; set; }
        public double K32 { get; set; }
        public double K33 { get; set; }
        public double K34 { get; set; }

        public double K41 { get; set; }
        public double K42 { get; set; }
        public double K43 { get; set; }
        public double K44 { get; set; }
    }
}

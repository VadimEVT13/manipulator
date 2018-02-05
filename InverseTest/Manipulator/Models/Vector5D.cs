using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Manipulator.Models
{
    public class Vector5D
    {
        public double K1 { get; set; }
        public double K2 { get; set; }
        public double K3 { get; set; }
        public double K4 { get; set; }
        public double K5 { get; set; }

        public Vector5D Copy()
        {
            return new Vector5D
            {
                K1 = this.K1,
                K2 = this.K2,
                K3 = this.K3,
                K4 = this.K4,
                K5 = this.K5
            };
        }
    }
}

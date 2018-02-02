using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Manipulator.Models
{
    public class Matrix4D
    {
        public double K00 { get; set; }
        public double K01 { get; set; }
        public double K02 { get; set; }
        public double K03 { get; set; }

        public double K10 { get; set; }
        public double K11 { get; set; }
        public double K12 { get; set; }
        public double K13 { get; set; }

        public double K20 { get; set; }
        public double K21 { get; set; }
        public double K22 { get; set; }
        public double K23 { get; set; }

        public double K30 { get; set; }
        public double K31 { get; set; }
        public double K32 { get; set; }
        public double K33 { get; set; }

        public static Matrix4D Multiply(Matrix4D mA, Matrix4D mB)
        {
            return new Matrix4D()
            {
                K00 = mA.K00 * mB.K00 + mA.K01 * mB.K10 + mA.K02 * mB.K20 + mA.K03 * mB.K30,
                K01 = mA.K00 * mB.K01 + mA.K01 * mB.K11 + mA.K02 * mB.K21 + mA.K03 * mB.K31,
                K02 = mA.K00 * mB.K02 + mA.K01 * mB.K12 + mA.K02 * mB.K22 + mA.K03 * mB.K32,
                K03 = mA.K00 * mB.K03 + mA.K01 * mB.K13 + mA.K02 * mB.K23 + mA.K03 * mB.K33,

                K10 = mA.K10 * mB.K00 + mA.K11 * mB.K10 + mA.K12 * mB.K20 + mA.K13 * mB.K30,
                K11 = mA.K10 * mB.K01 + mA.K11 * mB.K11 + mA.K12 * mB.K21 + mA.K13 * mB.K31,
                K12 = mA.K10 * mB.K02 + mA.K11 * mB.K12 + mA.K12 * mB.K22 + mA.K13 * mB.K32,
                K13 = mA.K10 * mB.K03 + mA.K11 * mB.K13 + mA.K12 * mB.K23 + mA.K13 * mB.K33,

                K20 = mA.K20 * mB.K00 + mA.K21 * mB.K10 + mA.K22 * mB.K20 + mA.K23 * mB.K30,
                K21 = mA.K20 * mB.K01 + mA.K21 * mB.K11 + mA.K22 * mB.K21 + mA.K23 * mB.K31,
                K22 = mA.K20 * mB.K02 + mA.K21 * mB.K12 + mA.K22 * mB.K22 + mA.K23 * mB.K32,
                K23 = mA.K20 * mB.K03 + mA.K21 * mB.K13 + mA.K22 * mB.K23 + mA.K23 * mB.K33,

                K30 = mA.K30 * mB.K00 + mA.K31 * mB.K10 + mA.K32 * mB.K20 + mA.K33 * mB.K30,
                K31 = mA.K30 * mB.K01 + mA.K31 * mB.K11 + mA.K32 * mB.K21 + mA.K33 * mB.K31,
                K32 = mA.K30 * mB.K02 + mA.K31 * mB.K12 + mA.K32 * mB.K22 + mA.K33 * mB.K32,
                K33 = mA.K30 * mB.K03 + mA.K31 * mB.K13 + mA.K32 * mB.K23 + mA.K33 * mB.K33,
            };
        }
    }
}

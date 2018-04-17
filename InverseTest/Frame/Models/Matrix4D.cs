using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Frame.Models
{
    class Matrix4D
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

        public static Matrix4D Multiply(Matrix4D A, Matrix4D B)
        {
            return new Matrix4D
            {
                K00 = A.K00 * B.K00 + A.K01 * B.K10 + A.K02 * B.K20 + A.K03 * B.K30,
                K01 = A.K00 * B.K01 + A.K01 * B.K11 + A.K02 * B.K21 + A.K03 * B.K31,
                K02 = A.K00 * B.K02 + A.K01 * B.K12 + A.K02 * B.K22 + A.K03 * B.K32,
                K03 = A.K00 * B.K03 + A.K01 * B.K13 + A.K02 * B.K23 + A.K03 * B.K33,

                K10 = A.K10 * B.K00 + A.K11 * B.K10 + A.K12 * B.K20 + A.K13 * B.K30,
                K11 = A.K10 * B.K01 + A.K11 * B.K11 + A.K12 * B.K21 + A.K13 * B.K31,
                K12 = A.K10 * B.K02 + A.K11 * B.K12 + A.K12 * B.K22 + A.K13 * B.K32,
                K13 = A.K10 * B.K03 + A.K11 * B.K13 + A.K12 * B.K23 + A.K13 * B.K33,

                K20 = A.K20 * B.K00 + A.K21 * B.K10 + A.K22 * B.K20 + A.K23 * B.K30,
                K21 = A.K20 * B.K01 + A.K21 * B.K11 + A.K22 * B.K21 + A.K23 * B.K31,
                K22 = A.K20 * B.K02 + A.K21 * B.K12 + A.K22 * B.K22 + A.K23 * B.K32,
                K23 = A.K20 * B.K03 + A.K21 * B.K13 + A.K22 * B.K23 + A.K23 * B.K33,

                K30 = A.K30 * B.K00 + A.K31 * B.K10 + A.K32 * B.K20 + A.K33 * B.K30,
                K31 = A.K30 * B.K01 + A.K31 * B.K11 + A.K32 * B.K21 + A.K33 * B.K31,
                K32 = A.K30 * B.K02 + A.K31 * B.K12 + A.K32 * B.K22 + A.K33 * B.K32,
                K33 = A.K30 * B.K03 + A.K31 * B.K13 + A.K32 * B.K23 + A.K33 * B.K33,
            };
        }
    }
}

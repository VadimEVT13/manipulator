using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InverseTest.ManipulatorV2;

namespace InverseTest.Manipulator
{
    public class ManipulatorCoordinatesController
    {

        private const double T2START_ANGLE = -90;
        private const double T3START_ANGLE = 90;

        public ManipulatorCoordinatesController()
        {
           
        }


        public double T1LocalToGlobal(double angle)
        {
            return angle - T2START_ANGLE;
        }

        public double T2LocalToGlobal(double angle)
        {
            return 0;
        }

       
    }
}

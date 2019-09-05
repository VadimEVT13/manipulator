using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISC_Rentgen.Rentgen_Parts.Manipulator_Components.Model
{
    public static class Manipulator_angle_ogranichenie
    {
        public static double O1_max { get { return 90; } }
        public static double O1_min { get { return -90; } }
        public static double O2_max { get { return 90; } }
        public static double O2_min { get { return -90; } }
        public static double O3_max { get { return 90; } }
        public static double O3_min { get { return -90; } }
        public static double O4_max { get { return 90; } }
        public static double O4_min { get { return -180; } }
        public static double O5_max { get { return 180; } }
        public static double O5_min { get { return 0; } }

        public static Angles_Manipulator Normalize_for_grbl(Angles_Manipulator angle)
        {
            Angles_Manipulator rezult = angle;

            if (rezult.O1 < O1_min) rezult.O1 = O1_min;
            if (rezult.O1 > O1_max) rezult.O1 = O1_max;

            if (rezult.O2 < O2_min) rezult.O2 = O2_min;
            if (rezult.O2 > O2_max) rezult.O2 = O2_max;

            if (rezult.O3 < O3_min) rezult.O3 = O3_min;
            if (rezult.O3 > O3_max) rezult.O3 = O3_max;
        
            if (rezult.O4 < O4_min) rezult.O4 = O4_min;
            if (rezult.O4 > O4_max) rezult.O4 = O4_max;

            if (rezult.O5 < O5_min) rezult.O5 = O5_min;
            if (rezult.O5 > O5_max) rezult.O5 = O5_max;

            return rezult;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISC_Rentgen.Rentgen_Parts.Portal_Components.Model
{
    public static class Portal_angle_ogranichenie
    {
        public static double O1_max { get { return 90; }}
        public static double O1_min { get { return -90; } }
        public static double O2_max { get { return 90; } }
        public static double O2_min { get { return -90; } }
        public static double X_max  { get { return 650; } }
        public static double X_min  { get { return 0; } }
        public static double Y_max  { get { return 800; } }
        public static double Y_min  { get { return 20; } }
        public static double Z_max  { get { return 760; } }
        public static double Z_min  { get { return 0; } }

        public static Angles_Portal Normalize_for_grbl(Angles_Portal angle)
        {
            Angles_Portal rezult = angle;
            // Сантиметры в миллиметры
            rezult.X = rezult.X * 10;
            rezult.Y = rezult.Y * 10;
            rezult.Z = rezult.Z * 10;

            if (rezult.O1 < O1_min) rezult.O1 = O1_min;
            if (rezult.O1 > O1_max) rezult.O1 = O1_max;

            if (rezult.O2 < O2_min) rezult.O2 = O2_min;
            if (rezult.O2 > O2_max) rezult.O2 = O2_max;

            if (rezult.X < X_min) rezult.X = X_min;
            if (rezult.X > X_max) rezult.X = X_max;

            if (rezult.Y < Y_min) rezult.Y = Y_min;
            if (rezult.Y > Y_max) rezult.Y = Y_max;

            if (rezult.Z < Z_min) rezult.Z = Z_min;
            if (rezult.Z > Z_max) rezult.Z = Z_max;

            return rezult;
        }
    }
}

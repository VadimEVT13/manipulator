using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Manipulator.Models
{
    public class Ogranichenie
    {
        public double O1min { get; set; }
        public double O1max { get; set; }
        public double O2min { get; set; }
        public double O2max { get; set; }
        public double O3min { get; set; }
        public double O3max { get; set; }
        public double O4min { get; set; }
        public double O4max { get; set; }
        public double O5min { get; set; }
        public double O5max { get; set; }

        public static bool IsOK(Ogranichenie ogr, Angle3D ang)
        {
            if (ang.O1 >= ogr.O1min & ang.O1 <= ogr.O1max &
                ang.O2 >= ogr.O2min & ang.O2 <= ogr.O2max &
                ang.O3 >= ogr.O3min & ang.O3 <= ogr.O3max &
                ang.O4 >= ogr.O4min & ang.O4 <= ogr.O4max &
                ang.O5 >= ogr.O5min & ang.O5 <= ogr.O5max)
            {
                return true;
            }

            return false;
        }
    }
}

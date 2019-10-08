using ISC_Rentgen.GUI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISC_Rentgen.Rentgen_Parts.Scan_Object_Components.Model
{
    public class Angles_Scan_Object : Property_change_base
    {
        private double z_rotation = 0;
        private double y_rotation = 0;
        public double Z_rotation { get { return z_rotation; } set { z_rotation = value; NotifyPropertyChanged(nameof(z_rotation)); } }
        public double Y_rotation { get { return y_rotation; } set { y_rotation = value; NotifyPropertyChanged(nameof(y_rotation)); } }
    }
}

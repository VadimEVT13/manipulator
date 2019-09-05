﻿using ISC_Rentgen.Rentgen_Parts.Manipulator_Components.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ISC_Rentgen.GUI.Model
{
    public class Key_Point
    {
        public Point3D Emitter_point { get; set; }
        public Point3D Scan_point { get; set; }

        public Angles_Manipulator Manipulator_Angle { get; set; }

        public Key_Point(Point3D Emitter_point, Point3D Scan_point)
        {
            this.Emitter_point  = Emitter_point;
            this.Scan_point     = Scan_point;
        }
    }
}

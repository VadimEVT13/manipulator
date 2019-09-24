using HelixToolkit.Wpf;
using ISC_Rentgen.Rentgen_Parts.Scan_Object_Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ISC_Rentgen.Model3d
{
    public static class Scan_Object_Parser
    {
        public static void Parse(Model3DGroup model)
        {
            foreach (Model3D m in model.Children)
                m.SetName(Model3DParts.ObjectParts.Scan_object);

            Scan_Object.getInstant.Model = model;
        }
    }
}

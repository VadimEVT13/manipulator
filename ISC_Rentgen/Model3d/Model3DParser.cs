using HelixToolkit.Wpf;
using ISC_Rentgen.Rentgen_Parts.Manipulator_Components;
using ISC_Rentgen.Rentgen_Parts.Portal_Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ISC_Rentgen.Model3d
{
    public static class Model3DParts
    {
        public class ManipulatorParts
        {
            // -- Manipulator
            public static string Join1 = "Manip.j1_j1";
            public static string Join2 = "Manip.j2_j2";
            public static string Join3 = "Manip.j3_j3";
            public static string Rentgen = "Manip.j4_j4";
            public static string Under_than_J1 = "ManipulatorTable";
        }

        public class ObjectParts
        {
            // -- Object
            public static string Scan_object = "Scan_object";
        }

        public class PortalParts
        {
            // -- Portal
            public static string Under_portal = "Prof_40_1_005_Cube.000";
            public static string X_portal_join = "Portal.X";
            public static string Y_portal_join = "Portal.Y";
            public static string Z_portal_join = "Portal.Z";
            public static string Portal_detector_join = "Portal.O1";
            public static string Detector = "Portal.O2";
        }
    }

    public static class Model3DParser
    {
        public static void Parse(Model3DGroup model_to_parse)
        {
            foreach (Model3D m in model_to_parse.Children)
            {
                Console.WriteLine(m.GetName());
            }

            // -- Задали модель манипулятора
            ManipulatorV3.Set_Models(
                model_to_parse.Children.Where(x => x.GetName() == Model3DParts.ManipulatorParts.Join1).First(),
                model_to_parse.Children.Where(x => x.GetName() == Model3DParts.ManipulatorParts.Join2).First(),
                model_to_parse.Children.Where(x => x.GetName() == Model3DParts.ManipulatorParts.Join3).First(),
                model_to_parse.Children.Where(x => x.GetName() == Model3DParts.ManipulatorParts.Rentgen).First());

            // -- Парсер портала
            PortalV3.Set_Models(
                model_to_parse.Children.Where(x => x.GetName() == Model3DParts.PortalParts.X_portal_join).First(),
                model_to_parse.Children.Where(x => x.GetName() == Model3DParts.PortalParts.Y_portal_join).First(),
                model_to_parse.Children.Where(x => x.GetName() == Model3DParts.PortalParts.Z_portal_join).First(),
                model_to_parse.Children.Where(x => x.GetName() == Model3DParts.PortalParts.Portal_detector_join).First(),
                model_to_parse.Children.Where(x => x.GetName() == Model3DParts.PortalParts.Detector).First());

            // -- Парсер объекта

        }
    }
}

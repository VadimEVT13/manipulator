using HelixToolkit.Wpf;
using ISC_Rentgen.Model3d;
using ISC_Rentgen.Rentgen_Parts.Portal_Components.Kinematic;
using ISC_Rentgen.Rentgen_Parts.Portal_Components.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ISC_Rentgen.Rentgen_Parts.Portal_Components
{
    public delegate void OnRefresh();

    public static class PortalV3
    {
        public static event OnRefresh OnRefreshSliders;

        public static Angles_Portal Angles { get { return angles; } }
        private static Angles_Portal angles = new Angles_Portal();

        public static double K { get; set; } // Кратность

        public static Model3DGroup Model { get { return model; } set { model = value; } }
        private static Model3DGroup model = new Model3DGroup();
        public static void Set_Models(Model3D X_portal_join, Model3D Y_portal_join, Model3D Z_portal_join, Model3D Portal_detector_join, Model3D Detector)
        {
            Model.Children.Clear();
            Model.Children.Add(X_portal_join);
            Model.Children.Add(Y_portal_join);
            Model.Children.Add(Z_portal_join);
            Model.Children.Add(Portal_detector_join);
            Model.Children.Add(Detector);
        }

        public static Join_Length_Portal Length { get; set; }
        public static Point3D Base_point { get; set; }

        public static Point3D Rotate(Angles_Portal _angles)
        {
            if (Model.Children.Count == 0) throw new Exception("Не задана модель");
            angles = _angles;

            List<Matrix3D> lm = Portal_Kinematic.DirectKinematic(Base_point, Length, _angles);
            Point3D first_join_point  = new Point3D(Base_point.X - Length.L1, Base_point.Y, Base_point.Z);

            Matrix3D T_x = new TranslateTransform3D(-angles.X, 0, 0).Value;
            Matrix3D T_z = new TranslateTransform3D(-angles.X, 0, angles.Z).Value;
            Matrix3D T_y = new TranslateTransform3D(-angles.X, angles.Y, angles.Z).Value;
            
            model.Children.Where(x => x.GetName() == Model3DParts.PortalParts.X_portal_join).First().Transform = new MatrixTransform3D(T_x);
            model.Children.Where(x => x.GetName() == Model3DParts.PortalParts.Y_portal_join).First().Transform = new MatrixTransform3D(T_y);
            model.Children.Where(x => x.GetName() == Model3DParts.PortalParts.Z_portal_join).First().Transform = new MatrixTransform3D(T_z);

            Matrix3D Rotate2 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), Angles.O2), first_join_point).Value;
            Matrix3D Rotate1 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(Rotate2.M31, Rotate2.M32, Rotate2.M33), Angles.O1), first_join_point).Value;


            Matrix3D Traslate = new Matrix3D() { OffsetX = -angles.X, OffsetY = angles.Y, OffsetZ = angles.Z };

            model.Children.Where(x => x.GetName() == Model3DParts.PortalParts.Portal_detector_join).First().Transform = new MatrixTransform3D(Matrix3D.Multiply(Rotate2, Traslate));
            model.Children.Where(x => x.GetName() == Model3DParts.PortalParts.Detector).First().Transform = new MatrixTransform3D(Matrix3D.Multiply(Matrix3D.Multiply(Rotate2, Rotate1), Traslate));
            
            return new Point3D(lm.Last().M14, lm.Last().M24, lm.Last().M34);
        }

        public static Angles_Portal Set_Position(Point3D Position, Point3D Scan_Position)
        {
            Portal_Kinematic pkin = new Portal_Kinematic(65, 80, 76, Base_point.X, Base_point.Y, Base_point.Z, Length.L1, 0, Length.L2);
            pkin.SetPointManipAndNab(Position.X, Position.Y, Position.Z, Scan_Position.X, Scan_Position.Y, Scan_Position.Z);

            Point3D p = new Point3D() {
                X = Scan_Position.X + (Scan_Position.X - Position.X),
                Y = Scan_Position.Y + (Scan_Position.Y - Position.Y),
                Z = Scan_Position.Z + (Scan_Position.Z - Position.Z)
            };
            
            PortalAngle rezult = pkin.InverseKinematic(p, Scan_Position, K);
            Rotate(new Angles_Portal() { O1 = rezult.O1, O2 = rezult.O2, X = rezult.X, Y = rezult.Y, Z = rezult.Z });

            Angles_Portal output = new Angles_Portal() { O1 = rezult.O1, O2 = rezult.O2, X = rezult.X, Y = rezult.Y, Z = rezult.Z };

            OnRefreshSliders?.Invoke();
            return output;
        }
    }
}

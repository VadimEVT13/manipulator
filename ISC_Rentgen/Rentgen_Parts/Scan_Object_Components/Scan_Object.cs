using ISC_Rentgen.Rentgen_Parts.Scan_Object_Components.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ISC_Rentgen.Rentgen_Parts.Scan_Object_Components
{
    public static class Scan_Object
    {
        public static  Model3DGroup Model { get { return model; } set { model = value; } }
        private static Model3DGroup model = new Model3DGroup();

        public static  Point3D Base_Point { get { return base_point; } }
        private static Point3D base_point = new Point3D();

        public static  Angles_Scan_Object Angles { get { return angles; } }
        private static Angles_Scan_Object angles = new Angles_Scan_Object();

        public static void Rotate(Angles_Scan_Object _angles)
        {
            if (Model.Children.Count() == 0)
                return;

            angles = _angles;

            Matrix3D T = new TranslateTransform3D(Base_Point.X, Base_Point.Y, Base_Point.Z).Value;

            Matrix3D rotate1 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), _angles.Z_rotation), Base_Point).Value;
            Matrix3D rotate2 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), _angles.Y_rotation), Base_Point).Value;

            Matrix3D R = Matrix3D.Multiply(T, Matrix3D.Multiply(rotate1, rotate2));          

            foreach (Model3D m in Model.Children)
            {
                m.Transform = new MatrixTransform3D(R);
            }          
        }

        public static void Base(Point3D _Base)
        {
            base_point = _Base;
            Rotate(Angles);
        }
    }
}

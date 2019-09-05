using HelixToolkit.Wpf;
using ISC_Rentgen.Model3d;
using ISC_Rentgen.Rentgen_Parts.Manipulator_Components.Kinematic;
using ISC_Rentgen.Rentgen_Parts.Manipulator_Components.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ISC_Rentgen.Rentgen_Parts.Manipulator_Components
{
    public delegate void OnRefresh();

    public class ManipulatorV3
    {
        public static event OnRefresh OnRefreshSliders;

        public static Angles_Manipulator Angles { get { return angles; } }
        private static Angles_Manipulator angles = new Angles_Manipulator();

        public static Model3DGroup Model { get { return model; } }
        private static Model3DGroup model = new Model3DGroup();
        public static void Set_Models(Model3D join1, Model3D join2, Model3D join3, Model3D join4)
        {
            model.Children.Clear();
            model.Children.Add(join1);
            model.Children.Add(join2);
            model.Children.Add(join3);
            model.Children.Add(join4);
        }

        public static Join_Length_Manipulator Length { get; set; }
        public static Point3D Base_Point { get; set; }

        public static Point3D Rotate(Angles_Manipulator _angles)
        {
            if (model.Children.Count == 0) throw new Exception("Не задана модель");
                angles = _angles;

            List<Matrix3D> list_matrix = Kinematic.Manipulator_Kinematic.Direct_Kinematic(Base_Point, Length, Angles);

            Point3D join_point_2 = new Point3D() { X = list_matrix[0].M14, Y = list_matrix[0].M24, Z = list_matrix[0].M34 };
            Point3D join_point_3 = new Point3D() { X = list_matrix[1].M14, Y = list_matrix[1].M24, Z = list_matrix[1].M34 };
            Point3D join_point_4 = new Point3D() { X = list_matrix[2].M14, Y = list_matrix[2].M24, Z = list_matrix[2].M34 };


            Matrix3D Rotate1 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), Angles.O1), Base_Point).Value;
            Matrix3D Rotate2 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(Rotate1.M21, Rotate1.M22, Rotate1.M23), Angles.O2), join_point_2).Value;
            Matrix3D Rotate3 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(Rotate1.M21, Rotate1.M22, Rotate1.M23), Angles.O3), join_point_3).Value;

            Matrix3D T = new Matrix3D();

            model.Children.Where(x => x.GetName() == Model3DParts.ManipulatorParts.Join1).First().Transform = new MatrixTransform3D(Rotate1);
            T = Matrix3D.Multiply(Rotate1, Rotate2);
            model.Children.Where(x => x.GetName() == Model3DParts.ManipulatorParts.Join2).First().Transform = new MatrixTransform3D(T);
            T = Matrix3D.Multiply(T, Rotate3);
            model.Children.Where(x => x.GetName() == Model3DParts.ManipulatorParts.Join3).First().Transform = new MatrixTransform3D(T);

            Matrix3D Rotate4 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(T.M11, T.M12, T.M13), Angles.O4), join_point_4).Value;
            T = Matrix3D.Multiply(T, Rotate4);
            model.Children.Where(x => x.GetName() == Model3DParts.ManipulatorParts.Rentgen).First().Transform = new MatrixTransform3D(T);
            Matrix3D Rotate5 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(T.M21, T.M22, T.M23), Angles.O5), join_point_4).Value;
            T = Matrix3D.Multiply(T, Rotate5);
            model.Children.Where(x => x.GetName() == Model3DParts.ManipulatorParts.Rentgen).First().Transform = new MatrixTransform3D(T);

            return new Point3D(list_matrix.Last().M14, list_matrix.Last().M24, list_matrix.Last().M34);
        }
        
        public static Angles_Manipulator Set_Position(Point3D Position, Point3D Scan_Position)
        {
            Manipulator_Kinematic.SetBase(Base_Point);
            Manipulator_Kinematic.SetLen(new LengthJoin() { J1 = Length.L1, J2 = Length.L2, J3 = Length.L3, J4 = Length.L4, J5 = Length.L5, Det = Length.Det });
            Stack<Angle3D> rezult = Manipulator_Kinematic.InverseNab(Position.X, Position.Y, Position.Z, Scan_Position.X, Scan_Position.Y, Scan_Position.Z);

            if (rezult.Count != 0)
            {
                Angle3D peek = rezult.Peek();
                Angles_Manipulator angle = new Angles_Manipulator() {
                    O1 = peek.O1 * 180.0 / Math.PI,
                    O2 = peek.O2 * 180.0 / Math.PI,
                    O3 = peek.O3 * 180.0 / Math.PI,
                    O4 = peek.O4 * 180.0 / Math.PI,
                    O5 = peek.O5 * 180.0 / Math.PI
                };
                ManipulatorV3.Rotate(angle);

                OnRefreshSliders?.Invoke();

                return angle;
            }

            return null;
        }
    }
}

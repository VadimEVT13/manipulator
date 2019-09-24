using ISC_Rentgen.Rentgen_Parts.Scan_Object_Components.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ISC_Rentgen.Rentgen_Parts.Scan_Object_Components
{
    public class Scan_Object : INotifyPropertyChanged
    {
        private static Scan_Object instant;
        public static Scan_Object getInstant {
            get
            {
                if (instant == null)
                {
                    instant = new Scan_Object();
                }
                return instant;
            }
        }

        public Model3DGroup Model { get { return model; } set { model = value; } }
        private Model3DGroup model = new Model3DGroup();

        public Point3D Base_Point { get { return base_point; } }
        private Point3D base_point = new Point3D();

        private double base_x = 0;
        public double Base_X { get { return base_x; }
            set
            {
                base_x = value;
                base_point = new Point3D(base_x, base_y, base_z);
                Rotate(Angles);
                NotifyPropertyChanged(nameof(base_x));
            }
        }
        private double base_y = 0;
        public double Base_Y { get { return base_y; }
            set
            {
                base_y = value;
                base_point = new Point3D(base_x, base_y, base_z);
                Rotate(Angles);
                NotifyPropertyChanged(nameof(base_y));
            }
        }
        private double base_z = 0;
        public double Base_Z { get { return base_z; }
            set
            {
                base_z = value;
                base_point = new Point3D(base_x, base_y, base_z);
                Rotate(Angles);
                NotifyPropertyChanged(nameof(base_z));
            }
        }

        public void SetBase(Point3D Point)
        {
            Base_X = Point.X;
            Base_Y = Point.Y;
            Base_Z = Point.Z;
        }

        public  Angles_Scan_Object Angles { get { return angles; } }
        private Angles_Scan_Object angles = new Angles_Scan_Object();

        public void Rotate(Angles_Scan_Object _angles)
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

        public void Base(Point3D _Base)
        {
            base_point = _Base;
            Rotate(Angles);
        }

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

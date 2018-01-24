using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace InverseTest.GUI.Model
{
    class ScanPoint:IScanPoint
    {

        const int length = 2;

        Model3D model;

        public event PositoinHandler PositoinChanged;

        public ScanPoint()
        {
            Point3D point = new Point3D(0, 0, 0);
            MeshGeometry3D boxMesh = new MeshGeometry3D();
            boxMesh.Positions = new Point3DCollection()
                {
                    new Point3D(point.X, point.Y, point.Z),
                    new Point3D(point.X + length, point.Y, point.Z),
                    new Point3D(point.X, point.Y + length, point.Z),
                    new Point3D(point.X+ length, point.Y + length, point.Z),
                    new Point3D(point.X, point.Y, point.Z + length),
                    new Point3D(point.X+ length, point.Y, point.Z + length),
                    new Point3D(point.X, point.Y+ length, point.Z+ length),
                    new Point3D(point.X+ length, point.Y + length, point.Z + length)
                };

            boxMesh.TriangleIndices = new Int32Collection() { 2, 3, 1, 2, 1, 0, 7, 1, 3, 7, 5, 1, 6, 5, 7, 6, 4, 5, 6, 2, 0, 6, 0, 4, 2, 7, 3, 2, 6, 7, 0, 1, 5, 0, 5, 4 };
            GeometryModel3D boxGeom = new GeometryModel3D();
            boxGeom.Geometry = boxMesh;
            DiffuseMaterial mat = new DiffuseMaterial(new SolidColorBrush(Colors.Blue));
            boxGeom.Material = mat;

            model = boxGeom;           
        }

        void IScanPoint.MoveToPositoin(Point3D newPositoin)
        {
            Point3D oldLocation = GetTargetPoint();
            // Вычисляем смещение для новой точки съемки относительно старой
            newPositoin.X = newPositoin.X - oldLocation.X;
            newPositoin.Y = newPositoin.Y - oldLocation.Y;
            newPositoin.Z = newPositoin.Z - oldLocation.Z;

            TranslateTransform3D transform = new TranslateTransform3D(newPositoin.X, newPositoin.Y, newPositoin.Z);
            Transform3D oldTransform = model.Transform;
            model.Transform = Transform3DHelper.CombineTransform(oldTransform, transform);
            PositoinChanged(model.Bounds.Location);
        }

        public Point3D GetTargetPoint()
        {
            Point3D targetPoint = model.Bounds.Location;
            targetPoint.Offset(length / 2, length / 2, length / 2);
            return targetPoint;
        }

        public void ChangeSize(double scale)
        {
            Point3D center = GetTargetPoint();
            ScaleTransform3D scaleTransform = new ScaleTransform3D(scale,scale,scale);
            Transform3D oldTransform = model.Transform;
            model.Transform = Transform3DHelper.CombineTransform(scaleTransform, oldTransform);
        }

        public Model3D GetModel()
        {
            return model;
        }
    }
}

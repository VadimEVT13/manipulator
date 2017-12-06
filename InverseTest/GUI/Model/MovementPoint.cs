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
    class MovementPoint:IMovementPoint
    {

        const int length = 2;

        SphereVisual3D point;

        public event PositoinHandler PositoinChanged;

        public MovementPoint(Color color)
        {

            Material material = MaterialHelper.CreateMaterial(color);

            point = new SphereVisual3D()
            {
                Center = new Point3D(0,0,0),
                Material = material,
                Radius = length
            };
        }

        void IMovementPoint.MoveToPositoin(Point3D newPositoin)
        {
            Point3D oldLocation = GetTargetPoint();
            // Вычисляем смещение для новой точки съемки относительно старой
            newPositoin.X = newPositoin.X - oldLocation.X;
            newPositoin.Y = newPositoin.Y - oldLocation.Y;
            newPositoin.Z = newPositoin.Z - oldLocation.Z;

            TranslateTransform3D transform = new TranslateTransform3D(newPositoin.X, newPositoin.Y, newPositoin.Z);
            Transform3D oldTransform = point.Model.Transform;
            point.Model.Transform = Transform3DHelper.CombineTransform(oldTransform, transform);
            PositoinChanged?.Invoke(GetTargetPoint());
        }

        public Point3D GetTargetPoint()
        {
            Point3D targetPoint = point.Model.Bounds.Location;
            targetPoint.Offset(length / 2, length / 2, length / 2);
            return targetPoint;
        }

        public void ChangeSize(double scale)
        {
            //TODO сделать изменение размера точки
        }

        public Model3D GetModel()
        {
            return point.Model;
        }
        
    }
}

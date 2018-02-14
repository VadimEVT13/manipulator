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

    class MovementPoint : IMovementPoint
    {
        private const int defaultRadius = 2;

        private SphereVisual3D point;

        public event PositoinHandler PositoinChanged;

        public MovementPoint(Color color)
        {

            Material material = MaterialHelper.CreateMaterial(color, 0.5);

            point = new SphereVisual3D()
            {
                Center = new Point3D(0, 60, 0),
                Material = material,
                Radius = defaultRadius
            };
        }

        public void Move(Point3D newPositoin)
        {
            Point3D oldLocation = GetTargetPoint();
            // Вычисляем смещение для новой точки съемки относительно старой
            newPositoin.X = newPositoin.X - oldLocation.X;
            newPositoin.Y = newPositoin.Y - oldLocation.Y;
            newPositoin.Z = newPositoin.Z - oldLocation.Z;

            TranslateTransform3D transform = new TranslateTransform3D(newPositoin.X, newPositoin.Y, newPositoin.Z);
            Transform3D oldTransform = point.Model.Transform;
            point.Model.Transform = Transform3DHelper.CombineTransform(oldTransform, transform);
        }

        public void MoveAndNotify(Point3D newPosition)
        {
            this.Move(newPosition);
            PositoinChanged?.Invoke(GetTargetPoint());
        }

        public Point3D GetTargetPoint()
        {
            Point3D targetPoint = point.Model.Bounds.Location;
            targetPoint.Offset(defaultRadius / 2.0, defaultRadius / 2.0, defaultRadius / 2.0);
            return targetPoint;
        }

        public void ChangeSize(PointState state)
        {
            if (state == PointState.ENLAGE)
            {
                this.point.Radius = defaultRadius * 1.5;
            }
            else this.point.Radius = defaultRadius;
        }

        public Model3D GetModel()
        {
            return point.Model;
        }
    }
}

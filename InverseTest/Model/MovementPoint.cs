using HelixToolkit.Wpf;
using InverseTest.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace InverseTest.GUI.Model
{

    public class MovementPoint
    {
        private const int defaultRadius = 2;

        private SphereVisual3D point;

        public event PositoinHandler PositoinChanged;

        public MovementPoint(Color color)
        {
            Material material = MaterialHelper.CreateMaterial(color, 0.4);

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
            var position = new Point3D();
            position.X= newPositoin.X - oldLocation.X;
            position.Y = newPositoin.Y - oldLocation.Y;
            position.Z = newPositoin.Z - oldLocation.Z;

            TranslateTransform3D transform = new TranslateTransform3D(position.X, position.Y, position.Z);
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
            Point3D targetPoint = MathUtils.GetRectCenter(GetModel().Bounds);
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

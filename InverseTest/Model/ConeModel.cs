using HelixToolkit.Wpf;
using InverseTest.Manipulator;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace InverseTest.GUI.Model
{
    class ConeModel
    {
        private TruncatedConeVisual3D cone;
        public ConeModel()
        {
            cone = new TruncatedConeVisual3D()
            {
                Material = MaterialHelper.CreateMaterial(Colors.Chocolate, opacity: 0.2d),
                Height = 40,
                BaseRadius = 6,
                TopRadius= 0,
                Visible = false
            }; 
        }

        public void ChangePosition(Point3D origin, Vector3D direction, double focuseDistance)
        {
            direction.Normalize();
            cone.Height = focuseDistance;
            double radiusVision = focuseDistance * Math.Cos(MathUtils.AngleToRadians(30));
            cone.BaseRadius = radiusVision;

            Vector3D newDirection =  Vector3D.Multiply(direction, focuseDistance);
            cone.Origin = Point3D.Add(origin, newDirection);
            direction.Negate();
            cone.Normal = direction;
            cone.UpdateModel();
        }

        public Model3D GetModel()
        {
            return cone.Model;
        }

        public void SetVisibility(bool visible)
        {
            cone.Visible = visible;
        }
    }
}

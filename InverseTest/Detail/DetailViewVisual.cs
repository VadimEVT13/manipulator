using InverseTest.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.GUI.Model
{
    public  class DetailVisual
    {
        private static Vector3D YRotateAxis = new Vector3D(0, 1, 0);

        public ModelVisual3D visual { get; }

        public DetailVisual(Model3D detailModel)
        {
            this.visual = new ModelVisual3D() { Content = detailModel };
        }

        public void Rotate(double angle)
        {
            Rect3D rectModel = visual.Content.Bounds;
            Point3D center = MathUtils.GetRectCenter(rectModel);
            RotateTransform3D rotate = new RotateTransform3D(new AxisAngleRotation3D(YRotateAxis, angle), center);
            visual.Transform = rotate;
        }
    }
}

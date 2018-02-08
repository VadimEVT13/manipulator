using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Frame
{
    abstract class  IDetectorFramePart
    {
        public abstract void TranslateTransform3D(Transform3D transform);

        public abstract void RotateTransform3D(Transform3D transform);

        public abstract Model3DGroup GetAllModelGroup();

        public abstract Model3D GetModelPart();

        public abstract Rect3D Bounds();

        public abstract void ResetTransforms();

public abstract Point3D GetCameraPosition();


    }
}

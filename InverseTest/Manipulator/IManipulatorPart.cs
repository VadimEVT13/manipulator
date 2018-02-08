using System.Windows.Media.Media3D;

namespace InverseTest.Manipulator
{
    public interface IManipulatorPart
    {
        void RotateTransform3D(Transform3D transform);

        Model3D GetModel();
    }
}

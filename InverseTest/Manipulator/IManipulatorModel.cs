using System.Windows.Media.Media3D;
using static InverseTest.ManipulatorV2;

namespace InverseTest.Manipulator
{
    public interface IManipulatorModel:IPositionChanged
    {
        Model3D GetManipulatorModel();

        Model3D GetManipulatorPart(ManipulatorParts part);

        void RotatePart(ManipulatorParts part, double angle);

        Vector3D GetCameraDirection();

        Point3D GetCameraPosition();

        void MoveManipulator(ManipulatorAngles angles, bool animate);

        Point3D GetPointJoint(ManipulatorRotatePoints point);

        void ResetModel();
    }
}

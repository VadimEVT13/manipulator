using InverseTest.Frame.Kinematic;
using System.Windows.Media.Media3D;
using static InverseTest.DetectorFrame;

namespace InverseTest
{
    public interface IDetectorFrame
    {
        Model3D GetDetectorFrameModel();

        Model3D GetDetectorFramePart(DetectorFrame.Parts part);

        void MoveDetectFrame(DetectorFramePosition p);

        void MovePart(DetectorFrame.Parts part, double offset);

        void RotatePart(DetectorFrame.Parts part, double angle, Vector3D rotateAxis);

        Vector3D GetScreenDirection();

        void ResetTransforms();
    }
}
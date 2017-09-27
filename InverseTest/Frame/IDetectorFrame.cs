using System.Windows.Media.Media3D;

namespace InverseTest
{
    public interface IDetectorFrame
    {
        Model3D GetDetectorFrameModel();

        Model3D GetDetectorFramePart(DetectorFrame.Parts part);
    }
}
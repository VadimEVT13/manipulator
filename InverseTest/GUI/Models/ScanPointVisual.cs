using System.Windows.Media.Media3D;
using InverseTest.Model;
using InverseTest.Path;

namespace InverseTest.GUI.Models
{
    public class ScanPointVisual : BaseScanPointVisual
    {
        public ModelVisual3D pointVisual { get; }

        public ScanPointVisual(ScanPoint orderedPoint) : base(orderedPoint)
        {
            this.pointVisual = new ModelVisual3D() { Content = model };
        }

        public void TransformPoint(Transform3D t)
        {
            this.pointVisual.Transform = t;
        }
    }
}

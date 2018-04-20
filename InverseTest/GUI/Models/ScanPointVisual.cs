using HelixToolkit.Wpf;
using InverseTest.Model;
using InverseTest.Path;
using System.Windows.Media.Media3D;

namespace InverseTest.GUI.Model
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

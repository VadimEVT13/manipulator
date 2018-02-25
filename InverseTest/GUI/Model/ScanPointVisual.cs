using HelixToolkit.Wpf;
using InverseTest.Model;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace InverseTest.GUI.Model
{
    class ScanPointVisual
    {
        const int DEFAULT_RADIUS = 1;

        public ModelVisual3D pointVisual { get; }
        public ScanPoint scanPoint{get;}

        public ScanPointVisual(ScanPoint orderedPoint)
        {
            this.scanPoint = orderedPoint;
            MeshBuilder builder = new MeshBuilder();
            builder.AddSphere(orderedPoint.point, DEFAULT_RADIUS);
            this.pointVisual = new ModelVisual3D()
            {
                Content = new GeometryModel3D(builder.ToMesh(), Materials.Red)
            };
        }
    }
}

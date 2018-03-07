using HelixToolkit.Wpf;
using InverseTest.Path;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Model
{
   public abstract class BaseScanPointVisual
    {
        private static Material DEFAULT_MATERIAL = Materials.Green;
        private static Material SELECTED_MATERIAL = Materials.Orange;

        const double DEFAULT_RADIUS = 1.5;

        public ScanPoint Point { get; }
        public GeometryModel3D model;

        public BaseScanPointVisual(ScanPoint p) {

            this.Point = p;
            MeshBuilder meshBuilder = new MeshBuilder();
            meshBuilder.AddSphere(Point.point, DEFAULT_RADIUS);

            model = new GeometryModel3D(meshBuilder.ToMesh(), DEFAULT_MATERIAL);
        }

        public void SetSelected(bool selected)
        {
            if (selected)
            {
                model.Material = SELECTED_MATERIAL;
            }
            else
            {
                model.Material = DEFAULT_MATERIAL;
            }
        }


    }
}

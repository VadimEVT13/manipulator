using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace InverseTest.Detail
{

    /// <summary>
    /// Класс для представления трехмерной детали которую будут сканировать
    /// </summary>
    class DetailModel
    {
        private readonly Model3D detailModel;

        public DetailModel(Model3D detailModel)
        {
            this.detailModel = detailModel;

        }

        public Model3D GetModel()
        {
            return detailModel;
        }

        public void SetJunctionsPoints(int[] indexes)
        {
            
            GeometryModel3D geomModel = detailModel as GeometryModel3D;
            
            Color defaultColor = (((geomModel.Material as MaterialGroup).Children[0] as DiffuseMaterial).Brush as SolidColorBrush).Color;
            RadialGradientBrush brash = new RadialGradientBrush(Colors.Red, Colors.Transparent);

           
            DiffuseMaterial junctionMaterial = new DiffuseMaterial(brash);
            (geomModel.Material as MaterialGroup).Children.Add(junctionMaterial);
            MeshGeometry3D meshGeom = geomModel.Geometry as MeshGeometry3D;

            Point[] texturesPoints = new Point[meshGeom.Positions.Count];

            for (int i = 0; i < texturesPoints.Length; i++)
                texturesPoints[i] = new Point(0.0, 0.0);

            for (int i = 0; i < indexes.Length; i++)
            {
                texturesPoints[indexes[i]] = new Point(1, 1);
            }

            meshGeom.TextureCoordinates = new PointCollection(texturesPoints);
        }
    }
}

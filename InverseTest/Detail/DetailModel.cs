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

        private readonly Visual3D detailVisual;
        private readonly Model3D detailModel;

        public DetailModel(Model3D detailModel)
        {
            this.detailVisual = new ModelVisual3D() { Content = detailModel };
            this.detailModel = detailModel;

        }

        public Model3D GetModel()
        {
            return detailModel;
        }

        public MeshGeometry3D GetCountours(Point3D point , Vector3D normal)
        {

            const int counturCount = 8;
            const double diametrTube = 0.05;

            MeshBuilder meshBuilder = new MeshBuilder();
            
            var bounds = detailModel.Bounds;

            double startX = bounds.Location.X;
            double endX = bounds.Location.X + bounds.SizeX;
            double delta = bounds.SizeX / counturCount;

            for (double i = startX; i <= endX; i += delta)
            {
                Point3D counturPoint = new Point3D(i, 0, 0);
                IList<Point3D> points = MeshGeometryHelper.GetContourSegments((detailModel as GeometryModel3D).Geometry as MeshGeometry3D, counturPoint, new Vector3D(1, 0, 0));
                var combine = MeshGeometryHelper.CombineSegments(points, 1e-6);
                foreach (var contour in combine.ToList())
                {
                    if (contour.Count == 0)
                        continue;
                    meshBuilder.AddTube(contour, diametrTube, counturCount, true);

                }
            }
            double startY = bounds.Location.Y;
            double endY = bounds.Location.Y + bounds.SizeY;
            delta = bounds.SizeY / counturCount;

            for (double i = startY; i <= endY; i += delta)
            {
                Point3D counturPoint = new Point3D(0, i, 0);
                IList<Point3D> points = MeshGeometryHelper.GetContourSegments((detailModel as GeometryModel3D).Geometry as MeshGeometry3D, counturPoint, new Vector3D(0, 1, 0));
                var combine = MeshGeometryHelper.CombineSegments(points, 1e-6);
                foreach (var contour in combine.ToList())
                {
                    if (contour.Count == 0)
                        continue;
                    meshBuilder.AddTube(contour, diametrTube, counturCount, true);

                }

            }
            double startZ = bounds.Location.Z;
            double endZ = bounds.Location.Z + bounds.SizeZ;
            delta = bounds.SizeZ / counturCount;

            for (double i = startZ; i <= endZ; i += delta)
            {
                Point3D counturPoint = new Point3D(0, 0, i);
                IList<Point3D> points = MeshGeometryHelper.GetContourSegments((detailModel as GeometryModel3D).Geometry as MeshGeometry3D, counturPoint, new Vector3D(0, 0, 1));
                var combine = MeshGeometryHelper.CombineSegments(points, 1e-6);
                foreach (var contour in combine.ToList())
                {
                    if (contour.Count == 0)
                        continue;
                    meshBuilder.AddTube(contour, diametrTube, counturCount, true);

                }
            }


            return meshBuilder.ToMesh();
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

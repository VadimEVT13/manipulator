using HelixToolkit.Wpf;
using InverseTest.GUI;
using InverseTest.GUI.Models;
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
    public class DetailModel : IVisualController, IPositionChanged
    {

        public MainVisual detailVisual { get; }

        public List<MainVisual> Visuals => new List<MainVisual>();

        public readonly Model3D detailModel;
        public readonly Visual3D counturVisual;

        public event PositionHandler onPositionChanged;
        public event ManualPositionHandler onManulaPositionChanged;

        public DetailModel(Model3D detailModel)
        {
            this.detailVisual = new MainVisual(detailModel);
            this.detailModel = detailModel;

            GeometryModel3D model = new GeometryModel3D(GetCountours(), Materials.Green);

            this.counturVisual = new ModelVisual3D()
            {
                Content = model
            };
        }

        public void Transform(Transform3D transform)
        {
            this.detailVisual.Transform(transform);
            onPositionChanged?.Invoke();
        }


        public Model3D GetModel()
        {
            return detailModel;
        }

        private MeshGeometry3D GetCountours()
        {
            const int counturCount = 10;
            const double diametrTube = 0.05;

            MeshBuilder meshBuilder = new MeshBuilder();
            
            var bounds = detailModel.Bounds;

            double start = bounds.Location.X;
            double end = bounds.Location.X + bounds.SizeX;

            for (int i = 1; i < counturCount; i++)
            {
                Point3D counturPoint = new Point3D(bounds.Location.X + bounds.SizeX * i/counturCount, 0, 0);
                IList<Point3D> points = MeshGeometryHelper.GetContourSegments((detailModel as GeometryModel3D).Geometry as MeshGeometry3D, counturPoint, new Vector3D(1, 0, 0));
                var combine = MeshGeometryHelper.CombineSegments(points, 1e-6);
                foreach (var contour in combine.ToList())
                {
                    if (contour.Count == 0)
                        continue;
                    meshBuilder.AddTube(contour, diametrTube, counturCount, true);
                }
            }

            start = bounds.Location.Y;
            end = bounds.Location.Y + bounds.SizeY;
            
            for (int i =1; i < counturCount; i++)
            {
                Point3D counturPoint = new Point3D(0, bounds.Location.Y + bounds.SizeY * i / counturCount, 0);
                IList<Point3D> points = MeshGeometryHelper.GetContourSegments((detailModel as GeometryModel3D).Geometry as MeshGeometry3D, counturPoint, new Vector3D(0, 1, 0));
                var combine = MeshGeometryHelper.CombineSegments(points, 1e-6);
                foreach (var contour in combine.ToList())
                {
                    if (contour.Count == 0)
                        continue;
                    meshBuilder.AddTube(contour, diametrTube, counturCount, true);

                }

            }
            start = bounds.Location.Z;
            end= bounds.Location.Z + bounds.SizeZ;
            
            for (int i = 1; i < counturCount; i++)
            {
                Point3D counturPoint = new Point3D(0, 0, bounds.Location.Z + bounds.Size.Z * i/counturCount);
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

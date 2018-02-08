using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using MIConvexHull;
using InverseTest.Collision;

namespace InverseTest
{
    class HullsV2
    {
        private List<Point3D> verts = new List<Point3D>(); //верршины меша
        private List<Point3D> verts_2;

        private List<DefaultConvexFace<Vertex>> CVXfaces; //коллекция треугольников меша
        private List<Vertex> CVXvertices; //колекция вершин (Vertex) меша

        private MeshGeometry3D mesh = new MeshGeometry3D(); //меш для получения всех вершин

        private List<ConvexHull<Vertex, DefaultConvexFace<Vertex>>> ConvHulls = new List<ConvexHull<Vertex, DefaultConvexFace<Vertex>>>(); //коллекция оболочек
        private List<MeshGeometry3D> meshs = new List<MeshGeometry3D>(); //коллекция мешей
        public void clear()
        {
            meshs.Clear();
        }

        //public void BuildShell (Model3DGroup Model_0, Model3DGroup Model_1, Transform3D trans_0, Transform3D trans_1)

        //~HullsV2() { }

        public void BuildShell(Model3DGroup Model)//, Transform3D trans)
        {
            Transform3D trans = Model.Transform;
            verts_2 = new List<Point3D>(verts);
            verts.Clear();
            foreach (var model in Model.Children)
            {
                if (typeof(GeometryModel3D).IsInstanceOfType(model))
                    if (typeof(MeshGeometry3D).IsInstanceOfType(((GeometryModel3D)model).Geometry))
                    {
                        mesh = (MeshGeometry3D)((GeometryModel3D)model).Geometry; //получаем меш модели
                        verts.AddRange(mesh.Positions); //получаем все вершины меша
                    }
            }
            //verts = verts.Distinct().ToList();
            CVXvertices = verts.Distinct(new PointComparer()).Select(p => new Vertex(p)).ToList(); //точки для построения оболочки
            
            var hull = ConvexHull.Create(CVXvertices); //построение оболочки
            ConvHulls.Add(hull);
            meshs.Add(mesh);

            verts.Clear();
            CVXvertices = hull.Points.ToList();  //получаем точки оболочки
            verts.AddRange(CVXvertices.Select(p => new Point3D(p.Position[0], p.Position[1], p.Position[2]))); //преобразуем в Point3D
            for (int j = 0; j < verts.Count; j++) //применяем трансформ к точкам оболочки
            {
                Point3D p = trans.Value.Transform(verts[j]);
                verts[j] = p;
            }
        }

        public void DisplayConvexHull()
        {
            List<Point3D> points = new List<Point3D>();
            
            CVXvertices.Clear();
            for (int i = 0; i < ConvHulls.Count; i++)
            {
                CVXvertices = ConvHulls[i].Points.ToList();
                CVXfaces = ConvHulls[i].Faces.ToList();
                points.AddRange(CVXvertices.Select(p => new Point3D(p.Position[0], p.Position[1], p.Position[2])));
                meshs[i].Positions = new Point3DCollection(points);
                var faceTriCollection = new Int32Collection();
                foreach (var f in CVXfaces)
                    foreach (var v in f.Vertices)
                        faceTriCollection.Add(CVXvertices.IndexOf(v));
                meshs[i].TriangleIndices = faceTriCollection;

                points.Clear();
                //faceTriCollection.Clear();
            }

            

        }

        public bool find()
        {
            GJKFinder finderGJK = new GJKFinder(verts_2, verts);

            if (finderGJK.testGJKIntersection())
            {
                return true;
            }
            Vector3D[] resultSimplex = finderGJK.getSimplex();
            if (resultSimplex != null)
            {
                return true;
            }
            return false;
        }
    }
}

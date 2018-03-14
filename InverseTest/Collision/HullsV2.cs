using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using MIConvexHull;
using InverseTest.Collision;
using InverseTest.Collision.Model;
using InverseTest.GUI;

namespace InverseTest
{
    class HullsV2
    {
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

        public List<Point3D> BuildShell(Model3DCollision Model)//, Transform3D trans)
        {
            List<Point3D> verts = new List<Point3D>();
            Transform3D trans = new MatrixTransform3D(Model.shape.Transform);
            List<Point3D>  points = Model.shape.Points;
           
                CVXvertices = points.Distinct(new PointComparer()).Select(p => new Vertex(p)).ToList(); //точки для построения оболочки
          
            var hull = ConvexHull.Create(CVXvertices); //построение оболочки
            ConvHulls.Add(hull);
            meshs.Add(mesh);

            CVXvertices = hull.Points.ToList();  //получаем точки оболочки
            verts.AddRange(CVXvertices.Select(p => new Point3D(p.Position[0], p.Position[1], p.Position[2]))); //преобразуем в Point3D
            for (int j = 0; j < verts.Count; j++) //применяем трансформ к точкам оболочки
            {
                Point3D p = trans.Value.Transform(verts[j]);
                verts[j] = p;
            }
            return verts;
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

        public bool find(List<Point3D> verts1, List<Point3D> verts2, SimplexView simplex)
        {
            GJKFinder finderGJK = new GJKFinder(verts1, verts2, simplex);

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

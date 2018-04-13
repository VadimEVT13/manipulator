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
        private List<Vertex> CVXvertices; //колекция вершин (Vertex) меша

        public List<Point3D> BuildShell(Model3DCollision Model)//, Transform3D trans)
        {
            List<Point3D> verts = new List<Point3D>();
            Transform3D trans = new MatrixTransform3D(Model.shape.Transform);
            List<Point3D>  points = Model.shape.Points;
           
            CVXvertices = points.Distinct(new PointComparer()).Select(p => new Vertex(p)).ToList(); //точки для построения оболочки
            var hull = ConvexHull.Create(CVXvertices); //построение оболочки

            CVXvertices = hull.Points.ToList();  //получаем точки оболочки
            verts.AddRange(CVXvertices.Select(p => new Point3D(p.Position[0], p.Position[1], p.Position[2]))); //преобразуем в Point3D
            for (int j = 0; j < verts.Count; j++) //применяем трансформ к точкам оболочки
            {
                Point3D p = trans.Value.Transform(verts[j]);
                verts[j] = p;
            }
            return verts;
        }

        public bool find(List<Point3D> verts1, List<Point3D> verts2)
        {
            GJKFinder finderGJK = new GJKFinder(verts1, verts2);

            if (finderGJK.testGJKIntersection())
            {
                return true;
            }
            return false;
        }
    }
}

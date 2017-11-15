using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using MIConvexHull;

namespace InverseTest
{
    public class Collision
    {
        //GUI.ManipulatorVisualizer ManipulatorVisualizer = new GUI.ManipulatorVisualizer();

        public void OnManipulatorPosChanged()
        {

        }








        internal class Point3DComparer : IEqualityComparer<Point3D>
        {
            #region Implementation of IEqualityComparer<in Point3D>

            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <returns>
            /// true if the specified objects are equal; otherwise, false.
            /// </returns>
            /// <param name="x">The first object of type <paramref name="T"/> to compare.</param><param name="y">The second object of type <paramref name="T"/> to compare.</param>
            bool IEqualityComparer<Point3D>.Equals(Point3D x, Point3D y)
            {
                return ((x - y).Length < 0.000000001);
            }

            /// <summary>
            /// Returns a hash code for the specified object.
            /// </summary>
            /// <returns>
            /// A hash code for the specified object.
            /// </returns>
            /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
            int IEqualityComparer<Point3D>.GetHashCode(Point3D obj)
            {
                var d = obj.ToVector3D().LengthSquared;
                return d.GetHashCode();
            }

            #endregion
        }

        class Vertex : IVertex
        {
            public double[] Position { get; set; }

            public Vertex(Point3D point)
            {
                Position = new double[3] { point.X, point.Y, point.Z };
            }
        }

        private List<DefaultConvexFace<Vertex>> CVXfaces;
        private List<Vertex> CVXvertices;

        public Model3DGroup Model { get; set; }
        //private List< MeshGeometry3D> mesh2 = new List<MeshGeometry3D>();
        private MeshGeometry3D mesh = new MeshGeometry3D();
        List<Vertex> vertices;

        //List<Model3DGroup> ListModels = new List<Model3DGroup>();
        List<ConvexHull<Vertex, DefaultConvexFace<Vertex>>> hulls = new List<ConvexHull<Vertex, DefaultConvexFace<Vertex>>>();
        List<MeshGeometry3D> meshs = new List<MeshGeometry3D>();

        public void BuildShell(Model3DGroup CurrentModel)
        {

            var verts = new List<Point3D>();
            mesh = null;
            foreach (var model in CurrentModel.Children)
            {
                if (typeof(GeometryModel3D).IsInstanceOfType(model))
                    if (typeof(MeshGeometry3D).IsInstanceOfType(((GeometryModel3D)model).Geometry))
                    {
                        mesh = (MeshGeometry3D)((GeometryModel3D)model).Geometry;
                        verts.AddRange(mesh.Positions);
                    }
            }
            vertices = verts.Distinct(new Point3DComparer()).Select(p => new Vertex(p)).ToList();

            CVXvertices = null;
            CVXfaces = null;
            MIConvexHull();
        }


        public void MIConvexHull()
        {
            try
            {
                var hull = ConvexHull.Create(vertices);
                hulls.Add(hull);
                meshs.Add(mesh);
                CVXvertices = hull.Points.ToList();
                CVXfaces = hull.Faces.ToList();
                List<ConvexHull<Vertex, DefaultConvexFace<Vertex>>> hulll = new List<ConvexHull<Vertex, DefaultConvexFace<Vertex>>>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }



        public void DisplayConvexHull()
        {
            for (int i = 0; i < hulls.Count; i++)
            {
                var verts = new List<Point3D>();
                CVXvertices = hulls[i].Points.ToList();
                CVXfaces = hulls[i].Faces.ToList();
                verts.AddRange(CVXvertices.Select(p => new Point3D(p.Position[0], p.Position[1], p.Position[2])));
                meshs[i].Positions = new Point3DCollection(verts);
                var faceTriCollection = new Int32Collection();
                foreach (var f in CVXfaces)
                    foreach (var v in f.Vertices)
                        faceTriCollection.Add(CVXvertices.IndexOf(v));
                meshs[i].TriangleIndices = faceTriCollection;
            }

        }
    }
}


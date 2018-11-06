using System.Collections.Generic;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using System.Linq;
using InverseTest.Collision;
using MIConvexHull;
using System;
using System.Windows;
using System.Windows.Media;

namespace InverseTest.GUI.Models
{
    public class MainVisual
    {
        public ModelVisual3D CamManip;
        public ModelVisual3D CamPortal;
        public ModelVisual3D Top;
        public ModelVisual3D Right;
        public ModelVisual3D Front;
        public ModelVisual3D _3D;

        private Model3D model;
        private List<Geometry3D> mg3d;

        private Material[] DefaultMaterials;
        private Material CollisionMaterial = Materials.Red;

        public MainVisual(Model3D model, bool camDisplayed = true)
        {

            this.model = model;
            saveMesh();


            StoreDefaultMaterials(model);
            if (camDisplayed)
            {
                CamManip = new ModelVisual3D() { Content = model };
                CamPortal = new ModelVisual3D() { Content = model };
            }

            this.Top = new ModelVisual3D() { Content = model };
            this.Right = new ModelVisual3D() { Content = model };
            this.Front = new ModelVisual3D() { Content = model };
            this._3D = new ModelVisual3D() { Content = model };
        }

        public void saveMesh()
        {
            mg3d = new List<Geometry3D>();
            if (model is Model3DGroup mg)
            {
                foreach (var m in mg.Children)
                {
                    mg3d.Add(GetGeometry(m));
                }
            }
            else
            {
                mg3d.Add(GetGeometry(this.model));
            }
        }

        private Geometry3D GetGeometry(Model3D m)
        {

            if (m is GeometryModel3D geomModel)
                if (geomModel.Geometry is Geometry3D geometry)
                {
                    return geometry;
                }

            return null;
        }
    

        public void BuildShell()
        {
            //verts.Clear();
            List<Point3D> verts = new List<Point3D>();
            List<Point3D> verts_2 = new List<Point3D>();
            List<List<Point3D>> verts_3 = new List<List<Point3D>>();
            MeshGeometry3D mesh = new MeshGeometry3D();
            List<Vertex> vertices;
            

        mesh = null;
            Model3DGroup mg;
            if (!(this.model is Model3DGroup))
            {
                mg = new Model3DGroup();
                mg.Children.Add(this.model);
            }
            else mg = this.model as Model3DGroup;
            foreach (var model in mg.Children)
            {
                if (typeof(GeometryModel3D).IsInstanceOfType(model))
                    if (typeof(MeshGeometry3D).IsInstanceOfType(((GeometryModel3D)model).Geometry))
                    {
                        mesh = (MeshGeometry3D)((GeometryModel3D)model).Geometry;
                        verts.AddRange(mesh.Positions);
                    }
            }
            verts_2 = verts.Distinct().ToList();
            vertices = verts.Distinct(new Point3DComparer()).Select(p => new Vertex(p)).ToList(); //точки для оболочки

            verts.Clear();
            verts_2.Clear();

            MIConvexHull(vertices, mesh);
        }

        public void MIConvexHull(List<Vertex> vertices, MeshGeometry3D mesh)
        {
            List<DefaultConvexFace<Vertex>> CVXfaces;
            List<Vertex> CVXvertices;
            List<ConvexHull<Vertex, DefaultConvexFace<Vertex>>> ConvHulls = new List<ConvexHull<Vertex, DefaultConvexFace<Vertex>>>();
            List<MeshGeometry3D> meshs = new List<MeshGeometry3D>();
            List<Point3D> verts = new List<Point3D>();
            try
            {
                var hull = ConvexHull.Create(vertices);
                CVXvertices = hull.Points.ToList();
                CVXfaces = hull.Faces.ToList();
                verts.AddRange(CVXvertices.Select(p => new Point3D(p.Position[0], p.Position[1], p.Position[2])));
                mesh.Positions = new Point3DCollection(verts);
                var faceTriCollection = new Int32Collection();
                foreach (var f in CVXfaces)
                    foreach (var v in f.Vertices)
                        faceTriCollection.Add(CVXvertices.IndexOf(v));
                mesh.TriangleIndices = faceTriCollection;
                verts.Clear();
            }
            catch (Exception ex)
            {
            }

        }

        public void removeShell()
        {
            if(model is Model3DGroup mg)
            {
                for(int i = 0; i < mg.Children.Count; i++)
                {
                    if(mg.Children[i] is GeometryModel3D gm)
                    {
                        gm.Geometry = mg3d[i]; 
                    }
                }
            }
            else
            {
                if(model is GeometryModel3D gm)
                {
                    gm.Geometry = mg3d[0];
                }
            }
        }

        public void Transform(Transform3D transform)
        {
            this.model.Transform = transform;
        }


        private void StoreDefaultMaterials(Model3D model) {

            List<Material> materials = new List<Material>();
            if (model is Model3DGroup modelGroup)
            {
                foreach (Model3D m in modelGroup.Children)
                {
                    Material newMaterial = CreateNewMaterial((m as GeometryModel3D).Material);
                    materials.Add(newMaterial);
                }
            }
            else
            {
                Material newMaterial = CreateNewMaterial((model as GeometryModel3D).Material);
                materials.Add(newMaterial);
            }
                this.DefaultMaterials = materials.ToArray();
        }

        public void SetCollisionCollor()
        {
            if (model is Model3DGroup modelGroup)
            {
                foreach (GeometryModel3D m in modelGroup.Children)
                {
                    m.Material = CollisionMaterial;
                }
            }
            else (model as GeometryModel3D).Material = CollisionMaterial;
        }

        public void SetDefaultColor()
        {
            if (model is Model3DGroup modelGroup)
            {
                for(int i= 0; i< modelGroup.Children.Count; i++)
                {
                    (modelGroup.Children[i] as GeometryModel3D).Material = DefaultMaterials[i];
                }
            }
            else (model as GeometryModel3D).Material = DefaultMaterials[0];
        }

        private Material CreateNewMaterial(Material material)
        {
            MaterialGroup newMaterialGroup = new MaterialGroup();
            if(material is MaterialGroup mg)
            {
                foreach(Material mt in mg.Children)
                {
                    if (mt is DiffuseMaterial dfm)
                    {
                        newMaterialGroup.Children.Add(new DiffuseMaterial(dfm.Brush));
                    }
                    else if (mt is SpecularMaterial spm) {
                        newMaterialGroup.Children.Add(new SpecularMaterial(spm.Brush, spm.SpecularPower));
                    }
                }
            }

            return newMaterialGroup;
        }

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
}

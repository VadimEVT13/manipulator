using HelixToolkit.Wpf;
using InverseTest.Detail;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace InverseTest
{
    class ModelPreprocessor
    {
        /// <summary>
        /// Логгирование
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private Model3DGroup scene;

        private delegate MeshGeometry3D Proccess(GeometryModel3D m);

        public ModelPreprocessor(Model3DGroup scene)
        {
            this.scene = scene;
        }


        public ModelPreprocessor RemoveIsolated()
        {
            Proccess proc = (m =>
            {
                MeshGeometry3D newGeometry = MeshGeometryHelper.RemoveIsolatedVertices(m.Geometry as MeshGeometry3D); return newGeometry;
            });

            Iterate(proc);
            return this;
        }

        public ModelPreprocessor NoSharedVertices()
        {

            Proccess proc = (m =>
            {
                MeshGeometry3D newGeometry = MeshGeometryHelper.NoSharedVertices(m.Geometry as MeshGeometry3D);
                return newGeometry;
            });

            Iterate(proc);
            return this;
        }

        public ModelPreprocessor Simplify(double eps)
        {
            Proccess proc = (m =>
            {
                MeshGeometry3D newGeometry = MeshGeometryHelper.Simplify(m.Geometry as MeshGeometry3D, eps);
                return newGeometry;

            });
            Iterate(proc);

            return this;
        }

        public ModelPreprocessor Simplification()
        {

            Proccess proc = ((m) =>
            {
                MeshSimplification ms = new MeshSimplification(m.Geometry as MeshGeometry3D);
                MeshGeometry3D newMs = ms.Simplify(25000, 8, false, false);
                return newMs;
            });

            Iterate(proc);

            return this;
        }

        public void check()
        {
            Proccess proc = new Proccess((m) =>
            {
                MeshGeometry3D geometry = m.Geometry as MeshGeometry3D;
                Point3DCollection points = geometry.Positions;
                Int32Collection triangles = geometry.TriangleIndices;
                PointCollection textures = geometry.TextureCoordinates;
                Vector3DCollection normals = geometry.Normals;


                for (int i = 0; i < points.Count-1; i++) {
                    for(int j = i; j<points.Count; j++)
                    {
                        if(points[i]==points[j])
                        {
                            if(normals[i] == normals[j])
                            {
                                logger.Trace("POINTS EQUALS: " + points[i] + "  " + normals[i] + "  " + points[j] + "  " +normals[j]);
                            }

                        }
                    }



                }



                return geometry;
            });


        }

        public bool Validate(Model3DGroup mg)
        {
            Model3DCollection models = new Model3DCollection();

            foreach (GeometryModel3D m in scene.Children)
            {
                logger.Trace("Validation: " + MeshGeometryHelper.Validate(m.Geometry as MeshGeometry3D));
            }

            return false;
        }


        private void Iterate(Proccess proc)
        {
            Model3DCollection models = new Model3DCollection();

            foreach (GeometryModel3D m in scene.Children)
            {
                MeshGeometry3D newGeom = proc(m);
                GeometryModel3D newModelGeometry = new GeometryModel3D(newGeom, m.Material);
                models.Add(newModelGeometry);
            }
            this.scene.Children = models;
        }
        
        public Model3DGroup GetProccessedModel()
        {
            return scene;
        }
    }
}

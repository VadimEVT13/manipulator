using HelixToolkit.Wpf;
using InverseTest.Detail;
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

        public ModelPreprocessor RemoveEqualsVertices()
        {
            Proccess proc = (m =>
            {
                MeshGeometry3D geometry = m.Geometry as MeshGeometry3D;
                Point3DCollection points = geometry.Positions;
                Int32Collection triangles = geometry.TriangleIndices;
                PointCollection textures = geometry.TextureCoordinates;
                Vector3DCollection normals = geometry.Normals;

                Dictionary<Point3D, int> pointToIndex = new Dictionary<Point3D, int>();
                Dictionary<Point3D, int> pointToNormal = new Dictionary<Point3D, int>();

                Int32Collection newTriangles = new Int32Collection();
                Vector3DCollection newNormal = new Vector3DCollection();
                Point3DCollection newPoints = new Point3DCollection();


                int pointIndex = 0;
                for (int triangleIndex = 0; triangleIndex < triangles.Count; triangleIndex += 3)
                {
                    int pIndex1 = triangles[triangleIndex];
                    int pIndex2 = triangles[triangleIndex + 1];
                    int pIndex3 = triangles[triangleIndex + 2];

                    Point3D p1 = points[pIndex1];
                    Point3D p2 = points[pIndex2];
                    Point3D p3 = points[pIndex3];

                    Vector3D normal1 = normals[pIndex1];
                    Vector3D normal2 = normals[pIndex2];
                    Vector3D normal3 = normals[pIndex3];


                    Point3D[] currenTrianglePoints = new Point3D[] { p1, p2, p3 };
                    Vector3D[] currentNormals = new Vector3D[] { normal1, normal2, normal3 };

                    for (int i = 0; i < currenTrianglePoints.Length; i++)
                    {
                        Point3D currP = currenTrianglePoints[i];

                        if (pointToIndex.ContainsKey(currP))
                        {
                            int index;
                            pointToIndex.TryGetValue(currP, out index);
                            newTriangles.Add(index);
                        }
                        else
                        {
                            pointToIndex.Add(currP, pointIndex);
                            newTriangles.Add(pointIndex);
                            newPoints.Add(currP);
                        }

                        pointIndex++;

                    }
                }
                
                //newNormal = MeshGeometryHelper.CalculateNormals(newPoints, newTriangles);

                MeshGeometry3D newGeometry = new MeshGeometry3D()
                {
                    Positions = newPoints,
                    TriangleIndices = newTriangles,
                    Normals = newNormal
                };
                return newGeometry;
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
                                Console.WriteLine("POINTS EQUALS: " + points[i] + "  " + normals[i] + "  " + points[j] + "  " +normals[j]);
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
                Console.WriteLine("Validation: " + MeshGeometryHelper.Validate(m.Geometry as MeshGeometry3D));
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

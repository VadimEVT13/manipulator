using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.JunctionDetect
{
    class JunctionDetectAlgorithm
    {

        private static Point3D[] meshVertices;
        private static int[] meshTriangles;


        public static Point3D[] Detect(Model3D model)
        {
            Point3D[] junctionPoints = new Point3D[0];

            MeshGeometry3D meshGeometry = ((MeshGeometry3D)((GeometryModel3D)model).Geometry);
            meshTriangles = meshGeometry.TriangleIndices.ToArray();
            meshVertices = meshGeometry.Positions.ToArray();

            double[] pointsCurvature = calculatePointsCurvature();



            return junctionPoints;
        }




        private static double[] calculatePointsCurvature()
        {
            double[] pointsCurvature = new double[meshVertices.Length];

            for (int i = 0; i < meshVertices.Length; i++)
            {

                double sumAnglesOfPolygons = 0f;
                double sumAreaAdjacentFace = 0f;

                List<int> indexesPolygons = getPolygonsAroundPoint(meshVertices[i]);

                foreach (int indexPolygon in indexesPolygons)
                {
                    List<Vector3D> vectors = getVectors(indexPolygon, meshVertices[i]);
                    sumAnglesOfPolygons += Vector3D.AngleBetween(vectors[0], vectors[1]);

                    double area = getArea(indexPolygon);
                    // Debug.Log("area: " + area);
                    sumAreaAdjacentFace += area;
                }

                pointsCurvature[i] = (3 * (2 * Math.PI - sumAnglesOfPolygons)) / sumAreaAdjacentFace;

            }





            return pointsCurvature;
        }


        /// <summary>
        ///  Возвращает список соседей у каждой точки
        /// </summary>
        /// <param name="N"></param>
        private static int[][] GetNeighbors(int N)
        {
            int[][] neighbors = new int[meshVertices.Length][];
            for (int i = 0; i < meshVertices.Length; i++)
            {

       //         neighbors[i] = GetPointNeighbors(i,  N);
            }

            return neighbors;
        }

       // private static int[] GetPointNeighbors(int pointIndex, int N)
     //   {
       //     List<int> neighborsIndexex = new List<int>();
         //   HashSet<int> viewedPoints = new HashSet<int>();
           // viewedPoints.Add(pointIndex);

//            for (int i = 0; i < N; i++)
  //          {
    //            meshTriangles.
      //      }
       // }

        //Возвращает точки вокруг заданной точки на уровне NEIGHBORHOOD_LAVEL
        private static int[] getNeighborhoodLevelPoints(int pointIndex, int N)
        {
            HashSet<int> previousLevelPoints = new HashSet<int>();
            HashSet<int> currentLevelPoints = new HashSet<int>();
            currentLevelPoints.Add(pointIndex);

            List<int> neighborhoodLevelPoints = new List<int>();

            for (int i = 0; i < N; i++)
            {
                neighborhoodLevelPoints.Clear();
                
                foreach (int currentPoint in currentLevelPoints)
                {
                    List<int> pointAround = getPointsAroundPoint(currentPoint);
                    foreach (int point2 in pointAround)
                    {
                        if (!previousLevelPoints.Contains(point2) && !currentLevelPoints.Contains(point2))
                        {
                            neighborhoodLevelPoints.Add(point2);
                        }
                    }
                }
                
                previousLevelPoints.UnionWith(currentLevelPoints);
                currentLevelPoints.Clear();
                currentLevelPoints.UnionWith(neighborhoodLevelPoints);
            }

            List<int> resPoints = new List<int>();
            for (int i = 0; i < neighborhoodLevelPoints.Count; i += N) 
                resPoints.Add(neighborhoodLevelPoints[i]);

            return resPoints.ToArray();
        }


        //Возвращает точки вокрут заданной точки
        private static List<int> getPointsAroundPoint(int point)
        {
            List<int> points = new List<int>();

            for (int pointIndex = 0; pointIndex < meshTriangles.Length; pointIndex++)
            {
                if (point == meshTriangles[pointIndex])
                {
                    int triangleIndex = pointIndex / 3;
                    for (int pointInTriangle = 3 * triangleIndex; pointInTriangle < pointInTriangle + 3; pointInTriangle++)
                    {
                        int currentPoint = meshTriangles[pointInTriangle];
                        if (currentPoint != point)
                        {
                            points.Add(currentPoint);
                        }
                    }
                }
            }
            return points;
        }


        private static List<Vector3D> getVectors(int indexPolygon, Point3D point)
        {
            List<Point3D> naightborPoints = new List<Point3D>();

            for (int i = 0; i < 3; i++)
            {
                Point3D currentPoint = meshVertices[meshTriangles[indexPolygon * 3 + i]];
                if (!isSamePoints(point, currentPoint))
                {
                    naightborPoints.Add(currentPoint);
                }
            }

            List<Vector3D> vectors = new List<Vector3D>();
            foreach (Point3D naightborPoint in naightborPoints)
            {
                vectors.Add(new Vector3D(naightborPoint.X - point.X, naightborPoint.Y - point.Y, naightborPoint.Z - point.Z));
            }

            return vectors;
        }

        /// <summary>
        /// Вычисляет площадь полигона
        /// </summary>
        /// <param name="indexPolygon"></param>
        /// <returns></returns>
        private static double getArea(int indexPolygon)
        {
            Point3D pointA = meshVertices[meshTriangles[indexPolygon * 3 + 0]];
            Point3D pointB = meshVertices[meshTriangles[indexPolygon * 3 + 1]];
            Point3D pointC = meshVertices[meshTriangles[indexPolygon * 3 + 2]];

            double AB = Distance(pointA, pointB);
            double AC = Distance(pointA, pointC);
            double BC = Distance(pointB, pointC);

            double semiperimeter = (AB + AC + BC) / 2;

            double area = Math.Sqrt((semiperimeter - AB) * (semiperimeter - AC) * (semiperimeter - BC) * semiperimeter);

            return area;

        }

        /// <summary>
        /// Вычисляет расстаяние между двумя точками
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        private static double Distance(Point3D point1, Point3D point2)
        {
            return Math.Sqrt(Math.Pow((point2.X - point1.X), 2) +
                Math.Pow((point2.Y - point1.Y), 2) +
                Math.Pow((point2.Z - point1.Z), 2)
                );
        }

        private static float getSumAnglesOfPolygonsAroundPoint(List<int> indexesPolygons, Point3D point)
        {

            int POINTS = 3;
            float sum = 0.0f;

            foreach (int i in indexesPolygons)
            {
                List<Point3D> pointsOfPolygon = new List<Point3D>();

                for (int j = 0; i < POINTS; j++)
                {
                    if (!isSamePoints(point, meshVertices[i * 3 + j]))
                    {
                        pointsOfPolygon.Add(meshVertices[i * 3 + j]);
                    }

                }
            }
            return sum;
        }

        /// <summary>
        //Возвращает список индексов полигонов вокруз точка
        /// </summary>
        /// <param name="point">Точка вокруг которой исщутся полигоны</param>
        /// <returns></returns>
        private static List<int> getPolygonsAroundPoint(Point3D point)
        {
            List<int> indexesOfTriangles = new List<int>();

            for (int pointIndex = 0; pointIndex < meshTriangles.Length; pointIndex++)
            {
                Point3D currentPoint = meshVertices[meshTriangles[pointIndex]];

                if (isSamePoints(point, currentPoint))
                {
                    indexesOfTriangles.Add(pointIndex / 3);
                }

            }
            return indexesOfTriangles;
        }



        private static bool isSamePoints(Point3D point1, Point3D point2)
        {
            return point1.X == point2.X && point1.Y == point2.Y && point1.Z == point2.Z;
        }





    }
}

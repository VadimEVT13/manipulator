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

            MeshGeometry3D meshGeometry = ((MeshGeometry3D)((GeometryModel3D)model).Geometry);
            meshTriangles = meshGeometry.TriangleIndices.ToArray();
            meshVertices = meshGeometry.Positions.ToArray();

            int[][] neighbors = GetNeighbors(1);

            Task<double[]> calculateAreaTask = new Task<double[]>(() => calculateAreas((
                (int[][])neighbors.Clone()),
                (Point3D[])meshVertices.Clone(),
                (int[])meshTriangles.Clone()));

            Task<double[]> calculateAnglesTask = new Task<double[]>(() => calculateAngles(neighbors, meshVertices, meshTriangles));

            calculateAreaTask.Start();
            calculateAnglesTask.Start();

            double[] areas = calculateAreaTask.Result;
            double[] angles = calculateAnglesTask.Result;

            calculateAreaTask.Wait();
            calculateAnglesTask.Wait();

            double[] curvatures = calculatePointsCurvature(angles, areas);

            Point3D[] curvedPoints = fillterPointsByCurvature(curvatures);



            return curvedPoints;
        }


        /// <summary>
        /// Вычисляет площадь полигонов вокруг всех точек
        /// </summary>
        /// <param name="neighbors"></param>
        /// <param name="points"></param>
        /// <param name="triangles"></param>
        /// <returns></returns>
        private static double[] calculateAreas(int[][] neighbors, Point3D[] points, int[] triangles)
        {
            List<double> areasList = new List<double>();

            for (int i = 0; i < neighbors.Length; i++)
            {
                Point3D currentPoint = points[i];
                double sumArreas = 0;
                for (int j = 0; j < neighbors[i].Length; j++)
                {
                    int indexPoint1 = neighbors[i][j];
                    int indexPoint2 = j == neighbors[i].Length - 1 ? indexPoint2 = 0 : indexPoint2 = j + 1;
                    sumArreas += getArea(currentPoint, points[indexPoint1], points[indexPoint2]);
                }
                areasList.Add(sumArreas);
            }

            return areasList.ToArray();
        }

        /// <summary>
        /// Вычисляет площадь полигона состоящего из точек
        /// </summary>
        /// <param name="currentPoint"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        private static double getArea(Point3D currentPoint, Point3D point1, Point3D point2)
        {
            double AB = Distance(currentPoint, point1);
            double AC = Distance(currentPoint, point2);
            double BC = Distance(point1, point2);

            double semiperimeter = (AB + AC + BC) / 2;

            double area = Math.Sqrt((semiperimeter - AB) * (semiperimeter - AC) * (semiperimeter - BC) * semiperimeter);

            return area;
        }
        /// <summary>
        /// Вычисляет сумму углов вокруг всех точек
        /// </summary>
        /// <param name="neighbors"></param>
        /// <param name="points"></param>
        /// <param name="triangles"></param>
        /// <returns></returns>
        private static double[] calculateAngles(int[][] neighbors, Point3D[] points, int[] triangles)
        {
            List<double> angles = new List<double>();

            for (int i = 0; i < neighbors.Length; i++)
            {
                Point3D currentPoint = points[i];
                double sumAngles = 0;
                for (int j = 0; j < neighbors[i].Length; j++)
                {
                    int indexPoint1 = neighbors[i][j];
                    int indexPoint2 = neighbors[i][j == neighbors[i].Length - 1 ? indexPoint2 = 0 : indexPoint2 = j + 1];
                    double calcAngle = getAngle(currentPoint, points[indexPoint1], points[indexPoint2]);
                    sumAngles+=calcAngle;
                }
                angles.Add(sumAngles);
            }

            return angles.ToArray();
        }

        /// <summary>
        /// Вычисляет угол между векторами из точки 
        /// </summary>
        /// <param name="currentPoint"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        private static double getAngle(Point3D currentPoint, Point3D point1, Point3D point2)
        {
            Vector3D vector = new Vector3D(point1.X - currentPoint.X, point1.Y - currentPoint.Y, point1.Z - currentPoint.Z);
            Vector3D vector2 = new Vector3D(point2.X - currentPoint.X, point2.Y - currentPoint.Y, point2.Z - currentPoint.Z);
            return Vector3D.AngleBetween(vector, vector2);
        }

        private static double[] calculatePointsCurvature(double[] angles, double[] areas)
        {
            List<double> pointsCurvature = new List<double>();

            for (int i = 0; i < meshVertices.Length; i++)
            {
                double curvature = (3 * (2 * Math.PI - angles[i])) / areas[i];
                pointsCurvature.Add(curvature);
            }

            return pointsCurvature.ToArray();
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
                neighbors[i] = getNeighborhoodLevelPoints(i, N);
            }

            return neighbors;
        }

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
                if (meshVertices[point].Equals( meshVertices[meshTriangles[pointIndex]]))
                {
                    int triangleIndex = pointIndex / 3;
                    for (int pointInTriangle =3*triangleIndex; pointInTriangle < 3*triangleIndex + 3; pointInTriangle++)
                    {
                       int currentPoint = meshTriangles[pointInTriangle];
                        if (!meshVertices[currentPoint].Equals(meshVertices[point]))
                        {
                            points.Add(currentPoint);
                        }
                    }
                }
            }
            return points;
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

        private static Point3D[] fillterPointsByCurvature(double[] curvatures)
        {
            List<Point3D> curvedPoints = new List<Point3D>();
            List<int> curveturesNegative = new List<int>();
            List<int> curveturesPositive = new List<int>();


            double avarageNegCurv = 0;
            double avaragePositCurv = 0;
            for (int i = 0; i < curvatures.Length; i++)
            {
                if (curvatures[i] >= 0)
                {
                    curveturesPositive.Add(i);
                    avaragePositCurv += curvatures[i];
                }
                else
                {
                    curveturesNegative.Add(i);
                    avarageNegCurv += curvatures[i];
                }
            }

            avarageNegCurv = avarageNegCurv / curveturesNegative.Count;
            avaragePositCurv = avaragePositCurv / curveturesPositive.Count;


            for (int i = 0; i < curvatures.Length; i++)
            {
                if (curvatures[i] > 0 && curvatures[i] > avaragePositCurv)
                    curvedPoints.Add(meshVertices[i]);
                if (curvatures[i] < 0 && curvatures[i] < avarageNegCurv)
                    curvedPoints.Add(meshVertices[i]);
            }

            return curvedPoints.ToArray();


        }
    }
}

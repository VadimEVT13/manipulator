using InverseTest.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.JunctionDetect
{
    public class JunctionDetectAlgorithm
    {

        private static Point3D[] meshVertices;
        private static int[] meshTriangles;


        public int[] Detect(Model3D model)
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

            int[] curvedPoints = fillterPointsByCurvature(curvatures);

            return curvedPoints;
        }


        /// <summary>
        /// Вычисляет площадь полигонов вокруг всех точек
        /// </summary>
        /// <param name="neighbors"></param>
        /// <param name="points"></param>
        /// <param name="triangles"></param>
        /// <returns></returns>
        private double[] calculateAreas(int[][] neighbors, Point3D[] points, int[] triangles)
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
        public double getArea(Point3D currentPoint, Point3D point1, Point3D point2)
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
        private double[] calculateAngles(int[][] neighbors, Point3D[] points, int[] triangles)
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
                    sumAngles += calcAngle;
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
        public double getAngle(Point3D currentPoint, Point3D point1, Point3D point2)
        {
            Vector3D vector = new Vector3D(point1.X - currentPoint.X, point1.Y - currentPoint.Y, point1.Z - currentPoint.Z);
            Vector3D vector2 = new Vector3D(point2.X - currentPoint.X, point2.Y - currentPoint.Y, point2.Z - currentPoint.Z);
            return Vector3D.AngleBetween(vector, vector2);
        }

        private double[] calculatePointsCurvature(double[] angles, double[] areas)
        {
            List<double> pointsCurvature = new List<double>();

            for (int i = 0; i < meshVertices.Length; i++)
            {
                double curvature = (3 * (2 * Math.PI - MathUtils.AngleToRadians(angles[i]))) / areas[i];
                pointsCurvature.Add(curvature);
            }

            return pointsCurvature.ToArray();
        }


        /// <summary>
        ///  Возвращает список соседей у каждой точки
        /// </summary>
        /// <param name="N"></param>
        private int[][] GetNeighbors(int N)
        {
            int[][] neighbors = new int[meshVertices.Length][];
            for (int i = 0; i < meshVertices.Length; i++)
            {
                neighbors[i] = getNeighborhoodLevelPoints(i, N);
            }

            return neighbors;
        }




        //Возвращает точки вокруг заданной точки на уровне NEIGHBORHOOD_LAVEL
        private int[] getNeighborhoodLevelPoints(int pointIndex, int N)
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
        private List<int> getPointsAroundPoint(int pointIndex)
        {

            LinkedList<int> points = new LinkedList<int>();
            List<int> othersTrianglePoints = new List<int>();
            List<List<int>> pointsInTriangle = new List<List<int>>();
            for (int index = 0; index < meshTriangles.Length; index++)
            {
                if (meshVertices[pointIndex].Equals(meshVertices[meshTriangles[index]]))
                {
                    int triangleIndex = index / 3;

                    List<int> pointsList = new List<int>();
                    for (int pointInTriangle = 3 * triangleIndex; pointInTriangle < 3 * triangleIndex + 3; pointInTriangle++)
                    {
                        int currentPoint = meshTriangles[pointInTriangle];
                        if (!meshVertices[currentPoint].Equals(meshVertices[pointIndex]))
                        {
                            pointsList.Add(currentPoint);
                        }
                    }
                    pointsInTriangle.Add(pointsList);
                }

            }

            int[] neighborsPoints = buildNeighborhoodLinks(pointsInTriangle);


            return new List<int>(neighborsPoints);
        }


        private int[] buildNeighborhoodLinks(List<List<int>> trianglePoints)
        {

            LinkedList<int> points = new LinkedList<int>();

            foreach (List<int> listPoints in trianglePoints)
            {
                if (points.Count == 0)
                {
                    points.AddLast(listPoints[0]);
                    points.AddLast(listPoints[1]);
                    continue;
                }

                if (meshVertices[listPoints[0]].Equals(meshVertices[points.First.Value]))
                {
                    points.AddFirst(listPoints[1]);
                    continue;
                }
                else if (meshVertices[listPoints[0]].Equals(meshVertices[points.Last.Value]))
                {
                    points.AddLast(listPoints[1]);
                    continue;
                }
                else if (meshVertices[listPoints[1]].Equals(meshVertices[points.First.Value]))
                {
                    points.AddFirst(listPoints[0]);
                    continue;
                }
                else if (meshVertices[listPoints[1]].Equals(meshVertices[points.Last.Value]))
                {
                    points.AddLast(listPoints[0]);
                    continue;
                }
            }


            return points.ToArray();


        }

        /// <summary>
        /// Вычисляет расстаяние между двумя точками
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        private double Distance(Point3D point1, Point3D point2)
        {
            return Math.Sqrt(Math.Pow((point2.X - point1.X), 2) +
                Math.Pow((point2.Y - point1.Y), 2) +
                Math.Pow((point2.Z - point1.Z), 2)
                );
        }

        private int[] fillterPointsByCurvature(double[] curvatures)
        {
            List<int> curvedPointsIndexes = new List<int>();
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
                if (curvatures[i] > 0 && curvatures[i] > avaragePositCurv*0.5)
                    curvedPointsIndexes.Add(i);
                if (curvatures[i] < 0 && curvatures[i] < avarageNegCurv*0.5)
                    curvedPointsIndexes.Add(i);
            }

            return curvedPointsIndexes.ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using InverseTest.Manipulator;

namespace InverseTest.JunctionDetect
{
    public class JunctionDetectAlgorithm
    {
        private Point3D[] vertices;
        private int[] triangles;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int[] Detect(Model3D model)
        {
            MeshGeometry3D meshGeometry = ((MeshGeometry3D)((GeometryModel3D)model).Geometry);
            triangles = meshGeometry.TriangleIndices.ToArray();
            vertices = meshGeometry.Positions.ToArray();

            int[][] neighbors = GetNeighbors(1);

            Task<double[]> calculateAreaTask = new Task<double[]>(() => CalculateAreas((int[][]) neighbors.Clone(), (Point3D[]) vertices.Clone()));
            Task<double[]> calculateAnglesTask = new Task<double[]>(() => CalculateAngles(neighbors, vertices));

            calculateAreaTask.Start();
            calculateAnglesTask.Start();

            double[] areas = calculateAreaTask.Result;
            double[] angles = calculateAnglesTask.Result;

            calculateAreaTask.Wait();
            calculateAnglesTask.Wait();

            double[] curvatures = CalculatePointsCurvature(angles, areas);
            return FillterPointsByCurvature(curvatures);
        }

        /// <summary>
        /// Вычисляет площадь полигонов вокруг всех точек
        /// </summary>
        /// <param name="neighbors"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        private double[] CalculateAreas(int[][] neighbors, Point3D[] points)
        {
            List<double> areasList = new List<double>();
            for (int i = 0; i < neighbors.Length; i++)
            {
                double areas = 0;
                int last = neighbors[i].Length - 1;
                for (int j = 0; j < last; j++)
                {
                    areas += GetArea(points[i], points[neighbors[i][j]], points[neighbors[i][j + 1]]);
                }
                areas += GetArea(points[i], points[neighbors[i][last]], points[neighbors[i][0]]);
                areasList.Add(areas);
            }
            return areasList.ToArray();
        }

        /// <summary>
        /// Вычисляет площадь полигона состоящего из точек
        /// </summary>
        /// <param name="currentPoint"></param>
        /// <param name="pointA"></param>
        /// <param name="pointB"></param>
        /// <returns></returns>
        public double GetArea(Point3D currentPoint, Point3D pointA, Point3D pointB)
        {
            double AB = Distance(currentPoint, pointA);
            double AC = Distance(currentPoint, pointB);
            double BC = Distance(pointA, pointB);
            double semiperimeter = (AB + AC + BC) / 2;
            return Math.Sqrt((semiperimeter - AB) * (semiperimeter - AC) * (semiperimeter - BC) * semiperimeter);
        }

        /// <summary>
        /// Вычисляет сумму углов вокруг всех точек
        /// </summary>
        /// <param name="neighbors"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        private double[] CalculateAngles(int[][] neighbors, Point3D[] points)
        {
            List<double> angles = new List<double>();
            for (int i = 0; i < neighbors.Length; i++)
            {
                double sumAngles = 0;
                int last = neighbors[i].Length - 1;
                for (int j = 0; j < last; j++)
                {
                    sumAngles += GetAngle(points[i], points[neighbors[i][j]], points[neighbors[i][j + 1]]);
                }
                sumAngles += GetAngle(points[i], points[neighbors[i][last]], points[neighbors[i][0]]);
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
        public double GetAngle(Point3D currentPoint, Point3D point1, Point3D point2)
        {
            Vector3D vector = new Vector3D(point1.X - currentPoint.X, point1.Y - currentPoint.Y, point1.Z - currentPoint.Z);
            Vector3D vector2 = new Vector3D(point2.X - currentPoint.X, point2.Y - currentPoint.Y, point2.Z - currentPoint.Z);
            return Vector3D.AngleBetween(vector, vector2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angles"></param>
        /// <param name="areas"></param>
        /// <returns></returns>
        private double[] CalculatePointsCurvature(double[] angles, double[] areas)
        {
            List<double> pointsCurvature = new List<double>();
            for (int i = 0; i < vertices.Length; i++)
            {
                double curvature = (3 * (2 * Math.PI - MathUtils.AngleToRadians(angles[i]))) / areas[i];
                pointsCurvature.Add(curvature);
            }
            return pointsCurvature.ToArray();
        }

        /// <summary>
        ///  Возвращает список соседей у каждой точки
        /// </summary>
        /// <param name="level"></param>
        private int[][] GetNeighbors(int level)
        {
            int[][] neighbors = new int[vertices.Length][];
            for (int i = 0; i < vertices.Length; i++)
            {
                neighbors[i] = GetNeighborhoodLevelPoints(i, level);
            }
            return neighbors;
        }

        /// <summary>
        /// Возвращает точки вокруг заданной точки на уровне NEIGHBORHOOD_LAVEL
        /// </summary>
        /// <param name="pointIndex"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private int[] GetNeighborhoodLevelPoints(int pointIndex, int level)
        {
            HashSet<int> previousLevelPoints = new HashSet<int>();
            HashSet<int> currentLevelPoints = new HashSet<int>
            {
                pointIndex
            };
            List<int> neighborhoodLevelPoints = new List<int>();
            for (int i = 0; i < level; i++)
            {
                neighborhoodLevelPoints.Clear();
                foreach (int currentPoint in currentLevelPoints)
                {
                    List<int> pointAround = GetPointsAroundPoint(currentPoint);
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
            for (int i = 0; i < neighborhoodLevelPoints.Count; i += level)
            {
                resPoints.Add(neighborhoodLevelPoints[i]);
            }
            return resPoints.ToArray();
        }

        /// <summary>
        /// Возвращает точки вокрут заданной точки
        /// </summary>
        /// <param name="pointIndex"></param>
        /// <returns></returns>
        private List<int> GetPointsAroundPoint(int pointIndex)
        {
            List<List<int>> pointsInTriangle = new List<List<int>>();
            for (int index = 0; index < triangles.Length; index++)
            {
                if (vertices[pointIndex].Equals(vertices[triangles[index]]))
                {
                    int triangleIndex = index / 3;
                    List<int> pointsList = new List<int>();
                    for (int pointInTriangle = 3 * triangleIndex; pointInTriangle < 3 * triangleIndex + 3; pointInTriangle++)
                    {
                        int currentPoint = triangles[pointInTriangle];
                        if (!vertices[currentPoint].Equals(vertices[pointIndex]))
                        {
                            pointsList.Add(currentPoint);
                        }
                    }
                    pointsInTriangle.Add(pointsList);
                }
            }
            int[] neighborsPoints = BuildNeighborhoodLinks(pointsInTriangle);
            return new List<int>(neighborsPoints);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trianglePoints"></param>
        /// <returns></returns>
        private int[] BuildNeighborhoodLinks(List<List<int>> trianglePoints)
        {
            LinkedList<int> points = new LinkedList<int>();
            foreach (List<int> listPoints in trianglePoints)
            {
                if (points.Count == 0)
                {
                    points.AddLast(listPoints[0]);
                    points.AddLast(listPoints[1]);
                }
                else if (vertices[listPoints[0]].Equals(vertices[points.First.Value]))
                {
                    points.AddFirst(listPoints[1]);
                }
                else if (vertices[listPoints[0]].Equals(vertices[points.Last.Value]))
                {
                    points.AddLast(listPoints[1]);
                }
                else if (vertices[listPoints[1]].Equals(vertices[points.First.Value]))
                {
                    points.AddFirst(listPoints[0]);
                }
                else if (vertices[listPoints[1]].Equals(vertices[points.Last.Value]))
                {
                    points.AddLast(listPoints[0]);
                }
            }
            return points.ToArray();
        }

        /// <summary>
        /// Вычисляет расстояние между двумя точками A и B.
        /// </summary>
        /// <param name="pointA">точка A</param>
        /// <param name="pointB">точка B</param>
        /// <returns></returns>
        private double Distance(Point3D pointA, Point3D pointB)
        {
            double dx = pointB.X - pointA.X;
            double dy = pointB.Y - pointA.Y;
            double dz = pointB.Z - pointA.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curvatures"></param>
        /// <returns></returns>
        private int[] FillterPointsByCurvature(double[] curvatures)
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
                if (curvatures[i] > avaragePositCurv)
                {
                    curvedPointsIndexes.Add(i);
                }
                if (curvatures[i] < avarageNegCurv)
                {
                    curvedPointsIndexes.Add(i);
                }
            }
            return curvedPointsIndexes.ToArray();
        }
    }
}

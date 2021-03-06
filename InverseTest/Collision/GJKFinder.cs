﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using InverseTest.Collision;
using MIConvexHull;
using System.Windows;
using System.Windows.Media;
using HelixToolkit.Wpf;
using InverseTest.GUI;

namespace InverseTest.Collision
{
    public class GJKFinder
    {

        List<Point3D> a, b; // два объекта, которые проверяются на пересечение 
        Vector3D[] resultSimplex; // конечный сиплекс
        
        SupportClass sc; // класс для работы со вспомогательными функциями

        public GJKFinder(List<Point3D> first, List<Point3D> second)
        {
            //this.simplex = simplex;
            resultSimplex = new Vector3D[4];
            sc = new SupportClass();
            a = first;
            b = second;
            Vector3D one = new Vector3D(1, 1, 1);
            Vector3D right = new Vector3D(1, 0, 0);
            Vector3D up = new Vector3D(0, 1, 0);
            Vector3D forward = new Vector3D(0, 0, 1);

            resultSimplex[0] = sc.supportFunction(a, one) - sc.supportFunction(b, -one);
            resultSimplex[1] = sc.supportFunction(a, -right) - sc.supportFunction(b, right);
            resultSimplex[2] = sc.supportFunction(a, -up) - sc.supportFunction(b, up);
            resultSimplex[3] = sc.supportFunction(a, -forward) -sc.supportFunction(b, forward);
        }

        //	проверка факта пересечения
        public bool testGJKIntersection()
        {
            List<Vector3D> deletedVertexes = new List<Vector3D>();
            Vector3D zero = new Vector3D(0, 0, 0);
            Plane symplex = new Plane();
            //Plane planeCSO = new Plane();//плоскость, которая остается после удаления дальней вершины симплекса

            while (sc.allDifferent(resultSimplex))
            {
                symplex = new Plane(resultSimplex[1], resultSimplex[2], resultSimplex[3]);
                if (symplex.isPlane(resultSimplex[0])) return false;

                //смотрим, где находится нуль, либо конец алгоритма, либо дальше идем
                if (
                (det3x3(zero, resultSimplex[0], resultSimplex[1], resultSimplex[2]) * det3x3(resultSimplex[3], resultSimplex[0], resultSimplex[1], resultSimplex[2]) >= 0) &&
                (det3x3(zero, resultSimplex[0], resultSimplex[1], resultSimplex[3]) * det3x3(resultSimplex[2], resultSimplex[0], resultSimplex[1], resultSimplex[3]) >= 0) &&
                (det3x3(zero, resultSimplex[0], resultSimplex[2], resultSimplex[3]) * det3x3(resultSimplex[1], resultSimplex[0], resultSimplex[2], resultSimplex[3]) >= 0) &&
                (det3x3(zero, resultSimplex[1], resultSimplex[2], resultSimplex[3]) * det3x3(resultSimplex[0], resultSimplex[1], resultSimplex[2], resultSimplex[3]) >= 0))

                {
                    return true;
                }

                int indexMaxDistance = 0; // номер дальней точки
                                          //поиск ближайшей точки

                Vector3D p0 = sc.closestPointToTriangle(new Plane(resultSimplex[1], resultSimplex[2], resultSimplex[3]));
                Vector3D p1 = sc.closestPointToTriangle(new Plane(resultSimplex[0], resultSimplex[2], resultSimplex[3]));
                Vector3D p2 = sc.closestPointToTriangle(new Plane(resultSimplex[0], resultSimplex[1], resultSimplex[3]));
                Vector3D p3 = sc.closestPointToTriangle(new Plane(resultSimplex[0], resultSimplex[1], resultSimplex[2]));
                Vector3D pRes;
                pRes = p0;

                if (p1.Length < pRes.Length)
                {
                    pRes = p1;
                    indexMaxDistance = 1;
                }

                if (p2.Length < pRes.Length)
                {
                    pRes = p2;
                    indexMaxDistance = 2;
                }

                if (p3.Length < pRes.Length)
                {
                    pRes = p3;
                    indexMaxDistance = 3;
                }

                deletedVertexes.Add(resultSimplex[indexMaxDistance]);

                //в направлении нормали к нулю ищем экстремальную точку 
                resultSimplex[indexMaxDistance] = sc.supportFunction(a, -pRes) - sc.supportFunction(b, pRes);
                if (deletedVertexes.IndexOf(resultSimplex[indexMaxDistance]) != -1)
                {
                    resultSimplex = null;
                    return false;
                }

            }
            //	если добавляется уже присутствующая точка в симплексе, то работа алгоритма завершается
            resultSimplex = null;
            return false;
        }

        double det3x3(Vector3D p0, Vector3D p1, Vector3D p2, Vector3D p3)
        {
            double temp = 0;
            temp=  (p0.X - p1.X) * (p2.Y - p1.Y) * (p3.Z - p1.Z) + (p2.X - p1.X) * (p3.Y - p1.Y) * (p0.Z - p1.Z) + (p3.X - p1.X) * (p0.Y - p1.Y) * (p2.Z - p1.Z)
            - (p3.X - p1.X) * (p2.Y - p1.Y) * (p0.Z - p1.Z) - (p0.X - p1.X) * (p3.Y - p1.Y) * (p2.Z - p1.Z) - (p2.X - p1.X) * (p0.Y - p1.Y) * (p3.Z - p1.Z);
            return temp;
        }
    }
}



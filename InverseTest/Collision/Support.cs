﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Collections;
using MIConvexHull;

namespace InverseTest.Collision
{
    public class SupportClass
    {

        //функция, возвращающая экстремальную точку в направлении вектора p 
        public Point3D supportFunction(List<Point3D> a, Vector3D p)
        {

            Point3D currPoint = a[0];
            Point3D maxPoint = a[0];

            double currScalar = currPoint.X * p.X + currPoint.Y * p.Y + currPoint.Z * p.Z;
            double maxScalar = currScalar;

            for (int i = 0; i < a.Count; i++)
            {

                currPoint = a[i];
                currScalar = currPoint.X * p.X + currPoint.Y * p.Y + currPoint.Z * p.Z;

                if (currScalar > maxScalar)
                {
                    maxPoint = currPoint;
                    maxScalar = currScalar;
                }
            }
            return maxPoint;
        }

        //функция возвращает true, если в списке все точки разные 
        public bool allDifferent(Vector3D[] points)
        {
            for (int i = 0; i < points.Length - 1; i++)
            {
                for (int j = i + 1; j < points.Length; j++)
                {
                    if (Equals(points[i],points[j]))
                        return false;
                }
            }
            return true;

        }

        //Поиск ближайшей точки отрезка
        public Vector3D closestPointToEdge(Vector3D ePoint1, Vector3D ePoint2)
        {
            double alfa;

            double scalarP1P1 = Vector3D.DotProduct(ePoint1, ePoint1);
            double scalarP2P2 = Vector3D.DotProduct(ePoint2, ePoint2);
            double scalarP1P2 = Vector3D.DotProduct(ePoint1, ePoint2);

            //x(p1-p2)=0 - перпендикулярность
            //x=alfa*p1+(1-alfa)*p2
            if (scalarP1P1 == scalarP2P2) alfa = 0;
            else
                alfa = (scalarP2P2 - scalarP1P2) / (scalarP1P1 - 2 * scalarP1P2 + scalarP2P2);

            if (alfa <= 0)
            {
                return ePoint2;
            }
            else if (alfa >= 1)
            {
                return ePoint1;
            }
            else
            {
                return alfa * ePoint1 + (1 - alfa) * ePoint2;
            }
        }

        //Поиск ближайшей точки треугольника(грани)
        public Vector3D closestPointToTriangle(Plane triangle)
        {
            Vector3D point1, point2;

            point1 = closestPointToEdge(triangle.points[0], triangle.points[1]);
            point2 = closestPointToEdge(triangle.points[1], triangle.points[2]);
            return closestPointToEdge(point1, point2);

        }
    }

}

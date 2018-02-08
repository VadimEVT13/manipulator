using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Collision
{
    public class Plane
    {

        public Plane()
        {
            points = new Vector3D[3];
        }
        public Plane(Vector3D p1, Vector3D p2, Vector3D p3)
        {

            points = new Vector3D[3];
            points[0] = p1;
            points[1] = p2;
            points[2] = p3;
        }
        //Три точки плоскости

        public Vector3D[] points;
        /*
        //коэффициенты плоскости
        public double getA()
        {

            return points[0].Y * (points[1].Z - points[2].Z) + points[1].Y * (points[2].Z - points[0].Z) + points[2].Y * 
                (points[0].Z - points[1].Z);

        }
        public double getB()
        {

            return points[0].Z * (points[1].X - points[2].X) + points[1].Z * (points[2].X - points[0].X) + points[2].Z * 
                (points[0].X - points[1].X);
        }
        public double getC()
        {

            return points[0].X * (points[1].Y - points[2].Y) + points[1].X * (points[2].Y - points[0].Y) + points[2].X * 
                (points[0].Y - points[1].Y);

        }
        public double getD()
        {

            return -(points[0].X * (points[1].Y * points[2].Z - points[1].Z * points[2].Y) + points[1].X * (points[2].Y * 
                points[0].Z - points[2].Z * points[0].Y) + points[2].X * (points[0].Y * points[1].Z - points[0].Z * points[1].Y));
        }

        public Vector3D normal()
        {

            return new Vector3D(getA(), getB(), getC());
        }*/
    }

}

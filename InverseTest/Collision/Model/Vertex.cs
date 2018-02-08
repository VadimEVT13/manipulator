using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using MIConvexHull;
using InverseTest.Collision;

namespace InverseTest.Collision
{
    public class Vertex : IVertex
    {
        public double[] Position { get; set; }

        public Vertex(Point3D point)
        {
            Position = new double[3] { point.X, point.Y, point.Z };
        }
    }
}

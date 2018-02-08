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
    class PointComparer : IEqualityComparer<Point3D>
    {
        #region Implementation of IEqualityComparer<in Point3D>

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <param name="x">The first object of type <paramref name="T"/> to compare.</param><param name="y">The second object of type <paramref name="T"/> to compare.</param>
        bool IEqualityComparer<Point3D>.Equals(Point3D x, Point3D y)
        {
            return ((x - y).Length < 0.000000001);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        /// A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
        int IEqualityComparer<Point3D>.GetHashCode(Point3D obj)
        {
            var d = obj.ToVector3D().LengthSquared;
            return d.GetHashCode();
        }

        #endregion
    }
}

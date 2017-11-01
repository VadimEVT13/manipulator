using Microsoft.VisualStudio.TestTools.UnitTesting;
using InverseTest.JunctionDetect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.JunctionDetect.Tests
{
    [TestClass()]
    public class JunctionDetectTest
    {
        [TestMethod()]
        public void getAreaTest()
        {

            JunctionDetectAlgorithm junctionDetect = new JunctionDetectAlgorithm();

            double area = junctionDetect.getArea(new Point3D(1, 4, 4), new Point3D(2, 2, 2), new Point3D(3, 0, 3));
            Console.WriteLine(area);


        }

        [TestMethod()]
        public void getAngleTest()
        {

            JunctionDetectAlgorithm junctionDetect = new JunctionDetectAlgorithm();

            double area = junctionDetect.getAngle(new Point3D(1, 4, 4), new Point3D(2, 2, 2), new Point3D(3, 0, 3));
            Console.WriteLine(area);

        }
    }
}
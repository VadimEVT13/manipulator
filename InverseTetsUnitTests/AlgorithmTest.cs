using System;
using System.Windows.Media.Media3D;
using InverseTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InverseTest.InverseAlgorithm;
using InverseTest.Manipulator; 

namespace InverseTetsUnitTests
{
    [TestClass]
    public class AlgorithmTest
    {
        private double delta = 0.0000000000001;

        


        [TestMethod]
        public void TestAlgorithm()
        {
            Joint joint0 = new Joint(new Point3D(0, 0, 0), new Point3D(0, 10, 0));
            Joint joint1 = new Joint(new Point3D(0, 10, 0), new Point3D(10, 10, 0));
            Joint joint2 = new Joint(new Point3D(10, 10, 0), new Point3D(14, 10, 0));
            ManipMathModel model = new ManipMathModel(new[] { joint0, joint1, joint2 });

            Point3D point = new Point3D(16, 8, 2);

            //Algorithm.DoStuff(model, point);

            Assert.AreEqual(new Point3D(0,0,0),model.Joints[0].Points[0]);

            Assert.AreEqual(16, model.Joints[2].Points[1].X, delta, "Не совпали координаты X конечной точки ребра 2");
            Assert.AreEqual(8, model.Joints[2].Points[1].Y, delta, "Не совпали координаты Y конечной точки ребра 2");
            Assert.AreEqual(2, model.Joints[2].Points[1].Z, delta, "Не совпали координаты Z конечной точки ребра 2");
            Console.WriteLine(model.Joints[2].Points[1]);
            
            //Console.WriteLine(model.Joints[0].GetTransform());
        }

        

    }
}

using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using InverseTest;
using InverseTest.InverseAlgorithm;
using InverseTest.Manipulator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InverseTetsUnitTests
{
    [TestClass]
    public class BrandNewTest
    {
        [TestMethod]
        public void TestJointLength()
        {
            Joint joint = new Joint(new Point3D(0,0,0), new Point3D(3,4,0));
            Assert.AreEqual(5, joint.GetCurrentLength(), 0.0000001);
        }

        [TestMethod]
        public void TestFabrik()
        {
            NewJoint joint0 = new NewJoint(new Point3D(0, 0, 0), new Point3D(0, 10, 0), new Vector3D(-1, 0, 0), new double[] { 90, 90 });
            NewJoint joint1 = new NewJoint(new Point3D(0, 10, 0), new Point3D(10, 10, 0), new Vector3D(0,1,0), new double[] { 90, 0 });
            NewJoint joint2 = new NewJoint(new Point3D(10, 10, 0), new Point3D(14, 10, 0), new Vector3D(0,1,0), new double[] { 90, 90 });

            JointsChain chain = new JointsChain(new []{joint0, joint1, joint2});

            Point3D targetPoint = new Point3D(16, 8, 2);

            JointsChain resultedChain = Algorithm.Solve(chain, targetPoint);

            Assert.AreEqual(new Point3D(0,0,0), resultedChain.Joints[0].StartPoint);

            Assert.AreEqual(16, resultedChain.Joints[2].EndPoint.X, Algorithm.Delta);
            Assert.AreEqual(8, resultedChain.Joints[2].EndPoint.Y, Algorithm.Delta);
            Assert.AreEqual(2, resultedChain.Joints[2].EndPoint.Z, Algorithm.Delta);
        }

        [TestMethod]
        public void TestAlgorithmOnManip()
        {
            ManipulatorV2 manipulator = new ManipulatorV2("Manip.obj");

            Point3D endPoint = new Point3D(300, 300, 0);

            Point3D startPoint = manipulator.ManipMathModel.Joints[0].StartPoint;

            Console.WriteLine(manipulator.ManipMathModel.Joints[0].StartPoint);
            Console.WriteLine(manipulator.ManipMathModel.Joints[2].EndPoint);

            Algorithm.Solve(manipulator.ManipMathModel, endPoint);

            Console.WriteLine(manipulator.ManipMathModel.Joints[0].StartPoint);
            Console.WriteLine(manipulator.ManipMathModel.Joints[2].EndPoint);

            Assert.AreEqual(startPoint.X, manipulator.ManipMathModel.Joints[0].StartPoint.X, Algorithm.Delta);
            Assert.AreEqual(startPoint.Y, manipulator.ManipMathModel.Joints[0].StartPoint.Y, Algorithm.Delta);
            Assert.AreEqual(startPoint.Z, manipulator.ManipMathModel.Joints[0].StartPoint.Z, Algorithm.Delta);

            Assert.AreEqual(endPoint.X, manipulator.ManipMathModel.Joints[2].EndPoint.X, Algorithm.Delta);
            Assert.AreEqual(endPoint.Y, manipulator.ManipMathModel.Joints[2].EndPoint.Y, Algorithm.Delta);
            Assert.AreEqual(endPoint.Z, manipulator.ManipMathModel.Joints[2].EndPoint.Z, Algorithm.Delta);


        }

        [TestMethod]
        public void TestAlgorithmOnModel()
        {
            Point3D point0 = new Point3D(0,0,0);
            Point3D point1 = new Point3D(0, 250, 0);
            Point3D point2 = new Point3D(250, 250, 0);
            Point3D point3 = new Point3D(300, 250, 0);

            Vector3D turnPlaneNormal = new Vector3D(0, 0, 1);

            NewJoint joint0 = new NewJoint(point0, point1, turnPlaneNormal, new double[] { 90, 90 });
            NewJoint joint1 = new NewJoint(point1, point2, turnPlaneNormal, new double[] { 90, 0 });
            NewJoint joint2 = new NewJoint(point2, point3, turnPlaneNormal, new double[] { 90, 90 });

            JointsChain model = new JointsChain(new[] { joint0, joint1, joint2 });

            Point3D endPoint = new Point3D(300, 100, 0);
            Point3D startPoint = model.Joints[0].StartPoint;

            Console.WriteLine(model.Joints[0].StartPoint);
            Console.WriteLine(model.Joints[2].EndPoint);

            Algorithm.Solve(model, endPoint);

            Console.WriteLine(model.Joints[0].StartPoint);
            Console.WriteLine(model.Joints[2].EndPoint);

            Assert.AreEqual(startPoint.X, model.Joints[0].StartPoint.X, Algorithm.Delta);
            Assert.AreEqual(startPoint.Y, model.Joints[0].StartPoint.Y, Algorithm.Delta);
            Assert.AreEqual(startPoint.Z, model.Joints[0].StartPoint.Z, Algorithm.Delta);

            Assert.AreEqual(endPoint.X, model.Joints[2].EndPoint.X, Algorithm.Delta);
            Assert.AreEqual(endPoint.Y, model.Joints[2].EndPoint.Y, Algorithm.Delta);
            Assert.AreEqual(endPoint.Z, model.Joints[2].EndPoint.Z, Algorithm.Delta);
        }

        [TestMethod]
        public void TestAngleBetweenVectors()
        {
            Vector3D baseVector = new Vector3D(1,0,0);
            Vector3D vectorToTurnTo = new Vector3D(1,0,0);

            Vector3D axis;
            double angle;
            Algorithm.GetAngleBetweenVectors(vectorToTurnTo, baseVector, out axis, out angle);

            Assert.AreEqual(0, angle, Algorithm.Delta);

            if (Math.Abs(angle) > Algorithm.Delta)
                Assert.AreEqual(new Vector3D(0,0,1), axis);
        }



        [TestMethod]
        public void TestVerticalJoint()
        {
            List<JointAxises> jointsToTest = new List<JointAxises>();
            jointsToTest.Add(new JointAxises(new Vector3D(0, 1, 0), new Vector3D(0, 0, 1)));
            jointsToTest.Add(new JointAxises(new Vector3D(1, 0, 0), new Vector3D(0, 0, 1)));

            List<double[]> anglesToTest = new List<double[]>();
            anglesToTest.Add(new double[] { 45, 45 });
            anglesToTest.Add(new double[] { 45, -45 });
            anglesToTest.Add(new double[] { -45, 45 });
            anglesToTest.Add(new double[] { -45, -45 });

            int testIndex = 0;
            foreach (JointAxises jointAxises in jointsToTest)
            {
                
                foreach (double[] angles in anglesToTest)
                {
                    double turnAngle;
                    double rotationAngle;
                    TestJointAxis(jointAxises, angles[0], angles[1], out turnAngle, out rotationAngle);
                    Assert.AreEqual(angles[0], turnAngle, Algorithm.Delta, $"Углы поворота не совпали на итерации {testIndex}");
                    Assert.AreEqual(angles[1], rotationAngle, Algorithm.Delta, $"Углы вращения не совпали на итерации {testIndex}");
                    testIndex++;
                }
                
            }
        }

        private void TestJointAxis(JointAxises jointAxises, double turnAngle, double rotationAngle, out double resultedTurnAngle, out double resultedRotationAngle)
        {
            RotateTransform3D rotateTransform = new RotateTransform3D(new AxisAngleRotation3D(jointAxises.RotationAxis, rotationAngle));
            RotateTransform3D turnTransform = new RotateTransform3D(new AxisAngleRotation3D(jointAxises.TurnAxis, turnAngle));

            Vector3D newDirection = turnTransform.Transform(jointAxises.DefaultLookVector);
            newDirection = rotateTransform.Transform(newDirection);
            
            jointAxises.ChangeDefaultDirection(newDirection);

            rotateTransform = new RotateTransform3D(new AxisAngleRotation3D(jointAxises.RotationAxis, rotationAngle));
            turnTransform = new RotateTransform3D(new AxisAngleRotation3D(jointAxises.TurnAxis, turnAngle));

            newDirection = turnTransform.Transform(jointAxises.DefaultLookVector);
            newDirection = rotateTransform.Transform(newDirection);

            jointAxises.ChangeDirection(newDirection);
            resultedRotationAngle = jointAxises.RotationAngle;
            resultedTurnAngle = jointAxises.TurnAngle;
        }

        

        [TestMethod]
        public void TestProjection()
        {
            Point3D targetPoint = new Point3D(1,1,1);

            Vector3D normal = new Vector3D(1,1,0);

            Point3D p = MyMath.VectorProjectionOnPlane(targetPoint, normal);

            Console.WriteLine(p);

        }
    }
}

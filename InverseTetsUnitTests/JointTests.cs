using System;
using System.Security.Policy;
using System.Windows.Media.Media3D;
using InverseTest.Manipulator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InverseTetsUnitTests
{
    [TestClass]
    public class JointTests
    {
        #region Дополнительные атрибуты тестирования
        //
        // При написании тестов можно использовать следующие дополнительные атрибуты:
        //
        // ClassInitialize используется для выполнения кода до запуска первого теста в классе
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // ClassCleanup используется для выполнения кода после завершения работы всех тестов в классе
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // TestInitialize используется для выполнения кода перед запуском каждого теста 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // TestCleanup используется для выполнения кода после завершения каждого теста
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        

        [TestMethod]
        public void TestVerticalJoint()
        {
            Point3D startPoint = new Point3D(0, 0, 0);
            Point3D endPoint = new Point3D(0, 1, 0);

            Joint verticalJoint = new Joint(startPoint, endPoint)
            {
                TurnAxisVector = new Vector3D(0,1,0),
                RotateAxisVector = new Vector3D(1,0,0),

                TurnPlaneVector = new Vector3D(0, 0, 1),
                RotatePlaneVector = new Vector3D(0, 1, 0)
            };

            TestJointPointsTransform(new Joint(startPoint, endPoint), new TranslateTransform3D(1,1,1), new Point3D(1,1,1), new Point3D(1,2,1));
            verticalJoint.Reset();
            TestJointAngles(verticalJoint, new TranslateTransform3D(0.5,-0.5,0.5), -45, 45);
            verticalJoint.Reset();
            TestJointAngles(verticalJoint, new TranslateTransform3D(0.5, -0.5, -0.5), -45, -45);
            verticalJoint.Reset();
            TestJointAngles(verticalJoint, new TranslateTransform3D(-0.5, -0.5, 0.5), 45, -45);
            verticalJoint.Reset();
            TestJointAngles(verticalJoint, new TranslateTransform3D(-0.5, -0.5, -0.5), 45, 45);
            verticalJoint.Reset();

        }

        [TestMethod]
        public void TestHorizontalJoint()
        {
            Point3D startPoint = new Point3D(0, 0, 0);
            Point3D endPoint = new Point3D(1, 0, 0);

            Joint horizontalJoint = new Joint(startPoint, endPoint)
            {
                TurnAxisVector = new Vector3D(1, 0, 0),
                RotateAxisVector = new Vector3D(0, -1, 0),

                TurnPlaneVector = new Vector3D(0, 0, 1),
                RotatePlaneVector = new Vector3D(1, 0, 0)
            };

            TestJointPointsTransform(new Joint(startPoint, endPoint), new TranslateTransform3D(1, 1, 1), new Point3D(1, 1, 1), new Point3D(2, 1, 1));
            horizontalJoint.Reset();
            TestJointAngles(horizontalJoint, new TranslateTransform3D(-0.5, 0.5, 0.5), 45, -45);
            horizontalJoint.Reset();
            TestJointAngles(horizontalJoint, new TranslateTransform3D(-0.5, 0.5, -0.5), 45, 45);
            horizontalJoint.Reset();
            TestJointAngles(horizontalJoint, new TranslateTransform3D(-0.5, -0.5, 0.5), -45, 45);
            horizontalJoint.Reset();
            TestJointAngles(horizontalJoint, new TranslateTransform3D(-0.5, -0.5, -0.5), -45, -45);
            horizontalJoint.Reset();
        }

        public void TestJointPointsTransform(Joint testJoint, TranslateTransform3D translateTransform, Point3D expectedStartPoint, Point3D expectedEndPoint)
        {
            testJoint.StartPointTransform = translateTransform;
            testJoint.EndPointTransform = translateTransform;

            Assert.AreEqual(expectedStartPoint, testJoint.Points[0]);
            Assert.AreEqual(expectedEndPoint, testJoint.Points[1]);
        }

        public void TestJointAngles(Joint testJoint, TranslateTransform3D transform, double expectedTurnAngle, double expectedRotateAngle)
        {
            testJoint.EndPointTransform = transform;

            double[] angles = testJoint.GetTurnAngle();
            
            Assert.AreEqual(expectedTurnAngle, angles[0], 0.001);
            Assert.AreEqual(expectedRotateAngle, angles[1], 0.001);
        }

    }
}

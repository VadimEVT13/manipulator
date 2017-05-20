using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using InverseTest.Manipulator;

namespace InverseTest.InverseAlgorithm
{
    public static class Algorithm
    {

        static public List<Point3D[]> Points { get; set; } = new List<Point3D[]>();

        public static double Delta { get; set; } = 0.000001;

        private static Point3D GetPointOnSegment(Point3D a, Point3D b, double distanceFromA)
        {
            double distance = MyMath.DistanceBetween(a, b);
            if (distance == 0)
            {
                Console.WriteLine("KEK!");
            }
            double dif = distance - distanceFromA;
            if (dif == 0)
                return b;
            double lambda = distanceFromA / dif;
            double x = (a.X + lambda * b.X) / (1 + lambda);

            double y = (a.Y + lambda * b.Y) / (1 + lambda);

            double z = (a.Z + lambda * b.Z) / (1 + lambda);
            return new Point3D(x, y, z);
        }

        private static JointsChain FabrikBackward(JointsChain chain, Point3D targetPoint)
        {
            chain.Joints[chain.Joints.Length - 1].EndPoint = targetPoint;
            for (int i = chain.Joints.Length - 1; i > -1; i--)
            {
                Point3D newStartPoint = GetPointOnSegment(chain.Joints[i].EndPoint,
                    chain.Joints[i].StartPoint, chain.Joints[i].JointLength);

                NewJoint prevJoint = null;

                if (i != chain.Joints.Length - 1)
                {
                    //newStartPoint = chain.Joints[i].ApplyReverseConstraints(newStartPoint, chain.Joints[i + 1]);
                    prevJoint = chain.Joints[i + 1];
                }
                else
                {
                    prevJoint = chain.Joints[i - 1];
                    //chain.Joints[i].JointAxises.ChangeDirection(chain.Joints[i].EndPoint - newStartPoint);
                }

                newStartPoint = chain.Joints[i].ApplyReverseConstraints(newStartPoint, prevJoint);

                chain.Joints[i].StartPoint = newStartPoint;

                if (i - 1 >= 0)
                    chain.Joints[i - 1].EndPoint = chain.Joints[i].StartPoint;
            }

            return chain;
        }

        private static JointsChain FabrikForward(JointsChain chain, Point3D originPoint)
        {
            chain.Joints[0].StartPoint = originPoint;

            for (int i = 0; i < chain.Joints.Length; i++)
            {
                // новая конечная точка ребра
                Point3D newEndPoint = GetPointOnSegment(chain.Joints[i].StartPoint,
                    chain.Joints[i].EndPoint, chain.Joints[i].JointLength);

                NewJoint prevJoint = null;
                if (i != 0)
                {
                    prevJoint = chain.Joints[i - 1];
                }
                newEndPoint = chain.Joints[i].ApplyConstraints(newEndPoint, prevJoint);

                chain.Joints[i].EndPoint = newEndPoint;
                
                // если ребро не последнее - ставим у следующего ребра точку начала в конец текущего ребра
                if (i + 1 < chain.Joints.Length)
                    chain.Joints[i + 1].StartPoint = chain.Joints[i].EndPoint;
            }
            return chain;
        }

        public static JointsChain Solve(JointsChain chain, Point3D targetPoint)
        {
            Points.Clear();
            JointsChain resultedChain = new JointsChain(chain.Joints);
            Point3D originPoint = chain.Joints[0].StartPoint;
            int iterationsCount = 0;
            double dif = MyMath.DistanceBetween(resultedChain.Joints[resultedChain.Joints.Length - 1].EndPoint, targetPoint);
            Points.Add(new Point3D[] { resultedChain.Joints[0].StartPoint, resultedChain.Joints[0].EndPoint, resultedChain.Joints[1].EndPoint, resultedChain.Joints[2].EndPoint });
            while (dif > Delta)
            {
                resultedChain = FabrikBackward(resultedChain, targetPoint);
                Points.Add(new Point3D[]{resultedChain.Joints[0].StartPoint, resultedChain.Joints[0].EndPoint, resultedChain.Joints[1].EndPoint, resultedChain.Joints[2].EndPoint});
                
                resultedChain = FabrikForward(resultedChain, originPoint);
                Points.Add(new Point3D[] { resultedChain.Joints[0].StartPoint, resultedChain.Joints[0].EndPoint, resultedChain.Joints[1].EndPoint, resultedChain.Joints[2].EndPoint });

                iterationsCount++;
                dif = MyMath.DistanceBetween(resultedChain.Joints[resultedChain.Joints.Length - 1].EndPoint, targetPoint);
                // Выходим из FABRIK если он работает слишком долго
                if (iterationsCount > 50) break;
            }
            int i = 0;
            foreach (var resultedChainJoint in resultedChain.Joints)
            {
                Console.WriteLine($"Joint {i} TurnAngle = {resultedChainJoint.JointAxises.TurnAngle}, RotateAngle = {resultedChainJoint.JointAxises.RotationAngle}");
                i++;

            }

            return resultedChain;
        }

        public static void GetAngleBetweenVectors(Vector3D vectorToTurn, Vector3D baseVector, out Vector3D axis, out double angle)
        {
            axis = Vector3D.CrossProduct(baseVector, vectorToTurn);
            axis.Normalize();
            angle = Math.Acos(Vector3D.DotProduct(baseVector, vectorToTurn)) * 180 / Math.PI;
        }
        

    }
}

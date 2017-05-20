using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using InverseTest.InverseAlgorithm;

namespace InverseTest.Manipulator
{
    public class NewJoint
    {
        public Point3D StartPoint { get; set; }
        public Point3D EndPoint { get; set; }

        public double JointLength { get; }

        public JointAxises JointAxises { get; set; }

        private double _upAngleConstraint;
        private double _downAngleConstraint;
        private double _rightAngleConstraint;
        private double _leftAngleConstraint;

        public Point3D DefaultStartPoint { get; set; }

        public Point3D DefaultEndPoint { get; set; }

        public void ResetDefaultPoints()
        {
            DefaultStartPoint = _tempDefStart;
            DefaultEndPoint = _tempDefEnd;
            //JointAxises.ChangeDefaultDirection(DefaultEndPoint - DefaultStartPoint);
        }

        private Point3D _tempDefStart;
        private Point3D _tempDefEnd;

        public NewJoint(Point3D startPoint, Point3D endPoint, Vector3D turnPlaneNormal, double[] anglesConstraints)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;

            DefaultStartPoint = startPoint;
            DefaultEndPoint = endPoint;

            _tempDefStart = startPoint;
            _tempDefEnd = endPoint;

            JointLength = GetLength();

            JointAxises = new JointAxises(EndPoint - StartPoint, turnPlaneNormal);

            switch (anglesConstraints.Length)
            {
                case 2:
                    _upAngleConstraint = anglesConstraints[0] / 2;
                    _downAngleConstraint = _upAngleConstraint;
                    _rightAngleConstraint = anglesConstraints[1] / 2;
                    _leftAngleConstraint = _rightAngleConstraint;
                    break;
                case 4:
                    _upAngleConstraint = anglesConstraints[0];
                    _downAngleConstraint = anglesConstraints[1];
                    _rightAngleConstraint = anglesConstraints[2];
                    _leftAngleConstraint = anglesConstraints[3];
                    break;
                default:
                    throw new Exception("Такого количества ограничевающих углов не может быть с точки зрения модели");
            }
        }



        /*public JointAxises GetAxises()
        {
            JointAxises.ChangeDirection(EndPoint - StartPoint);
            return JointAxises;
        }*/

        public double GetLength()
        {
            return MyMath.DistanceBetween(EndPoint, StartPoint);
        }

        public Point3D ApplyReverseConstraints(Point3D newStartPoint, NewJoint prevJoint)
        {
            Vector3D prevJointDefaultLookVector = prevJoint.DefaultEndPoint - prevJoint.DefaultStartPoint;
            prevJointDefaultLookVector.Normalize();

            Vector3D currJointDefaultLookVector = DefaultEndPoint - DefaultStartPoint;
            currJointDefaultLookVector.Normalize();

            double angle;
            Vector3D axis;

            Algorithm.GetAngleBetweenVectors(currJointDefaultLookVector, prevJointDefaultLookVector, out axis, out angle);

            Vector3D prevJointLookVector = prevJoint.JointAxises.CurrentLookVector;
            Vector3D prevJointTurnAxis = prevJoint.JointAxises.CurrentRightVector;

            double turnAxisScalar = Vector3D.DotProduct(axis, prevJointTurnAxis);
            if (turnAxisScalar < 0)
                angle *= -1;

            RotateTransform3D rottrans = new RotateTransform3D(new AxisAngleRotation3D(prevJointTurnAxis, angle));
            Vector3D vectorToProjectOnto = rottrans.Transform(prevJointLookVector);

            vectorToProjectOnto.Normalize();
            
            //JointAxises.DefaultLookVector;
            Vector3D newVector = (EndPoint - newStartPoint);
             double scalar = Vector3D.DotProduct(newVector, vectorToProjectOnto);

            Vector3D projection = scalar * vectorToProjectOnto;

            Vector3D adjust = newVector - projection;

            if (scalar < 0)
                projection = -projection;

            Vector3D rightVector;
            Vector3D upVector;

            JointAxises.GetUpAndRightVectors(vectorToProjectOnto, out upVector, out rightVector);


            double xaspect = Vector3D.DotProduct(adjust, rightVector);
            double yaspect = Vector3D.DotProduct(adjust, upVector);

            double left;
            double right;
            double up = projection.Length * Math.Tan(prevJoint._upAngleConstraint * Math.PI / 180);
            double down = -(projection.Length * Math.Tan(prevJoint._downAngleConstraint * Math.PI / 180));

            double xbound;
            double ybound;

            if (yaspect >= 0)
            {
                ybound = up;
                left = -(projection.Length * Math.Tan(prevJoint._upAngleConstraint * Math.PI / 180));
                right = projection.Length * Math.Tan(prevJoint._upAngleConstraint * Math.PI / 180);
            }

            else
            {
                ybound = down;
                left = -(projection.Length * Math.Tan(prevJoint._downAngleConstraint * Math.PI / 180));
                right = projection.Length * Math.Tan(prevJoint._downAngleConstraint * Math.PI / 180);
            }

            if (xaspect >= 0)
                xbound = right;
            else
                xbound = left;


            double ellipse = MyMath.PowerOf(xaspect, 2) / MyMath.PowerOf(xbound, 2) +
                             MyMath.PowerOf(yaspect, 2) / MyMath.PowerOf(ybound, 2);
            bool inbounds = ellipse <= 1;

            Vector3D v = projection + adjust;

            double a = Math.Atan2(yaspect, xaspect);

            double x = xbound * Math.Abs(Math.Cos(a));
            double y = ybound * Math.Abs(Math.Sin(a));

            if (x < Algorithm.Delta && x > -Algorithm.Delta) x = 0;
            if (y < Algorithm.Delta && y > -Algorithm.Delta) y = 0;


            if (!inbounds)
            {
                v = projection + rightVector * x + upVector * y;
                adjust = v - projection;
                xaspect = Vector3D.DotProduct(adjust, rightVector);
                yaspect = Vector3D.DotProduct(adjust, upVector);
            }

            Point p = new Point(0, ybound);
            RotateTransform rt;
            if (xaspect >= 0 && yaspect >= 0)
            {
                rt = new RotateTransform(-prevJoint._rightAngleConstraint);

            }
            else if (xaspect >= 0 && yaspect < 0)
            {
                rt = new RotateTransform(prevJoint._leftAngleConstraint);
            }
            else if (xaspect < 0 && yaspect >= 0)
            {
                rt = new RotateTransform(prevJoint._leftAngleConstraint);
            }
            else
            {
                rt = new RotateTransform(-prevJoint._rightAngleConstraint);
            }

            Point ap = rt.Transform(p);

            double d = (-ap.X * -y) - (-ap.Y * -x);
            if (d < 0 && ((xaspect > 0 && yaspect > 0) || (xaspect < 0 && yaspect < 0)))
            {
                Vector v1 = new Vector(ap.X, ap.Y);
                v1.Normalize();

                Vector v2 = new Vector(xaspect, yaspect);
                double tdScalar = v1.X * v2.X + v1.Y * v2.Y;
                
                v1 = v1 * tdScalar;
                v = projection + rightVector * v1.X + upVector * v1.Y;
            }
            else if (d > 0 && ((xaspect > 0 && yaspect < 0) || (xaspect < 0 && yaspect > 0)))
            {
                Vector v1 = new Vector(ap.X, ap.Y);
                v1.Normalize();
                
                Vector v2 = new Vector(xaspect, yaspect);
                double tdScalar = v1.X * v2.X + v1.Y * v2.Y;

                v1 = v1 * tdScalar;
                v = projection + rightVector * v1.X + upVector * v1.Y;
            }
            v.Normalize();
            v = v * newVector.Length;

            double l = MyMath.DistanceBetween(new Point3D(0, 0, 0), v.ToPoint3D());
            double ang = Vector3D.AngleBetween(JointAxises.DefaultLookVector, v);

            JointAxises.ChangeDirection(v);

            Vector3D dif = newVector - v;

            Point3D startPoint = EndPoint - v;
            return startPoint;
        }

        public Point3D ApplyConstraints(Point3D newEndPoint, NewJoint prevJoint)
        {
            Vector3D vectorToProjectOnto = JointAxises.DefaultLookVector;

            if (prevJoint != null)
            {
                Vector3D prevJointDefaultLookVector = prevJoint.DefaultEndPoint - prevJoint.DefaultStartPoint;
                prevJointDefaultLookVector.Normalize();

                Vector3D currJointDefaultLookVector = DefaultEndPoint - DefaultStartPoint;
                currJointDefaultLookVector.Normalize();

                double angle;
                Vector3D axis;

                Algorithm.GetAngleBetweenVectors(currJointDefaultLookVector, prevJointDefaultLookVector, out axis,
                    out angle);

                Vector3D prevJointLookVector = prevJoint.JointAxises.CurrentLookVector;
                Vector3D prevJointTurnAxis = prevJoint.JointAxises.CurrentRightVector;

                double turnAxisScalar = Vector3D.DotProduct(axis, prevJointTurnAxis);
                if (turnAxisScalar < 0)
                    angle *= -1;

                RotateTransform3D rottrans = new RotateTransform3D(new AxisAngleRotation3D(prevJointTurnAxis, angle));
                vectorToProjectOnto = rottrans.Transform(prevJointLookVector);

                vectorToProjectOnto.Normalize();
            }

            //Vector3D vectorToProjectOnto = JointAxises.DefaultLookVector;
            Vector3D newVector = newEndPoint - StartPoint;
            double scalar = Vector3D.DotProduct(newVector, vectorToProjectOnto) / vectorToProjectOnto.Length;
            vectorToProjectOnto.Normalize();
            Vector3D projection = scalar * vectorToProjectOnto;

            Vector3D adjust = newVector - projection;

            if (scalar < 0)
                projection = -projection;

            Vector3D rightVector;
            Vector3D upVector;

            JointAxises.GetUpAndRightVectors(vectorToProjectOnto, out upVector, out rightVector);

            double xaspect = Vector3D.DotProduct(adjust, rightVector);
            double yaspect = Vector3D.DotProduct(adjust, upVector);

            double left;
            double right;
            double up = projection.Length * Math.Tan(_upAngleConstraint * Math.PI / 180);
            double down = -(projection.Length * Math.Tan(_downAngleConstraint * Math.PI / 180));

            double xbound;
            double ybound;

            if (yaspect >= 0)
            {
                ybound = up;
                left = -(projection.Length * Math.Tan(_upAngleConstraint * Math.PI / 180));
                right = projection.Length * Math.Tan(_upAngleConstraint * Math.PI / 180);
            }

            else
            {
                ybound = down;
                left = -(projection.Length * Math.Tan(_downAngleConstraint * Math.PI / 180));
                right = projection.Length * Math.Tan(_downAngleConstraint * Math.PI / 180);
            }

            if (xaspect >= 0)
                xbound = right;
            else
                xbound = left;
            

            double ellipse = MyMath.PowerOf(xaspect, 2) / MyMath.PowerOf(xbound, 2) +
                             MyMath.PowerOf(yaspect, 2) / MyMath.PowerOf(ybound, 2);
            bool inbounds = ellipse <= 1;

            Vector3D v = projection + adjust;

            double a = Math.Atan2(yaspect, xaspect);

            double x = xbound * Math.Abs(Math.Cos(a));
            double y = ybound * Math.Abs(Math.Sin(a));

            if (x < Algorithm.Delta && x > -Algorithm.Delta) x = 0;
            if (y < Algorithm.Delta && y > -Algorithm.Delta) y = 0;

            if (!inbounds)
            {
                v = projection + rightVector * x + upVector * y;
                adjust = v - projection;
                xaspect = Vector3D.DotProduct(adjust, rightVector);
                yaspect = Vector3D.DotProduct(adjust, upVector);
            }

            Point p = new Point(0, ybound);
            RotateTransform rt;
            if (xaspect >= 0 && yaspect >= 0)
            {
                rt = new RotateTransform(-_rightAngleConstraint);

            }
            else if (xaspect >= 0 && yaspect < 0)
            {
                rt = new RotateTransform(_leftAngleConstraint);
            }
            else if (xaspect < 0 && yaspect >= 0)
            {
                rt = new RotateTransform(_leftAngleConstraint);
            }
            else
            {
                rt = new RotateTransform(-_rightAngleConstraint);
            }

            Point ap = rt.Transform(p);

            double d = (-ap.X * -y) - (-ap.Y * -x);
            if (d < 0 && ((xaspect > 0 && yaspect > 0) || (xaspect < 0 && yaspect < 0)))
            {
                Vector v1 = new Vector(ap.X, ap.Y);
                v1.Normalize();

                Vector v2 = new Vector(xaspect, yaspect);
                double tdScalar = v1.X * v2.X + v1.Y * v2.Y;
                
                v1 = v1 * tdScalar;
                v = projection + rightVector * v1.X + upVector * v1.Y;
            }
            else if (d > 0 && ((xaspect > 0 && yaspect < 0) || (xaspect < 0 && yaspect > 0)))
            {
                Vector v1 = new Vector(ap.X, ap.Y);
                v1.Normalize();

                Vector v2 = new Vector(xaspect, yaspect);
                double tdScalar = v1.X * v2.X + v1.Y * v2.Y;
                
                v1 = v1 * tdScalar;
                v = projection + rightVector * v1.X + upVector * v1.Y;
            }
            v.Normalize();
            v = v * newVector.Length;

            double l = MyMath.DistanceBetween(new Point3D(0, 0, 0), v.ToPoint3D());
            double ang = Vector3D.AngleBetween(JointAxises.DefaultLookVector, v);

            JointAxises.ChangeDirection(v);


            return StartPoint + v;

            
        }
    }
}

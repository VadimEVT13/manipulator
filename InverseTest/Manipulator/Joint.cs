using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using InverseTest.InverseAlgorithm;

namespace InverseTest.Manipulator
{
    public class Joint
    {

        

        private readonly Point3D _startPoint;
        private readonly Point3D _endPoint;

        public Vector3D TurnAxisVector { get; set; }
        public Vector3D RotateAxisVector { get; set; }

        public Vector3D TurnPlaneVector { get; set; }
        public Vector3D RotatePlaneVector { get; set; }

        public Point3D[] Points { get; set; } = new Point3D[2];

        private Transform3D _startPointTransform;
        public Transform3D StartPointTransform
        {
            get { return _startPointTransform; }
            set
            {
                _startPointTransform = value;

                Points[0] = _startPointTransform?.Transform(_startPoint) ?? _startPoint;
            }
        }


        private Transform3D _endPointTransform;
        public Transform3D EndPointTransform
        {
            get { return _endPointTransform; }
            set
            {
                _endPointTransform = value;
                Points[1] = _endPointTransform?.Transform(_endPoint) ?? _endPoint;
            }
        }

        public double Length { get; }

        public Joint(Point3D startPoint, Point3D endPoint)
        {
            _startPoint = startPoint;
            _endPoint = endPoint;

            Points[0] = startPoint;
            Points[1] = endPoint;
            Length = GetCurrentLength();
        }

        public double GetCurrentLength()
        {
            Vector3D lengthVector = Points[1] - Points[0];
            double length =
                Math.Sqrt(MyMath.PowerOf(lengthVector.X, 2) + MyMath.PowerOf(lengthVector.Y, 2) + MyMath.PowerOf(lengthVector.Z, 2));
            return length;
        }

        public void AddTransformToJointPoint(int pointIndex, Transform3D transform)
        {
            if (pointIndex == 0)
            {
                if (StartPointTransform == null)
                    StartPointTransform = transform;
                else
                    StartPointTransform = Transform3DHelper.CombineTransform(StartPointTransform, transform);
            }
            else if (pointIndex == 1)
            {
                if (EndPointTransform == null)
                    EndPointTransform = transform;
                else
                    EndPointTransform = Transform3DHelper.CombineTransform(EndPointTransform, transform);
            }
            else throw new IndexOutOfRangeException("Точек только две!");
        }

        public override string ToString()
        {
            return
                $"Point0({Points[0].X};{Points[0].Y};{Points[0].Z}), Point1({Points[1].X};{Points[1].Y};{Points[1].Z})\n";
        }

        public double[] GetTurnAngle()
        {
            Vector3D a = TurnAxisVector;

            // Вычисляем вектор нового положения ребра
            Vector3D newPositionVector = new Vector3D(Points[1].X - Points[0].X, Points[1].Y - Points[0].Y, Points[1].Z - Points[0].Z);

            Vector3D planeVector = new Vector3D(1, 1, 1) - TurnPlaneVector;

            // Сперва находим угол отклонения в плоскости XY
            // Для этого создаем проекцию вектора текущего положения ребра на плоскость XY
            Vector3D b = new Vector3D(newPositionVector.X * planeVector.X, newPositionVector.Y * planeVector.Y, newPositionVector.Z * planeVector.Z);

            double turnAngle;

            if (TurnPlaneVector.Equals(new Vector3D(0, 0, 1)))
            {
                turnAngle = Math.Atan2(a.X * b.Y - b.X * a.Y, a.X * b.X + a.Y * b.Y) * 180 / Math.PI;
            } else throw new Exception();



            planeVector = new Vector3D(1, 1, 1) - RotatePlaneVector;
            b = new Vector3D(newPositionVector.X * planeVector.X, newPositionVector.Y * planeVector.Y, newPositionVector.Z * planeVector.Z);

            if (turnAngle > 0)
                a = RotateAxisVector * -1;
            else
                a = RotateAxisVector;

            double rotateAngle;
            if (RotatePlaneVector.Equals(new Vector3D(1, 0, 0)))
            {
                rotateAngle = Math.Atan2(a.X * b.Z - b.X * a.Z, a.X * b.X + a.Z * b.Z) * 180 / Math.PI;
            } else if (RotatePlaneVector.Equals(new Vector3D(0, 1, 0)))
            {
                rotateAngle = Math.Atan2(a.Z * b.Y - b.Z * a.Y, a.Z * b.Z + a.Y * b.Y) * 180 / Math.PI;
            } else throw new Exception();

            double[] angles = new double[2];
            angles[0] = turnAngle;
            angles[1] = rotateAngle;
            /*else if (turnPlaneVector.Equals(new Vector3D(0, 1, 0)))
            {
                angle = Math.Atan2(a.X * b.Z - b.X * a.Z, a.X * b.X + a.Z * b.Z) * 180 / Math.PI;
            } else { throw new Exception("SHIT HAPPEND");}*/

            // Теперь используя проекцию, вычисляем угол между проекцией нового положения ребра и дефолтным положением
            return angles;
        }

        /*public double GetRotateAngle()
        {
            if (GetTurnAngle() < 0)
                RotateAxisVector = Vector3D.Multiply(RotateAxisVector, -1);
            
            // Вычисляем вектор нового положения ребра
            Vector3D newPositionVector = new Vector3D(Points[1].X - Points[0].X, Points[1].Y - Points[0].Y, Points[1].Z - Points[0].Z);
            // Находим угол отклонения в плоскости XZ
            // Для этого создаем проекцию вектора текущего положения ребра на плоскость XZ
            Vector3D proectionVector = new Vector3D(newPositionVector.X, 0, newPositionVector.Z);
            // Теперь используя проекцию, вычисляем угол между проекцией нового положения ребра и дефолтным положением
            Vector3D newVector;
            if (proectionVector.X < 0)
                newVector = RotateAxisVector - proectionVector;
            else
                newVector = proectionVector - RotateAxisVector;
            double angle = Math.Atan2(newVector.X, newVector.Z) * 180 / Math.PI;
            
            return angle;
        }*/

        public void Reset()
        {
            StartPointTransform = null;
            EndPointTransform = null;
        }
    }
}

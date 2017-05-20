using System;
using System.Windows;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using InverseTest.InverseAlgorithm;

namespace InverseTest.Manipulator
{
    public class JointAxises
    {
        private Vector3D _defaultLookVector;
        private Vector3D _defaultUpVector;
        private Vector3D _defaultRightVector;

        private double _defaultRotationAngle = 0;
        private double _defaultTurnAngle = 0;

        private Vector3D _currentLookVector;
        private Vector3D _currentUpVector;
        private Vector3D _currentRightVector;

        private double _rotationAngle = 0;
        private double _turnAngle = 0;


        public Vector3D DefaultLookVector => _defaultLookVector;
        public Vector3D DefaultRightVector => _defaultRightVector;
        public Vector3D DefaultUpVector => _defaultUpVector;

        public Vector3D TurnAxis => _defaultRightVector;
        public Vector3D RotationAxis => _defaultLookVector;

        public double DefaultRotationAngle => _defaultRotationAngle;
        public double DefaultTurnAngle => _defaultTurnAngle;

        public Vector3D CurrentLookVector => _currentLookVector;
        public Vector3D CurrentUpVector => _currentUpVector;
        public Vector3D CurrentRightVector => _currentRightVector;

        public double RotationAngle => _rotationAngle;
        public double TurnAngle => _turnAngle;

        private Vector3D _rotationVector;



        public JointAxises(Vector3D lookDirection, Vector3D turnPlaneNormal)
        {
            lookDirection.Normalize();
            turnPlaneNormal.Normalize();

            _defaultLookVector = lookDirection;
            _currentLookVector = _defaultLookVector;

            Vector3D upVector = Vector3D.CrossProduct(turnPlaneNormal, _defaultLookVector);
            upVector.Normalize();
            _defaultUpVector = upVector;
            _currentUpVector = _defaultUpVector;

            Vector3D rightVector = Vector3D.CrossProduct(_defaultLookVector, _defaultUpVector);
            rightVector.Normalize();
            _defaultRightVector = rightVector;
            _currentRightVector = _defaultRightVector;
        }

        public void ChangeDirection(Vector3D newDirection)
        {
            newDirection.Normalize();

            double turnAngle; 
            double rotateAngle; 
            ComputeAngles(newDirection, out turnAngle, out rotateAngle);

            // теперь необходимо выполнить непосредственно смену направления всех векторов
            RotateTransform3D turnTransform = new RotateTransform3D(new AxisAngleRotation3D(_defaultRightVector, turnAngle));
            RotateTransform3D rotateTransform = new RotateTransform3D(new AxisAngleRotation3D(_defaultLookVector, rotateAngle));

            _currentLookVector = turnTransform.Transform(_defaultLookVector);
            _currentRightVector = turnTransform.Transform(_defaultRightVector);
            _currentUpVector = turnTransform.Transform(_defaultUpVector);

            _currentLookVector = rotateTransform.Transform(_currentLookVector);
            _currentRightVector = rotateTransform.Transform(_currentRightVector);
            _currentUpVector = rotateTransform.Transform(_currentUpVector);

            _turnAngle = turnAngle;
            _rotationAngle = rotateAngle;

        }

        public void GetUpAndRightVectors(Vector3D newDirection, out Vector3D upVector, out Vector3D rightVector)
        {
            newDirection.Normalize();

            double turnAngle;
            double rotateAngle;
            ComputeAngles(newDirection, out turnAngle, out rotateAngle);

            // теперь необходимо выполнить непосредственно смену направления всех векторов
            RotateTransform3D turnTransform = new RotateTransform3D(new AxisAngleRotation3D(_defaultRightVector, turnAngle));
            RotateTransform3D rotateTransform = new RotateTransform3D(new AxisAngleRotation3D(_defaultLookVector, rotateAngle));

            rightVector = turnTransform.Transform(_defaultRightVector);
            upVector = turnTransform.Transform(_defaultUpVector);

            rightVector = rotateTransform.Transform(rightVector);
            upVector = rotateTransform.Transform(upVector);
        }

        public void ChangeDefaultDirection(Vector3D newDirection)
        {
            newDirection.Normalize();

            double turnAngle;
            double rotateAngle;

            ComputeAngles(newDirection, out turnAngle, out rotateAngle);

            RotateTransform3D turnTransform = new RotateTransform3D(new AxisAngleRotation3D(_defaultRightVector, turnAngle));
            RotateTransform3D rotateTransform = new RotateTransform3D(new AxisAngleRotation3D(_defaultLookVector, rotateAngle));

            _defaultLookVector = turnTransform.Transform(_defaultLookVector);
            _defaultRightVector = turnTransform.Transform(_defaultRightVector);
            _defaultUpVector = turnTransform.Transform(_defaultUpVector);

            _defaultLookVector = rotateTransform.Transform(_defaultLookVector);
            _defaultRightVector = rotateTransform.Transform(_defaultRightVector);
            _defaultUpVector = rotateTransform.Transform(_defaultUpVector);

            _defaultRotationAngle = rotateAngle;
            _defaultTurnAngle = turnAngle;

            ChangeDirection(_currentLookVector);
        }

        private void ComputeAngles(Vector3D newDirection, out double turnAngle, out double rotateAngle)
        {
            // проецируем новый вектор на вектора, определяющие верх и право. это дает нам координаты проекции вектора нового направления на плоскость, в которой будет вращаться ребро
            double xaspect = Vector3D.DotProduct(newDirection, _defaultUpVector);
            double yaspect = Vector3D.DotProduct(newDirection, _defaultRightVector);

            // вектор - проекция вектора нового направления на плоскость вращения
            Vector rotProection = new Vector(xaspect, yaspect);

            // вектор - проекция вектора "вверх" на плоскость вращения
            Vector upProection = new Vector(Vector3D.DotProduct(_defaultUpVector, _defaultUpVector), Vector3D.DotProduct(_defaultUpVector, _defaultRightVector));

            // теперь надо вычислить угол вращения между upDirection и rotationProection
            double angle1 = Math.Atan2(rotProection.Y, rotProection.X) * 180 / Math.PI;
            double angle2 = Math.Atan2(upProection.Y, upProection.X) * 180 / Math.PI;

            //double angle2 = Math.Atan2(_defaultUpVector.Y, _defaultUpVector.X) * 180 / Math.PI;
            rotateAngle = angle1 - angle2;

            // Угол поворота
            turnAngle = Vector3D.AngleBetween(_defaultLookVector, newDirection);

            // Теперь преобразуем углы так, чтобы вектор не перевернулся
            if (xaspect < 0)
            {
                turnAngle *= -1;
                if (yaspect >= 0)
                    rotateAngle = rotateAngle - 180;
                else
                    rotateAngle = rotateAngle + 180;
            }
        }

        public override string ToString()
        {
            return
                $"LookDirection = {CurrentLookVector};\n UpVector = {CurrentUpVector};\n RightVector = {CurrentRightVector}\n";
        }
    }
}

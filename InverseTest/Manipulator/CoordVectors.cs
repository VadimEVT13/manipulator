using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InverseTest.Manipulator
{
    public class CoordVectors
    {

        public enum RotationPlane
        {
            XY, XZ,
            YX, YZ,
            ZX, ZY
        }

        private Vector3D _lookDirection;
        private Vector3D _topVector;
        private Vector3D _rightVector;

        private readonly RotationPlane _upVectorRotationPlane;
        private readonly RotationPlane _rightVectorRotationPlane;

        public CoordVectors(Vector3D lookDirection, RotationPlane upVectorRotationPlane, RotationPlane rightVectoRotationPlane)
        {
            _lookDirection = lookDirection;
            _lookDirection.Normalize();
            _upVectorRotationPlane = upVectorRotationPlane;
            _rightVectorRotationPlane = rightVectoRotationPlane;

            if (_upVectorRotationPlane == RotationPlane.XY && _rightVectorRotationPlane == RotationPlane.XZ)
            {
                Vector3D fixedAxisVector = new Vector3D(0, 1, 0);
                if (fixedAxisVector.Equals(_lookDirection) || fixedAxisVector.Equals(_lookDirection * -1))
                    _rightVector = new Vector3D(0, 0, 1);
                else
                {
                    _rightVector = Vector3D.CrossProduct(_lookDirection, fixedAxisVector);
                    _rightVector.Normalize();
                }
                _topVector = Vector3D.CrossProduct(_rightVector, _lookDirection);
                _topVector.Normalize();
            } else if (_upVectorRotationPlane == RotationPlane.XY && _rightVectorRotationPlane == RotationPlane.YZ)
            {
                if (_lookDirection.Equals(new Vector3D(1, 0, 0)))
                {
                    _topVector = new Vector3D(0, 1, 0);
                    _rightVector = new Vector3D(0, 0, 1);
                }
                else
                {
                    // Вычисляем проекцию вектора на плоскость
                    Vector3D temp = new Vector3D(0, _lookDirection.Y, _lookDirection.Z);
                    if ((_lookDirection.Y >= 0 && _lookDirection.X >= 0) || (_lookDirection.Y < 0 && _lookDirection.X < 0))
                        _rightVector = Vector3D.CrossProduct(_lookDirection, temp);
                    else
                        _rightVector = Vector3D.CrossProduct(temp, _lookDirection);
                    _rightVector.Normalize();
                    _topVector = Vector3D.CrossProduct(_rightVector, _lookDirection);
                    _topVector.Normalize();
                }
            } else if (_upVectorRotationPlane == RotationPlane.YX && _rightVectorRotationPlane == RotationPlane.XZ)
            {
                if (_lookDirection.Equals(new Vector3D(0, 1, 0)))
                {
                    _topVector = new Vector3D(-1, 0, 0);
                    _rightVector = new Vector3D(0, 0, 1);
                } else if (_lookDirection.Equals(new Vector3D(0, -1, 0)))
                {
                    _topVector = new Vector3D(1, 0, 0);
                    _rightVector = new Vector3D(0, 0, 1);
                }
                else if (_lookDirection.Equals(new Vector3D(0, 0, 1)))
                {
                    _topVector = new Vector3D(0,1,0);
                    _rightVector = new Vector3D(-1, 0, 0);
                } else if (_lookDirection.Equals(new Vector3D(0, 0, -1)))
                {
                    _topVector = new Vector3D(0, 1, 0);
                    _rightVector = new Vector3D(1, 0, 0);
                }
                else
                {
                    Vector3D temp = new Vector3D(_lookDirection.X, 0, _lookDirection.Z);
                    if ((_lookDirection.Y > 0 && _lookDirection.X < 0) || (_lookDirection.Y < 0 && _lookDirection.X > 0))
                        _rightVector = Vector3D.CrossProduct(_lookDirection, temp);
                    else
                        _rightVector = Vector3D.CrossProduct(temp, _lookDirection);
                    _rightVector.Normalize();
                    _topVector = Vector3D.CrossProduct(_rightVector, _lookDirection);
                    _topVector.Normalize();
                }
            }
        }

        /*public void ChangeLookDirection(Vector3D newLookDirection)
        {
            newLookDirection.Normalize();
            
            _lookDirection = newLookDirection;
            if (!_lookDirection.Equals(_topVector))
            {
                Vector3D v = Vector3D.CrossProduct(_lookDirection, _topVector);
                v.Normalize();
                _rightVector = v;
            }
            

            Vector3D v2;
            if (_lookDirection.X >= 0)
                v2 = Vector3D.CrossProduct(_rightVector, _lookDirection);
            else 
                v2 = Vector3D.CrossProduct(_lookDirection, _rightVector);
            v2.Normalize();
            _topVector = v2;



        }*/
    }
}


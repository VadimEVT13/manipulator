﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Media3D;
using InverseTest.Manipulator;
using System.Windows.Threading;
using System.Linq;
using NLog;

namespace InverseTest
{


    /// <summary>
    /// Перечисление всех подвижных ребер манипулятора
    /// </summary>
    public enum ManipulatorParts
    {
        /// <summary>
        /// Камера манипулятора
        /// </summary>
        Camera,

        /// <summary>
        /// Стойка, на которой закреплена камера
        /// </summary>
        CameraBase,

        /// <summary>
        /// Вращающаяся часть верхнего ребра
        /// </summary>
        TopEdge,

        /// <summary>
        /// Поворотная часть стойки
        /// </summary>
        MiddleEdge,

        /// <summary>
        /// Вращающаяся часть нижнего ребра (т.е. столик)
        /// </summary>
        Table,

        /// <summary>
        /// Основание
        /// </summary>

        Platform
    }
    /// <summary>
    /// 
    /// </summary>
    public class ManipulatorV2 : IPositionChanged
    {
        /// <summary>
        /// Логгирование
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        // поле для хранения 3D модели манипулятора
        private readonly Model3DGroup _manipulator3DModel = new Model3DGroup();
        int numberMesh = 0;

        private static Vector3D DEFAULT_CAMERA_DIRECTION = new Vector3D(1, 0, 0);

        private Vector3D currentCameraDirection = DEFAULT_CAMERA_DIRECTION;

        public readonly Dictionary<ManipulatorParts, IManipulatorPart> parts = new Dictionary<ManipulatorParts, IManipulatorPart>();
        private Dictionary<ManipulatorParts, double> partAngles = new Dictionary<ManipulatorParts, double>();

        private ManipulatorAngles anglesToSet;

        private ManipulatorAnimator animator;

        /// <summary>
        /// Положение первого колена.
        /// </summary>
        public double TablePosition
        {
            get
            {
                return partAngles[ManipulatorParts.Table];
            }
            set
            {
                partAngles[ManipulatorParts.Table] = value;
                ConfirmRotation();
                onManulaPositionChanged?.Invoke();
            }
        }

        /// <summary>
        /// Положение второго колена.
        /// </summary>
        public double MiddleEdgePosition
        {
            get
            {
                return partAngles[ManipulatorParts.MiddleEdge];
            }
            set
            {
                partAngles[ManipulatorParts.MiddleEdge] = value;
                ConfirmRotation();
                onManulaPositionChanged?.Invoke();
            }
        }

        /// <summary>
        /// Положение третьего колена.
        /// </summary>
        public double TopEdgePosition
        {
            get
            {
                return partAngles[ManipulatorParts.TopEdge];
            }
            set
            {
                partAngles[ManipulatorParts.TopEdge] = value;
                ConfirmRotation();
                onManulaPositionChanged?.Invoke();
            }
        }

        /// <summary>
        /// Положение четвертого колена.
        /// </summary>
        public double CameraBasePosition
        {
            get
            {
                return partAngles[ManipulatorParts.CameraBase];
            }
            set
            {
                partAngles[ManipulatorParts.CameraBase] = value;
                ConfirmRotation();
                onManulaPositionChanged?.Invoke();
            }
        }

        /// <summary>
        /// Положение пятого колена.
        /// </summary>
        public double CameraPosition
        {
            get
            {
                return partAngles[ManipulatorParts.Camera];
            }
            set
            {
                partAngles[ManipulatorParts.Camera] = value;
                ConfirmRotation();
                onManulaPositionChanged?.Invoke();
            }
        }

        /// <summary>
        /// Перечисление всех точек относительно которых происходи вращение
        /// </summary>
        public enum ManipulatorRotatePoints
        {
            /// <summary>
            /// Точка на столе
            /// </summary>
            POINT_ON_TABLE,

            /// <summary>
            /// Точка на основном ребре, относительно которого крутиться верхнее ребро
            /// </summary>
            POINT_ON_MAIN_EDGE,

            /// <summary>
            /// Точка на ребре под камерой
            /// </summary>
            POINT_BELOW_CAMERA,

            /// <summary>
            /// Точка в которой стоит камера
            /// </summary>
            POINT_ON_CAMERA
        }

        // список мешей, расположенных в точках сочленений ребер манипулятора; используются для определения точек поворота ребер
        private readonly Model3DCollection _jointCubes = new Model3DCollection();

        // список мешей, представляющих ребра станка
        private readonly Model3DCollection _edges = new Model3DCollection();

        private readonly Model3D _cameraposition;


        public event PositionHandler onPositionChanged;
        public event ManualPositionHandler onManulaPositionChanged;

        /// <summary>
        /// Создание и конфигурация модели манипулятора
        /// </summary>
        /// <param name="file">Путь к файлу с 3D моделью станка</param>
        public ManipulatorV2(Model3DGroup machine3DModel)
        {
            try
            {
                // Камера
                Model3DGroup cam3DModel = new Model3DGroup();
                cam3DModel.Children.Add(machine3DModel.Children[8]);
                cam3DModel.Children.Add(machine3DModel.Children[9]);

                _cameraposition = machine3DModel.Children[13];

                // Стойка камеры
                Model3DGroup camEdge3DModel = new Model3DGroup();
                camEdge3DModel.Children.Add(machine3DModel.Children[7]);

                // Верхнее поворотное ребро
                Model3DGroup upperEdge3DModel = new Model3DGroup();
                upperEdge3DModel.Children.Add(machine3DModel.Children[5]);
                upperEdge3DModel.Children.Add(machine3DModel.Children[6]);
                // Стойка
                Model3DGroup middleEdge3DModel = new Model3DGroup();
                middleEdge3DModel.Children.Add(machine3DModel.Children[3]);
                middleEdge3DModel.Children.Add(machine3DModel.Children[4]);

                // Столик 
                Model3DGroup table3DModel = new Model3DGroup();
                table3DModel.Children.Add(machine3DModel.Children[2]);

                // Основание
                Model3DGroup base3DModel = new Model3DGroup();
                base3DModel.Children.Add(machine3DModel.Children[1]);
                base3DModel.Children.Add(machine3DModel.Children[0]);

                IManipulatorPart cam3D = new ManipulatorPart(cam3DModel);
                IManipulatorPart camEdge = new ManipulatorPart(camEdge3DModel);
                IManipulatorPart upperEdge = new ManipulatorPart(upperEdge3DModel);
                IManipulatorPart middleEdge = new ManipulatorPart(middleEdge3DModel);
                IManipulatorPart table = new ManipulatorPart(table3DModel);
                IManipulatorPart basePart = new ManipulatorPart(base3DModel);

                parts.Add(ManipulatorParts.Camera, cam3D);
                parts.Add(ManipulatorParts.CameraBase, camEdge);
                parts.Add(ManipulatorParts.MiddleEdge, middleEdge);
                parts.Add(ManipulatorParts.TopEdge, upperEdge);
                parts.Add(ManipulatorParts.Table, table);
                parts.Add(ManipulatorParts.Platform, basePart);

                // Заполняем список мешей
                _edges.Add(table3DModel);
                _edges.Add(middleEdge3DModel);
                _edges.Add(upperEdge3DModel);
                _edges.Add(camEdge3DModel);
                _edges.Add(cam3DModel);
                _edges.Add(base3DModel);

                partAngles[ManipulatorParts.Table] = 0;
                partAngles[ManipulatorParts.MiddleEdge] = 0;
                partAngles[ManipulatorParts.TopEdge] = 0;
                partAngles[ManipulatorParts.CameraBase] = 0;
                partAngles[ManipulatorParts.Camera] = 0;
                partAngles[ManipulatorParts.Platform] = 0;

                // Заполняем список мешей в точках сочленений
                _jointCubes.Add(machine3DModel.Children[10]);
                _jointCubes.Add(machine3DModel.Children[11]);
                _jointCubes.Add(machine3DModel.Children[12]);

                _manipulator3DModel.Children = _edges;


                this.animator = new ManipulatorAnimator(this);
            }
            catch (InvalidOperationException e)
            {
                logger.Error(e.Message);
                throw;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw;
            }
        }

        public virtual void MoveManipulator(ManipulatorAngles angles, bool animate)
        {

            if (animator.IsAnimated)
                animator.StopAnimation();

            if (animate)
                animator.StartAnimation(angles);
            else
            {
                try
                {
                    setAngles(angles);
                }
                catch (NullReferenceException ex)
                {
                    logger.Error(ex.Message);
                }
            }
        }
    
        public void setAngles(ManipulatorAngles angles)
        {
            partAngles = angles.partAngles;
            ConfirmRotation();
        }

        /// <summary>
        /// Метод вращает определенное ребро манипулятора на заданный угол вращения
        /// </summary>
        /// <param name="part">Часть манипулятора, которую необходимо повернуть</param>
        /// <param name="angle">Угол поворота части манипулятора</param>
        /// 
        public RotateTransform3D GetRotateTransfofm(ManipulatorParts part, double angle)
        {
            Point3D rotatePoint;
            IManipulatorPart modelToRotate;
            Vector3D rotationAxis;
            // для каждой части манипулятора необходимо задать свои точки поворота, ось вращения и угол вращения
            switch (part)
            {
                case ManipulatorParts.Table:
                    // Ребро, которое необходимо повернуть
                    modelToRotate = parts[ManipulatorParts.Table];
                    // Берем меш, находящийся в точке поворота ребра, получяем его позицию (она на одном из углов меша)
                    rotatePoint = MathUtils.GetRectCenter(_jointCubes[0].Bounds)
;
                    // Полученную точку сдвигаем в цетр меша, получая точку вращения
                    // Ось вращения
                    rotationAxis = new Vector3D(0, 1, 0);

                    break;

                case ManipulatorParts.MiddleEdge:
                    modelToRotate = parts[ManipulatorParts.MiddleEdge];
                    rotatePoint = MathUtils.GetRectCenter(_jointCubes[0].Bounds);
                    rotationAxis = new Vector3D(0, 0, 1);
                    break;

                case ManipulatorParts.TopEdge:
                    modelToRotate = parts[ManipulatorParts.TopEdge];
                    rotatePoint = MathUtils.GetRectCenter(_jointCubes[1].Bounds);
                    rotationAxis = new Vector3D(0, 0, 1);
                    break;

                case ManipulatorParts.CameraBase:
                    modelToRotate = parts[ManipulatorParts.CameraBase];
                    rotatePoint = MathUtils.GetRectCenter(_jointCubes[2].Bounds);
                    rotationAxis = new Vector3D(1, 0, 0);
                    break;

                case ManipulatorParts.Camera:
                    modelToRotate = parts[ManipulatorParts.Camera];
                    rotatePoint = MathUtils.GetRectCenter(_jointCubes[2].Bounds);
                    rotationAxis = new Vector3D(0, 0, 1);
                    break;

                default:
                    throw new InvalidEnumArgumentException();
            }
            // Поворачиваем ребро в соответствии с заданными параметрами
            return new RotateTransform3D(new AxisAngleRotation3D(rotationAxis, angle), rotatePoint);

        }

        public virtual Vector3D GetCameraDirection()
        {
            return currentCameraDirection;
        }

        public void RotatePart(ManipulatorParts part, double angle)
        {
            partAngles[part] = angle;

            ConfirmRotation();
        }

        /// <summary>
        /// Вычисляет текущее направление камеры манипулятора
        /// </summary>
        /// <param name="rotate"></param>
        private void CalculateCameraDirection(Transform3D transform)
        {
            Matrix3D m = Matrix3D.Identity;
            Matrix3D mx = transform.Value;
            currentCameraDirection = mx.Transform(DEFAULT_CAMERA_DIRECTION);
        }

        /// <summary>
        /// Устанавливает манипулятор в положение углов заданных в <see cref="partAngles"/>
        /// </summary>
        private void ConfirmRotation()
        {
            Transform3DGroup tableGroup = new Transform3DGroup();
            Transform3DGroup middleEdgeGroup = new Transform3DGroup();
            Transform3DGroup topEdgeBaseGroup = new Transform3DGroup();
            Transform3DGroup topEdgeGroup = new Transform3DGroup();
            Transform3DGroup cameraBaseGroup = new Transform3DGroup();
            Transform3DGroup cameraGroup = new Transform3DGroup();

            RotateTransform3D R = GetRotateTransfofm(ManipulatorParts.Table, partAngles[ManipulatorParts.Table]);
            tableGroup.Children.Add(R);

            R = GetRotateTransfofm(ManipulatorParts.MiddleEdge, partAngles[ManipulatorParts.MiddleEdge]);
            middleEdgeGroup.Children.Add(R);
            middleEdgeGroup.Children.Add(tableGroup);

            R = GetRotateTransfofm(ManipulatorParts.TopEdge, partAngles[ManipulatorParts.TopEdge]);
            topEdgeGroup.Children.Add(R);
            topEdgeGroup.Children.Add(middleEdgeGroup);

            R = GetRotateTransfofm(ManipulatorParts.CameraBase, partAngles[ManipulatorParts.CameraBase]);
            cameraBaseGroup.Children.Add(R);
            cameraBaseGroup.Children.Add(topEdgeGroup);

            R = GetRotateTransfofm(ManipulatorParts.Camera, partAngles[ManipulatorParts.Camera]);
            cameraGroup.Children.Add(R);
            cameraGroup.Children.Add(cameraBaseGroup);

            _cameraposition.Transform = cameraGroup;

            CalculateCameraDirection(cameraGroup);

            parts[ManipulatorParts.Camera].RotateTransform3D(cameraGroup);
            parts[ManipulatorParts.CameraBase].RotateTransform3D(cameraBaseGroup);
            parts[ManipulatorParts.TopEdge].RotateTransform3D(topEdgeGroup);
            parts[ManipulatorParts.MiddleEdge].RotateTransform3D(middleEdgeGroup);
            parts[ManipulatorParts.Table].RotateTransform3D(tableGroup);

            onPositionChanged?.Invoke();
        }

        public Model3D GetManipulatorModel()
        {
            return _manipulator3DModel;
        }

        public Model3D GetManipulatorPart(ManipulatorParts part)
        {
            return parts[part].GetModel();
        }

        public virtual void addNumberMesh(int number)
        {
            this.numberMesh = number;
        }

        public virtual void transformModel(Double x)
        {
            _manipulator3DModel.Children[numberMesh].Transform = new TranslateTransform3D(0, x, 0);
        }

        public virtual Point3D GetCameraPosition()
        {
            return _cameraposition.Bounds.Location;
        }

        public virtual void ResetModel()
        {
            foreach (ManipulatorParts part in Enum.GetValues(typeof(ManipulatorParts)))
                partAngles[part] = 0;

            ConfirmRotation();
        }


        /// <summary>
        /// Возвращает одну из точек вращения
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Point3D GetPointJoint(ManipulatorRotatePoints point)
        {
            Point3D resPoint;
            Rect3D bounds;
            switch (point)
            {
                case ManipulatorRotatePoints.POINT_ON_TABLE:
                    bounds = _jointCubes[0].Bounds;
                    break;
                case ManipulatorRotatePoints.POINT_ON_MAIN_EDGE:
                    bounds = _jointCubes[1].Bounds;
                    break;
                case ManipulatorRotatePoints.POINT_BELOW_CAMERA:
                    bounds = _jointCubes[2].Bounds;
                    break;
                case ManipulatorRotatePoints.POINT_ON_CAMERA:
                    bounds = _cameraposition.Bounds;
                    break;
                default: throw new InvalidEnumArgumentException();
            }

            resPoint = new Point3D(bounds.X + bounds.SizeX / 2, bounds.Y + bounds.SizeY / 2, bounds.Z + bounds.SizeZ / 2);
            return resPoint;
        }
    }
}

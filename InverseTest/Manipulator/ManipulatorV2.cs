using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Media3D;
using InverseTest.Manipulator;
using System.Windows.Threading;

namespace InverseTest
{
    /// <summary>
    /// 
    /// </summary>
    public class ManipulatorV2 : IManipulatorModel, IDebugModels
    {
        // поле для хранения 3D модели манипулятора
        private readonly Model3DGroup _manipulator3DModel = new Model3DGroup();
        int numberMesh = 0;

        private static Vector3D DEFAULT_CAMERA_DIRECTION = new Vector3D(1, 0, 0);

        private Vector3D currentCameraDirection = DEFAULT_CAMERA_DIRECTION;

        private Dictionary<ManipulatorParts, IManipulatorPart> parts = new Dictionary<ManipulatorParts, IManipulatorPart>();
        private Dictionary<ManipulatorParts, double> partAngles = new Dictionary<ManipulatorParts, double>();
        private Dictionary<ManipulatorParts, double> partDeltasToRotate = new Dictionary<ManipulatorParts, double>();
        private ManipulatorAngles anglesToSet;


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
            /// Поворотная часть верхнего ребра
            /// </summary>
            TopEdgeBase,

            /// <summary>
            /// Поворотная часть стойки
            /// </summary>
            MiddleEdge,

            /// <summary>
            /// Вращающаяся часть нижнего ребра (т.е. столик)
            /// </summary>
            Table
        }

        // список мешей, расположенных в точках сочленений ребер манипулятора; используются для определения точек поворота ребер
        private readonly Model3DCollection _jointCubes = new Model3DCollection();

        // список мешей, представляющих ребра станка
        private readonly Model3DCollection _edges = new Model3DCollection();

        private readonly Model3D _cameraposition;

        DispatcherTimer timer;
        bool isAnimated = false;

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
                cam3DModel.Children.Add(machine3DModel.Children[15]);
                cam3DModel.Children.Add(machine3DModel.Children[16]);
                cam3DModel.Children.Add(machine3DModel.Children[17]);

                _cameraposition = machine3DModel.Children[17];

                // Стойка камеры
                Model3DGroup camEdge3DModel = new Model3DGroup();
                camEdge3DModel.Children.Add(machine3DModel.Children[13]);
                camEdge3DModel.Children.Add(machine3DModel.Children[14]);

                ///     camEdge3DModel.Children.Add(machine3DModel.Children[17]);

                // Верхнее вращающееся ребро
                Model3DGroup upperTurningEdge3DModel = new Model3DGroup();
                upperTurningEdge3DModel.Children.Add(machine3DModel.Children[12]);

                // Верхнее поворотное ребро
                Model3DGroup upperEdge3DModel = new Model3DGroup();
                upperEdge3DModel.Children.Add(machine3DModel.Children[9]);
                upperEdge3DModel.Children.Add(machine3DModel.Children[10]);
                upperEdge3DModel.Children.Add(machine3DModel.Children[11]);

                //    upperEdge3DModel.Children.Add(machine3DModel.Children[18]);


                // Стойка
                Model3DGroup middleEdge3DModel = new Model3DGroup();
                middleEdge3DModel.Children.Add(machine3DModel.Children[7]);
                middleEdge3DModel.Children.Add(machine3DModel.Children[8]);

                //    middleEdge3DModel.Children.Add(machine3DModel.Children[19]);

                // Столик 
                Model3DGroup table3DModel = new Model3DGroup();
                table3DModel.Children.Add(machine3DModel.Children[4]);
                table3DModel.Children.Add(machine3DModel.Children[5]);
                table3DModel.Children.Add(machine3DModel.Children[6]);

                //   table3DModel.Children.Add(machine3DModel.Children[20]);


                // Основание
                Model3DGroup base3DModel = new Model3DGroup();
                base3DModel.Children.Add(machine3DModel.Children[0]);
                base3DModel.Children.Add(machine3DModel.Children[1]);
                base3DModel.Children.Add(machine3DModel.Children[2]);
                base3DModel.Children.Add(machine3DModel.Children[3]);

                IManipulatorPart cam3D = new ManipulatorPartDecorator(cam3DModel, null);
                IManipulatorPart camEdge = new ManipulatorPartDecorator(camEdge3DModel, cam3D);
                IManipulatorPart upperTurningEdge = new ManipulatorPartDecorator(upperTurningEdge3DModel, camEdge);
                IManipulatorPart upperEdge = new ManipulatorPartDecorator(upperEdge3DModel, upperTurningEdge);
                IManipulatorPart middleEdge = new ManipulatorPartDecorator(middleEdge3DModel, upperEdge);
                IManipulatorPart table = new ManipulatorPartDecorator(table3DModel, middleEdge);
                IManipulatorPart basePart = new ManipulatorPartDecorator(base3DModel, table);

                parts.Add(ManipulatorParts.Camera, cam3D);
                parts.Add(ManipulatorParts.CameraBase, camEdge);
                parts.Add(ManipulatorParts.MiddleEdge, middleEdge);
                parts.Add(ManipulatorParts.TopEdgeBase, upperEdge);
                parts.Add(ManipulatorParts.TopEdge, upperTurningEdge);
                parts.Add(ManipulatorParts.Table, table);

                // Заполняем список мешей
                _edges.Add(table3DModel);
                _edges.Add(middleEdge3DModel);
                _edges.Add(upperEdge3DModel);
                _edges.Add(upperTurningEdge3DModel);
                _edges.Add(camEdge3DModel);
                _edges.Add(cam3DModel);
                _edges.Add(base3DModel);

                partAngles[ManipulatorParts.Table] = 0;
                partAngles[ManipulatorParts.MiddleEdge] = 0;
                partAngles[ManipulatorParts.TopEdgeBase] = 0;
                partAngles[ManipulatorParts.TopEdge] = 0;
                partAngles[ManipulatorParts.CameraBase] = 0;
                partAngles[ManipulatorParts.Camera] = 0;



                // Заполняем список мешей в точках сочленений
                _jointCubes.Add(machine3DModel.Children[18]);
                _jointCubes.Add(machine3DModel.Children[19]);
                _jointCubes.Add(machine3DModel.Children[20]);
                _jointCubes.Add(machine3DModel.Children[21]);
                
                _manipulator3DModel.Children = _edges;

                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromTicks(1000);
                timer.Tick += animation_tick;

            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public virtual void MoveManipulator(ManipulatorAngles angles, bool animate)
        {
            if (animate)
                startAnimation(angles);
            else
                setAngles(angles);
        }

        private void startAnimation(ManipulatorAngles angles)
        {
            if (!isAnimated)
            {
                this.anglesToSet = angles;

                foreach (ManipulatorParts part in Enum.GetValues(typeof(ManipulatorParts)))
                    partDeltasToRotate[part] = (angles.partAngles[part] - partAngles[part]) / 1000;
                timer.Start();
                isAnimated = true;
            }
        }

        private void setAngles(ManipulatorAngles angles)
        {
            foreach (ManipulatorParts part in Enum.GetValues(typeof(ManipulatorParts)))
                partAngles[part] = angles.partAngles[part];
            ConfirmRotation();
        }



        void animation_tick(object sender, EventArgs arg)
        {

            List<bool> partOnRightPos = new List<bool>();
            foreach (ManipulatorParts part in Enum.GetValues(typeof(ManipulatorParts)))
            {
                bool onRightPos;
                partAngles[part] = checkedAngle(part, out onRightPos);
                partOnRightPos.Add(onRightPos);
            }

            ConfirmRotation();
            if (partOnRightPos.TrueForAll(b => b))
            {
                Console.WriteLine("ANgimation stoped");
                timer.Stop();
                isAnimated = false;
            }

        }


        private double checkedAngle(ManipulatorParts part, out bool onRightPosition)
        {
            double angle = partAngles[part] + partDeltasToRotate[part];
            onRightPosition = false;

            if (Math.Abs(anglesToSet.partAngles[part]) - Math.Abs(angle) <= 2*Math.Abs(partDeltasToRotate[part]))
            {
                angle = anglesToSet.partAngles[part];
                onRightPosition = true;
            }
            return angle;
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
                    rotatePoint = _jointCubes[3].Bounds.Location;
                    // Полученную точку сдвигаем в цетр меша, получая точку вращения
                    rotatePoint.Offset(1, 1, 1);
                    // Ось вращения
                    rotationAxis = new Vector3D(0, 1, 0);

                    break;

                case ManipulatorParts.MiddleEdge:
                    modelToRotate = parts[ManipulatorParts.MiddleEdge];
                    rotatePoint = _jointCubes[3].Bounds.Location;
                    rotatePoint.Offset(1, 1, 1);
                    rotationAxis = new Vector3D(0, 0, 1);
                    break;

                case ManipulatorParts.TopEdgeBase:
                    modelToRotate = parts[ManipulatorParts.TopEdgeBase];
                    rotatePoint = _jointCubes[2].Bounds.Location;
                    rotatePoint.Offset(1, 1, 1);
                    rotationAxis = new Vector3D(0, 0, 1);
                    break;

                case ManipulatorParts.TopEdge:
                    modelToRotate = parts[ManipulatorParts.TopEdge];
                    rotatePoint = _jointCubes[1].Bounds.Location;
                    rotationAxis = new Vector3D(1, 0, 0);
                    break;

                case ManipulatorParts.CameraBase:
                    modelToRotate = parts[ManipulatorParts.CameraBase];
                    rotatePoint = _jointCubes[0].Bounds.Location;
                    rotatePoint.Offset(1, 1, 1);
                    rotationAxis = new Vector3D(0, 0, 1);
                    break;

                case ManipulatorParts.Camera:
                    modelToRotate = parts[ManipulatorParts.Camera];
                    rotatePoint = _jointCubes[0].Bounds.Location;
                    rotatePoint.Offset(1, 1, 1);
                    rotationAxis = new Vector3D(1, 0, 0);
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

            R = GetRotateTransfofm(ManipulatorParts.TopEdgeBase, partAngles[ManipulatorParts.TopEdgeBase]);
            topEdgeBaseGroup.Children.Add(R);
            topEdgeBaseGroup.Children.Add(middleEdgeGroup);

            R = GetRotateTransfofm(ManipulatorParts.TopEdge, partAngles[ManipulatorParts.TopEdge]);
            topEdgeGroup.Children.Add(R);
            topEdgeGroup.Children.Add(topEdgeBaseGroup);


            R = GetRotateTransfofm(ManipulatorParts.CameraBase, partAngles[ManipulatorParts.CameraBase]);
            cameraBaseGroup.Children.Add(R);
            cameraBaseGroup.Children.Add(topEdgeGroup);

            R = GetRotateTransfofm(ManipulatorParts.Camera, partAngles[ManipulatorParts.Camera]);
            cameraGroup.Children.Add(R);
            cameraGroup.Children.Add(cameraBaseGroup);


            CalculateCameraDirection(cameraGroup);

            parts[ManipulatorParts.Camera].RotateTransform3D(cameraGroup);
            parts[ManipulatorParts.CameraBase].RotateTransform3D(cameraBaseGroup);
            parts[ManipulatorParts.TopEdge].RotateTransform3D(topEdgeGroup);
            parts[ManipulatorParts.TopEdgeBase].RotateTransform3D(topEdgeBaseGroup);
            parts[ManipulatorParts.MiddleEdge].RotateTransform3D(middleEdgeGroup);
            parts[ManipulatorParts.Table].RotateTransform3D(tableGroup);
        }

        Model3D IManipulatorModel.GetManipulatorModel()
        {
            return _manipulator3DModel;
        }


        Model3D IManipulatorModel.GetManipulatorPart(ManipulatorParts part)
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

    }
}

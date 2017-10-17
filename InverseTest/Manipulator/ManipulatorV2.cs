using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using InverseTest.Manipulator;
using System.Windows.Media.Animation;
using System.Windows;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;


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
        private Dictionary<ManipulatorParts, RotateTransform3D> partAngles = new Dictionary<ManipulatorParts, RotateTransform3D>();

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

                partAngles[ManipulatorParts.Table] = new RotateTransform3D();
                partAngles[ManipulatorParts.MiddleEdge] = new RotateTransform3D();
                partAngles[ManipulatorParts.TopEdgeBase] = new RotateTransform3D();
                partAngles[ManipulatorParts.TopEdge] = new RotateTransform3D();
                partAngles[ManipulatorParts.CameraBase] = new RotateTransform3D();
                partAngles[ManipulatorParts.Camera] = new RotateTransform3D();
               


                // Заполняем список мешей в точках сочленений
                _jointCubes.Add(machine3DModel.Children[17]);
                _jointCubes.Add(machine3DModel.Children[18]);
                _jointCubes.Add(machine3DModel.Children[19]);
                _jointCubes.Add(machine3DModel.Children[20]);


                _manipulator3DModel.Children = _edges;
                // _manipulator3DModel = machine3DModel;

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



        /// <summary>
        /// Метод вращает определенное ребро манипулятора на заданный угол вращения
        /// </summary>
        /// <param name="part">Часть манипулятора, которую необходимо повернуть</param>
        /// <param name="angle">Угол поворота части манипулятора</param>
        /// 
        public RotateTransform3D GetRotateTransfofm(ManipulatorParts part, double angle, out Point3D rotatePoint,out  IManipulatorPart modelToRotate, out Vector3D rotationAxis)
        {

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
            return DEFAULT_CAMERA_DIRECTION;
        }

        public void RotatePart(ManipulatorParts part, double angle)
        {
            Point3D rotatePoint;
            IManipulatorPart modelToRotate;
            Vector3D rotationAxis;

            RotateTransform3D rotate = GetRotateTransfofm(part, angle, out rotatePoint, out modelToRotate, out rotationAxis);
            partAngles[part] = rotate;

            ConfirmRotation();
            CalculateCameraDirection(rotate);
        }

        /// <summary>
        /// Вычисляет текущее направление камеры манипулятора
        /// </summary>
        /// <param name="rotate"></param>
        private void CalculateCameraDirection(RotateTransform3D rotate)
        {
            Matrix3D m = Matrix3D.Identity;
            AxisAngleRotation3D angleRotation = (AxisAngleRotation3D)rotate.Rotation;
            Quaternion horizQuaternion = new Quaternion(angleRotation.Axis, angleRotation.Angle);
            m.Rotate(horizQuaternion);
            currentCameraDirection = m.Transform(DEFAULT_CAMERA_DIRECTION);
        }

        private void ConfirmRotation()
        {
            Transform3DGroup tableGroup = new Transform3DGroup();
            Transform3DGroup middleEdgeGroup = new Transform3DGroup();
            Transform3DGroup topEdgeBaseGroup = new Transform3DGroup();
            Transform3DGroup topEdgeGroup = new Transform3DGroup();
            Transform3DGroup cameraBaseGroup = new Transform3DGroup();
            Transform3DGroup cameraGroup= new Transform3DGroup();

            
            RotateTransform3D R = partAngles[ManipulatorParts.Table];
            tableGroup.Children.Add(R);

            R = partAngles[ManipulatorParts.MiddleEdge];
            middleEdgeGroup.Children.Add(R);
            middleEdgeGroup.Children.Add(tableGroup);

            R = partAngles[ManipulatorParts.TopEdgeBase];
            topEdgeBaseGroup.Children.Add(R);
            topEdgeBaseGroup.Children.Add(middleEdgeGroup);

            R = partAngles[ManipulatorParts.TopEdge];
            topEdgeGroup.Children.Add(R);
            topEdgeGroup.Children.Add(topEdgeBaseGroup);


            R = partAngles[ManipulatorParts.CameraBase];
            cameraBaseGroup.Children.Add(R);
            cameraBaseGroup.Children.Add(topEdgeGroup);

            R = partAngles[ManipulatorParts.Camera];
            cameraGroup.Children.Add(R);
            cameraGroup.Children.Add(cameraBaseGroup);

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

    }
}

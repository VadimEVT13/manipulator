using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using InverseTest.Manipulator;

namespace InverseTest
{
    /// <summary>
    /// 
    /// </summary>
    public class ManipulatorV2: IManipulatorModel
    {
        // поле для хранения 3D модели манипулятора
        private readonly Model3DGroup _manipulator3DModel = new Model3DGroup();

        //public ManipMathModel ManipMathModel { get; set; }
        public JointsChain ManipMathModel { get; set; }

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
        public ManipulatorV2(string file)
        {   
            try
            {
                // Импортируем модель и собираем её составляющие
                Model3DGroup machine3DModel = new ModelImporter().Load(file);

                // Камера
                Model3DGroup cam3DModel = new Model3DGroup();
                cam3DModel.Children.Add(machine3DModel.Children[15]);
                cam3DModel.Children.Add(machine3DModel.Children[1]);

                // Стойка камеры
                Model3DGroup camEdge3DModel = new Model3DGroup();
                camEdge3DModel.Children.Add(machine3DModel.Children[11]);
                camEdge3DModel.Children.Add(machine3DModel.Children[12]);
                
                // Верхнее вращающееся ребро
                Model3DGroup upperTurningEdge3DModel = new Model3DGroup();
                upperTurningEdge3DModel.Children.Add(machine3DModel.Children[9]);
                upperTurningEdge3DModel.Children.Add(machine3DModel.Children[10]);
                upperTurningEdge3DModel.Children.Add(machine3DModel.Children[2]);

                // Верхнее поворотное ребро
                Model3DGroup upperEdge3DModel = new Model3DGroup();
                upperEdge3DModel.Children.Add(machine3DModel.Children[3]);
                upperEdge3DModel.Children.Add(machine3DModel.Children[4]);
                upperEdge3DModel.Children.Add(machine3DModel.Children[5]);
                

                // Стойка
                Model3DGroup middleEdge3DModel = new Model3DGroup();
                middleEdge3DModel.Children.Add(machine3DModel.Children[13]);
                middleEdge3DModel.Children.Add(machine3DModel.Children[14]);
                middleEdge3DModel.Children.Add(machine3DModel.Children[0]);

                // Столик
                Model3DGroup table3DModel = new Model3DGroup();
                table3DModel.Children.Add(machine3DModel.Children[6]);
                table3DModel.Children.Add(machine3DModel.Children[7]);
                table3DModel.Children.Add(machine3DModel.Children[8]);
                
                // Основание
                Model3DGroup base3DModel = new Model3DGroup();
                base3DModel.Children.Add(machine3DModel.Children[16]);
                base3DModel.Children.Add(machine3DModel.Children[17]);
                base3DModel.Children.Add(machine3DModel.Children[18]);
                base3DModel.Children.Add(machine3DModel.Children[19]);
                
                // Строим направленный граф модели
                camEdge3DModel.Children.Add(cam3DModel);
                upperTurningEdge3DModel.Children.Add(camEdge3DModel);
                upperEdge3DModel.Children.Add(upperTurningEdge3DModel);
                middleEdge3DModel.Children.Add(upperEdge3DModel);
                table3DModel.Children.Add(middleEdge3DModel);
                _manipulator3DModel.Children.Add(table3DModel);
                _manipulator3DModel.Children.Add(base3DModel);

                // Заполняем список мешей
                _edges.Add(table3DModel);
                _edges.Add(middleEdge3DModel);
                _edges.Add(upperEdge3DModel);
                _edges.Add(upperTurningEdge3DModel);
                _edges.Add(camEdge3DModel);
                _edges.Add(cam3DModel);
                
                // Заполняем список мешей в точках сочленений
                _jointCubes.Add(machine3DModel.Children[0]);
                _jointCubes.Add(machine3DModel.Children[2]);
                _jointCubes.Add(machine3DModel.Children[1]);

                ResetMathModel();
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
        public void RotatePart(ManipulatorParts part, double angle)
        {
            Point3D rotatePoint;
            Model3D modelToRotate;
            Vector3D rotationAxis;
            
            // для каждой части манипулятора необходимо задать свои точки поворота, ось вращения и угол вращения
            switch (part)
            {
                case ManipulatorParts.Table:
                    // Ребро, которое необходимо повернуть
                    modelToRotate = _edges[0];
                    // Берем меш, находящийся в точке поворота ребра, получяем его позицию (она на одном из углов меша)
                    rotatePoint = _jointCubes[0].Bounds.Location;
                    // Полученную точку сдвигаем в цетр меша, получая точку вращения
                    rotatePoint.Offset(1,1,1);
                    // Ось вращения
                    rotationAxis = new Vector3D(0, 1, 0);
                    break;
                
                case ManipulatorParts.MiddleEdge:
                    modelToRotate = _edges[1];
                    rotatePoint = _jointCubes[0].Bounds.Location;
                    rotatePoint.Offset(1, 1, 1);
                    rotationAxis = new Vector3D(0, 0, 1);
                    break;

                case ManipulatorParts.TopEdgeBase:
                    modelToRotate = _edges[2];
                    rotatePoint = _jointCubes[1].Bounds.Location;
                    rotatePoint.Offset(1, 1, 1);
                    rotationAxis = new Vector3D(0, 0, 1);
                    break;

                case ManipulatorParts.TopEdge:
                    modelToRotate = _edges[3];
                    rotatePoint = _jointCubes[1].Bounds.Location;
                    rotatePoint.Offset(1, 1, 1);
                    rotationAxis = new Vector3D(1, 0, 0);
                    break;

                case ManipulatorParts.CameraBase:
                    modelToRotate = _edges[4];
                    rotatePoint = _jointCubes[2].Bounds.Location;
                    rotatePoint.Offset(1, 1, 1);
                    rotationAxis = new Vector3D(0, 0, 1);
                    break;

                case ManipulatorParts.Camera:
                    modelToRotate = _edges[5];
                    rotatePoint = _jointCubes[2].Bounds.Location;
                    rotatePoint.Offset(1, 1, 1);
                    rotationAxis = new Vector3D(1, 0, 0);
                    break;

                default:
                    throw new InvalidEnumArgumentException();
            }
            // Поворачиваем ребро в соответствии с заданными параметрами
            Transform3D rotate = new RotateTransform3D(new AxisAngleRotation3D(rotationAxis, angle), rotatePoint);
            modelToRotate.Transform = rotate;
            

        }

        Model3D IManipulatorModel.GetManipulatorModel()
        {
            return _manipulator3DModel;
        }


        /*public void ResetMathModel()
        {
            Point3D point0 = _jointCubes[0].Bounds.Location;
            point0.Offset(1, 1, 1);
            Point3D point1 = _jointCubes[1].Bounds.Location;
            point1.Offset(1, 1, 1);
            Point3D point2 = _jointCubes[2].Bounds.Location;
            point2.Offset(1, 1, 1);
            Point3D point3 = new Point3D { X = point2.X - 50, Y = point2.Y, Z = point2.Z };

            Joint joint0 = new Joint(point0, point1)
            {
                TurnAxisVector = new Vector3D(0, 1, 0),
                RotateAxisVector = new Vector3D(-1, 0, 0),

                TurnPlaneVector = new Vector3D(0, 0, 1),
                RotatePlaneVector = new Vector3D(0, 1, 0)
            };


            Joint joint1 = new Joint(point1, point2)
            {
                TurnAxisVector = new Vector3D(-1, 0, 0),
                RotateAxisVector = new Vector3D(0, 1, 0),

                TurnPlaneVector = new Vector3D(0, 0, 1),
                RotatePlaneVector = new Vector3D(1, 0, 0)
            };
            

            Joint joint2 = new Joint(point2, point3)
            {
                TurnAxisVector = new Vector3D(-1, 0, 0),
                RotateAxisVector = new Vector3D(0, 1, 0),

                TurnPlaneVector = new Vector3D(0, 0, 1),
                RotatePlaneVector = new Vector3D(1, 0, 0)
            };


            ManipMathModel = new ManipMathModel(new[] { joint0, joint1, joint2 });
        }*/

        public void ResetMathModel()
        {
            Point3D point0 = _jointCubes[0].Bounds.Location;
            point0.Offset(1, 1, 1);
            Point3D point1 = _jointCubes[1].Bounds.Location;
            point1.Offset(1, 1, 1);
            Point3D point2 = _jointCubes[2].Bounds.Location;
            point2.Offset(1, 1, 1);
            Point3D point3 = new Point3D { X = point2.X + 50, Y = point2.Y, Z = point2.Z };

            Vector3D turnPlaneNormal = new Vector3D(0,0,1);

            NewJoint joint0 = new NewJoint(point0, point1, turnPlaneNormal, new double[] {90, 90});


            NewJoint joint1 = new NewJoint(point1, point2, turnPlaneNormal, new double[] {90, 0});



            NewJoint joint2 = new NewJoint(point2, point3, turnPlaneNormal, new double[] {90, 90});
            


            ManipMathModel = new JointsChain(new[] { joint0, joint1, joint2 });
        }

    }
}

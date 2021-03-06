﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelixToolkit.Wpf;
using InverseTest.Manipulator;
using InverseTest.GUI.Model;
using InverseTest.Detail;
using InverseTest.Collision.Model;
using InverseTest.Frame;
using InverseTest.GUI.Models;

namespace InverseTest.GUI.Views
{
    /// <summary>
    /// Класс визуализизации манипулятора
    /// </summary>
    public partial class ManipulatorVisualizer : UserControl
    {
        private PerspectiveCamera cameraFromManipulator;
        private PerspectiveCamera cameraFromPortal;
        private OrthographicCamera cam2DTop;
        private OrthographicCamera cam2DFront;
        private OrthographicCamera cam2DRight;
        private PerspectiveCamera cam3D;

        private DetectorFrame detectorFrame;
        private ManipulatorV2 manipulator;
        private MovementPoint scanPoint;
        
        private ModelMoverAboveSurf mover;
        private ModelMover manipulatorMover;

        private static int DISTANCE_TO_CAMERA = 1000;
        private static int CAMERA_BORDER_OFFSET = 50;

        public ManipulatorVisualizer()
        {
            InitializeComponent();

            //TODO: Подробнее разобраться с настройками камер: свойство Near/FarPlaneDistance
            //TODO: Запилить настройки камеры зависящими от размеров модели манипулятора

            
            // Настраиваем камеры
            cam2DTop = new OrthographicCamera
            {
                Position = new Point3D(450, 1800, 0),
                Width = 900,
                LookDirection = new Vector3D(0, -1, 0),
                UpDirection = new Vector3D(1, 0, 0)
            };
            ViewPort2DTop.Camera = cam2DTop;



            cam2DFront = new OrthographicCamera
            {
                Position = new Point3D(500, 0, 0),
                Width = 800,
                LookDirection = new Vector3D(-1, 0, 0)
            };
            ViewPort2DFront.Camera = cam2DFront;



            cam2DRight = new OrthographicCamera
            {
                Position = new Point3D(450, 300, 1200),
                Width = 1500,
                LookDirection = new Vector3D(0, 0, -1)
            };
            ViewPort2DRight.Camera = cam2DRight;
            
            cam3D = CameraHelper.CreateDefaultCamera();
            cam3D.FieldOfView = 61;
            cam3D.LookDirection = new Vector3D(-1, -1, -1);
            cam3D.Position = new Point3D(1000, 1000, 1000);
            cam3D.UpDirection = new Vector3D(0, 1, 0);
            cam3D.NearPlaneDistance = 0.1;
            cam3D.FarPlaneDistance = double.PositiveInfinity;

            ViewPort3D.Camera = cam3D;
            ViewPort3D.CameraMode = CameraMode.Inspect;
            ViewPort3D.CameraRotationMode = CameraRotationMode.Turntable;
            ViewPort3D.DefaultCamera = cam3D;
            ViewPort3D.RotateAroundMouseDownPoint = false;
            ViewPort3D.ModelUpDirection = new Vector3D(0, 1, 0);

            cameraFromPortal = new PerspectiveCamera();
            cameraFromPortal.FieldOfView = 60;
            ViewPortDetectorScreenCam.Camera = cameraFromPortal;

            cameraFromManipulator = new PerspectiveCamera();
            cameraFromManipulator.FieldOfView = 60;
            ViewPortManipulatorCam.Camera = cameraFromManipulator;

            // Настраиваем освещение
            Light ambientLight = new AmbientLight(Colors.White);
            Light ambientLight2 = new AmbientLight(Colors.DarkGray);
            DirectionalLight pointLight = new DirectionalLight();
            pointLight.Direction = new Vector3D(3, -3, 0);
            pointLight.Color = Colors.White;

            Visual3D lightVisualTop = new LightVisual3D() { Content = ambientLight };
            Visual3D lightVisualRight = new LightVisual3D() { Content = ambientLight };
            Visual3D lightVisualFront = new LightVisual3D() { Content = ambientLight };
            Visual3D lightVisual3D = new LightVisual3D() { Content = ambientLight2 };
            Visual3D pointLightVisual3D = new LightVisual3D() { Content = pointLight };
            Visual3D lightDetectorView = new LightVisual3D() { Content = ambientLight };
            Visual3D lightManipulatorVisual3D = new LightVisual3D() { Content = ambientLight };

            ViewPort2DTop.Children.Add(lightVisualTop);
            ViewPort2DRight.Children.Add(lightVisualRight);
            ViewPort2DFront.Children.Add(lightVisualFront);
            ViewPort3D.Children.Add(lightVisual3D);
            ViewPort3D.Children.Add(pointLightVisual3D);
            ViewPortDetectorScreenCam.Children.Add(lightDetectorView);
            ViewPortManipulatorCam.Children.Add(lightManipulatorVisual3D);
        }

        /// <summary>
        /// Регистрирует модели во вьюпортах, предоставляемых текущим компонентом
        /// </summary>
        /// <param name="model">Модель, которыу необходимо отобразить</param>
        public void RegisterScene(Model3D model)
        {
            AddModel(model);
        }

        public void setCameras(Model3D model)
        {
            Rect3D bound = model.Bounds;
            cam2DTop.Width = model.Bounds.SizeZ;
            cam2DTop.Position = new Point3D(0, 2 * DISTANCE_TO_CAMERA, 0);

            cam2DFront.Width = model.Bounds.SizeZ;
            cam2DFront.Position = new Point3D(DISTANCE_TO_CAMERA, model.Bounds.SizeY / 2, 0);

            cam2DRight.Width = model.Bounds.SizeX;
            cam2DRight.Position = new Point3D(0, bound.Y + bound.SizeY / 2, DISTANCE_TO_CAMERA);

            cam3D.LookDirection = new Vector3D(-DISTANCE_TO_CAMERA / 5, -DISTANCE_TO_CAMERA / 5, -DISTANCE_TO_CAMERA / 5);
            cam3D.Position = new Point3D(DISTANCE_TO_CAMERA / 5, DISTANCE_TO_CAMERA / 5, DISTANCE_TO_CAMERA / 5);
        }

        /// <summary>
        /// Добавляте контур детали на вид с камеры манипулятора
        /// </summary>
        /// <param name="detail"></param>
        public void AddCountur(DetailModel detail)
        {
            RemoveModelFromViewPort(ViewPortManipulatorCam, detail.GetModel());
            ViewPortManipulatorCam.Children.Add(detail.counturVisual);
        }

        public void DeleteCountur(DetailModel detail)
        {
            RemoveModelFromViewPort(ViewPortManipulatorCam, (detail.counturVisual as ModelVisual3D).Content);
            ModelVisual3D modelVisual = new ModelVisual3D() { Content = detail.GetModel() };
            ViewPortManipulatorCam.Children.Add(modelVisual);
        }
        
        public void setManipulatorModel(ManipulatorV2 manipulatorModel, ManipulatorVisual manipulatorVisual)
        {
            this.manipulator = manipulatorModel;
            Model3D camera = manipulatorModel.GetManipulatorPart(ManipulatorParts.Camera);
            Point3D position = new Point3D(camera.Bounds.Location.X + camera.Bounds.SizeX,
                camera.Bounds.Location.Y + camera.Bounds.SizeY / 2,
                camera.Bounds.Location.Z + camera.Bounds.SizeZ / 2);

            camera.Changed += ManipulatorChangedCam;

            cameraFromManipulator.Position = position;
            cameraFromManipulator.LookDirection = manipulatorModel.GetCameraDirection();

            AddVisuals(manipulatorVisual.Visuals);
        }

        public void AddVisuals(List<MainVisual> visuals)
        {
            foreach (MainVisual v in visuals)
            {
                AddVisual(v);
            }
        }

        public void AddVisual(MainVisual visual)
        {
            ViewPortManipulatorCam.Children.Add(visual.CamManip);
            ViewPortDetectorScreenCam.Children.Add(visual.CamPortal);
            ViewPort2DTop.Children.Add(visual.Top);
            ViewPort2DFront.Children.Add(visual.Front);
            ViewPort2DRight.Children.Add(visual.Right);
            ViewPort3D.Children.Add(visual._3D);
        }

        public void RemoveVisual(MainVisual visual)
        {
            RemoveVisualFromViewPort(ViewPort2DTop, visual.Top);
            RemoveVisualFromViewPort(ViewPort2DFront, visual.Front);
            RemoveVisualFromViewPort(ViewPort2DRight, visual.Right);
            RemoveVisualFromViewPort(ViewPort3D, visual._3D);
            RemoveVisualFromViewPort(ViewPortDetectorScreenCam, visual.CamPortal);
            RemoveVisualFromViewPort(ViewPortManipulatorCam, visual.CamManip);
        }
        
        private void ManipulatorChangedCam(object sender, EventArgs e)
        {
            Model3D cameraModel = manipulator.GetManipulatorPart(ManipulatorParts.Camera);
            Rect3D camBounds = cameraModel.Bounds;
            Point3D cameraPosition = manipulator.GetCameraPosition();
            cameraFromManipulator.Position = cameraPosition;
            cameraFromManipulator.LookDirection = manipulator.GetCameraDirection();
        }

        /// <summary>
        /// Устанавливает модель портала во ViewPort-ах и камеру 
        /// </summary>
        /// <param name="detectorFrame"></param>
        public void SetDetectFrameModel(DetectorFrame detectorFrame, DetectorFrameVisual portalVisual)
        {
            this.detectorFrame = detectorFrame;
            Model3D camera = detectorFrame.GetDetectorFramePart(DetectorFrame.Parts.Screen);
            camera.Changed += ChangedCam;
            cameraFromPortal.LookDirection = detectorFrame.GetScreenDirection();
            cameraFromPortal.Position = detectorFrame.GetCameraPosition();

            AddVisuals(portalVisual.Visuals);
        }

        public void SetManipulatorPoint(MovementPoint point)
        {
            manipulatorMover = new ModelMover(point);
            manipulatorMover.ModelToDetect =point.GetModel();
            AddListeners(manipulatorMover);
            AddModelWithoutCamView(point.GetModel());
        }

        /// <summary>
        /// Устанавливает модель точки сканирования и навешивает оброботчики её передвижения 
        /// </summary>
        /// <param name="scanPoint"></param>
        public void SetPoint(MovementPoint scanPoint, Model3D model)
        {
            this.mover = new ModelMoverAboveSurf(scanPoint, model);
            this.mover.ModelToDetect = scanPoint.GetModel();
            AddListeners(mover);
            AddModel(scanPoint.GetModel());
        }

        private void AddListeners(IModelMover mover)
        {
            ViewPort2DFront.MouseDown += mover.OnMouseDown;
            ViewPort2DFront.MouseUp += mover.OnMouseUp;
            ViewPort2DFront.MouseMove += mover.OnMouseMove;

            ViewPort2DRight.MouseDown += mover.OnMouseDown;
            ViewPort2DRight.MouseUp += mover.OnMouseUp;
            ViewPort2DRight.MouseMove += mover.OnMouseMove;

            ViewPort2DTop.MouseDown += mover.OnMouseDown;
            ViewPort2DTop.MouseUp += mover.OnMouseUp;
            ViewPort2DTop.MouseMove += mover.OnMouseMove;

            ViewPort3D.MouseDown += mover.OnMouseDown;
            ViewPort3D.MouseUp += mover.OnMouseUp;
            ViewPort3D.MouseMove += mover.OnMouseMove;
        }

        /// <summary>
        /// Добавляет модель на все виды кроме видов из камер
        /// </summary>
        /// <param name="model"></param>
        public void AddModelWithoutCamView(Model3D model)
        {
            ModelVisual3D topViewModel = new ModelVisual3D() { Content = model };
            ModelVisual3D frontViewModel = new ModelVisual3D() { Content = model };
            ModelVisual3D rightViewModel = new ModelVisual3D() { Content = model };
            ModelVisual3D fullViewModel = new ModelVisual3D() { Content = model };

            ViewPort2DTop.Children.Add(topViewModel);
            ViewPort2DFront.Children.Add(frontViewModel);
            ViewPort2DRight.Children.Add(rightViewModel);
            ViewPort3D.Children.Add(fullViewModel);
        }

        /// <summary>
        /// EventHandler. Обработчик изменения положения экрана портала. При изменении положения экрана изменяет положение камеры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChangedCam(object sender, EventArgs e)
        {
            cameraFromPortal.LookDirection = detectorFrame.GetScreenDirection();
            cameraFromPortal.Position = detectorFrame.GetCameraPosition();
        }


        /// <summary>
        /// Добавляет представления модели во все viewports
        /// </summary>
        /// <param name="model"></param>
        public void AddModel(Model3D model)
        {
            ModelVisual3D topViewModel = new ModelVisual3D() { Content = model };
            ModelVisual3D frontViewModel = new ModelVisual3D() { Content = model };
            ModelVisual3D rightViewModel = new ModelVisual3D() { Content = model };
            ModelVisual3D fullViewModel = new ModelVisual3D() { Content = model };
            ModelVisual3D cameraManipulatorModel = new ModelVisual3D() { Content = model };
            ModelVisual3D detectorScreenCamModel = new ModelVisual3D() { Content = model };

            ViewPort2DTop.Children.Add(topViewModel);
            ViewPort2DFront.Children.Add(frontViewModel);
            ViewPort2DRight.Children.Add(rightViewModel);
            ViewPort3D.Children.Add(fullViewModel);
            ViewPortDetectorScreenCam.Children.Add(cameraManipulatorModel);
            ViewPortManipulatorCam.Children.Add(detectorScreenCamModel);
        }
        

        /// <summary>
        /// Удаляет модель из всех viewports
        /// </summary>
        /// <param name="model"></param>
        public void RemoveModel(Model3D model)
        {
            RemoveModelFromViewPort(ViewPort2DTop, model);
            RemoveModelFromViewPort(ViewPort2DFront, model);
            RemoveModelFromViewPort(ViewPort2DRight, model);
            RemoveModelFromViewPort(ViewPort3D, model);
            RemoveModelFromViewPort(ViewPortDetectorScreenCam, model);
            RemoveModelFromViewPort(ViewPortManipulatorCam, model);
        }


        /// <summary>
        /// Удаляет все представления модели из указанного Viewport-а
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="model"></param>
        private void RemoveModelFromViewPort(HelixViewport3D viewport, Model3D model)
        {

            var childrens = viewport.Children;
            foreach (Visual3D visual3D in childrens)
            {
                if (visual3D is ModelVisual3D)
                {
                    ModelVisual3D modelVisual3D = visual3D as ModelVisual3D;
                    if (modelVisual3D.Content == model)
                    {
                        viewport.Children.Remove(modelVisual3D);
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Возвращает true если model добавлена в каком либо viewPort
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsModelExist(Model3D model)
        {
            var res = false;
            res |= checkViewPort(ViewPort2DFront, model);
            res |= checkViewPort(ViewPort2DRight, model);
            res |= checkViewPort(ViewPort3D, model);
            res |= checkViewPort(ViewPortDetectorScreenCam, model);
            res |= checkViewPort(ViewPortManipulatorCam, model);
            return res;
        }



        /// <summary>
        /// Проверяет если ли входная модель в данном viewPort
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool checkViewPort(HelixViewport3D viewport,Model3D model)
        {

            foreach(Visual3D v in viewport.Children)
            {
                if(v is ModelVisual3D mv)
                {
                    if (mv.Content == model)
                        return true;
                }
            }

            return false;
        }



        private void RemoveVisualFromViewPort(HelixViewport3D viewport, Visual3D visual)
        {
            foreach (Visual3D v in viewport.Children)
            {
                if (v.Equals(visual))
                {
                    viewport.Children.Remove(v);
                    return;
                }
            }
        }

        public Visual3D FindVisual(HelixViewport3D viewport, Model3D model)
        {
            ModelVisual3D modelToRemove = null;

            foreach (Visual3D visual3D in viewport.Children)
            {
                if (visual3D is ModelVisual3D)
                {
                    ModelVisual3D modelVisual3D = visual3D as ModelVisual3D;
                    if (modelVisual3D.Content == model)
                    {
                        modelToRemove = modelVisual3D;
                    }
                }
            }

            return modelToRemove;
        }


        public void showBorders(Model3D frame, int i)
        {
                Model3D part = frame;//.GetDetectorFramePart(DetectorFrame.Parts.Screen);
                Rect3D rect = part.Bounds;


                BoxVisual3D rectagnle3D = new BoxVisual3D()
                {
                    Center = new Point3D(rect.Location.X + rect.SizeX / 2, rect.Location.Y + rect.SizeY / 2, rect.Location.Z + rect.SizeZ / 2),
                    Fill = Brushes.DarkBlue,
                    Width = rect.SizeY,
                    Length = rect.SizeX,
                    Height = rect.SizeZ
                };



                // ViewPort2DFront.Children.Add(rectagnle3D);
                //  ViewPort2DTop.Children.Add(rectagnle3D);
                ///   ViewPort2DRight.Children.Add(rectagnle3D);
                ViewPort3D.Children.Add(rectagnle3D);
            }

        public void showBordersPortal(ManipulatorSnapshot frame, int i)
        {
                foreach (PartShape s in frame.parts)
                {

                    Rect3D rect = s.Bounds;
                    BoxVisual3D rectagnle3D = new BoxVisual3D()
                    {
                        Center = new Point3D(rect.Location.X + rect.SizeX / 2, rect.Location.Y + rect.SizeY / 2, rect.Location.Z + rect.SizeZ / 2),
                        Fill = Brushes.DarkBlue,
                        Width = rect.SizeY,
                        Length = rect.SizeX,
                        Height = rect.SizeZ

                    };



                    // ViewPort2DFront.Children.Add(rectagnle3D);
                    //  ViewPort2DTop.Children.Add(rectagnle3D);
                    ///   ViewPort2DRight.Children.Add(rectagnle3D);
                    ViewPort3D.Children.Add(rectagnle3D);
                }
        }

        public void ShowPoints(Point3D[] points)
        {
            ShowPointsInViewport(points, ViewPort3D);
            ShowPointsInViewport(points, ViewPort2DFront);
            ShowPointsInViewport(points, ViewPort2DRight);
            ShowPointsInViewport(points, ViewPort2DTop);
        }

        private void ShowPointsInViewport(Point3D[] points, HelixViewport3D viewPort)
        {
            LinesVisual3D modelToRemove = null;
            foreach (Visual3D visual3D in viewPort.Children)
            {
                if (visual3D is LinesVisual3D)
                {
                    LinesVisual3D modelVisual3D = visual3D as LinesVisual3D;
                    modelToRemove = modelVisual3D;
                }
            }
            viewPort.Children.Remove(modelToRemove);

            Point3DCollection pointsCollection = new Point3DCollection()
            {
                points[0], points[1], points[1], points[2], points[2], points[3]
            };

            LinesVisual3D linesVisual = new LinesVisual3D();
            linesVisual.Thickness = 5;
            linesVisual.Points = pointsCollection;

            viewPort.Children.Add(linesVisual);
        }

        public void RemoveAllMathModels()
        {
            RemoveMathModel(ViewPort2DFront);
            RemoveMathModel(ViewPort2DTop);
            RemoveMathModel(ViewPort2DRight);
            RemoveMathModel(ViewPort3D);
        }

        private void RemoveMathModel(HelixViewport3D viewPort)
        {
            LinesVisual3D modelToRemove = null;
            foreach (Visual3D visual3D in viewPort.Children)
            {
                if (visual3D is LinesVisual3D)
                {
                    LinesVisual3D modelVisual3D = visual3D as LinesVisual3D;
                    modelToRemove = modelVisual3D;
                }
            }
            viewPort.Children.Remove(modelToRemove);
        }

        public HelixViewport3D getViewPort()
        {
            return this.ViewPort2DFront;
        }
    }
}

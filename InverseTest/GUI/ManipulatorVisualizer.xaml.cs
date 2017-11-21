using System;
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

namespace InverseTest.GUI
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

        private IDetectorFrame detectorFrame;
        private IManipulatorModel manipulator;
        private IMovementPoint scanPoint;


        private ModelMover mover;
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
                Position = new Point3D(1200,300, 0),
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
         

            cam3D = new PerspectiveCamera
            {
                Position = new Point3D(1300, 900, 800),
                FieldOfView = 45,
                LookDirection = new Vector3D(-1, -1, -1)
            };
            ViewPort3D.Camera = cam3D;
          

            cameraFromPortal = new PerspectiveCamera();
            cameraFromPortal.FieldOfView = 45;
            ViewPortDetectorScreenCam.Camera = cameraFromPortal;

            cameraFromManipulator = new PerspectiveCamera();
            cameraFromManipulator.FieldOfView = 45;
            ViewPortManipulatorCam.Camera = cameraFromManipulator;
            
            // Настраиваем освещение
            Light ambientLight = new AmbientLight(Colors.White);
            Light ambientLight2 = new AmbientLight(Colors.DarkGray);
            DirectionalLight pointLight = new DirectionalLight();
            //Colors.White, new Vector3D(-3, -3, -3)
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
            cam2DFront.Position = new Point3D(DISTANCE_TO_CAMERA,model.Bounds.SizeY/2, 0);

            cam2DRight.Width = model.Bounds.SizeX;
            cam2DRight.Position = new Point3D(0, bound.Y + bound.SizeY / 2, DISTANCE_TO_CAMERA);

            cam3D.LookDirection = new Vector3D(-1, -1, -1);
            cam3D.Position = new Point3D(DISTANCE_TO_CAMERA/3,DISTANCE_TO_CAMERA/3 ,DISTANCE_TO_CAMERA /3);
        }

      
        private Model3D createSmallCube(Point3D point)
        {
            double length = 0.2;
            MeshGeometry3D boxMesh = new MeshGeometry3D();
            boxMesh.Positions = new Point3DCollection()
                {
                    new Point3D(point.X, point.Y, point.Z),
                    new Point3D(point.X + length, point.Y, point.Z),
                    new Point3D(point.X, point.Y + length, point.Z),
                    new Point3D(point.X+ length, point.Y + length, point.Z),
                    new Point3D(point.X, point.Y, point.Z + length),
                    new Point3D(point.X+ length, point.Y, point.Z + length),
                    new Point3D(point.X, point.Y+ length, point.Z+ length),
                    new Point3D(point.X+ length, point.Y + length, point.Z + length)
                };

            boxMesh.TriangleIndices = new Int32Collection() { 2, 3, 1, 2, 1, 0, 7, 1, 3, 7, 5, 1, 6, 5, 7, 6, 4, 5, 6, 2, 0, 6, 0, 4, 2, 7, 3, 2, 6, 7, 0, 1, 5, 0, 5, 4 };
            GeometryModel3D boxGeom = new GeometryModel3D();
            boxGeom.Geometry = boxMesh;
            DiffuseMaterial mat = new DiffuseMaterial(new SolidColorBrush(Colors.Red));
            boxGeom.Material = mat;

            return boxGeom;
        }


        public void setManipulatorModel(IManipulatorModel manipulatorModel)
        {
            this.manipulator = manipulatorModel;
            Model3D camera = manipulatorModel.GetManipulatorPart(ManipulatorV2.ManipulatorParts.Camera);
            Point3D position = new Point3D(camera.Bounds.Location.X + camera.Bounds.SizeX,
                camera.Bounds.Location.Y + camera.Bounds.SizeY /2 ,
                camera.Bounds.Location.Z + camera.Bounds.SizeZ / 2);

            camera.Changed += ManipulatorChangedCam;

            cameraFromManipulator.Position = position;
            cameraFromManipulator.LookDirection = manipulatorModel.GetCameraDirection();

            AddModel(manipulatorModel.GetManipulatorModel());
            
        }

        private void ManipulatorChangedCam(object sender, EventArgs e)
        {
            Model3D cameraModel = manipulator.GetManipulatorPart(ManipulatorV2.ManipulatorParts.Camera);
            Rect3D camBounds = cameraModel.Bounds;
            Point3D cameraPosition = manipulator.GetCameraPosition();
            cameraFromManipulator.Position = cameraPosition;
            cameraFromManipulator.LookDirection = manipulator.GetCameraDirection();


        }

        /// <summary>
        /// Устанавливает модель портала во ViewPort-ах и камеру 
        /// </summary>
        /// <param name="detectorFrame"></param>
        public void setDetectFrameModel(IDetectorFrame detectorFrame)
        {
            this.detectorFrame = detectorFrame;
            Model3D camera = detectorFrame.GetDetectorFramePart(DetectorFrame.Parts.Screen);
            camera.Changed += ChangedCam;
            cameraFromPortal.LookDirection = detectorFrame.GetScreenDirection();
            cameraFromPortal.Position = detectorFrame.GetCameraPosition();
            AddModel(detectorFrame.GetDetectorFrameModel());
        }



        public void SetManipulatorPoint(IMovementPoint point)
        {
            manipulatorMover = new ModelMover(point);
            Model3D modelGroup = manipulator.GetManipulatorPart(ManipulatorV2.ManipulatorParts.Camera);
            manipulatorMover.modelToDetect = (modelGroup as Model3DGroup).Children[1];
            AddListeners(manipulatorMover);
            AddModel(point.GetModel());
        }

        /// <summary>
        /// Устанавливает модель точки сканирования и навешивает оброботчики её передвижения 
        /// </summary>
        /// <param name="scanPoint"></param>
        public void SetPoint(IMovementPoint scanPoint)
        {
            this.mover = new ModelMover(scanPoint);
            this.mover.modelToDetect = scanPoint.GetModel();
            AddListeners(mover);
            AddModel(scanPoint.GetModel());
        }

        private void AddListeners(ModelMover mover)
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
      


        public void AddConeFromCamera(Model3D model)
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

        public void AddModel(Model3D model)
        {
            
            ModelVisual3D topViewModel = new ModelVisual3D() {Content = model};
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
        


        public void RemoveModel(Model3D model)
        {
            RemoveModelFromViewPort(ViewPort2DTop, model);
            RemoveModelFromViewPort(ViewPort2DFront, model);
            RemoveModelFromViewPort(ViewPort2DRight, model);
            RemoveModelFromViewPort(ViewPort3D, model);
        }

        private void RemoveModelFromViewPort(HelixViewport3D viewport, Model3D model)
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
            viewport.Children.Remove(modelToRemove);
        }


        public void showBordersPortal(IDetectorFrame frame)
        {
            Model3D part = frame.GetDetectorFramePart(DetectorFrame.Parts.VerticalFrame);
            Rect3D rect = part.Bounds;


            BoxVisual3D rectagnle3D = new BoxVisual3D()
            {       
                   Center = new Point3D(rect.Location.X+rect.SizeX/2, rect.Location.Y + rect.SizeY/ 2, rect.Location.Z + rect.SizeZ / 2),
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

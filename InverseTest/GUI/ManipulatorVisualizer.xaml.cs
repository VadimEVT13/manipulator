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

namespace InverseTest.GUI
{
    /// <summary>
    /// Класс визуализизации манипулятора
    /// </summary>
    public partial class ManipulatorVisualizer : UserControl
    {
        public ManipulatorVisualizer()
        {
            InitializeComponent();

            //TODO: Подробнее разобраться с настройками камер: свойство Near/FarPlaneDistance
            //TODO: Запилить настройки камеры зависящими от размеров модели манипулятора

            // Настраиваем камеры
            OrthographicCamera cam2DTop = new OrthographicCamera
            {
                Position = new Point3D(100, 1200, 0),
                Width = 800,
                LookDirection = new Vector3D(0, -1, 0),
                UpDirection = new Vector3D(1, 0, 0)
            };
            ViewPort2DTop.Camera = cam2DTop;
            
            OrthographicCamera cam2DFront = new OrthographicCamera
            {
                Position = new Point3D(1200, 200, 0),
                Width = 800,
                LookDirection = new Vector3D(-1, 0, 0)
            };
            ViewPort2DFront.Camera = cam2DFront;

            OrthographicCamera cam2DRight = new OrthographicCamera
            {
                Position = new Point3D(100, 200, 1200),
                Width = 800,
                LookDirection = new Vector3D(0, 0, -1)
            };
            ViewPort2DRight.Camera = cam2DRight;

            PerspectiveCamera cam3D = new PerspectiveCamera
            {
                Position = new Point3D(800, 900, 800),
                FieldOfView = 45,
                LookDirection = new Vector3D(-3, -3, -3)
            };
            ViewPort3D.Camera = cam3D;

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

            ViewPort2DTop.Children.Add(lightVisualTop);
            ViewPort2DRight.Children.Add(lightVisualRight);
            ViewPort2DFront.Children.Add(lightVisualFront);
            ViewPort3D.Children.Add(lightVisual3D);
            ViewPort3D.Children.Add(pointLightVisual3D);
        }

        /// <summary>
        /// Регистрирует манипулятор во вьюпортах, предоставляемых текущим компонентом
        /// </summary>
        /// <param name="manipulator">Манипулятор, который необходимо отобразить</param>
        public void RegisterManipulator(IManipulatorModel manipulator)
        {
            Model3D manipulatorModel = manipulator.GetManipulatorModel();
            AddModel(manipulatorModel);
        }

        public void AddModel(Model3D model)
        {
            ModelVisual3D topViewModel = new ModelVisual3D() {Content = model};
            ModelVisual3D frontViewModel = new ModelVisual3D() { Content = model };
            ModelVisual3D rightViewModel = new ModelVisual3D() { Content = model };
            ModelVisual3D fullViewModel = new ModelVisual3D() { Content = model };

            ViewPort2DTop.Children.Add(topViewModel);
            ViewPort2DFront.Children.Add(frontViewModel);
            ViewPort2DRight.Children.Add(rightViewModel);
            ViewPort3D.Children.Add(fullViewModel);
        }

        public void RemoveModel(Model3D model)
        {
            RemoveModelFromViewPort(ViewPort2DTop, model);
            RemoveModelFromViewPort(ViewPort2DFront, model);
            RemoveModelFromViewPort(ViewPort2DRight, model);
            RemoveModelFromViewPort(ViewPort3D, model);
        }

        private void RemoveModelFromViewPort(Viewport3D viewport, Model3D model)
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

        public void ShowMathModel(ManipulatorV2 manip)
        {
            RemoveMathModel(ViewPort2DFront);
            RemoveMathModel(ViewPort2DTop);
            RemoveMathModel(ViewPort2DRight);
            RemoveMathModel(ViewPort3D);

            Point3DCollection points = new Point3DCollection()
            {
                manip.ManipMathModel.Joints[0].StartPoint,
                manip.ManipMathModel.Joints[0].EndPoint,
                manip.ManipMathModel.Joints[1].StartPoint,
                manip.ManipMathModel.Joints[1].EndPoint,
                manip.ManipMathModel.Joints[2].StartPoint,
                manip.ManipMathModel.Joints[2].EndPoint
            };

            LinesVisual3D l3d = new LinesVisual3D();
            l3d.Thickness = 5;
            l3d.Points = points;

            LinesVisual3D l2t = new LinesVisual3D();
            l2t.Thickness = 5;
            l2t.Points = points;
            LinesVisual3D l2r = new LinesVisual3D();
            l2r.Thickness = 5;
            l2r.Points = points;
            LinesVisual3D l2f = new LinesVisual3D();
            l2f.Thickness = 5;
            l2f.Points = points;

            ViewPort2DFront.Children.Add(l2f);
            ViewPort2DTop.Children.Add(l2t);
            ViewPort2DRight.Children.Add(l2r);
            ViewPort3D.Children.Add(l3d);
        }

        public void ShowPoints(Point3D[] points)
        {
            ShowPointsInViewport(points, ViewPort3D);
            ShowPointsInViewport(points, ViewPort2DFront);
            ShowPointsInViewport(points, ViewPort2DRight);
            ShowPointsInViewport(points, ViewPort2DTop);
        }

        private void ShowPointsInViewport(Point3D[] points, Viewport3D viewPort)
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

        private void RemoveMathModel(Viewport3D viewPort)
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
    }
}

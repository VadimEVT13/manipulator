using HelixToolkit.Wpf;
using InverseTest.Detail;
using InverseTest.GUI.Models;
using InverseTest.Manipulator;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace InverseTest.GUI.Views
{
    /// <summary>
    /// Логика взаимодействия для DetailView.xaml
    /// </summary>
    public partial class DetailView : Window
    {
        private PerspectiveCamera cam3D;
        private DetailPointsCreator pointsCreator;

        private Dictionary<Visual3D, ScanPointVisual> orderedPoints;

        private DetailVisual detailVisual;
        private DetailModel detailModel;

        public DetailView()
        {
            InitializeComponent();
            setCamera();
            setLight();
            configViewPort();
            this.orderedPoints = new Dictionary<Visual3D, ScanPointVisual>();
            this.Closing += WindowClosing;
            Path.ScanPath.Instance.PointTransformed += this.TransformPointVisual;
        }

        private void WindowClosing(object sender, CancelEventArgs e) {
            e.Cancel = true;
            this.Hide();
        }

        private void setCamera()
        {
            cam3D = CameraHelper.CreateDefaultCamera();
            DetailViewPort.Camera = cam3D;
            DetailViewPort.CameraMode = CameraMode.Inspect;
            DetailViewPort.CameraRotationMode = CameraRotationMode.Turntable;
            DetailViewPort.DefaultCamera = cam3D;
            DetailViewPort.RotateAroundMouseDownPoint = false;
            DetailViewPort.ModelUpDirection = new Vector3D(0, 1, 0);
        }

        private void configCamera(DetailModel detail)
        {
            Point3D pointToView = MathUtils.GetRectCenter(detail.GetModel().Bounds);
            DetailViewPort.Camera.Position = new Point3D(pointToView.X, pointToView.Y, pointToView.Z - 70);
            DetailViewPort.Camera.LookDirection = new Vector3D(0, 0, 1);
            DetailViewPort.Camera.UpDirection = new Vector3D(0, 1, 0);
        }

        private void configViewPort()
        {
            this.DetailViewPort.RotateGesture = new MouseGesture(MouseAction.MiddleClick, ModifierKeys.Control);
        }

        private void setLight()
        {
            Light ambientLight = new AmbientLight(Colors.White);
            Light ambientLight2 = new AmbientLight(Colors.White);
            DirectionalLight pointLight = new DirectionalLight();
            pointLight.Direction = new Vector3D(3, -3, 0);
            pointLight.Color = Colors.White;

            Visual3D lightVisualTop = new LightVisual3D() { Content = ambientLight };
            Visual3D lightVisualRight = new LightVisual3D() { Content = ambientLight };
            Visual3D pointLightVisual = new LightVisual3D() { Content = pointLight };

            DetailViewPort.Children.Add(lightVisualRight);
            DetailViewPort.Children.Add(lightVisualTop);
            DetailViewPort.Children.Add(pointLightVisual);
        }

        private void initPointsCreator(DetailModel detail)
        {
            this.pointsCreator = new DetailPointsCreator(detail);
            DetailViewPort.MouseLeftButtonDown += this.pointsCreator.OnMouseLeftDown;
            DetailViewPort.MouseRightButtonDown += this.pointsCreator.OnMouseRightDown;
            this.pointsCreator.AddNewPointCallback += AddNewPoint;
            this.pointsCreator.RemovePointCallback += RemovePoint;
        }

        private void AddNewPoint(Point3D point)
        {
            Path.ScanPoint orderedPoint = new Path.ScanPoint(point);
            Path.ScanPath.getInstance.AddPoint(orderedPoint);
            ScanPointVisual pointVisual = new ScanPointVisual(orderedPoint);
            DetailViewPort.Children.Add(pointVisual.pointVisual);
            orderedPoints.Add(pointVisual.pointVisual, pointVisual);
        }

        private void RemovePoint(Visual3D point)
        {
            foreach (Visual3D v in DetailViewPort.Children)
            {
                if (v.Equals(point))
                {
                    if (orderedPoints.ContainsKey(v))
                    {
                        DetailViewPort.Children.Remove(v);
                        orderedPoints.TryGetValue(v, out ScanPointVisual orderedPoint);
                        Path.ScanPath.getInstance.RemovePoint(orderedPoint.Point);
                        orderedPoints.Remove(v);
                        break;
                    }
                }
            }
        }

        public void AddDetailModel(DetailModel detailModel)
        {
            this.detailModel = detailModel;
            this.detailVisual = new DetailVisual(detailModel.GetModel());
            this.DetailViewPort.Children.Add(this.detailVisual.visual);
            initPointsCreator(detailModel);
            configCamera(detailModel);
        }

        public void RemoveDetailMode(DetailModel detailModel)
        {
            foreach(Visual3D v in this.DetailViewPort.Children)
            {
                if(v is ModelVisual3D mv)
                {
                    if(mv.Content.Equals(detailModel.detailModel))
                    {
                        this.DetailViewPort.Children.Remove(v);
                        return;
                    }
                }
            }
        }

        public void TransformPointVisual(Transform3D trans)
        {
            orderedPoints.Values.ToList().ForEach(x => x.TransformPoint(trans));
        }

        public void OnPointSelected(Path.ScanPoint p)
        {
            orderedPoints.Values.ToList().ForEach(x => x.SetSelected(x.Point.Equals(p)));
        }

        private void RotateDetailSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Point3D center = MathUtils.GetRectCenter(this.detailModel.GetModel().Bounds);
            cam3D.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), e.NewValue), center);
        }
    }
}

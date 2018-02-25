using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using InverseTest.Manipulator;
using InverseTest.Detail;
using InverseTest.Frame.Kinematic;
using InverseTest.GUI.Model;
using InverseTest.GUI;
using System.Threading.Tasks;
using InverseTest.Frame;
using InverseTest.Workers;
using System.Threading;
using System.Windows.Threading;
using static InverseTest.Collision.AABB;
using InverseTest.Manipulator.Models;
using InverseTest.Collision.Model;
using InverseTest.Collision;
using static InverseTest.DetectorFrame;
using InverseTest.Collision.Mappers;
using InverseTest.Bound;
using System.Windows.Interop;

namespace InverseTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// Откланение от центра координат
        /// </summary>
        private static Point3D MANIPULATOR_OFFSET = new Point3D(-80, 0, 0);


        private bool solveKinematics = true;
        private bool animate = false;


        private Kinematic manipKinematic;

        public ManipulatorV2 manipulator;
        public DetectorFrame detectorFrame;
        public IMovementPoint scanPoint;
        private IMovementPoint manipulatorCamPoint;
        private IConeModel coneModel;

        //private Model3DGroup platform = new Model3DGroup();
        private Model3D platform;

        //Точка сканирования 
        private Model3D targetBox;
        //Точка камеры манипулятора
        private Model3D pointManip;
        //Точка в которую должен встать экран портала
        private Model3D pointPortal;

        private DetailModel detail;

        private ManipulatorKinematicWorker<SystemPosition> manipWorker;
        private GJKWorker<SceneSnapshot> collisionWorker;
        private CollisionDetector collisoinDetector;

        private double distanceToScreen = 50;
        private double focuseEnlagment = 1;
        Model3DGroup allModels;

        private ObservableCollection<SystemPosition> targetPoints { get; set; }

        private int selectedIndexPoint = -1;
        private List<UIElement> childrens;

        int numMesh = 0;

        DetailView detailView;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += this.MainWindowLoaded;

            detailView = new DetailView();
        }

        public void MainWindowLoaded(object sender, RoutedEventArgs arg)
        {
            allModels = new ModelImporter().Load("./3DModels/Detector Frame.obj");
            Model3DGroup newAllModels = new Model3DGroup();
            ManipulatorVisualizer.setCameras(allModels);        

            ModelPreprocessor preproccessor = new ModelPreprocessor(allModels);
            allModels = preproccessor.Simplification().GetProccessedModel();

            ModelParser parser = new ModelParser(allModels);
            parser.Parse();
            detectorFrame = parser.Frame;
            detectorFrame.onPositionChanged += OnDetectorFramePositionChanged;
            ManipulatorVisualizer.setDetectFrameModel(detectorFrame);

            PortalBoundController portalBounds = new PortalBoundController();
            portalBounds.CalculateBounds(detectorFrame);
            detectorFrame.boundController = portalBounds;

            platform = parser.DetailPlatform;
            ManipulatorVisualizer.AddModel(platform);

            manipulator = parser.Manipulator;
            manipulator.onPositionChanged += OnManipulatorPisitionChanged;
            ManipulatorVisualizer.setManipulatorModel(manipulator);

            ManipulatorVisualizer.AddModel(parser.Others);

            //Вычисляет длины ребер манипулятора для вычисления кинематики
            double[] edges = ManipulatorUtils.CalculateManipulatorLength(manipulator);
            this.manipKinematic = new Kinematic(MANIPULATOR_OFFSET.X, MANIPULATOR_OFFSET.Y, MANIPULATOR_OFFSET.Z);
            this.manipKinematic.SetLen(edges[0], edges[1], edges[2], edges[3], edges[4]);
            this.manipKinematic.det = ManipulatorUtils.CalculateManipulatorDet(manipulator);
            this.manipWorker = new ManipulatorKinematicWorker<SystemPosition>(manipKinematic);
            this.manipWorker.kinematicSolved += manipulatorSolved;

            detail = parser.Detail;
            ManipulatorVisualizer.AddModel(detail.GetModel());
            detailView.AddDetailModel(detail);

            ManipulatorVisualizer.AddModel(parser.Others);

            //Коллизии
            GJKSolver gjkSolver = new GJKSolver();

            AABB aabb = new AABB();
            aabb.MakeListExcept(manipulator, detectorFrame, detail, platform);
            collisionWorker = new GJKWorker<SceneSnapshot>(aabb, gjkSolver);
            collisoinDetector = new CollisionDetector(manipulator, detectorFrame, detail, platform, collisionWorker);

            //Точка сканирования
            scanPoint = new MovementPoint(Colors.Blue);
            ManipulatorVisualizer.SetPoint(scanPoint, detail.GetModel());
            scanPoint.PositoinChanged += OnScanPointPositoinChanged;

            //Точка камеры манипулятора
            manipulatorCamPoint = new MovementPoint(Colors.Red);
            ManipulatorVisualizer.SetManipulatorPoint(manipulatorCamPoint);
            manipulatorCamPoint.PositoinChanged += OnManipulatorCamPointPositoinChanged;

            //Конус из камеры манипулятора
            coneModel = new ConeModel();
            ManipulatorVisualizer.AddModelWithoutCamView(coneModel.GetModel());
            coneModel.ChangePosition(manipulator.GetCameraPosition(),
                manipulator.GetCameraDirection(),
                manipulatorCamPoint.GetTargetPoint().DistanceTo(scanPoint.GetTargetPoint()));

            manipulatorCamPoint.MoveAndNotify(new Point3D(-10, 60, 0));
            scanPoint.MoveAndNotify(new Point3D(0, 60, 0));

            collisionWorker.onCollision += OnCollisoinsDetected;
            FocueEnlargmentTextBox.Text = focuseEnlagment.ToString();

            this.detailView.Owner = this;
            detailView.Show();
        }

        public void OnCollisoinsDetected(List<CollisionPair> pair)
        {
            if (pair == null || pair.Count == 0)
            {
                CollisionListBox.ItemsSource = null;
            }
            else
            {
                CollisionListBox.ItemsSource = pair;
                SetPositionValid(false);
            }
        }

        public void manipulatorSolved(ManipulatorAngles angles)
        {
            Console.WriteLine("ManipulatorPoint: " + manipulator.GetCameraPosition());
            SetPositionValid(angles.isValid);
            manipulator.MoveManipulator(angles, false);
        }

        private void SetPositionValid(bool valid)
        {
            if (valid)
            {
                PositionValid.Background = Brushes.Green;
            }
            else
            {
                PositionValid.Background = Brushes.Red;
            }
        }

        /// <summary>
        /// Вызывается каждый раз когда "портал" меняет свое положение
        /// </summary>
        public void OnDetectorFramePositionChanged()
        {
            SetPortalPositionsTextBoxes();
        }

        public void SetDistanceToPoint()
        {
            this.distanceToScreen = scanPoint.GetTargetPoint().DistanceTo(manipulatorCamPoint.GetTargetPoint());
            FocusDistanceTextBox.Text = Math.Round(distanceToScreen, 3).ToString();
        }

        /// <summary>
        /// Вызывается каждый раз когда манипулятор меняет свое положение
        /// </summary>
        public void OnManipulatorPisitionChanged()
        {
            double distanceToPoint = scanPoint.GetTargetPoint().DistanceTo(manipulatorCamPoint.GetTargetPoint());
            coneModel.ChangePosition(manipulator.GetCameraPosition(), manipulator.GetCameraDirection(), distanceToPoint);

            this.distanceToScreen = distanceToPoint;

            var anglesState = ((ManipulatorV2)manipulator).partAngles;

            T1TextBox.Text = Math.Round(anglesState[ManipulatorParts.Table], 3).ToString();
            T2TextBox.Text = Math.Round(anglesState[ManipulatorParts.MiddleEdge], 3).ToString();
            T3TextBox.Text = Math.Round(anglesState[ManipulatorParts.TopEdge], 3).ToString();
            T4TextBox.Text = Math.Round(anglesState[ManipulatorParts.CameraBase], 3).ToString();
            T5TextBox.Text = Math.Round(anglesState[ManipulatorParts.Camera], 3).ToString();
            collisoinDetector.FindCollisoins();

            SetManipCamPointTextBoxes(manipulator.GetCameraPosition());
            SetDistanceToPoint();
        }

        public void SetPortalPositionsTextBoxes()
        {
            Dictionary<Parts, double> partOffset = (detectorFrame as DetectorFrame).partOffset;
            var verticalAngle = (detectorFrame as DetectorFrame).verticalAngle;
            var horizontalAngle = (detectorFrame as DetectorFrame).horizontalAngle;

            VerticalFrameSliderTextBox.Text = Math.Round(partOffset[Parts.VerticalFrame], 3).ToString();
            ScreenHolderTextBox.Text = Math.Round(partOffset[Parts.ScreenHolder], 3).ToString();
            HorizontalBarTextView.Text = Math.Round(partOffset[Parts.HorizontalBar], 3).ToString();
            ScreenVerticalAngleTextBox.Text = Math.Round(verticalAngle, 3).ToString();
            ScreenHorizontalAngleTextBox.Text = Math.Round(horizontalAngle, 3).ToString();
        }

        public void SetManipCamPointTextBoxes(Point3D point)
        {
            PointManipulatorXTextBox.Text = Math.Round(point.X, 3).ToString();
            PointManipulatorYTextBox.Text = Math.Round(point.Y, 3).ToString();
            PointManipulatorZTextBox.Text = Math.Round(point.Z, 3).ToString();
        }

        /// <summary>
        /// Вызываетсяс каждый раз когда изменяется позиция точки в которую становится манипулятор
        /// </summary>
        /// <param name="newPosition"></param>
        public void OnManipulatorCamPointPositoinChanged(Point3D newPosition)
        {
            if (solveKinematics)
            {
                this.manipWorker.solve(new SystemPosition(newPosition, scanPoint.GetTargetPoint()));
                SolvePortalKinematic(newPosition, scanPoint.GetTargetPoint(), false);
            }

            collisoinDetector.FindCollisoins();
        }

        public void OnScanPointPositoinChanged(Point3D newPosition)
        {
            TargetPointXTextBox.Text = Math.Round(newPosition.X, 3).ToString();
            TargetPointYTextBox.Text = Math.Round(newPosition.Y, 3).ToString();
            TargetPointZTextBox.Text = Math.Round(newPosition.Z, 3).ToString();

            if (solveKinematics)
            {
                this.manipWorker.solve(new SystemPosition(manipulatorCamPoint.GetTargetPoint(), newPosition));
                SolvePortalKinematic(manipulatorCamPoint.GetTargetPoint(), newPosition, false);
            }


        }

        private void T1Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            manipulator.RotatePart(ManipulatorParts.Table, -e.NewValue);
        }

        private void T2Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            manipulator.RotatePart(ManipulatorParts.MiddleEdge, -e.NewValue);
        }

        private void T3Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            manipulator.RotatePart(ManipulatorParts.TopEdge, -e.NewValue);
        }

        private void T4Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            manipulator.RotatePart(ManipulatorParts.CameraBase, -e.NewValue);
        }

        private void T5Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            manipulator.RotatePart(ManipulatorParts.Camera, -e.NewValue);
        }

        /// <summary>
        /// Считываем координаты точки из полей ввода
        /// </summary>
        private void ParsePointsAndMove()
        {
            double x, y, z;
            Console.WriteLine("ParsePointAndMove");
            double.TryParse(TargetPointXTextBox.Text, out x);
            double.TryParse(TargetPointYTextBox.Text, out y);
            double.TryParse(TargetPointZTextBox.Text, out z);
            scanPoint.Move(new Point3D(x, y, z));

            double manip_x, manip_y, manip_z;
            double.TryParse(PointManipulatorXTextBox.Text, out manip_x);
            double.TryParse(PointManipulatorYTextBox.Text, out manip_y);
            double.TryParse(PointManipulatorZTextBox.Text, out manip_z);
            manipulatorCamPoint.Move(new Point3D(manip_x, manip_y, manip_z));
        }


        private void SolvePortalKinematic(Point3D manip, Point3D scannedPoint, bool animate)
        {
            PortalKinematic p = new PortalKinematic(500, 500, 500, 140, 10, 51, 10, 0, 30);
            p.setPointManipAndNab(manip.X, manip.Z, manip.Y, scannedPoint.X, scannedPoint.Z, scannedPoint.Y);

            double[] rez = p.portalPoint(manip.DistanceTo(scannedPoint), this.focuseEnlagment);
            if (rez != null)
            {
                ///ХЗ почему со знаком -
                DetectorFramePosition detectp = new DetectorFramePosition(new Point3D(rez[5], rez[7], rez[6]), -rez[4], rez[3]);
                detectorFrame.MoveDetectFrame(detectp, animate);
            }
            else
            {
                MessageBox.Show("Не существует такой точки");
            }
        }

        private void RotateManipulatorButton_OnClick(object sender, RoutedEventArgs e)
        {
            recalculateKinematic();
        }

        private void ResetManipulatorButton_OnClick(object sender, RoutedEventArgs e)
        {
            resetManip();
        }

        private void HideManipModelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            IManipulatorModel m = manipulator;

            ManipulatorVisualizer.RemoveModel(m.GetManipulatorModel());
        }

        private List<Point3D[]> list;
        private int listCounter = 0;

        private void NextIteration_OnClick(object sender, RoutedEventArgs e)
        {
            if (listCounter < list.Count)
            {
                Point3D[] points = list[listCounter];
                ManipulatorVisualizer.ShowPoints(points);
                listCounter++;
            }
            else
            {
                listCounter = 0;
                ManipulatorVisualizer.RemoveAllMathModels();
            }

        }

        private void AddPointToList_Click(object sender, RoutedEventArgs e)
        {
            double x, y, z;

            double.TryParse(TargetPointXTextBox.Text, out x);
            double.TryParse(TargetPointYTextBox.Text, out y);
            double.TryParse(TargetPointZTextBox.Text, out z);


            SystemPosition lastPoint = targetPoints.LastOrDefault();

            if (lastPoint != null)
            {
                Point3D newPoint = new Point3D(x, y, z);
                if (lastPoint.Equals(newPoint))
                {
                    MessageBox.Show("Точка уже в списке!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    targetPoints.Add(new SystemPosition(manipulatorCamPoint.GetTargetPoint(), scanPoint.GetTargetPoint()));
                }
            }
            else
            {
                targetPoints.Add(new SystemPosition(manipulatorCamPoint.GetTargetPoint(), scanPoint.GetTargetPoint()));
            }


        }
        

        //Запрогать загрузку и замену детальки
        private void LoadModel_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

       
       private void HelpMenuItem_Click(object sender, RoutedEventArgs e)
        {
        }


        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void resetManip()
        {
            manipulator.ResetModel();
            detectorFrame.ResetTransforms();
        }

        private void PointDown_Click(object sender, RoutedEventArgs e)
        {
            if (selectedIndexPoint > -1 && selectedIndexPoint < targetPoints.Count - 1)
            {
                SystemPosition point = targetPoints[selectedIndexPoint];
                int index = selectedIndexPoint;
                targetPoints.RemoveAt(index);
                targetPoints.Insert(index + 1, point);

            }
        }

        private void PointUp_Click(object sender, RoutedEventArgs e)
        {
            if (selectedIndexPoint > -1 && selectedIndexPoint > 0 && selectedIndexPoint <= targetPoints.Count - 1)
            {
                SystemPosition point = targetPoints[selectedIndexPoint];
                int index = selectedIndexPoint;
                targetPoints.RemoveAt(index);

                targetPoints.Insert(index - 1, point);

            }
        }

        private void ShowBorders_Click(object sender, RoutedEventArgs e)
        {
            ManipulatorVisualizer.showBorders(detectorFrame.GetDetectorFramePart(Parts.VerticalFrame));
            //ManipulatorVisualizer.showBordersPortal(ManipulatorMapper.ManipulatorToSnapshot(manipulator));
        }

        private void VerticalFrameSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            detectorFrame.MovePart(DetectorFrame.Parts.VerticalFrame, e.NewValue);
        }

        private void HorizontalBarSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            detectorFrame.MovePart(DetectorFrame.Parts.HorizontalBar, e.NewValue);
        }

        private void ScreenHolderSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            detectorFrame.MovePart(DetectorFrame.Parts.ScreenHolder, e.NewValue);
        }

        private void ScreenVerticalAngleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            detectorFrame.RotatePart(DetectorFrame.Parts.Screen, e.NewValue, DetectorFrame.ZRotateAxis);
        }

        private void ScreenHorizontalAngleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            detectorFrame.RotatePart(DetectorFrame.Parts.Screen, e.NewValue, DetectorFrame.YRotateAxis);
        }

        private void MoveMesh_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((IDebugModels)manipulator).transformModel(e.NewValue);
           // allModels.Children[numMesh].Transform = new TranslateTransform3D(0, (int)e.NewValue, 0);
        }

        private void NumMesh_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            numMesh = (int)e.NewValue;
            ((IDebugModels)manipulator).addNumberMesh(numMesh);
            NumMeshTextBox.Text = numMesh.ToString();
        }

        private void CalculateJunctionsButton_Click(object sender, RoutedEventArgs e)
        {

            //Task task = new Task(() =>
            //{
            var watch = System.Diagnostics.Stopwatch.StartNew();
            JunctionDetect.JunctionDetectAlgorithm junctiondetec = new JunctionDetect.JunctionDetectAlgorithm();
            int[] junctions = junctiondetec.Detect(detail.GetModel());
            watch.Stop();
            detail.SetJunctionsPoints(junctions);

            Console.WriteLine("Time: " + watch.ElapsedMilliseconds);
            //});

            //task.Start();
        }

        private void CameraVisibleArea_Checked(object sender, RoutedEventArgs e)
        {
            coneModel.SetVisibility(true);
        }

        private void CameraVisibleArea_Unchecked(object sender, RoutedEventArgs e)
        {
            coneModel.SetVisibility(false);
        }

        private void DetailProjection_Unchecked(object sender, RoutedEventArgs e)
        {
            ManipulatorVisualizer.DeleteCountur(detail);
        }

        private void DetailProjection_Checked(object sender, RoutedEventArgs e)
        {
            ManipulatorVisualizer.AddCountur(detail);
        }


        private void ToggleAnimation_Checked(object sender, RoutedEventArgs e)
        {
            solveKinematics = false;
            animate = true;
        }

        private void ToggleAnimation_Unchecked(object sender, RoutedEventArgs e)
        {
            solveKinematics = true;
            animate = false;
        }


        private void TargetPointTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                ParsePointsAndMove();
            }
        }

        private void PointManipulatorTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                ParsePointsAndMove();
            }
        }

        private void TargetPointsListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SystemPosition point = ((ListViewItem)sender).Content as SystemPosition;

            if (point != null)
            {
                manipulatorCamPoint.MoveAndNotify(point.manipPoint);
                scanPoint.MoveAndNotify(point.targetPoint);
            }
        }

        private void recalculateKinematic()
        {
            Point3D manip = manipulatorCamPoint.GetTargetPoint();
            Point3D targetPoint = scanPoint.GetTargetPoint();

            manipWorker.solve(new SystemPosition(manip, targetPoint));
            SolvePortalKinematic(manip, targetPoint, false);
        }

        private void FocusEnlargementSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.focuseEnlagment = e.NewValue;
            recalculateKinematic();
        }

        private void FocusDistanceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        private void FocusDistance_Checked(object sender, RoutedEventArgs e)
        {
            FocusDistancePopup.IsOpen = true;
        }

        private void FocusDistance_Unchecked(object sender, RoutedEventArgs e)
        {
            FocusDistancePopup.IsOpen = false;

        }

        private void PositionValid_Checked(object sender, RoutedEventArgs e)
        {
            CollisionPopup.IsOpen = true;
        }

        private void PositionValid_Unchecked(object sender, RoutedEventArgs e)
        {
            CollisionPopup.IsOpen = false;
        }
    }
}

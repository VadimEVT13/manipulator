using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using InverseTest.Manipulator;
using InverseTest.Detail;
using InverseTest.Frame.Kinematic;
using InverseTest.GUI.Model;
using InverseTest.GUI;
using InverseTest.Frame;
using InverseTest.Workers;
using InverseTest.Collision.Model;
using InverseTest.Collision;
using static InverseTest.DetectorFrame;
using InverseTest.Bound;
using InverseTest.Path;
using InverseTest.Manipulator.Models;
using InverseTest.Model;
using System.Windows.Controls;

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
        private MovementPoint manipulatorCamPoint;
        private ConeModel coneModel;

        private Model3D platform;

        private DetailModel detail;

        private KinematicWorker<SystemPosition, SystemState> kinematicWorker;
        private GJKWorker<SceneSnapshot, List<CollisionPair>> collisionWorker;
        private CollisionDetector collisoinDetector;
        private CollisionVisualController collisoinVisual;
        private DetailPathController detailPathController;
        private ScanPathVisualController pathVisual;
        private DetailVisualCollisionController detailVisControlle;
        
        private double distanceToScreen = 50;
        private double focuseEnlagment = 1;
        Model3DGroup allModels;

        private ObservableCollection<SystemPosition> targetPoints { get; set; }

        private int selectedIndexPoint = -1;

        int numMesh = 0;

        private ScanPoint targetPoint = new ScanPoint(new Point3D(0, 60, 0));

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
            DetectorFrameVisual portalVisual = DetectorFrameVisualFactory.CreateDetectorFrameVisual(detectorFrame);

            detectorFrame.onPositionChanged += Detector_PositionChanged;
            ManipulatorVisualizer.SetDetectFrameModel(detectorFrame, portalVisual);

            manipulator = parser.Manipulator;
            ManipulatorVisual manipulatorVisual = ManipulatorVisualFactory.CreateManipulator(manipulator);

            ManipulatorVisualizer.setManipulatorModel(manipulator, manipulatorVisual);
            manipulator.onPositionChanged += Manipulator_PositionChanged;

            ManipulatorVisualizer.AddModel(parser.Others);

            //Вычисляет длины ребер манипулятора для вычисления кинематики
            double[] edges = ManipulatorUtils.CalculateManipulatorLength(manipulator);
            this.manipKinematic = new Kinematic(new Vertex3D { X = MANIPULATOR_OFFSET.X, Y = MANIPULATOR_OFFSET.Y, Z = MANIPULATOR_OFFSET.Z });
            this.manipKinematic.SetLen(new LengthJoin { J1 = edges[0], J2 = edges[1], J3 = edges[2], J4 = edges[3], J5 = edges[4], Det = ManipulatorUtils.CalculateManipulatorDet(manipulator) });

            PortalKinematic portalKinematic = new PortalKinematic(500, 500, 500, 140, 10, 51, 10, 0, 30);
            PortalBoundController portalBounds = new PortalBoundController();
            portalBounds.CalculateBounds(detectorFrame);

            this.kinematicWorker = new KinematicWorker<SystemPosition, SystemState>(
                manipKinematic,
                portalKinematic,
                portalBounds,
                new ManipulatorAnglesBounds()
                );
            this.kinematicWorker.OnComplete += KinematicSolved;

            detail = parser.Detail;
            platform = parser.DetailPlatform;

            this.detailVisControlle = DetailVisualFactory.CreateDetailVisual(detail, platform);
            this.collisoinVisual = new CollisionVisualController(manipulatorVisual, portalVisual, detailVisControlle);

            ManipulatorVisualizer.AddVisuals(detailVisControlle.Visuals);
            detailView.AddDetailModel(detail);


            ManipulatorVisualizer.AddModel(platform);

            this.detailPathController = new DetailPathController(detail, ScanPath.getInstance);
            this.pathVisual = new ScanPathVisualController(ManipulatorVisualizer);
            PathListView.OnSelectedPoint += this.pathVisual.OnPointSelected;
            PathListView.OnSelectedPoint += this.OnScanPointSelected;
            PathListView.OnSelectedPoint += this.detailView.OnPointSelected;

            ManipulatorVisualizer.AddModel(parser.Others);

            //Коллизии
            GJKSolver gjkSolver = new GJKSolver();

            AABB aabb = new AABB();
            aabb.MakeListExcept(manipulator, detectorFrame, detail, platform);
            collisionWorker = new GJKWorker<SceneSnapshot, List<CollisionPair>>(aabb, gjkSolver);
            collisoinDetector = new CollisionDetector(manipulator, detectorFrame, detail, platform, collisionWorker);

            //Точка камеры манипулятора
            manipulatorCamPoint = new MovementPoint(Colors.Red);
            ManipulatorVisualizer.SetManipulatorPoint(manipulatorCamPoint);
            manipulatorCamPoint.PositoinChanged += OnManipulatorCamPointPositoinChanged;

            //Конус из камеры манипулятора
            coneModel = new ConeModel();
            ManipulatorVisualizer.AddModelWithoutCamView(coneModel.GetModel());
            
            manipulatorCamPoint.MoveAndNotify(new Point3D(-10, 60, 0));

            collisionWorker.OnComplete += OnCollisoinsDetected;
            FocueEnlargmentTextBox.Text = focuseEnlagment.ToString();

            this.detailView.Owner = this;
            detailView.Show();
        }

        public void OnCollisoinsDetected(List<CollisionPair> pair)
        {
            this.collisoinVisual.Collisions(pair);
        }
        
        public void KinematicSolved(SystemState state)
        {
            if(state.Angles!=null)
            manipulator.MoveManipulator(state.Angles, false);

            if(state.PortalPosition!=null)
            detectorFrame.MoveDetectFrame(state.PortalPosition, false);
        }

         /// <summary>
        /// Обработчик для выбора точки из списка точек пути. 
        /// </summary>
        /// <param name="p"></param>
        public void OnScanPointSelected(ScanPoint p)
        {
            if (p != null)
            {
                this.targetPoint = p;
                recalculateKinematic();
                TargetPointXTextBox.Text = Math.Round(p.point.X, 3).ToString();
                TargetPointYTextBox.Text = Math.Round(p.point.Y, 3).ToString();
                TargetPointZTextBox.Text = Math.Round(p.point.Z, 3).ToString();
            }
        }


        /// <summary>
        /// Обработчик изменения положения детектора.
        /// </summary>
        public void Detector_PositionChanged()
        {
            VerticalFrameSlider.Value = DetectorPositionController.XGlobalToLocal(detectorFrame.VerticalFramePosition);
            HorizontalBarSlider.Value = DetectorPositionController.YGlobalToLocal(detectorFrame.HorizontalBarPosition);
            ScreenHolderSlider.Value = DetectorPositionController.ZGlobalToLocal(detectorFrame.ScreenHolderPosition);
            ScreenVerticalAngleSlider.Value = DetectorPositionController.AGlobalToLocal(detectorFrame.VerticalAngle);
            ScreenHorizontalAngleSlider.Value = DetectorPositionController.BGlobalToLocal(detectorFrame.HorizontalAngle);
            collisoinDetector.FindCollisoins();
        }

        public void SetDistanceToPoint()
        {
            this.distanceToScreen = targetPoint.point.DistanceTo(manipulatorCamPoint.GetTargetPoint());
            FocusDistanceTextBox.Text = Math.Round(distanceToScreen, 3).ToString();
        }

        /// <summary>
        /// Обработчик изменения положения манипулятора.
        /// </summary>
        public void Manipulator_PositionChanged()
        {
            double distanceToPoint = targetPoint.point.DistanceTo(manipulatorCamPoint.GetTargetPoint());
            coneModel.ChangePosition(manipulator.GetCameraPosition(), manipulator.GetCameraDirection(), distanceToPoint);

            SetManipCamPointTextBoxes(manipulator.GetCameraPosition());
            T1Slider.Value = ManipulatorPositionController.T1GlobalToLocal(manipulator.TablePosition);
            T2Slider.Value = ManipulatorPositionController.T2GlobalToLocal(manipulator.MiddleEdgePosition);
            T3Slider.Value = ManipulatorPositionController.T3GlobalToLocal(manipulator.TopEdgePosition);
            T4Slider.Value = ManipulatorPositionController.T4GlobalToLocal(manipulator.CameraBasePosition);
            T5Slider.Value = ManipulatorPositionController.T5GlobalToLocal(manipulator.CameraPosition);
            collisoinDetector.FindCollisoins();
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
                recalculateKinematic();
            }
        }

        /// <summary>
        /// Обработка изменения положения первого колена.
        /// </summary>
        /// <param name="sender">инициатор изменения</param>
        /// <param name="e">событие</param>
        private void T1Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (manipulator != null)
            {
                double value = ManipulatorPositionController.T1LocalToGlobal(e.NewValue);
                if (Math.Abs(manipulator.TablePosition - value) > 1e-2)
                {
                    manipulator.TablePosition = value;
                }
            }
        }

        /// <summary>
        /// Обработка изменения положения второго колена.
        /// </summary>
        /// <param name="sender">инициатор изменения</param>
        /// <param name="e">событие</param>
        private void T2Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (manipulator != null)
            {
                double value = ManipulatorPositionController.T2LocalToGlobal(e.NewValue);
                if (Math.Abs(manipulator.MiddleEdgePosition - value) > 1e-2)
                {
                    manipulator.MiddleEdgePosition = value;
                }
            }
        }

        /// <summary>
        /// Обработка изменения положения третьего колена.
        /// </summary>
        /// <param name="sender">инициатор изменения</param>
        /// <param name="e">событие</param>
        private void T3Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (manipulator != null)
            {
                double value = ManipulatorPositionController.T3LocalToGlobal(e.NewValue);
                if (Math.Abs(manipulator.TopEdgePosition - value) > 1e-2)
                {
                    manipulator.TopEdgePosition = value;
                }
            }
        }

        /// <summary>
        /// Обработка изменения положения четвертого колена.
        /// </summary>
        /// <param name="sender">инициатор изменения</param>
        /// <param name="e">событие</param>
        private void T4Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (manipulator != null)
            {
                double value = ManipulatorPositionController.T4LocalToGlobal(e.NewValue);
                if (Math.Abs(manipulator.CameraBasePosition - value) > 1e-2)
                {
                    manipulator.CameraBasePosition = value;
                }
            }
        }

        /// <summary>
        /// Обработка изменения положения пятого колена.
        /// </summary>
        /// <param name="sender">инициатор изменения</param>
        /// <param name="e">событие</param>
        private void T5Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (manipulator != null)
            {
                double value = ManipulatorPositionController.T5LocalToGlobal(e.NewValue);
                if (Math.Abs(manipulator.CameraPosition - value) > 1e-2)
                {
                    manipulator.CameraPosition = value;
                }
            }
        }

        /// <summary>
        /// Обработка изменения положения вертикальной рамки.
        /// </summary>
        /// <param name="sender">инициатор изменения</param>
        /// <param name="e">событие</param>
        private void VerticalFrameSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (detectorFrame != null)
            {
                double value = DetectorPositionController.XLocalToGlobal(e.NewValue);
                if (Math.Abs(detectorFrame.VerticalFramePosition - value) > 1e-2)
                {
                    detectorFrame.VerticalFramePosition = value;
                }
            }
        }

        /// <summary>
        /// Обработка изменения положения горизонтальной планки.
        /// </summary>
        /// <param name="sender">инициатор изменения</param>
        /// <param name="e">событие</param>
        private void HorizontalBarSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (detectorFrame != null)
            {
                double value = DetectorPositionController.YLocalToGlobal(e.NewValue);
                if (Math.Abs(detectorFrame.HorizontalBarPosition - value) > 1e-2)
                {
                    detectorFrame.HorizontalBarPosition = value;
                }
            }
        }

        /// <summary>
        /// Обработка изменения положения держателя экрана.
        /// </summary>
        /// <param name="sender">инициатор изменения</param>
        /// <param name="e">событие</param>
        private void ScreenHolderSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (detectorFrame != null)
            {
                double value = DetectorPositionController.ZLocalToGlobal(e.NewValue);
                if (Math.Abs(detectorFrame.ScreenHolderPosition - value) > 1e-2)
                {
                    detectorFrame.ScreenHolderPosition = value;
                }
            }
        }

        /// <summary>
        /// Обработка изменения положения экрана детектора по вертикали.
        /// </summary>
        /// <param name="sender">инициатор изменения</param>
        /// <param name="e">событие</param>
        private void ScreenVerticalAngleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (detectorFrame != null)
            {
                double value = DetectorPositionController.ALocalToGlobal(e.NewValue);
                if (Math.Abs(detectorFrame.VerticalAngle - value) > 1e-2)
                {
                    detectorFrame.VerticalAngle = value;
                }
            }
        }

        /// <summary>
        /// Обработка изменения положения экрана детектора по горизонтали.
        /// </summary>
        /// <param name="sender">инициатор изменения</param>
        /// <param name="e">событие</param>
        private void ScreenHorizontalAngleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (detectorFrame != null)
            {
                double value = DetectorPositionController.BLocalToGlobal(e.NewValue);
                if (Math.Abs(detectorFrame.HorizontalAngle - value) > 1e-2)
                {
                    detectorFrame.HorizontalAngle = value;
                }
            }
        }

        /// <summary>
        /// Считываем координаты точки из полей ввода
        /// </summary>
        private void ParsePointsAndMove()
        {
            double manip_x, manip_y, manip_z;
            double.TryParse(PointManipulatorXTextBox.Text, out manip_x);
            double.TryParse(PointManipulatorYTextBox.Text, out manip_y);
            double.TryParse(PointManipulatorZTextBox.Text, out manip_z);
            manipulatorCamPoint.Move(new Point3D(manip_x, manip_y, manip_z));
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
            ManipulatorV2 m = manipulator;

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


        //Запрогать загрузку и замену детальки
        private void LoadModel_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
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

        private void MoveMesh_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            detectorFrame.transformModel(e.NewValue);
            //allModels.Children[numMesh].Transform = new TranslateTransform3D(0, (int)e.NewValue, 0);
        }

        private void NumMesh_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            numMesh = (int)e.NewValue;
            detectorFrame.addNumberMesh(numMesh);
            NumMeshTextBox.Text = numMesh.ToString();
        }

        private void CalculateJunctionsButton_Click(object sender, RoutedEventArgs e)
        {

            JunctionDetect.JunctionDetectAlgorithm junctiondetec = new JunctionDetect.JunctionDetectAlgorithm();
            int[] junctions = junctiondetec.Detect(detail.GetModel());
            detail.SetJunctionsPoints(junctions);
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

        private void recalculateKinematic()
        {
            Point3D manip = manipulatorCamPoint.GetTargetPoint();
            Point3D targetPoint = this.targetPoint.point;

            kinematicWorker.Solve(new SystemPosition(manip, targetPoint, manip.DistanceTo(targetPoint), focuseEnlagment));
        }

        private void FocusEnlargementSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.focuseEnlagment = e.NewValue;
            recalculateKinematic();
        }

        private void FocusDistanceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var distance = e.NewValue;
            var manipCurrentPoint = manipulator.GetCameraPosition();
            var targetPoint = this.targetPoint.point;

            var direction = new Vector3D(manipCurrentPoint.X - targetPoint.X,
                manipCurrentPoint.Y - targetPoint.Y, manipCurrentPoint.Z - targetPoint.Z);
         
            direction.Normalize();
           // var newPoint = ; 

           // manipulatorCamPoint.MoveAndNotify(newPoint);
        }

        private void FocusDistance_Checked(object sender, RoutedEventArgs e)
        {
            FocusDistancePopup.IsOpen = true;
        }

        private void FocusDistance_Unchecked(object sender, RoutedEventArgs e)
        {
            FocusDistancePopup.IsOpen = false;
        }

        private void DetailViewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.detailView.Show();
        }

        private void RotateDetailSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.detailPathController.Rotate(e.NewValue);
        }

        private void ResetCamers_Click(object sender, RoutedEventArgs e)
        {
            ManipulatorVisualizer.setCameras(allModels);
        }

        /// <summary>
        /// Обработчик события - блокировка оси X.
        /// </summary>
        /// <param name="sender">Отправитель события</param>
        /// <param name="e">Событие</param>
        private void CheckX_Checked(object sender, RoutedEventArgs e)
        {
            PointManipulatorXTextBox.IsReadOnly = true;
        }

        /// <summary>
        /// Обработчик события - разблокировка оси X.
        /// </summary>
        /// <param name="sender">Отправитель события</param>
        /// <param name="e">Событие</param>
        private void CheckX_Unchecked(object sender, RoutedEventArgs e)
        {
            PointManipulatorXTextBox.IsReadOnly = false;
        }

        /// <summary>
        /// Обработчик события - блокировка оси Y.
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e">Событие</param>
        private void CheckY_Checked(object sender, RoutedEventArgs e)
        {
            PointManipulatorYTextBox.IsReadOnly = true;
        }

        /// <summary>
        /// Обработчик события - разблокировка оси Y.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckY_Unchecked(object sender, RoutedEventArgs e)
        {
            PointManipulatorYTextBox.IsReadOnly = false;
        }

        /// <summary>
        /// Обработчик события - блокировка оси Z.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckZ_Checked(object sender, RoutedEventArgs e)
        {
            PointManipulatorZTextBox.IsReadOnly = true;
        }

        /// <summary>
        /// Обработчик события - разблокировка оси Z.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckZ_Unchecked(object sender, RoutedEventArgs e)
        {
            PointManipulatorZTextBox.IsReadOnly = false;
        }
    }
}

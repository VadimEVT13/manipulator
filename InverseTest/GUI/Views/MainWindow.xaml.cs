using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using InverseTest.Manipulator;
using InverseTest.Detail;
using InverseTest.GUI.Model;
using InverseTest.Frame;
using InverseTest.Workers;
using InverseTest.Collision.Model;
using InverseTest.Collision;
using static InverseTest.DetectorFrame;
using InverseTest.Bound;
using InverseTest.Path;
using InverseTest.Manipulator.Models;
using InverseTest.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using InverseTest.GUI.ViewModels;

namespace InverseTest.GUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Откланение от центра координат
        /// </summary>
        private static Point3D MANIPULATOR_OFFSET = new Point3D(-80, 0, 0);

        private bool solveKinematics = true;
        

        private Kinematic manipKinematic;

        
        private ConeModel coneModel;

        private Model3D platform;

        private DetailModel detail;

        private KinematicWorker<SystemPosition, SystemState> kinematicWorker;
        private GJKWorker<SceneSnapshot, List<CollisionPair>> collisionWorker;
        private CollisionDetector collisoinDetector;
        private CollisionVisualController collisoinVisual;
        private DetailVisualCollisionController detailVisControlle;

        /// <summary>
        /// Собирает отдельные модели из списка всех мешей
        /// </summary>
        private ModelParser ModelParser;

        

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        

        Model3DGroup allModels;

        private ObservableCollection<SystemPosition> targetPoints { get; set; }

        private int selectedIndexPoint = -1;

        private ScanPoint targetPoint = new ScanPoint(new Point3D(0, 60, 0));

        // Добавлено
        private ScanPoint ShvatPoint = new ScanPoint(new Point3D(-10, 60, 0));

        DetailView detailView;

        //TODO
        public MainViewModel MainVM { get; set; }

        public MainWindow(MainViewModel mainVM)
        {
            MainVM = mainVM;
            InitializeComponent();
            this.Loaded += this.MainWindowLoaded;
            //this.DataContext = this;
            this.detailView = new DetailView();
        }

        public void MainWindowLoaded(object sender, RoutedEventArgs arg)
        {
            allModels = new ModelImporter().Load("./3DModels/Detector Frame.obj");

            Model3DGroup newAllModels = new Model3DGroup();
            ManipulatorVisualizer.setCameras(allModels);

            this.ModelParser = new ModelParser(allModels);
            ModelParser.Parse();
            MainVM.Detector = ModelParser.Frame;
            DetectorFrameVisual portalVisual = DetectorFrameVisualFactory.CreateDetectorFrameVisual(MainVM.Detector);

            MainVM.Detector.onPositionChanged += MainVM.DetectorPositionChanged;
            MainVM.Detector.onManulaPositionChanged += DetecterManual_PositionChanged;
            ManipulatorVisualizer.SetDetectFrameModel(MainVM.Detector, portalVisual);

            MainVM.Manipulator = ModelParser.Manipulator;
            ManipulatorVisual manipulatorVisual = ManipulatorVisualFactory.CreateManipulator(MainVM.Manipulator);

            ManipulatorVisualizer.setManipulatorModel(MainVM.Manipulator, manipulatorVisual);
            MainVM.Manipulator.onPositionChanged += Manipulator_PositionChanged;
            MainVM.Manipulator.onManulaPositionChanged += MainVM.ManipulatorPositionChanged;

            ManipulatorVisualizer.AddModel(ModelParser.Others);

            //Вычисляет длины ребер манипулятора для вычисления кинематики
            double[] edges = ManipulatorUtils.CalculateManipulatorLength(MainVM.Manipulator);
            this.manipKinematic = new Kinematic(new Vertex3D { X = MANIPULATOR_OFFSET.X, Y = MANIPULATOR_OFFSET.Z, Z = MANIPULATOR_OFFSET.Y });
            this.manipKinematic.SetLen(new LengthJoin
            {
                J1 = edges[0],
                J2 = edges[1],
                J3 = edges[2],
                J4 = edges[3],
                J5 = edges[4],
                Det = ManipulatorUtils.CalculateManipulatorDet(MainVM.Manipulator)
            });
            //this.manipKinematic.SetLen(new LengthJoin
            //{
            //    J1 = 71,
            //    J2 = 90.5,
            //    J3 = 92,
            //    J4 = 16,
            //    J5 = 20,
            //    Det = 11
            //});
            
            //PortalKinematic portalKinematic     = new PortalKinematic(500, 500, 500, 71.529724, 38.631, -0.0025140, 10, 0, 19.5);
            PortalKinematic portalKinematic     = new PortalKinematic(500, 500, 500, 71.529724, 38.631, -0.0025140, 10, 0, 19.5);
            PortalBoundController portalBounds  = new PortalBoundController();
            portalBounds.CalculateBounds(MainVM.Detector);

            this.kinematicWorker = new KinematicWorker<SystemPosition, SystemState>(
                manipKinematic,
                portalKinematic,
                portalBounds,
                new ManipulatorAnglesBounds()
                );
            this.kinematicWorker.OnComplete += KinematicSolved;

            detail = ModelParser.LopatkaDetail;
            detail.onPositionChanged += Deteil_PositionChenged;
            platform = ModelParser.DetailPlatform;

            this.detailVisControlle = DetailVisualFactory.CreateDetailVisual(detail, platform);
            this.collisoinVisual = new CollisionVisualController(manipulatorVisual, portalVisual, detailVisControlle);

            ManipulatorVisualizer.AddVisuals(detailVisControlle.Visuals);
            detailView.AddDetailModel(detail);

            ManipulatorVisualizer.AddModel(platform);

            MainVM.DetailVM.Detail = detail;
            MainVM.DetailVM.Path = ScanPath.getInstance;

            MainVM.PathVM.Scene = ManipulatorVisualizer;

            PathListView.OnSelectedPoint += this.MainVM.PathVM.OnPointSelected;
            PathListView.OnSelectedPoint += this.OnScanPointSelected;
            PathListView.OnSelectedPoint += this.detailView.OnPointSelected;

            MethodicView.OnManipulatorPoint += this.OnManipulatorPointSelected;

            ManipulatorVisualizer.AddModel(ModelParser.Others);

            //Коллизии
            GJKSolver gjkSolver = new GJKSolver();

            AABB aabb = new AABB();
            aabb.MakeListExcept(MainVM.Manipulator, MainVM.Detector, detail, platform);
            collisionWorker = new GJKWorker<SceneSnapshot, List<CollisionPair>>(aabb, gjkSolver);
            collisoinDetector = new CollisionDetector(MainVM.Manipulator, MainVM.Detector, detail, platform, collisionWorker);

            //Точка камеры манипулятора
            MainVM.ManipulatorCamPoint = new MovementPoint(Colors.Red);
            ManipulatorVisualizer.SetManipulatorPoint(MainVM.ManipulatorCamPoint);
            MainVM.ManipulatorCamPoint.PositoinChanged += OnManipulatorCamPointPositoinChanged;

            //Конус из камеры манипулятора
            coneModel = new ConeModel();
            ManipulatorVisualizer.AddModelWithoutCamView(coneModel.GetModel());

            MainVM.ManipulatorCamPoint.MoveAndNotify(new Point3D(-10, 60, 0));

            collisionWorker.OnComplete += OnCollisoinsDetected;
            FocueEnlargmentTextBox.Text = MainVM.FocusEnlagment.ToString();

            this.detailView.Owner = this;
            detailView.Show();
        }

        public void OnCollisoinsDetected(List<CollisionPair> pair)
        {
            this.collisoinVisual.Collisions(pair);          
        }

        public void KinematicSolved(SystemState state)
        {
            if (state.Angles != null)
                MainVM.Manipulator.MoveManipulator(state.Angles, MainVM.Animate);

            if (state.PortalPosition != null)
                MainVM.Detector.MoveDetectFrame(state.PortalPosition, MainVM.Animate);
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
                MainVM.TargetX = p.point.X;
                MainVM.TargetY = p.point.Y;
                MainVM.TargetZ = p.point.Z;
            }
        }

        public void OnManipulatorPointSelected(ScanPoint p)
        {
            if (p != null && this.targetPoint != null)
            {
                Point3D manipPoint = p.point;
                Point3D scanPoint = this.targetPoint.point;

                recalculateKinematic(manipPoint, scanPoint);
            }
        }

        public void Deteil_PositionChenged()
        {
            this.collisoinDetector.FindCollisoins();
        }

        /// <summary>
        /// Обработчик изменения положения детектора при ручном управлении
        /// </summary>
        public void DetecterManual_PositionChanged()
        {
            this.collisoinDetector.FindCollisoins();
        }

        public void SetDistanceToPoint()
        {
            var focusMm =targetPoint.point.DistanceTo(MainVM.ManipulatorCamPoint.GetTargetPoint());
            MainVM.Focus = Math.Round(focusMm,2);
        }

        /// <summary>
        /// Обработчик изменения положения манипулятора.
        /// Вызывается при любом изменении положения. И при ручном и при автоматическом
        /// </summary>
        public void Manipulator_PositionChanged()
        {
            double distanceToPoint = targetPoint.point.DistanceTo(MainVM.ManipulatorCamPoint.GetTargetPoint());
            coneModel.ChangePosition(MainVM.Manipulator.GetCameraPosition(), MainVM.Manipulator.GetCameraDirection(), distanceToPoint);
            MainVM.ManipulatorVM.Z = ManipulatorPositionController.T1GlobalToLocal(MainVM.Manipulator.TablePosition);
            MainVM.ManipulatorVM.X = ManipulatorPositionController.T2GlobalToLocal(MainVM.Manipulator.MiddleEdgePosition);
            MainVM.ManipulatorVM.Y = ManipulatorPositionController.T3GlobalToLocal(MainVM.Manipulator.TopEdgePosition);
            //MainVM.ManipulatorVM.X = ManipulatorPositionController.T1GlobalToLocal(MainVM.Manipulator.TablePosition);
            //MainVM.ManipulatorVM.Y = ManipulatorPositionController.T2GlobalToLocal(MainVM.Manipulator.MiddleEdgePosition);
            //MainVM.ManipulatorVM.Z = ManipulatorPositionController.T3GlobalToLocal(MainVM.Manipulator.TopEdgePosition);
            MainVM.ManipulatorVM.A = ManipulatorPositionController.T4GlobalToLocal(MainVM.Manipulator.CameraBasePosition);
            MainVM.ManipulatorVM.B = ManipulatorPositionController.T5GlobalToLocal(MainVM.Manipulator.CameraPosition);
            collisoinDetector.FindCollisoins();

            SetDistanceToPoint();

            SetCameraPosition();
        }


        /// <summary>
        /// Устанавливает позицию наконечника манипулятора в TextBox-ы 
        /// </summary>
        public void SetCameraPosition()
        {
            var cameraPosition = MainVM.Manipulator.GetCameraPosition();
            MainVM.ManipulatorX = cameraPosition.X;
            MainVM.ManipulatorY = cameraPosition.Y;
            MainVM.ManipulatorZ = cameraPosition.Z;
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
            ManipulatorV2 m = MainVM.Manipulator;

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

        private void resetManip()
        {
            MainVM.Manipulator.ResetModel();
            MainVM.Detector.ResetTransforms();
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

        
        private void ShowBorders_Click()//(object sender, RoutedEventArgs e)
        {
            
            //ManipulatorVisualizer.showBorders(detectorFrame.GetDetectorFramePart(Parts.VerticalFrame),i);
            //ManipulatorVisualizer.showBorders(manipulator.GetManipulatorPart(ManipulatorParts.Camera),i);
            //ManipulatorVisualizer.showBorders(detail.detailModel,i);
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
            MainVM.Animate = true;
        }

        private void ToggleAnimation_Unchecked(object sender, RoutedEventArgs e)
        {
            solveKinematics = true;
            MainVM.Animate = false;
        }

        private void recalculateKinematic()
        {
            try
            {
                Point3D manipPoint = MainVM.ManipulatorCamPoint.GetTargetPoint();
                Point3D scannedPoint = this.targetPoint.point;
                kinematicWorker.Solve(new SystemPosition(manipPoint, scannedPoint, MainVM.Focus, MainVM.FocusEnlagment));
            }

            /*ManipulatorCamPoint может быть null когда инициализируется окно, и срабатывает листенер 
                у слайдера    
             */
            catch (NullReferenceException ex) { }
        }

        private void recalculateKinematic(Point3D manipulatorPoint, Point3D scannedPoint)
        {
            try
            {
                kinematicWorker.Solve(new SystemPosition(manipulatorPoint, scannedPoint, MainVM.Focus, MainVM.FocusEnlagment));
            }
            catch (NullReferenceException ex) { }
        }

        private void FocusEnlargementSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MainVM.FocusEnlagment = e.NewValue;
            recalculateKinematic();
        }

        private void FocusDistanceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Фокусное расстояние
            var distance = e.NewValue;

            //Точка наконечника манипулятора
            var manipCurrentPoint = MainVM.ManipulatorCamPoint.GetTargetPoint();

            //Точка сканирования
            var targetPoint = this.targetPoint.point;

            //Нахождение вектора направления
            var direction = new Vector3D(manipCurrentPoint.X - targetPoint.X,
                manipCurrentPoint.Y - targetPoint.Y, manipCurrentPoint.Z - targetPoint.Z);

            //Нормализация вектора
            direction.Normalize();

            //Получение нового вектора
            var newVector = Vector3D.Multiply(direction, distance);

            var point = new Point3D(newVector.X + targetPoint.X,
                newVector.Y + targetPoint.Y, newVector.Z + targetPoint.Z);

            //Перемещение точки наконечника 
            MainVM.ManipulatorCamPoint.MoveAndNotify(point);
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
            this.detailView.Owner = this;
        }

        private void ResetCamers_Click(object sender, RoutedEventArgs e)
        {
            ManipulatorVisualizer.setCameras(allModels);
        }

        /// <summary>
        /// Обработчик события - выбор детали шпангоут
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShpangountDetail_Click(object sender, RoutedEventArgs e)
        {
            DetailModel shpangout = ModelParser.ShpangoutDetail;
            if (!ManipulatorVisualizer.IsModelExist(shpangout.detailModel))
            {
                ManipulatorVisualizer.RemoveModel(ModelParser.LopatkaDetail.detailModel);
                collisoinDetector.Detail = ModelParser.ShpangoutDetail;
                MainVM.DetailVM.Detail = ModelParser.ShpangoutDetail;
                ManipulatorVisualizer.AddModel(ModelParser.ShpangoutDetail.detailModel);
                this.collisoinVisual.Detail = DetailVisualFactory.CreateDetailVisual(shpangout, ModelParser.DetailPlatform);
                this.detail = ModelParser.ShpangoutDetail;
                this.detailView.RemoveDetailMode(ModelParser.LopatkaDetail);
                this.detailView.AddDetailModel(shpangout);
            }
        }

        /// <summary>
        /// Обработчик события - выбор детали лопатка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LopatkaDetail_Click(object sender, RoutedEventArgs e)
        {
            DetailModel lopatka = ModelParser.LopatkaDetail;
            if (!ManipulatorVisualizer.IsModelExist(lopatka.detailModel))
            {
                ManipulatorVisualizer.RemoveModel(ModelParser.ShpangoutDetail.detailModel);
                collisoinDetector.Detail = ModelParser.LopatkaDetail;
                MainVM.DetailVM.Detail = ModelParser.LopatkaDetail;
                ManipulatorVisualizer.AddModel(ModelParser.LopatkaDetail.detailModel);
                this.detail = ModelParser.LopatkaDetail;
                this.detailView.RemoveDetailMode(ModelParser.ShpangoutDetail);
                this.detailView.AddDetailModel(lopatka);
                this.collisoinVisual.Detail = DetailVisualFactory.CreateDetailVisual(lopatka, ModelParser.DetailPlatform);
            }
        }
    }
}

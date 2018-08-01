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
        private bool animate = false;
        public bool Animate
        {
            get { return animate; }
            set
            {
                animate = value;

                NotifyPropertyChanged("Animate");
            }
        }

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

        /// <summary>
        /// Собирает отдельные модели из списка всех мешей
        /// </summary>
        private ModelParser ModelParser;

        private double focus = 50;
        public double Focus
        {
            get { return focus; }
            set
            {
                focus = value;
                NotifyPropertyChanged("Focus");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private double focuseEnlagment = 1d;
        public double FocusEnlagment
        {
            get { return focuseEnlagment; }
            set
            {
                focuseEnlagment = value;
                NotifyPropertyChanged("FocusEnlagment");
            }
        }

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
            detectorFrame = ModelParser.Frame;
            DetectorFrameVisual portalVisual = DetectorFrameVisualFactory.CreateDetectorFrameVisual(detectorFrame);

            detectorFrame.onPositionChanged += Detector_PositionChanged;
            detectorFrame.onManulaPositionChanged += DetecterManual_PositionChanged;
            ManipulatorVisualizer.SetDetectFrameModel(detectorFrame, portalVisual);

            manipulator = ModelParser.Manipulator;
            ManipulatorVisual manipulatorVisual = ManipulatorVisualFactory.CreateManipulator(manipulator);

            ManipulatorVisualizer.setManipulatorModel(manipulator, manipulatorVisual);
            manipulator.onPositionChanged += Manipulator_PositionChanged;
            manipulator.onManulaPositionChanged += ManipulatorManual_PositinChanged;

            ManipulatorVisualizer.AddModel(ModelParser.Others);

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

            detail = ModelParser.LopatkaDetail;
            detail.onPositionChanged += Deteil_PositionChenged;
            platform = ModelParser.DetailPlatform;

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

            ManipulatorVisualizer.AddModel(ModelParser.Others);

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
            if (state.Angles != null)
                manipulator.MoveManipulator(state.Angles, Animate);

            if (state.PortalPosition != null)
                detectorFrame.MoveDetectFrame(state.PortalPosition, Animate);
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
                TargetXSlider.Value = Math.Round(p.point.X, 3);
                TargetYSlider.Value = Math.Round(p.point.Y, 3);
                TargetZSlider.Value = Math.Round(p.point.Z, 3);
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
            var focusMm =targetPoint.point.DistanceTo(manipulatorCamPoint.GetTargetPoint());
            this.Focus = Math.Round(focusMm,2);
        }

        /// <summary>
        /// Обработчик изменения положения манипулятора.
        /// Вызывается при любом изменении положения. И при ручном и при автоматическом
        /// </summary>
        public void Manipulator_PositionChanged()
        {
            double distanceToPoint = targetPoint.point.DistanceTo(manipulatorCamPoint.GetTargetPoint());
            coneModel.ChangePosition(manipulator.GetCameraPosition(), manipulator.GetCameraDirection(), distanceToPoint);

            T1Slider.Value = ManipulatorPositionController.T1GlobalToLocal(manipulator.TablePosition);
            T2Slider.Value = ManipulatorPositionController.T2GlobalToLocal(manipulator.MiddleEdgePosition);
            T3Slider.Value = ManipulatorPositionController.T3GlobalToLocal(manipulator.TopEdgePosition);
            T4Slider.Value = ManipulatorPositionController.T4GlobalToLocal(manipulator.CameraBasePosition);
            T5Slider.Value = ManipulatorPositionController.T5GlobalToLocal(manipulator.CameraPosition);
            collisoinDetector.FindCollisoins();

            SetDistanceToPoint();

            SetCameraPosition();
        }


        /// <summary>
        /// Устанавливает позицию наконечника манипулятора в TextBox-ы 
        /// </summary>
        public void SetCameraPosition()
        {
            var cameraPosition = this.manipulator.GetCameraPosition();

            ManipulatorXSlider.Value = Math.Round(cameraPosition.X, 3);
            ManipulatorYSlider.Value = Math.Round(cameraPosition.Y, 3);
            ManipulatorZSlider.Value = Math.Round(cameraPosition.Z, 3);
        }

        /// <summary>
        /// Обработчик изменения пложения манипулятора при ручном управлении 
        /// </summary>
        public void ManipulatorManual_PositinChanged()
        {
            this.manipulatorCamPoint.Move(manipulator.GetCameraPosition());
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
            animate = true;
        }

        private void ToggleAnimation_Unchecked(object sender, RoutedEventArgs e)
        {
            solveKinematics = true;
            animate = false;
        }

        private void Manipulator_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Point3D position = new Point3D()
            {
                X = ManipulatorXSlider.Value,
                Y = ManipulatorYSlider.Value,
                Z = ManipulatorZSlider.Value
            };

            this.manipulatorCamPoint.Move(position);
         
        }

        private void recalculateKinematic()
        {
            try
            {
                Point3D manipPoint = manipulatorCamPoint.GetTargetPoint();
                Point3D scannedPoint = this.targetPoint.point;
                kinematicWorker.Solve(new SystemPosition(manipPoint, scannedPoint, Focus, focuseEnlagment));
            }

            /*ManipulatorCamPoint может быть null когда инициализируется окно, и срабатывает листенер 
                у слайдера    
             */
            catch (NullReferenceException ex) { }
        }

        private void FocusEnlargementSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.FocusEnlagment = e.NewValue;
            recalculateKinematic();
        }

        private void FocusDistanceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Фокусное расстояние
            var distance = e.NewValue;

            //Точка наконечника манипулятора
            var manipCurrentPoint = manipulatorCamPoint.GetTargetPoint();

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
            manipulatorCamPoint.MoveAndNotify(point);
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

        private void RotateDetailSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.detailPathController.Rotate(e.NewValue);
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
                detailPathController.Detail = ModelParser.ShpangoutDetail;
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
                detailPathController.Detail = ModelParser.LopatkaDetail;
                ManipulatorVisualizer.AddModel(ModelParser.LopatkaDetail.detailModel);
                this.detail = ModelParser.LopatkaDetail;
                this.detailView.RemoveDetailMode(ModelParser.ShpangoutDetail);
                this.detailView.AddDetailModel(lopatka);
                this.collisoinVisual.Detail = DetailVisualFactory.CreateDetailVisual(lopatka, ModelParser.DetailPlatform);
            }
        }
        
        /// <summary>
        /// Обработка события - поднимает деталь 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiseDetail_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var mmValue = DetectorPositionController.MmToSm(e.NewValue);
            var transoform = new TranslateTransform3D(0, mmValue, 0);
            this.detailPathController.Transform(transoform);
        }
    }
}

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

namespace InverseTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// Длины ребер манипулятора 
        /// </summary>
        private static double MANIP_EDGE_LENGTH_1;
        private static double MANIP_EDGE_LENGTH_2;
        private static double MANIP_EDGE_LENGTH_3;
        private static double MANIP_EDGE_LENGTH_4;
        private static double MANIP_EDGE_LENGTH_5;


        /// <summary>
        /// Откланение от центра координат
        /// </summary>
        private static Point3D MANIPULATOR_OFFSET = new Point3D(-80, 0, 0);


        private bool solveKinematics = true;
        private bool animate = false;


        private Kinematic manipKinematic;

        private IManipulatorModel manipulator;
        private IDetectorFrame detectorFrame;
        private IMovementPoint scanPoint;
        private IMovementPoint manipulatorCamPoint;
        private IConeModel coneModel;
        private Model3DGroup platform = new Model3DGroup();
        //Точка сканирования 
        private Model3D targetBox;
        //Точка камеры манипулятора
        private Model3D pointManip;
        //Точка в которую должен встать экран портала
        private Model3D pointPortal;

        private DetailModel detail;

        private ManipulatorKinematicWorker<SystemPosition> manipWorker;
        private CollisionWorker<DummyCrutch> collisionWorker;

        private double distanceToScreen = 50;
        private double focuseEnlagment = 1;
        Model3DGroup allModels;



        private ObservableCollection<SystemPosition> targetPoints { get; set; }

        private int selectedIndexPoint = -1;
        private List<UIElement> childrens;

        int numMesh = 0;

        public MainWindow()
        {
            InitializeComponent();
            this.targetPoints = new ObservableCollection<SystemPosition>();
            TargetPointsListView.ItemsSource = targetPoints;
            this.Loaded += this.MainWindowLoaded;
        }

        public void MainWindowLoaded(object sender, RoutedEventArgs arg)
        {

            allModels = new ModelImporter().Load("./3DModels/Detector Frame.obj");
            Model3DGroup newAllModels = new Model3DGroup();

            ManipulatorVisualizer.setCameras(allModels);




            ModelParser parser = new ModelParser(allModels);
            parser.Parse();
            detectorFrame = parser.frame;
            detectorFrame.onPositionChanged += OnDetectorFramePositionChanged;
            ManipulatorVisualizer.setDetectFrameModel(detectorFrame);

            manipulator = parser.manipulator;
            manipulator.onPositionChanged += OnManipulatorPisitionChanged;
            ManipulatorVisualizer.setManipulatorModel(manipulator);

            ManipulatorVisualizer.AddModel(parser.others);

            //Вычисляет длины ребер манипулятора для вычисления кинематики
            CalculateEdgesLength(manipulator);
            this.manipKinematic = new Kinematic(MANIPULATOR_OFFSET.X, MANIPULATOR_OFFSET.Y, MANIPULATOR_OFFSET.Z);
            this.manipKinematic.setLen(MANIP_EDGE_LENGTH_1, MANIP_EDGE_LENGTH_2, MANIP_EDGE_LENGTH_3, MANIP_EDGE_LENGTH_4, MANIP_EDGE_LENGTH_5);
            this.manipKinematic.det = 8.137991;
            this.manipWorker = new ManipulatorKinematicWorker<SystemPosition>(manipKinematic);
            this.manipWorker.kinematicSolved += manipulatorSolved;

            detail = parser.detail;




            Model3DGroup mg = new Model3DGroup();
            mg.Children = new Model3DCollection();
            mg.Children.Add(detail.GetModel());


            //ModelPreprocessor prerpocessor = new ModelPreprocessor(mg);
            //var f = new Func<Model3DGroup>(delegate ()
            //{
            //    return prerpocessor.Simplify(0.1).GetProccessedModel();
            //});
            //DispatcherOperation<Model3DGroup> dispOper = Dispatcher.InvokeAsync<Model3DGroup>(f);
            //dispOper.Wait();
            //Model3DGroup simplDetail = dispOper.Result;



            ManipulatorVisualizer.AddModel(detail.GetModel());

            ManipulatorVisualizer.AddModel(parser.others);
            collisionWorker = new CollisionWorker<DummyCrutch>(this.manipulator, this.detectorFrame, this.detail, new Model3DGroup());

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

            manipulatorCamPoint.MoveToPositoin(new Point3D(-10, 60, 0));
            scanPoint.MoveToPositoin(new Point3D(0, 60, 0));

            collisionWorker.onCollision += OnCollisoinsDetected;


        }

        public void OnCollisoinsDetected(List<Except> collisions)
        {
            if (collisions?.Count > 0)
            {
                CollisionTextBox.Text = collisions[0].S1 + " " + collisions[0].S2;
            }
        }

     
        /// <summary>
        /// Вычисляет размеры ребер манипулятора
        /// </summary>
        /// <param name="manipulator"></param>
        private void CalculateEdgesLength(IManipulatorModel manipulator)
        {
            MANIP_EDGE_LENGTH_1 = manipulator.GetPointJoint(ManipulatorV2.ManipulatorRotatePoints.POINT_ON_TABLE).Y;

            MANIP_EDGE_LENGTH_2 = manipulator.GetPointJoint(ManipulatorV2.ManipulatorRotatePoints.POINT_ON_TABLE)
                .DistanceTo(manipulator.GetPointJoint(ManipulatorV2.ManipulatorRotatePoints.POINT_ON_MAIN_EDGE));

            Point3D pointOnMainEdge = manipulator.GetPointJoint(ManipulatorV2.ManipulatorRotatePoints.POINT_ON_MAIN_EDGE);
            Point3D pointBelowCam = manipulator.GetPointJoint(ManipulatorV2.ManipulatorRotatePoints.POINT_BELOW_CAMERA);

            //Точка над основным ребром на уровне точки под камерой
            Point3D point1 = new Point3D(pointOnMainEdge.X, pointOnMainEdge.Y, pointBelowCam.Z);
            MANIP_EDGE_LENGTH_3 = point1.DistanceTo(manipulator.GetPointJoint(ManipulatorV2.ManipulatorRotatePoints.POINT_BELOW_CAMERA));

            Point3D pointCamera = manipulator.GetPointJoint(ManipulatorV2.ManipulatorRotatePoints.POINT_ON_CAMERA);
            //Точка на уровне камеры, 
            Point3D point = new Point3D(pointBelowCam.X, pointCamera.Y, pointBelowCam.Z);
            MANIP_EDGE_LENGTH_4 = pointBelowCam.DistanceTo(point);
            MANIP_EDGE_LENGTH_5 = point.DistanceTo(pointCamera);
        }

        public void manipulatorSolved(ManipulatorAngles angles)
        {
            manipulator.MoveManipulator(angles, false);
        }

        /// <summary>
        /// Вызывается каждый раз когда "портал" меняет свое положение
        /// </summary>
        public void OnDetectorFramePositionChanged()
        {
            collisionWorker.findCollision(new DummyCrutch(this.manipulator, this.detectorFrame, this.detail, new Model3DGroup()));

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

            T1TextBox.Text = Math.Round(anglesState[ManipulatorV2.ManipulatorParts.Table], 3).ToString();
            T2TextBox.Text = Math.Round(anglesState[ManipulatorV2.ManipulatorParts.MiddleEdge], 3).ToString();
            T3TextBox.Text = Math.Round(anglesState[ManipulatorV2.ManipulatorParts.TopEdge], 3).ToString();
            T4TextBox.Text = Math.Round(anglesState[ManipulatorV2.ManipulatorParts.CameraBase], 3).ToString();
            T5TextBox.Text = Math.Round(anglesState[ManipulatorV2.ManipulatorParts.Camera], 3).ToString();
            collisionWorker.findCollision(new DummyCrutch(this.manipulator, this.detectorFrame, this.detail, new Model3DGroup()));
        }

        /// <summary>
        /// Вызываетсяс каждый раз когда изменяется позиция точки в которую становится манипулятор
        /// </summary>
        /// <param name="newPosition"></param>
        public void OnManipulatorCamPointPositoinChanged(Point3D newPosition)
        {
            PointManipulatorXTextBox.Text = Math.Round(newPosition.X, 3).ToString();
            PointManipulatorYTextBox.Text = Math.Round(newPosition.Y, 3).ToString();
            PointManipulatorZTextBox.Text = Math.Round(newPosition.Z, 3).ToString();

            if (solveKinematics)
            {

                this.manipWorker.solve(new SystemPosition(newPosition, scanPoint.GetTargetPoint()));
                SolvePortalKinematic(newPosition, scanPoint.GetTargetPoint(), false);
            }

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
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.Table, -e.NewValue);
            T1TextBox.Text = e.NewValue.ToString();
        }

        private void T2Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.MiddleEdge, -e.NewValue);
            T2TextBox.Text = e.NewValue.ToString();
        }

        private void T3Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.TopEdge, -e.NewValue);
            T3TextBox.Text = e.NewValue.ToString();
        }

        private void T4Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.CameraBase, -e.NewValue);
            T4TextBox.Text = e.NewValue.ToString();
        }

        private void T5Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.Camera, -e.NewValue);
            T5TextBox.Text = e.NewValue.ToString();
        }

        /// <summary>
        /// Считываем координаты точки из полей ввода
        /// </summary>
        private void ParsePointsAndMove()
        {
            try
            {
                double x, y, z;
                Console.WriteLine("ParsePointAndMove");
                double.TryParse(TargetPointXTextBox.Text, out x);
                double.TryParse(TargetPointYTextBox.Text, out y);
                double.TryParse(TargetPointZTextBox.Text, out z);
                scanPoint.MoveToPositoin(new Point3D(x, y, z));

                double manip_x, manip_y, manip_z;
                double.TryParse(PointManipulatorXTextBox.Text, out manip_x);
                double.TryParse(PointManipulatorYTextBox.Text, out manip_y);
                double.TryParse(PointManipulatorZTextBox.Text, out manip_z);
                manipulatorCamPoint.MoveToPositoin(new Point3D(manip_x, manip_y, manip_z));
            }
            catch (NullReferenceException ex)
            { }
        }


        private void SolvePortalKinematic(Point3D manip, Point3D scannedPoint, bool animate)
        {
            PortalKinematic p = new PortalKinematic(500, 500, 500, 140, 10, 51, 10, 0, 30);
            p.setPointManipAndNab(manip.X, manip.Z, manip.Y, scannedPoint.X, scannedPoint.Z, scannedPoint.Y);

            double[] rez = p.portalPoint(manip.DistanceTo(scannedPoint), this.focuseEnlagment);
            if (rez != null)
            {
                DetectorFramePosition detectp = new DetectorFramePosition(new Point3D(rez[5], rez[7], rez[6]), -rez[4], -rez[3]);
                detectorFrame.MoveDetectFrame(detectp, animate);
            }
            else
            {
                MessageBox.Show("Не существует такой точки");
            }
        }

        private void SolveManipulatorKinematic(Point3D manip, Point3D scannedPoint, bool animatation)
        {
            Stack<double[]> rezults;
            Console.WriteLine("ScanndedPOint: " + scannedPoint.ToString());
            rezults = this.manipKinematic.InverseNab(manip.X, manip.Z, manip.Y, scannedPoint.X, scannedPoint.Z, scannedPoint.Y);

            //TODO Перенести проверку ограничений в библиотеку кинематики, добавить функцию для задания ограничений
            // по умолчанию сделать все ограничения int.MaxValue. Если позиция не достижима то выкидывать исключение
            // PositionUnattainableException
            if (rezults.Count > 0)
            {
                Stack<double[]> satisfied = new Stack<double[]>();

                foreach (double[] one in rezults)
                {

                    if (
                       (MathUtils.RadiansToAngle(one[0]) < 90 && MathUtils.RadiansToAngle(one[0]) > -90) &
                       (MathUtils.RadiansToAngle(one[1]) < 90 && MathUtils.RadiansToAngle(one[1]) > -90) &
                       (MathUtils.RadiansToAngle(one[2]) < 70 && MathUtils.RadiansToAngle(one[2]) > -70) &
                       (MathUtils.RadiansToAngle(one[3]) < 220 && MathUtils.RadiansToAngle(one[3]) > -220) &
                       (MathUtils.RadiansToAngle(one[4]) < 170 && MathUtils.RadiansToAngle(one[4]) > 0)
                       )
                    {
                        satisfied.Push(one);
                        Position.Background = Brushes.Green;
                    }
                    else
                    {
                        satisfied.Push(one);
                        Position.Background = Brushes.Red;
                    }
                }

                if (satisfied.Count > 0)
                {
                    double[] rez = satisfied.Pop();
                    ManipulatorAngles angles = new ManipulatorAngles(
                        MathUtils.RadiansToAngle(rez[0]),
                        MathUtils.RadiansToAngle(rez[1]),
                        MathUtils.RadiansToAngle(rez[2]),
                        MathUtils.RadiansToAngle(rez[3]),
                        MathUtils.RadiansToAngle(rez[4])
                        );

                    manipulator.MoveManipulator(angles, animatation);
                }
                //else
                //{
                //    MessageBox.Show(String.Format("Ошибка: манипулятор не может достигнуть позиции [{0}; {1}; {2}]\nи наблюдать за точкой [{3}; {4}; {5}]",
                //        manip.X, manip.Y, manip.Z, scannedPoint.X, scannedPoint.X, scannedPoint.X));
                //}
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

        private void EditPoint_Click(object sender, RoutedEventArgs e)
        {

            if (targetPoints.Count > 0 && selectedIndexPoint >= 0)
            {
                selectedIndexPoint = TargetPointsListView.SelectedIndex;
                SystemPosition point = targetPoints[selectedIndexPoint];
                TargetPointXTextBox.Text = point.targetPoint.X.ToString();
                TargetPointYTextBox.Text = point.targetPoint.Y.ToString();
                TargetPointZTextBox.Text = point.targetPoint.Z.ToString();

                PointManipulatorXTextBox.Text = point.manipPoint.X.ToString();
                PointManipulatorYTextBox.Text = point.manipPoint.Y.ToString();
                PointManipulatorZTextBox.Text = point.manipPoint.Z.ToString();



                if (selectedIndexPoint != -1)
                {

                    Button confirm = new Button();
                    confirm.Content = "Принять";
                    confirm.Click += new RoutedEventHandler(onConfirmChangesClick);

                    Grid.SetColumn(confirm, 0);

                    Button cancel = new Button();
                    cancel.Content = "Отменить";
                    cancel.Click += new RoutedEventHandler(onCancelChangesClick);

                    Grid.SetColumn(cancel, 1);



                    childrens = new List<UIElement>();
                    for (int i = 0; i < TargetPointsListButtonsGrid.Children.Count; i++)
                        childrens.Add(TargetPointsListButtonsGrid.Children[i]);



                    TargetPointsListButtonsGrid.Children.Clear();
                    TargetPointsListButtonsGrid.ColumnDefinitions.Clear();

                    TargetPointsListButtonsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    TargetPointsListButtonsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    TargetPointsListButtonsGrid.Children.Add(confirm);
                    TargetPointsListButtonsGrid.Children.Add(cancel);


                }
            }

        }


        //Запрогать загрузку и замену детальки
        private void LoadModel_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }


        private void returnNormalGrid()
        {
            TargetPointsListButtonsGrid.Children.Clear();
            TargetPointsListButtonsGrid.ColumnDefinitions.Clear();
            ColumnDefinition column1 = new ColumnDefinition();
            column1.Width = new GridLength(53.3);
            ColumnDefinition column2 = new ColumnDefinition();
            column2.Width = new GridLength(53.3);
            ColumnDefinition column3 = new ColumnDefinition();
            column3.Width = new GridLength(53.3);


            TargetPointsListButtonsGrid.ColumnDefinitions.Add(column1);
            TargetPointsListButtonsGrid.ColumnDefinitions.Add(column2);
            TargetPointsListButtonsGrid.ColumnDefinitions.Add(column3);

            for (int i = 0; i < childrens.Count; i++)
                TargetPointsListButtonsGrid.Children.Add(childrens[i]);


        }

        private void onCancelChangesClick(object sender, RoutedEventArgs e)
        {
            returnNormalGrid();
        }

        private void onConfirmChangesClick(object sender, RoutedEventArgs e)
        {
            double x, y, z;

            double.TryParse(TargetPointXTextBox.Text, out x);
            double.TryParse(TargetPointYTextBox.Text, out y);
            double.TryParse(TargetPointZTextBox.Text, out z);

            double x2, y2, z2;
            double.TryParse(PointManipulatorXTextBox.Text, out x2);
            double.TryParse(PointManipulatorYTextBox.Text, out y2);
            double.TryParse(PointManipulatorZTextBox.Text, out z2);


            targetPoints[selectedIndexPoint] = new SystemPosition(new Point3D(x2, y2, z2), new Point3D(x, y, z));

            returnNormalGrid();
        }

        private void DeletePoint_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = TargetPointsListView.SelectedIndex;
            if (selectedIndex != -1)
                targetPoints.RemoveAt(selectedIndex);

        }

        private void TargetPointsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedIndexPoint = TargetPointsListView.SelectedIndex;
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
            ManipulatorVisualizer.showBordersPortal(detectorFrame);
            ManipulatorVisualizer.showBordersPortal(manipulator);
        }

        private void VerticalFrameSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            detectorFrame.MovePart(DetectorFrame.Parts.VerticalFrame, e.NewValue);
            VerticalFrameSliderTextBox.Text = e.NewValue.ToString();
        }

        private void HorizontalBarSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            detectorFrame.MovePart(DetectorFrame.Parts.HorizontalBar, e.NewValue);
            HorizontalBarTextView.Text = e.NewValue.ToString();
        }

        private void ScreenHolderSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            detectorFrame.MovePart(DetectorFrame.Parts.ScreenHolder, e.NewValue);
            ScreenHolderTextBox.Text = e.NewValue.ToString();
        }

        private void ScreenVerticalAngleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            detectorFrame.RotatePart(DetectorFrame.Parts.Screen, e.NewValue, DetectorFrame.ZRotateAxis);
            ScreenVerticalAngleTextBox.Text = e.NewValue.ToString();
        }

        private void ScreenHorizontalAngleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            detectorFrame.RotatePart(DetectorFrame.Parts.Screen, e.NewValue, DetectorFrame.YRotateAxis);
            ScreenHorizontalAngleTextBox.Text = e.NewValue.ToString();
        }

        private void MoveMesh_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //   ((IDebugModels)detectorFrame).transformModel(e.NewValue);
            // allModels.Children[numMesh].Transform = new TranslateTransform3D(0, (int)e.NewValue, 0);
        }

        private void NumMesh_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            numMesh = (int)e.NewValue;
            // ((IDebugModels)detectorFrame).addNumberMesh(numMesh);
            //NumMeshTextBox.Text = numMesh.ToString();

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
                manipulatorCamPoint.MoveToPositoin(point.manipPoint);
                scanPoint.MoveToPositoin(point.targetPoint);
            }
        }

        private void recalculateKinematic()
        {
            Point3D manip = manipulatorCamPoint.GetTargetPoint();
            Point3D targetPoint = scanPoint.GetTargetPoint();

            SolveManipulatorKinematic(manip, targetPoint, animate);
            SolvePortalKinematic(manip, targetPoint, animate);
        }

        private void FocusEnlargementSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.focuseEnlagment = e.NewValue;
            recalculateKinematic();
        }

        private void FocusDistanceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.distanceToScreen = e.NewValue;
            recalculateKinematic();
        }

        private void FocusDistance_Checked(object sender, RoutedEventArgs e)
        {
            FocusDistancePopup.IsOpen = true;
        }

        private void FocusDistance_Unchecked(object sender, RoutedEventArgs e)
        {
            FocusDistancePopup.IsOpen = false;

        }


    }
}

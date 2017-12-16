﻿using System;
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

namespace InverseTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int PORTAL_START_INDEX = 29;
        private const int PORTAL_END_INDEX = 59;
        private const int MANIPULATOR_START_INDEX = 9;
        private const int MANIPULATOR_END_INDEX = 29;
        private const int LOPATKA_INDEX = 8;

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



        private Kinematic manipKinematic;

        private IManipulatorModel manipulator;
        private IDetectorFrame detectorFrame;
        private IMovementPoint scanPoint;
        private IMovementPoint manipulatorCamPoint;
        private IConeModel coneModel;
        private Model3DGroup platform=new Model3DGroup();
        //Точка сканирования 
        private Model3D targetBox;
        //Точка камеры манипулятора
        private Model3D pointManip;
        //Точка в которую должен встать экран портала
        private Model3D pointPortal;

        private DetailModel detail;

        
        private ObservableCollection<Point3D> targetPoints { get; set; }

        private int selectedIndexPoint = -1;
        private List<UIElement> childrens;
        
        Model3DGroup allModels;
        int numMesh = 0;

        public MainWindow()
        {
            InitializeComponent();
            allModels = new ModelImporter().Load("./3DModels/Detector Frame.obj");
            ManipulatorVisualizer.setCameras(allModels);

            Model3DGroup portal = new Model3DGroup();
            portal.Children = new Model3DCollection(allModels.Children.ToList().GetRange(PORTAL_START_INDEX, PORTAL_END_INDEX - PORTAL_START_INDEX));
            portal.Children.Add(allModels.Children[62]);

            detectorFrame = new DetectorFrame(portal);
            detectorFrame.onPositionChanged += OnDetectorFramePositionChanged;
            ManipulatorVisualizer.setDetectFrameModel(detectorFrame);


            //Определяем модельку манипулятора
            Model3DGroup manipulatorGroup = new Model3DGroup();
            manipulatorGroup.Children = new Model3DCollection(allModels.Children.ToList()
             .GetRange(MANIPULATOR_START_INDEX, MANIPULATOR_END_INDEX - MANIPULATOR_START_INDEX));
            manipulatorGroup.Children.Add(allModels.Children[0]);
            manipulatorGroup.Children.Add(allModels.Children[59]);
            manipulatorGroup.Children.Add(allModels.Children[60]);
            manipulatorGroup.Children.Add(allModels.Children[61]);
            manipulator = new ManipulatorV2(manipulatorGroup);
            manipulator.onPositionChanged += OnManipulatorPisitionChanged;
            ManipulatorVisualizer.setManipulatorModel(manipulator);

            //Вычисляет длины ребер манипулятора для вычисления кинематики
            CalculateEdgesLength(manipulator);
            this.manipKinematic = new Kinematic(MANIPULATOR_OFFSET.X, MANIPULATOR_OFFSET.Y, MANIPULATOR_OFFSET.Z);
            this.manipKinematic.setLen(MANIP_EDGE_LENGTH_1, MANIP_EDGE_LENGTH_2, MANIP_EDGE_LENGTH_3, MANIP_EDGE_LENGTH_4, MANIP_EDGE_LENGTH_5);


            Collision collisions = new Collision();

            Model3D lopatka = allModels.Children[LOPATKA_INDEX];
            detail = new DetailModel(lopatka);
            ManipulatorVisualizer.AddModel(detail.GetModel());
            
            //Добавляем остальные мешы
            Model3DGroup others = new Model3DGroup();
            List<Model3D> othersModels = new List<Model3D>();
            othersModels.AddRange(allModels.Children.ToList().GetRange(0, LOPATKA_INDEX-1));
            othersModels.AddRange(allModels.Children.ToList().GetRange(LOPATKA_INDEX + 1, MANIPULATOR_START_INDEX - (LOPATKA_INDEX - 1)));
            others.Children = new Model3DCollection(othersModels);
            ManipulatorVisualizer.AddModel(others);

            

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
            ManipulatorVisualizer.AddConeFromCamera(coneModel.GetModel());
            coneModel.ChangePosition(manipulator.GetCameraPosition(),
                manipulator.GetCameraDirection(),
                manipulatorCamPoint.GetTargetPoint().DistanceTo(scanPoint.GetTargetPoint()));

            manipulatorCamPoint.MoveToPositoin(new Point3D(-10, 60, 0));
            scanPoint.MoveToPositoin(new Point3D(0, 60, 0));

            
            foreach (ManipulatorV2.ManipulatorParts part in Enum.GetValues(typeof(ManipulatorV2.ManipulatorParts)))
            {
                collisions.BuildShell((Model3DGroup)manipulator.GetManipulatorPart(part));
            }
            foreach (DetectorFrame.Parts part_frame in Enum.GetValues(typeof(DetectorFrame.Parts)))
            {
                collisions.BuildShell((Model3DGroup)detectorFrame.GetDetectorFramePart(part_frame));
            }

            Model3DGroup detailNewGroup = new Model3DGroup();
            //  detailNewGroup.Children.Add(detail.GetModel());
            // collisions.BuildShell(detailNewGroup);

            // collisions.BuildShell(platform);

            //   collisions.DisplayConvexHull();

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


        /// <summary>
        /// Вызывается каждый раз когда "портал" меняет свое положение
        /// </summary>
        public void OnDetectorFramePositionChanged()
        {
            //Find_Collision();
        }
        
        /// <summary>
        /// Вызывается каждый раз когда манипулятор меняет свое положение
        /// </summary>
        public void OnManipulatorPisitionChanged()
        {

            //manipulatorCamPoint.MoveToPositoin(manipulator.GetCameraPosition());
            double distanceToPoint = scanPoint.GetTargetPoint().DistanceTo(manipulatorCamPoint.GetTargetPoint());
            coneModel.ChangePosition(manipulator.GetCameraPosition(), manipulator.GetCameraDirection(), distanceToPoint);
            //Find_Collision();
        }
        
        /// <summary>
        /// Вызываетсяс каждый раз когда изменяется позиция точки в которую становится манипулятор
        /// </summary>
        /// <param name="newPosition"></param>
        public void OnManipulatorCamPointPositoinChanged(Point3D newPosition)
        {
            Console.WriteLine("On manipulator cam changed");
            SolveManipulatorKinematic(newPosition, scanPoint.GetTargetPoint(), false);
            SolvePortalKinematic(newPosition, scanPoint.GetTargetPoint(), false);
        }

        public void OnScanPointPositoinChanged(Point3D newPosition)
        {
            Console.WriteLine("On portal cam changed");
            SolveManipulatorKinematic(manipulatorCamPoint.GetTargetPoint(), newPosition, false);
            SolvePortalKinematic(manipulatorCamPoint.GetTargetPoint(), newPosition, false);
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
     

        // Ставим в точку съемки кубик
        private void TargetPointAcceptButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Считываем координаты точки из полей ввода
            double x, y, z;

            double.TryParse(TargetPointXTextBox.Text, out x);
            double.TryParse(TargetPointYTextBox.Text, out y);
            double.TryParse(TargetPointZTextBox.Text, out z);
            scanPoint.MoveToPositoin(new Point3D(x,y,z));
            
            double manip_x, manip_y, manip_z;
            double.TryParse(PointManipulatorXTextBox.Text, out manip_x);
            double.TryParse(PointManipulatorYTextBox.Text, out manip_y);
            double.TryParse(PointManipulatorZTextBox.Text, out manip_z);
            manipulatorCamPoint.MoveToPositoin(new Point3D(manip_x, manip_y, manip_z));
        }
        
        private void SolvePortalKinematic(Point3D manip, Point3D scannedPoint, bool animate)
        {
            Portal.PortalKinematic p = new Portal.PortalKinematic(500, 500, 500, 140, 10, 51, 10, 0, 30);
            p.setPointManipAndNab(manip.X, manip.Z, manip.Y, scannedPoint.X, scannedPoint.Z, scannedPoint.Y);

            double[] rez = p.portalPoint(1);
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
        
        private void SolveManipulatorKinematic(Point3D manip, Point3D scannedPoint, bool animate)
        {
           
            Stack<double[]> rezults;
            rezults = this.manipKinematic.InverseNab(manip.X, manip.Z, manip.Y, scannedPoint.X, scannedPoint.Z, scannedPoint.Y);
            
            if (rezults.Count > 0)
            {
                double[] rez = rezults.Pop();
                Stack<double[]> satisfied = new Stack<double[]>();

                foreach(double[] one in rezults)
                {
                    if (
                       (rez[0] <  90 & rez[0] >  -90) &
                       (rez[1] <  90 & rez[1] >  -90) &
                       (rez[2] <  90 & rez[2] >  -90) &
                       (rez[3] < 220 & rez[3] > -220) &
                       (rez[4] <  90 & rez[4] >  -90)
                       )
                    {
                        satisfied.Push(one);
                    }
                }

                if (satisfied.Count > 0)
                {
                    ManipulatorAngles angles = new ManipulatorAngles(
                        MathUtils.RadiansToAngle(rez[0]),
                        MathUtils.RadiansToAngle(rez[1]),
                        MathUtils.RadiansToAngle(rez[2]),
                        MathUtils.RadiansToAngle(rez[3]),
                        MathUtils.RadiansToAngle(rez[4])
                        );

                    manipulator.MoveManipulator(angles, animate);                    
                }
                else
                {
                    MessageBox.Show(String.Format("Ошибка: манипулятор не может достигнуть позиции [{0}; {1}; {2}]\nи наблюдать за точкой [{3}; {4}; {5}]",
                        manip.X, manip.Y, manip.Z, scannedPoint.X, scannedPoint.X, scannedPoint.X));
                }
            }            
        }
        
        private void RotateManipulatorButton_OnClick(object sender, RoutedEventArgs e)
        {
            double manip_x, manip_y, manip_z;
            double.TryParse(PointManipulatorXTextBox.Text, out manip_x);
            double.TryParse(PointManipulatorYTextBox.Text, out manip_y);
            double.TryParse(PointManipulatorZTextBox.Text, out manip_z);

            double pointX, pointY, pointZ;
            double.TryParse(TargetPointXTextBox.Text, out pointX);
            double.TryParse(TargetPointYTextBox.Text, out pointY);
            double.TryParse(TargetPointZTextBox.Text, out pointZ);

            SolveManipulatorKinematic(new Point3D(manip_x, manip_y, manip_z), new Point3D(pointX, pointY, pointZ), false);
            SolvePortalKinematic(new Point3D(manip_x, manip_y, manip_z), new Point3D(pointX, pointY, pointZ), false);
            
        }

        private void ResetManipulatorButton_OnClick(object sender, RoutedEventArgs e)
        {
            /*ManipMathModel model = manipulator.ManipMathModel;
            foreach (Joint modelJoint in model.Joints)
            {
                modelJoint.Reset();
            }
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.MiddleEdge, 0);
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.TopEdgeBase, 0);
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.CameraBase, 0);*/
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

            if (targetPoints.Count >= 10)
            {

                MessageBox.Show("Уже 10 точек!!!!");
            }
            else
            {
                double x, y, z;

                double.TryParse(TargetPointXTextBox.Text, out x);
                double.TryParse(TargetPointYTextBox.Text, out y);
                double.TryParse(TargetPointZTextBox.Text, out z);


                Point3D lastPoint = targetPoints.LastOrDefault();

                if (lastPoint != null)
                {
                    Point3D newPoint = new Point3D(x, y, z);
                    if (lastPoint.Equals(newPoint))
                    {
                        MessageBox.Show("Точка уже в списке!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        targetPoints.Add(new Point3D(x, y, z));
                    }
                }
                else
                {
                    targetPoints.Add(new Point3D(x, y, z));
                }
            }

        }




        private void EditPoint_Click(object sender, RoutedEventArgs e)
        {

            if (targetPoints.Count > 0 && selectedIndexPoint >= 0)
            {
                selectedIndexPoint = TargetPointsListView.SelectedIndex;
                Point3D point = targetPoints[selectedIndexPoint];
                TargetPointXTextBox.Text = point.X.ToString();
                TargetPointYTextBox.Text = point.Y.ToString();
                TargetPointZTextBox.Text = point.Z.ToString();




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

            targetPoints[selectedIndexPoint] = new Point3D(x, y, z);

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
            T1Slider.Value = 0;
            T2Slider.Value = 0;
            T3Slider.Value = 0;
            T4Slider.Value = 0;
            T5Slider.Value = 0;

            detectorFrame.ResetTransforms();
        }

        private void PointDown_Click(object sender, RoutedEventArgs e)
        {
            if (selectedIndexPoint > -1 && selectedIndexPoint < targetPoints.Count - 1)
            {
                Point3D point = targetPoints[selectedIndexPoint];
                int index = selectedIndexPoint;
                targetPoints.RemoveAt(index);
                targetPoints.Insert(index + 1, point);

            }
        }

        private void PointUp_Click(object sender, RoutedEventArgs e)
        {
            if (selectedIndexPoint > -1 && selectedIndexPoint > 0 && selectedIndexPoint <= targetPoints.Count - 1)
            {
                Point3D point = targetPoints[selectedIndexPoint];
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

        void Find_Collision()
        {
            TB_Alert.Text = "no collision";

            //проерка столкновениия манипулятора с собой
            if (manipulator.GetManipulatorPart(ManipulatorV2.ManipulatorParts.Camera).Bounds.IntersectsWith(manipulator.GetManipulatorPart(ManipulatorV2.ManipulatorParts.MiddleEdge).Bounds)
                || (manipulator.GetManipulatorPart(ManipulatorV2.ManipulatorParts.Camera).Bounds.IntersectsWith(manipulator.GetManipulatorPart(ManipulatorV2.ManipulatorParts.Table).Bounds)))
            {
                TB_Alert.Text = "collision manip with manip";
                return;
            }


            foreach (ManipulatorV2.ManipulatorParts part in Enum.GetValues(typeof(ManipulatorV2.ManipulatorParts)))
            {

                if (manipulator.GetManipulatorPart(part).Bounds.IntersectsWith(detail.GetModel().Bounds)) //столкновение манипулятора с деталью
                {
                    TB_Alert.Text = "manip with detail";
                    return;
                }
                foreach (DetectorFrame.Parts part_frame in Enum.GetValues(typeof(DetectorFrame.Parts)))
                {
                    if (manipulator.GetManipulatorPart(part).Bounds.IntersectsWith(detectorFrame.GetDetectorFramePart(part_frame).Bounds)) //с детектором
                    {
                        TB_Alert.Text = "manip with detector";
                        return;
                    }
                    if (detectorFrame.GetDetectorFramePart(part_frame).Bounds.IntersectsWith(detail.GetModel().Bounds))
                    {
                        TB_Alert.Text = "detector with detail";
                        return;
                    }
                    if (manipulator.GetManipulatorPart(part).Bounds.IntersectsWith(platform.Bounds)) //с платформой
                    {
                        TB_Alert.Text = "manip with platform";
                        return;
                    }
                }


                if (manipulator.GetManipulatorPart(part).Bounds.IntersectsWith(platform.Bounds)) //с платформой
                {
                    TB_Alert.Text = "collision with platform";
                    return;
                }
            }
            return;
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
            allModels.Children[numMesh].Transform = new TranslateTransform3D(0, (int)e.NewValue, 0);
        }

        private void NumMesh_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            numMesh = (int)e.NewValue;
           // ((IDebugModels)detectorFrame).addNumberMesh(numMesh);
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
    }
}

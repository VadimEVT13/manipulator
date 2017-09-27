using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelixToolkit.Wpf;
using InverseTest.InverseAlgorithm;
using InverseTest.Manipulator;
using Microsoft.Win32;
using InverseTest.Frame;
using InverseTest.Frame.Kinematic;


namespace InverseTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ManipulatorV2 manipulator;
        private DetectorFrame detectorFrame;
        private Model3D platform;
        //Точка сканирования 
        private Model3D targetBox;
        //Точка камеры манипулятора
        private Model3D pointManip;

        private Model3D detail;
        private DetectFrameKinematic kinematic;

        private Point3D scannedPoint = new Point3D(0, 0, 0);



        Animator animator;

        AxisAngleRotation3D ax3d;
        RotateTransform3D myRotateTransform;
        TranslateTransform3D platformTranform;
        double length = 10; // кубик будет размером 10 единиц

        private ObservableCollection<Point3D> targetPoints{get; set;}

        private int selectedIndexPoint = -1;
        private List<UIElement> childrens;

        public MainWindow()
        {

            targetPoints = new ObservableCollection<Point3D>();
            InitializeComponent();

             manipulator = new ManipulatorV2(@"./3DModels/Manip.obj");
             ManipulatorVisualizer.RegisterManipulator(manipulator);
            detectorFrame = new DetectorFrame(@"./3DModels/Frame.obj");
            ManipulatorVisualizer.RegisterDetectorFrame(detectorFrame);

            MeshGeometry3D geometryMesh = new MeshGeometry3D();


            platform = IOFile.loadObjModel(@"./3DModels/cyl_7.obj");
            platformTranform =  new TranslateTransform3D(450, 0, 0);

            platform.Transform = platformTranform;
            ManipulatorVisualizer.AddModel(platform);

            animator = new Animator(manipulator);


         //   TargetPointsListView.ItemsSource = targetPoints;

            ax3d = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 1);
            myRotateTransform = new RotateTransform3D(ax3d);

            ///центр платформы
            myRotateTransform.CenterX = 450;
            myRotateTransform.CenterY = 0;
            myRotateTransform.CenterZ = 0;
            Point3D point = ((IDetectorFrame)detectorFrame).GetDetectorFrameModel().Bounds.Location;
            
            


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
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.TopEdgeBase, -e.NewValue);
            T3TextBox.Text = e.NewValue.ToString();
        }

        private void T4Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.TopEdge, -e.NewValue);
            T4TextBox.Text = e.NewValue.ToString();
        }

        private void T5Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.CameraBase, -e.NewValue);
            T5TextBox.Text = e.NewValue.ToString();
        }

        private void T6Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.Camera, -e.NewValue);
            T6TextBox.Text = e.NewValue.ToString();
        }

        // Ставим в точку съемки кубик
        private void TargetPointAcceptButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Считываем координаты точки из полей ввода
            double x, y, z;
            
            double.TryParse(TargetPointXTextBox.Text, out x);
            double.TryParse(TargetPointYTextBox.Text, out y);
            double.TryParse(TargetPointZTextBox.Text, out z);

            scannedPoint = new Point3D(x, y, z);
            createCube(ref targetBox, new Point3D(x, y, z), Colors.Blue);


            double manip_x, manip_y, manip_z;
            double.TryParse(PointManipulatorXTextBox.Text, out manip_x);
            double.TryParse(PointManipulatorYTextBox.Text, out manip_y);
            double.TryParse(PointManipulatorZTextBox.Text, out manip_z);


            createCube(ref pointManip, new Point3D(manip_x, manip_y, manip_z), Colors.Red);
        }

        private void createCube(ref Model3D model, Point3D point, Color color)
        {

            if (model == null)
            {
                // Смещаем полученную точку. Мы хотим чтобы кубик был по центру снимаемой точки
                point.X = point.X - (length / 2);
                point.Y = point.Y - (length / 2);
                point.Z = point.Z - (length / 2);
                // Создаем кубик
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
                DiffuseMaterial mat = new DiffuseMaterial(new SolidColorBrush(color));
                boxGeom.Material = mat;

                model = boxGeom;
                // Отображаем кубик во вьюпортах


                ManipulatorVisualizer.AddModel(model);
            }
            else
            {
                // Получаем текущую точку съемки
                Point3D oldLocation = GetTargetPoint(model);
                // Вычисляем смещение для новой точки съемки относительно старой
                point.X = point.X - oldLocation.X;
                point.Y = point.Y - oldLocation.Y;
                point.Z= point.Z - oldLocation.Z;
                // Смещаем кубик
                TranslateTransform3D transform = new TranslateTransform3D(point.X, point.Y, point.Z);
                Transform3D oldTransform = model.Transform;
                model.Transform = Transform3DHelper.CombineTransform(oldTransform, transform);
            }

        }
        
        public Point3D GetTargetPoint(Model3D model)
        {
            Point3D targetPoint = model.Bounds.Location;
            targetPoint.Offset(length / 2, length / 2, length / 2);
            return targetPoint;
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



            Kinematic k = new Kinematic();
            Angle.o1 = 0;
            Angle.o2 = 0;
            Angle.o3 = 0;
            Angle.o4 = 0;
            Angle.o5 = 0;
            Angle.o6 = 0;
            Angle.a = 0;
            Angle.b = 0;
            Angle.g = 0;

            
            k.InverseNab(manip_x, manip_z, manip_y, pointX, pointY, pointZ);

            T1Slider.Value = Angle.o1 * 180 / Math.PI;
            T2Slider.Value = Angle.o2 * 180 / Math.PI;
            T3Slider.Value = Angle.o3 * 180 / Math.PI;
            T4Slider.Value = Angle.o4 * 180 / Math.PI;
            T5Slider.Value = Angle.o5 * 180 / Math.PI;
            T6Slider.Value = Angle.o6 * 180 / Math.PI;

            Portal.PortalKinematic p = new Portal.PortalKinematic(1500, 1500, 1500);
            p.setPortal(-500, -500, -500);
            p.setPortalLen(61, 110);
            p.setPointManip(manip_x, manip_y, manip_z);
            p.setPointNab(400, 300, 0);

            double[] angles = p.getAlfAndBet();
            double[] pointers = p.portalPoint(1);
            double[] point = p.ustPoint();
            if (point != null)
            {
                //createTargetCube(400, 300, 0);
                DetectorFramePosition detectp = new DetectorFramePosition(new Point3D(pointers[3], pointers[4], pointers[5]), angles[1], angles[0]);
                detectorFrame.MoveDetectFrame(detectp);

            }
            else
                MessageBox.Show("Не существует такой точки");

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

        private void ShowMathModelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ManipulatorVisualizer.ShowMathModel(manipulator);
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

        private void ImportAnglesEngine_MouseClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog importAnglesDialog = new SaveFileDialog();
            importAnglesDialog.Filter = "Text file (*.txt) | *.txt";
            
            if (importAnglesDialog.ShowDialog() == true)
            {
                String anglesToOut = formatAnglesToString();
                String filename = importAnglesDialog.FileName;
                IOFile.WriteAnglesToFile(anglesToOut, filename);
            }
        }

        
        private String formatAnglesToString()
        {
            StringBuilder anglesString = new StringBuilder(); 
           foreach(Point3D point in targetPoints)
            {
                foreach (double angle in solveAngles(point))
                {
                    anglesString.Append(angle + "\t");
                }
                anglesString.Append("\n");
            }
            return anglesString.ToString();


        }

        //TODO может заменить лист на hashmap чтоб соотносить угол с ребром?
        private List<double> solveAngles(Point3D point)
        {
            JointsChain resultedChain = Algorithm.Solve(manipulator.ManipMathModel, point);

            double edje0RotateAngle = resultedChain.Joints[0].JointAxises.RotationAngle;
            double edje0TurnAngle = resultedChain.Joints[0].JointAxises.TurnAngle;

            double edje1RotateAngle = resultedChain.Joints[1].JointAxises.RotationAngle;
            double edje1TurnAngle = resultedChain.Joints[1].JointAxises.TurnAngle;

            double edje2RotateAngle = resultedChain.Joints[2].JointAxises.RotationAngle;
            double edje2TurnAngle = resultedChain.Joints[2].JointAxises.TurnAngle;

            List<double> andglesList = new List<double>();
            andglesList.Add(edje0RotateAngle);
            andglesList.Add(edje0TurnAngle);

            andglesList.Add(edje1RotateAngle);
            andglesList.Add(edje1TurnAngle);

                andglesList.Add(edje2RotateAngle);
            andglesList.Add(edje2TurnAngle);

            return andglesList;
        }

        

        private void AddPointToList_Click(object sender, RoutedEventArgs e)
        {

            if(targetPoints.Count>=10)
            {

                MessageBox.Show("Уже 10 точек!!!!");
            }
            else
            { double x, y, z;

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

            if (targetPoints.Count > 0 && selectedIndexPoint>=0)
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

            for(int i=0; i<childrens.Count; i++)
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

        private void LoadModel_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Blender file (*.obj)|*.obj";

            if (fileDialog.ShowDialog() == true)
            {
                Model3D detail = IOFile.loadObjModel(fileDialog.FileName);
                TranslateTransform3D transform = new TranslateTransform3D(450, 15, 0);
                detail.Transform = transform;

             

                ManipulatorVisualizer.AddModel(detail);

                Transform3DGroup transformGroup = new Transform3DGroup();

               transformGroup.Children.Add(transform);
                transformGroup.Children.Add(myRotateTransform);
                detail.Transform = transformGroup;

                
                Transform3DGroup transformGroupPlatform = new Transform3DGroup();
                transformGroupPlatform.Children.Add(platformTranform);
                transformGroupPlatform.Children.Add(myRotateTransform);
                platform.Transform = transform;



            }

        }

        private void HelpMenuItem_Click(object sender, RoutedEventArgs e)
        {
        }

        private void DetailSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ax3d.Angle=e.NewValue;
         }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void StartSimulation_Click(object sender, RoutedEventArgs e)
        {
          if (targetPoints.Count>0 && !animator.animating)
            animator.startAnimation(targetPoints.ToList());
        }

        private void resetManip()
        {
            manipulator.ResetMathModel();
            T1Slider.Value = 0;
            T2Slider.Value = 0;
            T3Slider.Value = 0;
            T4Slider.Value = 0;
            T5Slider.Value = 0;
        }

        private void StopSimulation_Click(object sender, RoutedEventArgs e)
        {
            if(animator!=null)
            animator.stopAnimation();
            resetManip();
        }

        private void PointDown_Click(object sender, RoutedEventArgs e)
        {
            if (selectedIndexPoint > -1 && selectedIndexPoint < targetPoints.Count-1) {
                Point3D point = targetPoints[selectedIndexPoint];
                int index = selectedIndexPoint;
                targetPoints.RemoveAt(index);
                targetPoints.Insert(index+1, point);
            
        }
        }

        private void PointUp_Click(object sender, RoutedEventArgs e)
        {
            if (selectedIndexPoint > -1 && selectedIndexPoint > 0 && selectedIndexPoint<= targetPoints.Count-1) 
            {

                
                Point3D point = targetPoints[selectedIndexPoint];
                int index = selectedIndexPoint;
                targetPoints.RemoveAt(index);

                targetPoints.Insert(index- 1, point);

            }
        }

        private void MeshSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            detectorFrame.transformModel(e.NewValue);
        }

        private void MeshNumberSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            

            detectorFrame.addNumberMesh((int)e.NewValue);
            DebugOutput.Text =   e.NewValue.ToString();
        }
    }
}

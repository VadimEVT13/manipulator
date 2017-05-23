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

namespace InverseTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ManipulatorV2 manipulator;

        private Model3D targetBox;

        private Model3D detail;
        double length = 10; // кубик будет размером 10 единиц

        private ObservableCollection<Point3D> targetPoints{get; set;}

        private int selectedIndexPoint = -1;
        private List<UIElement> childrens;

        public MainWindow()
        {

            targetPoints = new ObservableCollection<Point3D>();
            InitializeComponent();

         //  manipulator = new ManipulatorV2(@"Manip.obj");
           //ManipulatorVisualizer.RegisterManipulator(manipulator);

            TargetPointsListView.ItemsSource = targetPoints;

        }

        private void T1Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.Table, e.NewValue);
            T1TextBox.Text = e.NewValue.ToString();
        }

        private void T2Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.MiddleEdge, e.NewValue);
            T2TextBox.Text = e.NewValue.ToString();
        }

        private void T3Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.TopEdgeBase, e.NewValue);
            T3TextBox.Text = e.NewValue.ToString();
        }

        private void T4Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.TopEdge, e.NewValue);
            T4TextBox.Text = e.NewValue.ToString();
        }

        private void T5Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.CameraBase, e.NewValue);
            T5TextBox.Text = e.NewValue.ToString();
        }

        private void T6Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.Camera, e.NewValue);
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

            createTargetCube(x, y,z);
                        
           
            
        }


        private void createTargetCube(double x, double y, double z)
        {

            if (targetBox == null)
            {
                // Смещаем полученную точку. Мы хотим чтобы кубик был по центру снимаемой точки
                x = x - (length / 2);
                y = y - (length / 2);
                z = z - (length / 2);
                // Создаем кубик
                MeshGeometry3D boxMesh = new MeshGeometry3D();
                boxMesh.Positions = new Point3DCollection()
                {
                    new Point3D(x, y, z),
                    new Point3D(x + length, y, z),
                    new Point3D(x, y + length, z),
                    new Point3D(x + length, y + length, z),
                    new Point3D(x, y, z + length),
                    new Point3D(x + length, y, z + length),
                    new Point3D(x, y + length, z + length),
                    new Point3D(x + length, y + length, z + length)
                };

                boxMesh.TriangleIndices = new Int32Collection() { 2, 3, 1, 2, 1, 0, 7, 1, 3, 7, 5, 1, 6, 5, 7, 6, 4, 5, 6, 2, 0, 6, 0, 4, 2, 7, 3, 2, 6, 7, 0, 1, 5, 0, 5, 4 };
                GeometryModel3D boxGeom = new GeometryModel3D();
                boxGeom.Geometry = boxMesh;
                DiffuseMaterial mat = new DiffuseMaterial(new SolidColorBrush(Colors.Red));
                boxGeom.Material = mat;

                targetBox = boxGeom;
                // Отображаем кубик во вьюпортах
                ManipulatorVisualizer.AddModel(targetBox);
            }
            else
            {
                // Получаем текущую точку съемки
                Point3D oldLocation = GetTargetPoint();
                // Вычисляем смещение для новой точки съемки относительно старой
                x = x - oldLocation.X;
                y = y - oldLocation.Y;
                z = z - oldLocation.Z;
                // Смещаем кубик
                TranslateTransform3D transform = new TranslateTransform3D(x, y, z);
                Transform3D oldTransform = targetBox.Transform;
                targetBox.Transform = Transform3DHelper.CombineTransform(oldTransform, transform);
            }
        }

        public Point3D GetTargetPoint()
        {
            Point3D targetPoint = targetBox.Bounds.Location;
            targetPoint.Offset(length / 2, length / 2, length / 2);
            return targetPoint;
        }

        private void RotateManipulatorButton_OnClick(object sender, RoutedEventArgs e)
        {
            /*ManipMathModel model = manipulator.ManipMathModel;
            Point3D targetPoint = GetTargetPoint();
            Algorithm.DoStuff(model,targetPoint);
            double[] firstJointAngles = model.Joints[0].GetTurnAngle();
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.MiddleEdge, firstJointAngles[0]);
            double[] secondJointAngles = model.Joints[1].GetTurnAngle();
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.TopEdgeBase, secondJointAngles[0]-firstJointAngles[0]);
            double[] thirdJointAngles = model.Joints[2].GetTurnAngle();
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.CameraBase, thirdJointAngles[0] - secondJointAngles[0]);*/
            //manipulator.RotatePart(ManipulatorV2.ManipulatorParts.MiddleEdge, model.Joints[0].GetTurnAngle());
            //manipulator.RotatePart(ManipulatorV2.ManipulatorParts.Table, model.Joints[0].GetRotateAngle());
            DebugOutput.Text = "";

            JointsChain resultedChain = Algorithm.Solve(manipulator.ManipMathModel, GetTargetPoint() );

            double edje0RotateAngle = resultedChain.Joints[0].JointAxises.RotationAngle;
            double edje0TurnAngle = resultedChain.Joints[0].JointAxises.TurnAngle;

            double edje1RotateAngle = resultedChain.Joints[1].JointAxises.RotationAngle;
            double edje1TurnAngle = resultedChain.Joints[1].JointAxises.TurnAngle;

            double edje2RotateAngle = resultedChain.Joints[2].JointAxises.RotationAngle;
            double edje2TurnAngle = resultedChain.Joints[2].JointAxises.TurnAngle;

            /*manipulator.RotatePart(ManipulatorV2.ManipulatorParts.Table, edje0RotateAngle);
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.MiddleEdge, edje0TurnAngle);

            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.TopEdgeBase, edje1TurnAngle);

            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.TopEdge, edje2RotateAngle);
            manipulator.RotatePart(ManipulatorV2.ManipulatorParts.CameraBase, edje2TurnAngle);*/
            T1Slider.Value = edje0RotateAngle;
            T2Slider.Value = edje0TurnAngle;
            T3Slider.Value = edje1TurnAngle;
            T4Slider.Value = edje2RotateAngle;
            T5Slider.Value = edje2TurnAngle;

            list = Algorithm.Points;
            listCounter = 0;

            DebugOutput.Text =
                $"{edje0RotateAngle} \n {edje0TurnAngle} \n {edje1RotateAngle} \n {edje1TurnAngle} \n {edje2RotateAngle} \n {edje2TurnAngle}";
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
            manipulator.ResetMathModel();
            T1Slider.Value = 0;
            T2Slider.Value = 0;
            T3Slider.Value = 0;
            T4Slider.Value = 0;
            T5Slider.Value = 0;
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




        private void EditPoint_Click(object sender, RoutedEventArgs e)
        {

            if (targetPoints.Count > 0)
            {
                selectedIndexPoint = TargetPointsListView.SelectedIndex;
                Point3D point = targetPoints[selectedIndexPoint];
                TargetPointXTextBox.Text = point.X.ToString();
                TargetPointYTextBox.Text = point.Y.ToString();
                TargetPointZTextBox.Text = point.Z.ToString();

                createTargetCube(point.X, point.Y, point.Z);



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
            TargetPointsListButtonsGrid.ColumnDefinitions.Add(new ColumnDefinition());

            TargetPointsListButtonsGrid.ColumnDefinitions.Add(new ColumnDefinition());

            TargetPointsListButtonsGrid.ColumnDefinitions.Add(new ColumnDefinition());

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
            fileDialog.Filter = "Blender file (*.obj)| *.obj";

            if (fileDialog.ShowDialog() == true)
            {
                Model3D detail = IOFile.loadObjModel(fileDialog.FileName);
                Visual3D vis;
                
                ManipulatorVisualizer.AddModel(detail);
                

            }

        }
    }
}

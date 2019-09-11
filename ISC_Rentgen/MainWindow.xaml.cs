using HelixToolkit.Wpf;
using ISC_Rentgen.GUI.Controllers;
using ISC_Rentgen.GUI.Model;
using ISC_Rentgen.GUI.ModelView;
using ISC_Rentgen.Model3d;
using ISC_Rentgen.Rentgen_Parts.Manipulator_Components;
using ISC_Rentgen.Rentgen_Parts.Manipulator_Components.Model;
using ISC_Rentgen.Rentgen_Parts.Portal_Components;
using ISC_Rentgen.Rentgen_Parts.Portal_Components.Model;
using ISC_Rentgen.Rentgen_Parts.Scan_Object_Components;
using ISC_Rentgen.Test;
using Microsoft.Win32;
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

namespace ISC_Rentgen
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Model3DGroup Model { get { return model; } set { model = value; } }
        private Model3DGroup model = new ModelImporter().Load(@"Model3d/Detector Frame.obj");
        public Model3DGroup Detal { get { return detal; } set { detal = value; } }
        private Model3DGroup detal = new Model3DGroup();

        public string Example { get { return example; } set { example = value; } }
        private string example = "Example";

        // отображение дуги на главной вьюшке
        private Key_Point_List_controller KPL_controller_main;
        // отображение дуги на дополнительной вьюшке
        private Key_Point_List_controller KPL_controller_second;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Visual_View.DataContext = this;                                                                                         // Для передачи глобальной переменной Model
            Scan_View.DataContext = this;
            Emitter_and_scan_point_controller.Group = Model;                                                                        // Для отображения точек излучателя и сканирования
            Model3d.Model3DParser.Parse(Model);                                                                                     // Парсинг модели для разбора на компоненты
            ManipulatorV3.Base_Point = new Point3D(-80, 0, 0);                                                                      // Базовое положение манипулятора
            ManipulatorV3.Length = new Join_Length_Manipulator() { L1 = 70.5, L2 = 90, L3 = 91.75, L4 = 15.5, L5 = 19, Det = 11 };  // Длины манипулятора
            PortalV3.Base_point = new Point3D(131, -40, 35);                                                                        // Базовое положение портала
            PortalV3.Length = new Join_Length_Portal() { L1 = 81, L2 = 10 };                                                        // Длины портала

            Point3D p = PortalV3.Rotate(new Angles_Portal() { X = 0, Y = 0, Z = 0, O1 = 0, O2 = 0 });

            ManipulatorV3.OnRefreshSliders += Control_View.Refresh_Sliders;
            PortalV3.OnRefreshSliders += Control_View.Refresh_Sliders;

            Controller.PositionChanged += Test;
            Scan_View.OnSphereChanged += SphereAdder;
            Scan_View.OnSphereDelite += SphereDelete;

            KPL_controller_main = new Key_Point_List_controller() { Model = Model, Detal = Detal };
            Key_Point_List.getInstance.PointAdd += KPL_controller_main.PointAdd;
            Key_Point_List.getInstance.PointRemove += KPL_controller_main.PointRemove;
            Key_Point_List.getInstance.PointsClear += KPL_controller_main.PointsClear;

            //ManipulatorV3.Set_Position(new Point3D(-10, 10, 60), new Point3D(0, 0, 60));
            //// -- Установка шарика в схват манипулятора --
            //Point3D p2 = ManipulatorV3.Rotate(ManipulatorV3.Angles);
            //MeshBuilder mb = new MeshBuilder(true, true);
            //mb.AddSphere(p2, 10);
            //GeometryModel3D gm = new GeometryModel3D() { Geometry = mb.ToMesh(), Material = Materials.Green };
            //gm.SetName("Test");
            //Model.Children.Add(gm);
        }

        private void SphereAdder(Sphere_Params SP)
        {
            if (Detal != null & Model != null)
            {
                while (Detal.Children.Where(x => x.GetName() == Sphere_Params.SphereName).Count() > 0)
                {
                    Detal.Children.Remove(Detal.Children.Where(x => x.GetName() == Sphere_Params.SphereName).ToList().First());
                }
                while (Model.Children.Where(x => x.GetName() == Sphere_Params.SphereName).Count() > 0)
                {
                    Model.Children.Remove(Model.Children.Where(x => x.GetName() == Sphere_Params.SphereName).ToList().First());
                }

                var gm = new GeometryModel3D();
                MeshBuilder mb = new MeshBuilder();
                mb.AddSphere(SP.Position, SP.Radius);
                gm.Geometry = mb.ToMesh();
                gm.Material = MaterialHelper.CreateMaterial(Colors.Red, 0.5);
                

                Model3D m = gm;
                m.SetName(Sphere_Params.SphereName);

                Detal.Children.Add(m);
                Model.Children.Add(m);
            }
        }

        private void SphereDelete()
        {
            if (Detal != null & Model != null)
            {
                while (Detal.Children.Where(x => x.GetName() == Sphere_Params.SphereName).Count() > 0)
                {
                    Detal.Children.Remove(Detal.Children.Where(x => x.GetName() == Sphere_Params.SphereName).ToList().First());
                }
                while (Model.Children.Where(x => x.GetName() == Sphere_Params.SphereName).Count() > 0)
                {
                    Model.Children.Remove(Model.Children.Where(x => x.GetName() == Sphere_Params.SphereName).ToList().First());
                }
            }
        }

        private void Test(Angles_Manipulator angle)
        {
            ManipulatorV3.Rotate(angle);
        }

        private void Download_object_button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open_file_dialog = new OpenFileDialog();
            open_file_dialog.Filter = "Obj files (*.obj)|*.obj";
            if (open_file_dialog.ShowDialog() == true)
            {
                while (Model.Children.Where(x => x.GetName() == Model3DParts.ObjectParts.Scan_object).Count() > 0)
                {
                    Model.Children.Remove(Model.Children.Where(x => x.GetName() == Model3DParts.ObjectParts.Scan_object).First());
                }

                Scan_Object_Parser.Parse(new ModelImporter().Load(open_file_dialog.FileName));
                foreach (Model3D m in Scan_Object.Model.Children)
                {
                    Model.Children.Add(m);
                }

                while (Detal.Children.Where(x => x.GetName() == Model3DParts.ObjectParts.Scan_object).Count() > 0)
                {
                    Detal.Children.Remove(Detal.Children.Where(x => x.GetName() == Model3DParts.ObjectParts.Scan_object).First());
                }                
                foreach (Model3D m in Scan_Object.Model.Children)
                {
                    Detal.Children.Add(m);
                }
            }
        }
    }
}

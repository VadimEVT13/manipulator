using HelixToolkit.Wpf;
using ISC_Rentgen.GUI.Controllers;
using ISC_Rentgen.GUI.Model;
using ISC_Rentgen.GUI.ModelView;
using ISC_Rentgen.Model3d;
using ISC_Rentgen.Model3d.Detals.Controller;
using ISC_Rentgen.Model3d.Detals.Model;
using ISC_Rentgen.Rentgen_Parts.Manipulator_Components;
using ISC_Rentgen.Rentgen_Parts.Manipulator_Components.Model;
using ISC_Rentgen.Rentgen_Parts.Portal_Components;
using ISC_Rentgen.Rentgen_Parts.Portal_Components.Model;
using ISC_Rentgen.Rentgen_Parts.Scan_Object_Components;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

        // отображение дуги на всех вьюшках
        private Key_Point_List_controller KPL_controller_main;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Visual_View.DataContext = this;                                                                                         // Для передачи глобальной переменной Model
            Scan_View.DataContext = this;
            Emitter_and_scan_point_controller.getInstance.Model = Model;                                                                        // Для отображения точек излучателя и сканирования
            Emitter_and_scan_point_controller.getInstance.Detal = Detal;
            Model3d.Model3DParser.Parse(Model);                                                                                     // Парсинг модели для разбора на компоненты
            ManipulatorV3.Base_Point = new Point3D(-80, 0, 0);                                                                      // Базовое положение манипулятора
            ManipulatorV3.Length = new Join_Length_Manipulator() { L1 = 70.5, L2 = 90, L3 = 91.75, L4 = 15.5, L5 = 19, Det = 11 };  // Длины манипулятора
            PortalV3.Base_point = new Point3D(131, -40, 35);                                                                        // Базовое положение портала
            PortalV3.Length = new Join_Length_Portal() { L1 = 81, L2 = 10 };                                                        // Длины портала

            Point3D p = PortalV3.Rotate(new Angles_Portal() { X = 0, Y = 0, Z = 0, O1 = 0, O2 = 0 });

            ManipulatorV3.OnRefreshSliders += Control_View.Refresh_Sliders;
            PortalV3.OnRefreshSliders += Control_View.Refresh_Sliders;
            
            Addition_Sphere.getInstance.Sphere_Added += SphereAdder;
            Addition_Sphere.getInstance.Sphere_Deleted += SphereDelete;

            KPL_controller_main = new Key_Point_List_controller() { Model = Model, Detal = Detal };
            Key_Point_List.getInstance.PointAdd += KPL_controller_main.PointAdd;
            Key_Point_List.getInstance.PointRemove += KPL_controller_main.PointRemove;
            Key_Point_List.getInstance.PointsClear += KPL_controller_main.PointsClear;
            Key_Point_List.getInstance.ModifAngle += KPL_controller_main.AngleModif;

            ManipulatorV3.Set_Position(Emitter_and_scan_point_controller.getInstance.Emitter_and_scan_point.Emitter_point,
                Emitter_and_scan_point_controller.getInstance.Emitter_and_scan_point.Scan_point);
            PortalV3.Set_Position(Emitter_and_scan_point_controller.getInstance.Emitter_and_scan_point.Emitter_point,
                Emitter_and_scan_point_controller.getInstance.Emitter_and_scan_point.Scan_point);




            //ManipulatorV3.Set_Position(new Point3D(-10, 10, 60), new Point3D(0, 0, 60));
            //// -- Установка шарика в схват манипулятора --
            //Point3D p2 = ManipulatorV3.Rotate(ManipulatorV3.Angles);
            //MeshBuilder mb = new MeshBuilder(true, true);
            //mb.AddSphere(p2, 10);
            //GeometryModel3D gm = new GeometryModel3D() { Geometry = mb.ToMesh(), Material = Materials.Green };
            //gm.SetName("Test");
            //Model.Children.Add(gm);
        }

        /// <summary>
        /// Добавление сферы ОИ
        /// </summary>
        /// <param name="SP"></param>
        private void SphereAdder(Model3D sphere)
        {
            if (Detal != null & Model != null)
            {
                string sphere_name = Addition_Sphere.getInstance.SphereName;

                while (Detal.Children.Where(x => x.GetName() == sphere_name).Count() > 0)
                {
                    Detal.Children.Remove(Detal.Children.Where(x => x.GetName() == sphere_name).ToList().First());
                }
                while (Model.Children.Where(x => x.GetName() == sphere_name).Count() > 0)
                {
                    Model.Children.Remove(Model.Children.Where(x => x.GetName() == sphere_name).ToList().First());
                }                

                Detal.Children.Add(sphere);
                Model.Children.Add(sphere);
            }
        }

        private void SphereDelete(Model3D sphere)
        {
            if (Detal != null & Model != null)
            {
                string sphere_name = Addition_Sphere.getInstance.SphereName;

                while (Detal.Children.Where(x => x.GetName() == sphere_name).Count() > 0)
                {
                    Detal.Children.Remove(Detal.Children.Where(x => x.GetName() == sphere_name).ToList().First());
                }
                while (Model.Children.Where(x => x.GetName() == sphere_name).Count() > 0)
                {
                    Model.Children.Remove(Model.Children.Where(x => x.GetName() == sphere_name).ToList().First());
                }
            }
        }

        private void Test(Angles_Manipulator angle)
        {
            ManipulatorV3.Rotate(angle);
        }

        private void Download_object_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog open_file_dialog = new OpenFileDialog()
                {
                    Filter = "Obj files (*.obj)|*.obj",
                    InitialDirectory = System.IO.Path.GetFullPath(@"Model3d\Detals")
                };
                if (open_file_dialog.ShowDialog() == true)
                {
                    while (Model.Children.Where(x => x.GetName() == Model3DParts.ObjectParts.Scan_object).Count() > 0)
                    {
                        Model.Children.Remove(Model.Children.Where(x => x.GetName() == Model3DParts.ObjectParts.Scan_object).First());
                    }

                    Scan_Object_Parser.Parse(new ModelImporter().Load(open_file_dialog.FileName));
                    foreach (Model3D m in Scan_Object.getInstant.Model.Children)
                    {
                        Model.Children.Add(m);
                    }

                    while (Detal.Children.Where(x => x.GetName() == Model3DParts.ObjectParts.Scan_object).Count() > 0)
                    {
                        Detal.Children.Remove(Detal.Children.Where(x => x.GetName() == Model3DParts.ObjectParts.Scan_object).First());
                    }
                    foreach (Model3D m in Scan_Object.getInstant.Model.Children)
                    {
                        Detal.Children.Add(m);
                    }

                    // Загрузка конфигов
                    string ConfigPath = System.IO.Path.GetFullPath(@"Model3d/Detals/Config/" + 
                        System.IO.Path.GetFileNameWithoutExtension(open_file_dialog.FileName) + ".txt");
                    if (File.Exists(ConfigPath))
                    {
                        Detal_Config_Parser.Load_Config(ConfigPath);
                        Scan_Object.getInstant.SetBase(Detal_Config.getInstance.Detal_Base);
                        Key_Point_List.getInstance.Clear();
                        var list = Detal_Config.getInstance.Position_ListToKey_Point_List(Detal_Config.getInstance.Positions);
                        foreach (Key_Point kp in list)
                        {
                            Key_Point_List.getInstance.AddPoint(kp);
                        }

                        if (Key_Point_List.getInstance.Points_List.Count > 0)
                        {
                            Key_Point kp = Key_Point_List.getInstance.Points_List.First();

                            Emitter_and_scan_point_controller.getInstance.AddEmitter(kp.Emitter_point);
                            Emitter_and_scan_point_controller.getInstance.AddScan(kp.Scan_point);

                            ManipulatorV3.Set_Position(Emitter_and_scan_point_controller.getInstance.Emitter_and_scan_point.Emitter_point,
                                Emitter_and_scan_point_controller.getInstance.Emitter_and_scan_point.Scan_point);
                            PortalV3.Set_Position(Emitter_and_scan_point_controller.getInstance.Emitter_and_scan_point.Emitter_point,
                                Emitter_and_scan_point_controller.getInstance.Emitter_and_scan_point.Scan_point);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

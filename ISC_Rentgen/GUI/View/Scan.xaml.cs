using ISC_Rentgen.GUI.Controllers;
using ISC_Rentgen.GUI.Model;
using ISC_Rentgen.GUI.ModelView;
using ISC_Rentgen.Rentgen_Parts.Manipulator_Components;
using ISC_Rentgen.Rentgen_Parts.Portal_Components;
using ISC_Rentgen.Rentgen_Parts.Scan_Object_Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ISC_Rentgen.GUI.View
{
    /// <summary>
    /// Логика взаимодействия для Scan.xaml
    /// </summary>
    public partial class Scan : UserControl
    {
        public Scan()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Добавить точку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_key_point_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Key_Point_List.getInstance.AddPoint(new Key_Point(
                    Emitter_and_scan_point_controller.getInstance.Emitter_and_scan_point.Emitter_point,
                    Emitter_and_scan_point_controller.getInstance.Emitter_and_scan_point.Scan_point));
                
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        /// <summary>
        /// Отобразить положение системы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Example_position_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Point3D Scan_point = new Point3D(double.Parse(Scan_x.Text), double.Parse(Scan_y.Text), double.Parse(Scan_z.Text));

                ManipulatorV3.Set_Position(Emitter_and_scan_point_controller.getInstance.Emitter_and_scan_point.Emitter_point, 
                    Emitter_and_scan_point_controller.getInstance.Emitter_and_scan_point.Scan_point);
                PortalV3.Set_Position(Emitter_and_scan_point_controller.getInstance.Emitter_and_scan_point.Emitter_point, 
                    Emitter_and_scan_point_controller.getInstance.Emitter_and_scan_point.Scan_point);                  
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        /// <summary>
        /// Отобразить положение системы при выборе точки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TargetPointsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lv = e.OriginalSource as ListView;
            var selected = lv.SelectedItem as Key_Point;

            if (selected != null)
            {
                Key_Point_List.getInstance.ModifAngles(
                    selected, 
                    ManipulatorV3.Set_Position(selected.Emitter_point, selected.Scan_point), 
                    PortalV3.Set_Position(selected.Emitter_point, selected.Scan_point));
                //selected.Manipulator_Angle = ManipulatorV3.Set_Position(selected.Emitter_point, selected.Scan_point);
                //selected.Portal_Angle = PortalV3.Set_Position(selected.Emitter_point, selected.Scan_point);
                                      
                Emitter_and_scan_point_controller.getInstance.AddEmitter(selected.Emitter_point);
                Emitter_and_scan_point_controller.getInstance.AddScan(selected.Scan_point);
            }
        }
               
        /// <summary>
        /// Автоматическое создание
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetPointsButton_Click(object sender, RoutedEventArgs e)
        {
            if (Auto_gen_model.getInstance.Methodic_name == "")
                return;

            switch (Auto_gen_model.getInstance.Methodic_name)
            {
                case "Шпангоут":
                    ShpangoutMethodic();
                    break;
                case "Лопатка":
                    LopatkaMethodic();
                    break;
                case "Шпангоут (дугами)":
                    Shpangout_Duga();
                    break;
            }
        }

        void ShpangoutMethodic()
        {
            Emitter_and_scan_point_controller.getInstance.AddScan(new Point3D(
                Scan_Object.getInstant.Base_Point.X + 111, 
                Scan_Object.getInstant.Base_Point.Y, 
                Scan_Object.getInstant.Base_Point.Z));
            Key_Point KP = Emitter_and_scan_point_controller.getInstance.Emitter_and_scan_point;

            // Очистка таблиц
            Key_Point_List.getInstance.Clear();

            // Добавление точек по дуге вокруг точки сканирования
            for (double i = 0; i <= Math.PI; i += Math.PI / (Auto_gen_model.getInstance.Num - 1))
            {
                Key_Point_List.getInstance.AddPoint(new Key_Point(new Point3D()
                {
                    X = KP.Scan_point.X - Math.Sin(i) * Addition_Sphere.getInstance.Radius,
                    Y = KP.Scan_point.Y,
                    Z = KP.Scan_point.Z + Math.Cos(i) * Addition_Sphere.getInstance.Radius
                },
                KP.Scan_point));
            }
        }

        void LopatkaMethodic()
        {
            Point3D KP = Scan_Object.getInstant.Base_Point;
            
            // Очистка таблиц
            Key_Point_List.getInstance.Clear();

            Key_Point_List.getInstance.AddPoint(new Key_Point(
                new Point3D(KP.X - Addition_Sphere.getInstance.Radius, KP.Y, KP.Z + 37), 
                new Point3D(KP.X, KP.Y, KP.Z + 37)));
            Key_Point_List.getInstance.AddPoint(new Key_Point(
                new Point3D(KP.X - Addition_Sphere.getInstance.Radius, KP.Y, KP.Z + 47),
                new Point3D(KP.X, KP.Y, KP.Z + 47)));
            Key_Point_List.getInstance.AddPoint(new Key_Point(
                new Point3D(KP.X - Addition_Sphere.getInstance.Radius, KP.Y, KP.Z + 57),
                new Point3D(KP.X, KP.Y, KP.Z + 57)));
            Key_Point_List.getInstance.AddPoint(new Key_Point(
                new Point3D(KP.X - Addition_Sphere.getInstance.Radius, KP.Y, KP.Z + 67 - Addition_Sphere.getInstance.Radius),
                new Point3D(KP.X, KP.Y, KP.Z + 67)));
        }

        void Shpangout_Duga()
        {
            int n = 0;
            if (!int.TryParse(NumberOfPoints.Text, out n))
                return;

            Point3D s = Emitter_and_scan_point_controller.getInstance.Emitter_and_scan_point.Scan_point; // центр сферы
            
            double alfa1 = GetAngle((e1.X - s.X), (e1.Y - s.Y));
            double beta1 = GetAngle((e1.X - s.X) / Math.Cos(alfa1), (e1.Z - s.Z));
            double alfa2 = GetAngle((e2.X - s.X), (e2.Y - s.Y));
            double beta2 = GetAngle((e2.X - s.X) / Math.Cos(alfa2), (e2.Z - s.Z));

            if (!Duga_mode)
            {
                if (((alfa1 < 0 & alfa2 > 0) || (alfa1 > 0 & alfa2 < 0)))
                {
                    if (alfa1 > 0)
                        alfa1 -= Math.PI * 2;
                    if (alfa2 > 0)
                        alfa2 -= Math.PI * 2;
                }
            }
            else
            {
                if (((alfa1 < 0 & alfa2 < 0) || (alfa1 > 0 & alfa2 > 0)))
                {
                    if (alfa2 > 0)
                        alfa2 -= Math.PI * 2;
                    else
                        alfa2 += Math.PI * 2;
                }

            }

            for (int i = 0; i < n; i++)
            {
                double delta_a = (double)(alfa2 - alfa1) / (n - 1);
                double delta_b = (double)(beta2 - beta1) / (n - 1);

                double x = Math.Cos(beta1 + delta_b * i) * Math.Cos(alfa1 + delta_a * i) * Addition_Sphere.getInstance.Radius;
                double y = Math.Cos(beta1 + delta_b * i) * Math.Sin(alfa1 + delta_a * i) * Addition_Sphere.getInstance.Radius;
                double z = Math.Sin(beta1 + delta_b * i) * Addition_Sphere.getInstance.Radius;

                Key_Point_List.getInstance.AddPoint(new Key_Point(new Point3D(s.X + x, s.Y + y, s.Z + z), s));
            }
        }
        
        private static double GetAngle(double X, double Y)
        {
            if (X == 0 && Y == 0)
            {
                return 0;
            }
            if (X == 0)
            {
                if (Y > 0)
                {
                    return Math.PI / 2;
                }
                else
                {
                    return -Math.PI / 2;
                }
            }
            if (X > 0)
            {
                return Math.Atan(Y / X);
            }
            else
            {
                if (Y >= 0)
                {
                    return Math.Atan(Y / X) + Math.PI;
                }
                else
                {
                    return Math.Atan(Y / X) - Math.PI;
                }
            }
        }

        private void Clear_Key_Points_Button_Click(object sender, RoutedEventArgs e)
        {
            Key_Point_List.getInstance.Clear();
        }

        private void Kratnost_TextChanged(object sender, TextChangedEventArgs e)
        {
            double K = 1;
            if (double.TryParse((sender as TextBox).Text.Replace('.',','), out K))
            {
                if (K <= 0)
                    K = 1;
            }
            PortalV3.K = K;
        }
        
        Point3D e1 = new Point3D();
        Point3D e2 = new Point3D();
                
        private void Detal_View_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Addition_Sphere.getInstance.If_Sphere_Exist)
            {
                Point mousePos = e.GetPosition(Detal_View);
                RayMeshGeometry3DHitTestResult result = VisualTreeHelper.HitTest(Detal_View, mousePos) as RayMeshGeometry3DHitTestResult;
                if (result != null)
                {
                    e1 = e2;
                    e2 = result.PointHit;

                    Emitter_and_scan_point_controller.getInstance.AddEmitter(e2);
                }
            }
            else
            {
                Point mousePos = e.GetPosition(Detal_View);
                RayMeshGeometry3DHitTestResult result = VisualTreeHelper.HitTest(Detal_View, mousePos) as RayMeshGeometry3DHitTestResult;
                
                if (result != null)
                {
                    Emitter_and_scan_point_controller.getInstance.AddScan(result.PointHit);

                    Addition_Sphere.getInstance.Position = Emitter_and_scan_point_controller.getInstance.Emitter_and_scan_point.Scan_point;
                    Addition_Sphere.getInstance.If_Sphere_Exist = true;
                }
                else
                {
                    return;
                }
            }
        }
        
        private void Sphere_delite(object sender, RoutedEventArgs e)
        {
            Addition_Sphere.getInstance.If_Sphere_Exist = false;
        }

        private bool Duga_mode = false;
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            Duga_mode = (bool)(sender as CheckBox).IsChecked;
        }
        
        private void TargetPointsListView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as ListView).SelectedItem != null)
            {
                Key_Point_List.getInstance.RemovePoint((sender as ListView).SelectedItem as Key_Point);
            }
        }
    }
}

using ISC_Rentgen.GUI.Model;
using ISC_Rentgen.GUI.ModelView;
using ISC_Rentgen.Rentgen_Parts.Manipulator_Components;
using ISC_Rentgen.Rentgen_Parts.Portal_Components;
using ISC_Rentgen.Test;
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
    public delegate void Sphere_Params_delegate(Sphere_Params SP);
    public delegate void Sphere_delite_delegate();

    /// <summary>
    /// Логика взаимодействия для Scan.xaml
    /// </summary>
    public partial class Scan : UserControl
    {
        public Sphere_Params_delegate OnSphereChanged;
        public Sphere_delite_delegate OnSphereDelite;

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
                Point3D Emitter_point   = new Point3D(double.Parse(Emitter_x.Text), double.Parse(Emitter_y.Text), double.Parse(Emitter_z.Text));
                Point3D Scan_point      = new Point3D(double.Parse(Scan_x.Text), double.Parse(Scan_y.Text), double.Parse(Scan_z.Text));

                Key_Point_List.getInstance.AddPoint(new Key_Point(Emitter_point, Scan_point));
                
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
                Point3D Emitter_point = new Point3D(double.Parse(Emitter_x.Text), double.Parse(Emitter_y.Text), double.Parse(Emitter_z.Text));
                Point3D Scan_point = new Point3D(double.Parse(Scan_x.Text), double.Parse(Scan_y.Text), double.Parse(Scan_z.Text));

                ManipulatorV3.Set_Position(Emitter_point, Scan_point);
                PortalV3.Set_Position(Emitter_point, Scan_point);
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
                ManipulatorV3.Set_Position(selected.Emitter_point, selected.Scan_point);
                PortalV3.Set_Position(selected.Emitter_point, selected.Scan_point);

                Emitter_and_scan_point_controller.AddEitter(selected.Emitter_point);
                Emitter_and_scan_point_controller.AddScan(selected.Scan_point);
            }
        }

        /// <summary>
        /// Смена положения точки излучателя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Emitter_TextChanged(object sender, TextChangedEventArgs e)
        {
            double x, y, z;
            if (double.TryParse(Emitter_x.Text, out x) & double.TryParse(Emitter_y.Text, out y) & double.TryParse(Emitter_z.Text, out z))
            {
                Emitter_and_scan_point_controller.AddEitter(new Point3D(x, y, z));
            }
        }
        
        /// <summary>
        /// Смена положения точки сканирования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Scan_TextChanged(object sender, TextChangedEventArgs e)
        {
            double x, y, z;
            if (double.TryParse(Scan_x.Text, out x) & double.TryParse(Scan_y.Text, out y) & double.TryParse(Scan_z.Text, out z))
            {
                Emitter_and_scan_point_controller.AddScan(new Point3D(x, y, z));
            }
        }
                
        /// <summary>
        /// Смена точки сканирования во вкладке автоматического создания
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Automatic_Scan_TextChanged(object sender, TextChangedEventArgs e)
        {
            double x, y, z;
            if (double.TryParse(Automatic_Scan_X.Text, out x) & double.TryParse(Automatic_Scan_Y.Text, out y) & double.TryParse(Automatic_Scan_Z.Text, out z))
            {
                    Emitter_and_scan_point_controller.AddScan(new Point3D(x, y, z));
            }
        }

        /// <summary>
        /// Автоматическое создание
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetPointsButton_Click(object sender, RoutedEventArgs e)
        {
            if (Method.SelectedItem == null)
                return;

            switch ((Method.SelectedItem as TextBlock).Text)
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
            double x, y, z;
            if (double.TryParse(Automatic_Scan_X.Text, out x) & double.TryParse(Automatic_Scan_Y.Text, out y) & double.TryParse(Automatic_Scan_Z.Text, out z))
            {
                Emitter_and_scan_point_controller.AddScan(new Point3D(x, y, z));
            }

            Key_Point KP = Emitter_and_scan_point_controller.Emitter_and_scan_point;
            double r, n;

            if (!double.TryParse(Radius.Text, out r) || !double.TryParse(NumberOfPoints.Text, out n))
                return;

            if (n <= 0)
                return;

            // Очистка таблиц
            Key_Point_List.getInstance.Clear();

            // Добавление точек по дуге вокруг точки сканирования
            for (double i = 0; i <= Math.PI; i += Math.PI / (n - 1))
            {
                Key_Point_List.getInstance.AddPoint(new Key_Point(new Point3D()
                {
                    X = KP.Scan_point.X - Math.Sin(i) * r,
                    Y = KP.Scan_point.Y,
                    Z = KP.Scan_point.Z + Math.Cos(i) * r
                },
                KP.Scan_point));
            }
        }

        void LopatkaMethodic()
        {
            double x, y, z;
            if (double.TryParse(Automatic_Scan_X.Text, out x) & double.TryParse(Automatic_Scan_Y.Text, out y) & double.TryParse(Automatic_Scan_Z.Text, out z))
            {
                Emitter_and_scan_point_controller.AddScan(new Point3D(x, y, z));
            }

            Key_Point KP = Emitter_and_scan_point_controller.Emitter_and_scan_point;
            double r;

            if (!double.TryParse(Radius.Text, out r))
                return;
            
            // Очистка таблиц
            Key_Point_List.getInstance.Clear();

            Key_Point_List.getInstance.AddPoint(new Key_Point(
                new Point3D(KP.Scan_point.X - r, KP.Scan_point.Y, KP.Scan_point.Z + 15), 
                new Point3D(KP.Scan_point.X, KP.Scan_point.Y, KP.Scan_point.Z + 15)));
            Key_Point_List.getInstance.AddPoint(new Key_Point(
                new Point3D(KP.Scan_point.X - r, KP.Scan_point.Y, KP.Scan_point.Z + 30),
                new Point3D(KP.Scan_point.X, KP.Scan_point.Y, KP.Scan_point.Z + 30)));
            Key_Point_List.getInstance.AddPoint(new Key_Point(
                new Point3D(KP.Scan_point.X - r, KP.Scan_point.Y, KP.Scan_point.Z + 45),
                new Point3D(KP.Scan_point.X, KP.Scan_point.Y, KP.Scan_point.Z + 45)));
            Key_Point_List.getInstance.AddPoint(new Key_Point(
                new Point3D(KP.Scan_point.X - r, KP.Scan_point.Y, KP.Scan_point.Z + 60 - r),
                new Point3D(KP.Scan_point.X, KP.Scan_point.Y, KP.Scan_point.Z + 60)));
        }

        void Shpangout_Duga()
        {
            int n = 0;
            double r = 0;
            if (!int.TryParse(NumberOfPoints.Text, out n) || !double.TryParse(Radius.Text, out r))
                return;

            Point3D s = Emitter_and_scan_point_controller.Emitter_and_scan_point.Scan_point; // центр сферы
            
            double alfa1 = GetAngle((e1.X - s.X), (e1.Y - s.Y));
            double beta1 = GetAngle((e1.X - s.X) / Math.Cos(alfa1), (e1.Z - s.Z));
            double alfa2 = GetAngle((e2.X - s.X), (e2.Y - s.Y));
            double beta2 = GetAngle((e2.X - s.X) / Math.Cos(alfa2), (e2.Z - s.Z));

            if (!Duga_mode)
            {
                if (alfa1 < 0 & alfa2 > 0)
                    alfa2 -= Math.PI * 2;
                if (alfa1 > 0 & alfa2 < 0)
                    alfa1 -= Math.PI * 2;
                if (beta1 < 0 & beta2 > 0)
                    beta2 -= Math.PI * 2;
                if (beta1 > 0 & beta2 < 0)
                    beta1 -= Math.PI * 2;
            }

            for (int i = 0; i < n; i++)
            {
                double delta_a = (double)(alfa2 - alfa1) / (n - 1);
                double delta_b = (double)(beta2 - beta1) / (n - 1);

                double x = Math.Cos(beta1 + delta_b * i) * Math.Cos(alfa1 + delta_a * i) * r;
                double y = Math.Cos(beta1 + delta_b * i) * Math.Sin(alfa1 + delta_a * i) * r;
                double z = Math.Sin(beta1 + delta_b * i) * r;

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
                
        bool If_Shpere_Exist = false;

        private void Detal_View_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (If_Shpere_Exist)
            {
                //Point mousePos = e.GetPosition(Detal_View);
                //RayMeshGeometry3DHitTestResult result = VisualTreeHelper.HitTest(Detal_View, mousePos) as RayMeshGeometry3DHitTestResult;
                //if (result != null)
                //{
                //    Console.WriteLine(string.Format("[{0};{1};{2}]", result.PointHit.X, result.PointHit.Y, result.PointHit.Z));
                //    Scan_x.Text = result.PointHit.X.ToString();
                //    Scan_y.Text = result.PointHit.Y.ToString();
                //    Scan_z.Text = result.PointHit.Z.ToString();
                //}

                double r = 0;

                if (!double.TryParse(Radius.Text, out r))
                    return;

                Point mousePos = e.GetPosition(Detal_View);
                RayMeshGeometry3DHitTestResult result = VisualTreeHelper.HitTest(Detal_View, mousePos) as RayMeshGeometry3DHitTestResult;
                if (result != null)
                {
                    Point3D Sphere_center = Emitter_and_scan_point_controller.Emitter_and_scan_point.Scan_point;

                    e1 = e2;
                    e2 = new Point3D()
                    {
                        X = result.PointHit.X,
                        Y = result.PointHit.Y,
                        Z = result.PointHit.Z
                    };

                    Emitter_x.Text = e2.X.ToString();
                    Emitter_y.Text = e2.Y.ToString();
                    Emitter_z.Text = e2.Z.ToString();
                }
            }
            else
            {
                double r = 0;

                if (!double.TryParse(Radius.Text, out r))
                    return;

                Point mousePos = e.GetPosition(Detal_View);
                RayMeshGeometry3DHitTestResult result = VisualTreeHelper.HitTest(Detal_View, mousePos) as RayMeshGeometry3DHitTestResult;
                if (result != null)
                {
                    Scan_x.Text = result.PointHit.X.ToString();
                    Scan_y.Text = result.PointHit.Y.ToString();
                    Scan_z.Text = result.PointHit.Z.ToString();

                    OnSphereChanged?.Invoke(new Sphere_Params()
                    {
                        Position = Emitter_and_scan_point_controller.Emitter_and_scan_point.Scan_point,
                        Radius = r
                    });

                    If_Shpere_Exist = true;
                }
                else
                {
                    return;
                }
            }
        }

        private void Detal_View_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            double r = 0;

            if (!double.TryParse(Radius.Text, out r))
                return;

            Point mousePos = e.GetPosition(Detal_View);
            RayMeshGeometry3DHitTestResult result = VisualTreeHelper.HitTest(Detal_View, mousePos) as RayMeshGeometry3DHitTestResult;
            if (result != null)
            {
                Scan_x.Text = result.PointHit.X.ToString();
                Scan_y.Text = result.PointHit.Y.ToString();
                Scan_z.Text = result.PointHit.Z.ToString();
            }

            OnSphereChanged?.Invoke(new Sphere_Params() {
                Position = Emitter_and_scan_point_controller.Emitter_and_scan_point.Scan_point,
                Radius = r });
        }

        private void Sphere_delite(object sender, RoutedEventArgs e)
        {
            OnSphereDelite?.Invoke();
            If_Shpere_Exist = false;
        }

        private bool Duga_mode = false;
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            Duga_mode = (bool)(sender as CheckBox).IsChecked;
        }
    }
}

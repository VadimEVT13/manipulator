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
    /// <summary>
    /// Логика взаимодействия для Scan.xaml
    /// </summary>
    public partial class Scan : UserControl
    {
        public Scan()
        {
            InitializeComponent();
        }

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

        private void Emitter_TextChanged(object sender, TextChangedEventArgs e)
        {
            double x, y, z;
            if (double.TryParse(Emitter_x.Text, out x) & double.TryParse(Emitter_y.Text, out y) & double.TryParse(Emitter_z.Text, out z))
            {
                Emitter_and_scan_point_controller.AddEitter(new Point3D(x, y, z));
            }
        }
        
        private void Scan_TextChanged(object sender, TextChangedEventArgs e)
        {
            double x, y, z;
            if (double.TryParse(Scan_x.Text, out x) & double.TryParse(Scan_y.Text, out y) & double.TryParse(Scan_z.Text, out z))
            {
                Emitter_and_scan_point_controller.AddScan(new Point3D(x, y, z));
            }
        }
                
        private void Automatic_Scan_TextChanged(object sender, TextChangedEventArgs e)
        {
            double x, y, z;
            if (double.TryParse(Automatic_Scan_X.Text, out x) & double.TryParse(Automatic_Scan_Y.Text, out y) & double.TryParse(Automatic_Scan_Z.Text, out z))
            {
                    Emitter_and_scan_point_controller.AddScan(new Point3D(x, y, z));
            }
        }

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
    }
}

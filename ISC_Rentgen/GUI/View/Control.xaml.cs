using ISC_Rentgen.Grbl.Models;
using ISC_Rentgen.Rentgen_Parts.Manipulator_Components;
using ISC_Rentgen.Rentgen_Parts.Manipulator_Components.Model;
using ISC_Rentgen.Rentgen_Parts.Portal_Components;
using ISC_Rentgen.Rentgen_Parts.Portal_Components.Model;
using ISC_Rentgen.Rentgen_Parts.Scan_Object_Components;
using ISC_Rentgen.Rentgen_Parts.Scan_Object_Components.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Roentgen.Devices.Models;
using ISC_Rentgen.Model3d.Detals.Controller;
using ISC_Rentgen.Model3d.Detals.Model;
using ISC_Rentgen.GUI.Model;
using Microsoft.Win32;

namespace ISC_Rentgen.GUI.View
{
    /// <summary>
    /// Логика взаимодействия для Control.xaml
    /// </summary>
    public partial class Control : UserControl
    {
        public ComPort manip_port  = new ComPort();
        public ComPort portal_port = new ComPort();

        public GPort MPort { get; set; }
        public GPort DPort { get; set; }

        public Control()
        {
            InitializeComponent();
            MPort = GManipulator.getInstance().Port;
            DPort = GDetector.getInstance().Port;
        }

        #region sliders

        private void Manipulator_Angle1_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {        
            Angles_Manipulator old_angle = ManipulatorV3.Angles;
            old_angle.O1 = e.NewValue;

            ManipulatorV3.Rotate(old_angle);
        }

        private void Manipulator_Angle2_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Angles_Manipulator old_angle = ManipulatorV3.Angles;
            old_angle.O2 = e.NewValue;

            ManipulatorV3.Rotate(old_angle);
        }

        private void Manipulator_Angle3_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Angles_Manipulator old_angle = ManipulatorV3.Angles;
            old_angle.O3 = e.NewValue;

            ManipulatorV3.Rotate(old_angle);
        }

        private void Manipulator_Angle4_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Angles_Manipulator old_angle = ManipulatorV3.Angles;
            old_angle.O4 = e.NewValue;

            ManipulatorV3.Rotate(old_angle);
        }

        private void Manipulator_Angle5_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Angles_Manipulator old_angle = ManipulatorV3.Angles;
            old_angle.O5 = e.NewValue;

            ManipulatorV3.Rotate(old_angle);
        }

        private void Portal_AngleX_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Angles_Portal old_angle = PortalV3.Angles;
            old_angle.X = e.NewValue;

            PortalV3.Rotate(old_angle);
        }

        private void Portal_AngleY_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Angles_Portal old_angle = PortalV3.Angles;
            old_angle.Y = e.NewValue;

            PortalV3.Rotate(old_angle);
        }

        private void Portal_AngleZ_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Angles_Portal old_angle = PortalV3.Angles;
            old_angle.Z = e.NewValue;

            PortalV3.Rotate(old_angle);
        }

        private void Portal_Angle1_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Angles_Portal old_angle = PortalV3.Angles;
            old_angle.O1 = e.NewValue;

            PortalV3.Rotate(old_angle);
        }

        private void Portal_Angle2_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Angles_Portal old_angle = PortalV3.Angles;
            old_angle.O2 = e.NewValue;

            PortalV3.Rotate(old_angle);
        }
        #endregion

        private void PortalOn_Click(object sender, RoutedEventArgs e)
        {
            DPort.Open();
            //try {
            //    portal_port = new ComPort(Portal_com_port.SelectedItem.ToString(), 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            //    Console.WriteLine(portal_port.Open());
            //}
            //catch (Exception ex) { MessageBox.Show(ex.Message); }
        } 

        private void PortalPlay_Click(object sender, RoutedEventArgs e)
        {
            DPort.Global(new GPoint() {
                X = PortalV3.Angles.X,
                Y = PortalV3.Angles.Y,
                Z = PortalV3.Angles.Z,
                A = PortalV3.Angles.O1,
                B = PortalV3.Angles.O2
            });
            DPort.Start();
            //portal_port.Play(PortalV3.Angles);
        }

        private void PortalHome_Click(object sender, RoutedEventArgs e)
        {
            DPort.Home();
            //portal_port.Home();
        }

        private void PortalOff_Click(object sender, RoutedEventArgs e)
        {
            DPort.Close();
            //portal_port.Close();
        }

        private void ManipulatorOn_Click(object sender, RoutedEventArgs e)
        {
            MPort.Open();
        }

        private void ManipulatorPlay_Click(object sender, RoutedEventArgs e)
        { 
            MPort.Global(new GPoint()
            {
                X = ManipulatorV3.Angles.O1,
                Y = ManipulatorV3.Angles.O2,
                Z = ManipulatorV3.Angles.O3,
                A = ManipulatorV3.Angles.O4,
                B = ManipulatorV3.Angles.O5
            });
            MPort.Start();
        }

        private void ManipulatorOff_Click(object sender, RoutedEventArgs e)
        {
            MPort.Close();
        }


        public void Refresh_Sliders()
        {
            Manipulator_Angle1_Slider.Value = ManipulatorV3.Angles.O1;
            Manipulator_Angle2_Slider.Value = ManipulatorV3.Angles.O2;
            Manipulator_Angle3_Slider.Value = ManipulatorV3.Angles.O3;
            Manipulator_Angle4_Slider.Value = ManipulatorV3.Angles.O4;
            Manipulator_Angle5_Slider.Value = ManipulatorV3.Angles.O5;

            Portal_Angle1_Slider.Value = PortalV3.Angles.O1;
            Portal_Angle2_Slider.Value = PortalV3.Angles.O2;
            Portal_AngleX_Slider.Value = PortalV3.Angles.X;
            Portal_AngleY_Slider.Value = PortalV3.Angles.Y;
            Portal_AngleZ_Slider.Value = PortalV3.Angles.Z;
        }

        private void Object_Z_rotation_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Scan_Object.getInstant.Rotate(new Angles_Scan_Object() { Z_rotation = e.NewValue, Y_rotation = Scan_Object.getInstant.Angles.Y_rotation });
        }

        private void Object_Y_rotation_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Scan_Object.getInstant.Rotate(new Angles_Scan_Object() { Z_rotation = Scan_Object.getInstant.Angles.Z_rotation, Y_rotation = e.NewValue });
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            double x = e.GetPosition((Image)sender).X;
            double y = e.GetPosition((Image)sender).Y;

            double W = 200.0;
            double H = 360.0;

            double AW = (sender as Image).ActualWidth;
            double AH = (sender as Image).ActualHeight;

            double y_new = 76 - x * W / AW;
            double x_new = 156.6 - y * H / AH;
            //Console.WriteLine(string.Format("Точка x={0}; y={1}", x_new, y_new));

            Scan_Object.getInstant.Base_X = x_new;
            Scan_Object.getInstant.Base_Y = y_new;
        }

        private void Configure_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save_file_dialog = new SaveFileDialog()
            {
                Filter = "Txt files (*.txt)|*.txt",
                InitialDirectory = System.IO.Path.GetFullPath(@"Model3d\Detals\Config")
            };
            if (save_file_dialog.ShowDialog() == true)
            {
                Detal_Config config = Detal_Config.getInstance;
                Auto_gen_model auto = Auto_gen_model.getInstance;

                config.Detal_Base = Scan_Object.getInstant.Base_Point;
                config.Positions = Detal_Config.getInstance.Key_Point_ListToPosition_List(Key_Point_List.getInstance);
                config.Radius = auto.Radius;
                config.Methodic_name = auto.Methodic_name;
                config.Num = auto.Num;

                Detal_Config_Parser.Save_Config(System.IO.Path.GetFullPath(save_file_dialog.FileName));
            }
        }
    }
}

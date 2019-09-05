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

namespace ISC_Rentgen.GUI.View
{
    /// <summary>
    /// Логика взаимодействия для Control.xaml
    /// </summary>
    public partial class Control : UserControl
    {
        public ComPort manip_port  = new ComPort();
        public ComPort portal_port = new ComPort();

        public Control()
        {
            InitializeComponent();
        }

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
        

        private void PortalOn_Click(object sender, RoutedEventArgs e)
        {
            try {
                portal_port = new ComPort(Portal_com_port.SelectedItem.ToString(), 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
                Console.WriteLine(portal_port.Open());
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        } 

        private void PortalPlay_Click(object sender, RoutedEventArgs e)
        {
            portal_port.Play(PortalV3.Angles);
        }

        private void PortalHome_Click(object sender, RoutedEventArgs e)
        {
            portal_port.Home();
        }

        private void PortalOff_Click(object sender, RoutedEventArgs e)
        {
            portal_port.Close();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            string[] port_names = manip_port.Get_ports();
            foreach (string port_name in port_names)
            {
                Manip_com_port.Items.Add(port_name);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Manip_com_port.Items.Clear();
            Portal_com_port.Items.Clear();
            string[] port_names = manip_port.Get_ports();
            foreach (string port_name in port_names)
            {
                Manip_com_port.Items.Add(port_name);
                Portal_com_port.Items.Add(port_name);
            }
        }

        private void ManipulatorOn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                manip_port = new ComPort(Manip_com_port.SelectedItem.ToString(), 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
                Console.WriteLine(manip_port.Open());
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ManipulatorPlay_Click(object sender, RoutedEventArgs e)
        {
            manip_port.Play(ManipulatorV3.Angles);
        }

        private void ManipulatorOff_Click(object sender, RoutedEventArgs e)
        {
            manip_port.Close();
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
            Scan_Object.Rotate(new Angles_Scan_Object() { Z_rotation = e.NewValue, Y_rotation = Scan_Object.Angles.Y_rotation });
        }

        private void Object_Y_rotation_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Scan_Object.Rotate(new Angles_Scan_Object() { Z_rotation = Scan_Object.Angles.Z_rotation, Y_rotation = e.NewValue });
        }

        private void Scan_Object_TextChanged(object sender, TextChangedEventArgs e)
        {
            double x, y, z;
            if (double.TryParse(Scan_Object_x.Text, out x) & double.TryParse(Scan_Object_y.Text, out y) & double.TryParse(Scan_Object_z.Text, out z))
            {
                Scan_Object.Base(new Point3D(x, y, z));
            }
        }
    }
}

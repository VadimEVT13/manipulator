using ISC_Rentgen.Rentgen_Parts.Manipulator_Components.Model;
using ISC_Rentgen.Rentgen_Parts.Portal_Components.Model;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ISC_Rentgen.Grbl.Models
{
    public class ComPort
    {
        public SerialPort Port { get { return port; } }
        private SerialPort port = new SerialPort();

        public string Port_name { get { return port_name; } }
        private string port_name = "COM1";

        public int Baud_rate { get { return baud_rate; } }
        private int baud_rate = 19200;

        public Parity Port_parity { get { return port_parity; } }
        private Parity port_parity = Parity.None;

        public int Data_bits { get { return data_bits; } }
        private int data_bits = 8;

        public StopBits Stop_bits { get { return stop_bits; } }
        private StopBits stop_bits = StopBits.One;

        public ComPort(string PortName, int BaudRate, Parity Port_parity, int DataBits, StopBits StopBits)
        {
            port_name = PortName;
            baud_rate = BaudRate;
            port_parity = Port_parity;
            data_bits = DataBits;
            stop_bits = StopBits;

            port = new SerialPort(port_name, baud_rate, Port_parity, DataBits, StopBits);
            port.Handshake = Handshake.None;
            port.WriteTimeout = 1000;
            port.ReadTimeout  = 1000;
        }

        public ComPort()
        {
            port = new SerialPort();
        }

        public string[] Get_ports()
        {
            return SerialPort.GetPortNames();
        }

        public void Play(Angles_Portal angles)
        {
            if (port.IsOpen)
            {
                Angles_Portal Grbl_angles = Portal_angle_ogranichenie.Normalize_for_grbl(angles);
                string command = string.Format("g90 X{0} Y{1} Z{2} A{3} B{4}", Grbl_angles.X, Grbl_angles.Y - 20, Grbl_angles.Z, Grbl_angles.O1 + 59, -Grbl_angles.O2 + 91.5);
                command = command.Replace(',', '.');
                port.WriteLine(command);
                Console.WriteLine(command);
            }
        }

        public void Play(Angles_Manipulator angles)
        {
            if (port.IsOpen)
            {
                Angles_Manipulator Grbl_angles = Manipulator_angle_ogranichenie.Normalize_for_grbl(angles);
                string command = string.Format("g90 Z{0} X{1} Y{2} A{3} B{4}", -Grbl_angles.O1 + 126.519, Grbl_angles.O2 + 234.075, -Grbl_angles.O3 + 136.758, -Grbl_angles.O4 + 32.344, Grbl_angles.O5 - 90 + 185.757);
                command = command.Replace(',', '.');
                port.WriteLine(command);
                Console.WriteLine(command);
            }
            //182.329
        }

        public bool Open()
        {
            if (!port.IsOpen)
            {
                port.Open();
                Thread.Sleep(100);
                port.WriteLine("$X");
            }
            return port.IsOpen;
        }

        public void Close()
        {
            if (port.IsOpen)
            {
                port.Close();
            }
        }

        public void Write_to_port(string message)
        {
            port.WriteLine(message);
        }

        public void Home()
        {
            if (port.IsOpen)
            {
                port.WriteLine("$H");
            }
        }
    }
}

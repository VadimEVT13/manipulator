using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using log4net;
using Manipulator.GRBL.Utils;

/// <summary>
/// Драйвер работы с COM-портом GRBL устройства.
/// </summary>
namespace Manipulator.GRBL.Models
{

    /// <summary>
    /// Делегат для принятия данных из serailPort
    /// </summary>
    /// <param name="data"></param>
    public delegate void DataReceived(GState data);

    public class GPort
    {
        /// <summary>
        /// Событие вызываемое при получении данных из последовательного порта
        /// </summary>
        public event DataReceived OnDataReceived;


        private GState state;

        /// <summary>
        /// Логгер класса.
        /// </summary>
        private static ILog LOG = LogManager.GetLogger("GrblPort");
        /// <summary>
        /// Команда вызова CTRL + X.
        /// </summary>
        private static char CMD_CTRL_X = Convert.ToChar(24);
        /// <summary>
        /// Команда мягкого сброса сессий устройства.
        /// </summary>
        private static String SOFT_RESET = Convert.ToString(CMD_CTRL_X);
        /// <summary>
        /// COM-порт.
        /// </summary>
        private SerialPort serialPort;

        private object locker = new object();

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public GPort(GDevice settings)
        {
            this.Settings = settings;
        }


        public bool IsOpen
        {
            get
            {
                return serialPort != null && serialPort.IsOpen;
            }
        }

        public bool IsPlay
        {
            get
            {
                return serialPort != null && serialPort.IsOpen
                    && state != null && !state.Status.Equals(GStatus.HOLD);
            }
        }



        /// <summary>
        /// Настройки устройства.
        /// </summary>
        public GDevice Settings { get; set; }

        /// <summary>
        /// Открытие порта на основе настроек.
        /// </summary>
        /// <returns>состояние</returns>
        public bool Open()
        {
            if (Settings == null)
            {
                LOG.Error("Exception: Device not null");
                return false;
            }
            if (Settings.PortName == null)
            {
                Settings = GPortFind.FindPort(Settings);
                if (Settings.PortName == null)
                {
                    LOG.Error("Exception: Port not null");
                    return false;
                }
            }
            serialPort = new SerialPort(Settings.PortName, Settings.BaudRate, Settings.Parity, Settings.DataBits, Settings.StopBits);
            //Подписываемся на получение данных из последовательного порта
            serialPort.DataReceived += onDataReceivedFromSerialPort;
            if (serialPort.IsOpen)
            {
                LOG.Error("Exception: Port is opened");
                return false;
            }
            try
            {
                serialPort.Open();
                serialPort.WriteLine(SOFT_RESET);
                Thread.Sleep(Settings.Timeout);
                serialPort.ReadExisting();
                LOG.Info("Open port");
                return true;
            }
            catch (UnauthorizedAccessException e)
            {
                LOG.Error("Error open port:" + e);
            }
            catch (ArgumentOutOfRangeException e)
            {
                LOG.Error("Error open port:" + e);
            }
            catch (ArgumentException e)
            {
                LOG.Error("Error open port:" + e);
            }
            catch (IOException e)
            {
                LOG.Error("Error open port:" + e);
            }
            catch (InvalidOperationException e)
            {
                LOG.Error("Error open port:" + e);
            }
            return false;
        }


        private void onDataReceivedFromSerialPort(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = sender as SerialPort;
            if (serialPort.BytesToRead > 0)
            {
                var lineData = serialPort.ReadLine();
                if (lineData.Length > 0)
                {
                    Console.WriteLine("LineData: " + lineData);
                    try
                    {
                        var state = GConvert.ToState(lineData);

                        if (state != null)
                        {
                            this.state = state;
                            OnDataReceived?.Invoke(state);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Запрос состояния устройства.
        /// </summary>
        public void State()
        {
            if (IsOpen)
            {
                LOG.Info("Write: ?");
                serialPort.WriteLine("?");
            }
        }

        /// <summary>
        /// Запуск выполнения программ.
        /// </summary>
        public void Start()
        {
            if (IsOpen)
            {
                LOG.Info("Write: ~");
                serialPort.WriteLine("~");
                this.State();
            }
        }

        /// <summary>
        /// Прерывание выполнения программ.
        /// </summary>
        public void Pause()
        {
            if (IsOpen)
            {
                LOG.Info("Write: !");
                serialPort.WriteLine("!");
                this.State();
            }
        }

        /// <summary>
        /// Выполнение команды возврат домой.
        /// </summary>
        public void Home()
        {
            if (IsOpen)
            {
                LOG.Info("Write: $H");
                serialPort.WriteLine("$H");
                this.State();
            }
        }

        /// <summary>
        /// Выполнение команды снятия блокировки.
        /// </summary>
        public void Unlock()
        {
            if (IsOpen)
            {
                LOG.Info("Write: $X");
                serialPort.WriteLine("$X");
                this.State();
            }
        }

        /// <summary>
        /// Перемещение устройства в глобальную точку.
        /// </summary>
        /// <param name="point">точка перемещения</param>
        public void Global(GPoint point)
        {
            if (IsOpen)
            {
                String cmd = PointToString(point);
                LOG.Info("Write: G90");
                serialPort.WriteLine("G90");
                LOG.Info("Write: " + cmd);
                serialPort.WriteLine(cmd);
                this.State();
            }
        }

        /// <summary>
        /// Преобразование точки в команду GRBL
        /// </summary>
        /// <param name="point">точка</param>
        /// <returns>точка в GRBL</returns>
        public String PointToString(GPoint point)
        {
            StringBuilder cmd = new StringBuilder();
            if (Settings.IsX)
            {
                cmd.Append(" X ");
                cmd.Append(Convert.ToString(point.X).Replace(",", "."));
            }
            if (Settings.IsY)
            {
                cmd.Append(" Y ");
                cmd.Append(Convert.ToString(point.Y).Replace(",", "."));
            }
            if (Settings.IsZ)
            {
                cmd.Append(" Z ");
                cmd.Append(Convert.ToString(point.Z).Replace(",", "."));
            }
            if (Settings.IsA)
            {
                cmd.Append(" A ");
                cmd.Append(Convert.ToString(point.A).Replace(",", "."));
            }
            if (Settings.IsB)
            {
                cmd.Append(" B ");
                cmd.Append(Convert.ToString(point.B).Replace(",", "."));
            }
            if (Settings.IsC)
            {
                cmd.Append(" C ");
                cmd.Append(Convert.ToString(point.C).Replace(",", "."));
            }
            if (Settings.IsD)
            {
                cmd.Append(" D ");
                cmd.Append(Convert.ToString(point.D).Replace(",", "."));
            }
            if (Settings.IsE)
            {
                cmd.Append(" E ");
                cmd.Append(Convert.ToString(point.E).Replace(",", "."));
            }
            return cmd.ToString();
        }

        /// <summary>
        /// Перемещение устройства в локальную точку.
        /// </summary>
        /// <param name="point">точка перемещения</param>
        public void Local(GPoint point)
        {
            if (IsOpen)
            {
                String cmd = PointToString(point);
                LOG.Info("Write: G91");
                serialPort.WriteLine("G91");
                LOG.Info("Write: " + cmd);
                serialPort.WriteLine(cmd);
                this.State();
            }
        }

        /// <summary>
        /// Закрытие порта.
        /// </summary>
        public void Close()
        {
            if (IsOpen)
            {
                try
                {
                    serialPort.Close();
                    LOG.Info("Close port");
                }
                catch (InvalidOperationException e)
                {
                    LOG.Error("Exception close port:" + e);
                }
            }
        }
    }
}

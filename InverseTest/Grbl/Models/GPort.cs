using System;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using InverseTest.Grbl.Finders;
using Newtonsoft.Json;
using NLog;

/// <summary>
/// Драйвер работы с COM-портом GRBL устройства.
/// </summary>
namespace InverseTest.Grbl.Models
{
    public class GPort : INotifyPropertyChanged
    {
        /// <summary>
        /// Логгирование
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private GStatus _state;
        public GStatus Status
        {
            get { return _state; }
            set
            {
                _state = value;
                RaiseProperty("Status");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaiseProperty(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public GPort(GDevice settings)
        {
            this.Settings = settings;
            Status = new GStatus();
        }


        public bool IsOpen
        {
            get
            {
                return serialPort != null && serialPort.IsOpen;
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
        public void Open()
        {
            if (Settings == null)
            {
                logger.Error("Device not null");
                return;
            }
            if (Settings.PortName == null)
            {
                Settings = GPortFind.FindPort(Settings);
                if (Settings.PortName == null)
                {
                    logger.Error("Port not null");
                    return;
                }
            }
            serialPort = new SerialPort(Settings.PortName, Settings.BaudRate, Settings.Parity, Settings.DataBits, Settings.StopBits);
            //Подписываемся на получение данных из последовательного порта
            serialPort.DataReceived += onDataReceivedFromSerialPort;
            if (serialPort.IsOpen)
            {
                logger.Error("Port is opened");
                return;
            }
            try
            {
                serialPort.Open();
                serialPort.WriteLine(SOFT_RESET);
                Thread.Sleep(100);
                serialPort.ReadExisting();
                logger.Info("Open port");
            }
            catch (UnauthorizedAccessException e)
            {
                logger.Error("Error open port:" + e);
            }
            catch (ArgumentOutOfRangeException e)
            {
                logger.Error("Error open port:" + e);
            }
            catch (ArgumentException e)
            {
                logger.Error("Error open port:" + e);
            }
            catch (IOException e)
            {
                logger.Error("Error open port:" + e);
            }
            catch (InvalidOperationException e)
            {
                logger.Error("Error open port:" + e);
            }
            Thread.Sleep(100);
            State();
        }


        private void onDataReceivedFromSerialPort(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = sender as SerialPort;
            if (serialPort.BytesToRead > 0)
            {
                var lineData = serialPort.ReadLine();
                if (lineData.Length > 0 && lineData[0] == '{')
                {
                    logger.Debug("Data: {0}",lineData);
                    GStatus newState = null;
                    try
                    {
                        newState = JsonConvert.DeserializeObject<GStatus>(lineData);
                    }
                    catch (JsonSerializationException ex)
                    {
                        logger.Error("Error read data", ex);
                    }
                    if (newState != null)
                    {
                        Status = newState;
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
                logger.Info("Write: ?");
                serialPort.WriteLine("?");
            }
            else
            {
                Status.Status = GState.DISCONNECT;
            }
        }

        /// <summary>
        /// Запуск выполнения программ.
        /// </summary>
        public void Start()
        {
            if (IsOpen)
            {
                logger.Info("Write: ~");
                serialPort.WriteLine("~");
            }
            State();
        }

        /// <summary>
        /// Прерывание выполнения программ.
        /// </summary>
        public void Pause()
        {
            if (IsOpen)
            {
                logger.Info("Write: !");
                serialPort.WriteLine("!");
            }
            State();
        }

        /// <summary>
        /// Выполнение команды возврат домой.
        /// </summary>
        public void Home()
        {
            if (IsOpen)
            {
                logger.Info("Write: $H");
                serialPort.WriteLine("$H");
            }
            State();
        }

        /// <summary>
        /// Выполнение команды снятия блокировки.
        /// </summary>
        public void Unlock()
        {
            if (IsOpen)
            {
                logger.Info("Write: $X");
                serialPort.WriteLine("$X");
            }
            State();
        }

        /// <summary>
        /// Перемещение устройства в глобальную точку.
        /// </summary>
        /// <param name="point">точка перемещения</param>
        public void Global(GPoint point)
        {
            if (IsOpen)
            {
                var builder = new StringBuilder();
                builder.Append("G90 ");
                builder.Append(PointToString(point));
                builder.Append("\n");
                logger.Info("Send: " + builder.ToString());
                serialPort.WriteLine(builder.ToString());
                State();
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
                logger.Info("Write: G91");
                serialPort.WriteLine("G91");
                logger.Info("Write: " + cmd);
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
                    logger.Info("Close port");
                }
                catch (InvalidOperationException e)
                {
                    logger.Error("Exception close port:" + e);
                }
            }
            State();
        }
    }
}

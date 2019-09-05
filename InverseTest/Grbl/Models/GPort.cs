using System;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using InverseTest.Grbl.Finders;
using Newtonsoft.Json;
using NLog;
using SimpleTCP;

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
        /// Порт.
        /// </summary>
        private SimpleTcpClient client;

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
                return client != null;
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
            logger.Info("Connected to device");
            if (Settings == null || Settings.PortName == null)
            {
                logger.Error("Device not defined");
                return;
            }
            client = new SimpleTcpClient();
            client.StringEncoder = Encoding.UTF8;
            try
            {
                client.Connect(Settings.PortName, 2000);
            }
            catch (SocketException)
            {
                client = null;
                State();
                logger.Error("Device not found");
                return;
            }
            client.DataReceived += onDataReceivedFromSerialPort;
            client.WriteLineAndGetReply(SOFT_RESET, TimeSpan.FromSeconds(5));
            State();
        }

        private void onDataReceivedFromSerialPort(object sender, Message m)
        {
            var lineData = m.MessageString;
            if (lineData.Length > 0 && lineData[0] == '{')
            {
                logger.Debug("Data: {0}", lineData);
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

        /// <summary>
        /// Запрос состояния устройства.
        /// </summary>
        public void State()
        {
            if (IsOpen)
            {
                logger.Info("Write: ?");
                client.WriteLineAndGetReply("?", TimeSpan.FromSeconds(5));
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
                client.WriteLineAndGetReply("~", TimeSpan.FromSeconds(5));
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
                client.WriteLineAndGetReply("!", TimeSpan.FromSeconds(5));
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
                client.WriteLineAndGetReply("$H", TimeSpan.FromSeconds(5));
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
                client.WriteLineAndGetReply("$X", TimeSpan.FromSeconds(5));
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
                client.WriteLineAndGetReply(builder.ToString(), TimeSpan.FromSeconds(5));
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
                var cmd = new StringBuilder();
                cmd.AppendLine("G91");
                cmd.AppendLine("PointToString(point)");
                logger.Info("Write: {0}", cmd.ToString());
                client.WriteLineAndGetReply(cmd.ToString(), TimeSpan.FromSeconds(5));
                State();
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
                    client.Disconnect();
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

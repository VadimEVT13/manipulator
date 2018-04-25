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
    public class GPort
    {
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
        private GPort() { }

        /// <summary>
        /// Статический класс для паттерна одиночка.
        /// </summary>
        private static class Holder
        {
            /// <summary>
            /// Статический компонент для паттерна одиночка.
            /// </summary>
            public static readonly GPort INSTANCE = new GPort();
        }

        /// <summary>
        /// Паттерн одиночка.
        /// </summary>
        /// <returns>экземляр класса</returns>
        public static GPort GetInstance()
        {
            return Holder.INSTANCE;
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

        /// <summary>
        /// Получение состояния устройства.
        /// </summary>
        /// <returns>состояние устройства</returns>
        public GState State()
        {
            if (serialPort == null || !serialPort.IsOpen)
            {
                return new GState
                {
                    Status = GStatus.DISCONNECT
                };
            }
            GState state = null;
            lock (locker)
            {
                try
                {
                    while (serialPort.IsOpen && state == null)
                    {
                        serialPort.ReadExisting();
                        serialPort.WriteLine("?");
                        LOG.Info("Write: ?");
                        Thread.Sleep(Settings.Timeout);
                        string line = null;
                        if (serialPort.BytesToRead > 0)
                        {
                            line = serialPort.ReadLine();
                        }
                        if (line != null)
                        {
                            LOG.Info("State: " + line);
                            state = GConvert.ToState(line);
                        }
                    }
                }
                catch (InvalidOperationException e)
                {
                    LOG.Error("Exception state port:" + e);
                    state = new GState
                    {
                        Status = GStatus.ALARM
                    };
                }
                catch (IOException e)
                {
                    LOG.Error("Exception state port:" + e);
                    state = new GState
                    {
                        Status = GStatus.ALARM
                    };
                }
            }
            return state;
        }

        /// <summary>
        /// Запуск выполнения программ.
        /// </summary>
        public void Start()
        {
            if (serialPort == null || !serialPort.IsOpen)
            {
                return;
            }
            try
            {
                LOG.Info("Write: ~");
                serialPort.WriteLine("~");
            }
            catch (InvalidOperationException e)
            {
                LOG.Error("Exception start port:" + e);
            }
        }

        /// <summary>
        /// Прерывание выполнения программ.
        /// </summary>
        public void Pause()
        {
            if (serialPort == null || !serialPort.IsOpen)
            {
                return;
            }
            try
            {
                LOG.Info("Write: !");
                serialPort.WriteLine("!");
            }
            catch (InvalidOperationException e)
            {
                LOG.Error("Exception pause port:" + e);
            }
        }

        /// <summary>
        /// Выполнение команды возврат домой.
        /// </summary>
        public void Home()
        {
            if (serialPort == null || !serialPort.IsOpen)
            {
                return;
            }
            lock (locker)
            {
                try
                {
                    LOG.Info("Write: $H");
                    serialPort.WriteLine("$H");
                    int i = 0;
                    if (Settings.IsX) i++;
                    if (Settings.IsY) i++;
                    if (Settings.IsZ) i++;
                    if (Settings.IsA) i++;

                    if (Settings.IsB) i++;
                    if (Settings.IsC) i++;
                    if (Settings.IsD) i++;
                    if (Settings.IsE) i++;

                    while (serialPort.IsOpen && i > 0)
                    {
                        Thread.Sleep(Settings.Timeout);
                        string line = null;
                        if (serialPort.BytesToRead > 0)
                        {
                            line = serialPort.ReadLine();
                        }
                        if (line != null)
                        {
                            LOG.Info("State: " + line);
                            if (line.Contains("Idle"))
                            {
                                i -= 1;
                            }
                            if (line.Contains("ALARM"))
                            {
                                return;
                            }
                        }
                    }
                }
                catch (InvalidOperationException e)
                {
                    LOG.Error("Exception home port:" + e);
                }
            }
        }

        /// <summary>
        /// Выполнение команды снятия блокировки.
        /// </summary>
        public void Unlock()
        {
            if (serialPort == null || !serialPort.IsOpen)
            {
                return;
            }
            lock (locker)
            {
                try
                {
                    LOG.Info("Write: $X");
                    serialPort.WriteLine("$X");
                    Thread.Sleep(Settings.Timeout);
                    serialPort.ReadExisting();
                }
                catch (InvalidOperationException e)
                {
                    LOG.Error("Exception unlock port:" + e);
                }
            }
        }

        /// <summary>
        /// Перемещение устройства в глобальную точку.
        /// </summary>
        /// <param name="point">точка перемещения</param>
        public void Global(GPoint point)
        {
            if (serialPort == null || !serialPort.IsOpen)
            {
                return;
            }
            String cmd = PointToString(point);
            lock (locker)
            {
                try
                {
                    LOG.Info("Write: G90");
                    serialPort.WriteLine("G90");
                    LOG.Info("Write: " + cmd);
                    serialPort.WriteLine(cmd);
                    while (serialPort.IsOpen)
                    {
                        Thread.Sleep(Settings.Timeout);
                        string line = null;
                        if (serialPort.BytesToRead > 0)
                        {
                            line = serialPort.ReadLine();
                        }
                        if (line != null)
                        {
                            LOG.Info("State: " + line);
                            if (line.Contains("Idle"))
                            {
                                return;
                            }
                            if (line.Contains("ALARM"))
                            {
                                return;
                            }
                        }
                    }
                }
                catch (InvalidOperationException e)
                {
                    LOG.Info("Exception state port:" + e);
                }
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
            if (serialPort == null || !serialPort.IsOpen)
            {
                return;
            }
            String cmd = PointToString(point);
            lock (locker)
            {
                try
                {
                    LOG.Info("Write: G91");
                    serialPort.WriteLine("G91");
                    LOG.Info("Write: " + cmd);
                    serialPort.WriteLine(cmd);
                    while (serialPort.IsOpen)
                    {
                        Thread.Sleep(Settings.Timeout);
                        string line = null;
                        if (serialPort.BytesToRead > 0)
                        {
                            line = serialPort.ReadLine();
                        }
                        if (line != null)
                        {
                            LOG.Info("State: " + line);
                            if (line.Contains("Idle"))
                            {
                                return;
                            }
                            if (line.Contains("ALARM"))
                            {
                                return;
                            }
                        }
                    }
                }
                catch (InvalidOperationException e)
                {
                    LOG.Error("Exception state port:" + e);
                }
            }
        }

        /// <summary>
        /// Закрытие порта.
        /// </summary>
        public void Close()
        {
            if (serialPort != null && serialPort.IsOpen)
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

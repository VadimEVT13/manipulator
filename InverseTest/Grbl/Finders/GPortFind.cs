using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using InverseTest.Grbl.Models;
using log4net;

/// <summary>
/// Класс поиска имени порта по приветствию.
/// </summary>
namespace InverseTest.Grbl.Finders
{
    public class GPortFind
    {
        /// <summary>
        /// Логгер класса.
        /// </summary>
        private static ILog LOG = LogManager.GetLogger("PortFinder");
        /// <summary>
        /// Команда вызова CTRL + X.
        /// </summary>
        private static char CMD_CTRL_X = Convert.ToChar(24);
        /// <summary>
        /// Команда мягкого сброса сессий устройства.
        /// </summary>
        private static String SOFT_RESET = Convert.ToString(CMD_CTRL_X);

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        private GPortFind() { }

        /// <summary>
        /// Поиск имени порта устройства.
        /// </summary>
        /// <param name="device">модель настроек устройства</param>
        /// <returns>модель с найденным именем порта устройства</returns>
        public static GDevice FindPort(GDevice device)
        {
            LOG.Info("Find Port");
            GDevice result = device;
            result.PortName = null;
            foreach (String port in SerialPort.GetPortNames())
            {
                SerialPort serialPort = new SerialPort(port, device.BaudRate, device.Parity, device.DataBits, device.StopBits);
                if (!serialPort.IsOpen)
                {
                    try
                    {
                        serialPort.Open();
                        serialPort.WriteLine(SOFT_RESET);
                        Thread.Sleep(device.Timeout);
                        for (string line = serialPort.ReadLine(); serialPort.BytesToRead > 0; line = serialPort.ReadLine())
                        {
                            if (line.Contains(device.Name))
                            {
                                result.PortName = port;
                                LOG.Info("Port is " + port);
                                break;
                            }
                        }
                        serialPort.Close();
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        LOG.Error("Exception: Error open port:" + e);
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        LOG.Error("Exception: Error open port:" + e);
                    }
                    catch (ArgumentException e)
                    {
                        LOG.Error("Exception: Error open port:" + e);
                    }
                    catch (IOException e)
                    {
                        LOG.Error("Exception: Error open port:" + e);
                    }
                    catch (InvalidOperationException e)
                    {
                        LOG.Error("Exception: Error open port:" + e);
                    }
                }
            }
            return result;
        }
    }
}

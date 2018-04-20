using System;
using System.IO.Ports;

/// <summary>
/// Модель данных устройства.
/// </summary>
namespace Manipulator.GRBL.Models
{
    public class GDevice
    {
        /// <summary>
        /// Стандартная скорость обмена.
        /// </summary>
        private static int DEFAULT_BAUD_RATE = 115200;
        /// <summary>
        /// Стандартное число битов данных в байте.
        /// </summary>
        private static int DEFAULT_DATA_BITS = 8;
        /// <summary>
        /// Стандартное число стоповых битов в байте.
        /// </summary>
        private static StopBits DEFAULT_STOP_BITS = StopBits.One;
        /// <summary>
        /// Стандартный протокол контроля четности.
        /// </summary>
        private static Parity DEFAULT_PARITY = Parity.None;
        /// <summary>
        /// Стандартный таймаут на чтение и запись.
        /// </summary>
        private static int DEFAULT_TIMEOUT = 50;

        /// <summary>
        /// Имя последовательного порта.
        /// </summary>
        public String PortName { get; set; }

        /// <summary>
        /// Имя устройства.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Скорость передачи последовательного порта
        /// </summary>
        public int BaudRate { get; set; }

        /// <summary>
        /// Число битов данных в байте.
        /// </summary>
        public int DataBits { get; set; }

        /// <summary>
        /// Число стоповых битов в байте.
        /// </summary>
        public StopBits StopBits { get; set; }

        /// <summary>
        /// Протокол контроля четности.
        /// </summary>
        public Parity Parity { get; set; }

        /// <summary>
        /// Ожидание в миллисекундах для завершения операции записи и чтения.
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public GDevice()
        {
            BaudRate = DEFAULT_BAUD_RATE;
            DataBits = DEFAULT_DATA_BITS;
            Parity = DEFAULT_PARITY;
            StopBits = DEFAULT_STOP_BITS;
            Timeout = DEFAULT_TIMEOUT;
        }
    }
}

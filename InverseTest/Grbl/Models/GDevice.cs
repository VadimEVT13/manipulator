using System;
using System.IO.Ports;

/// <summary>
/// Модель данных устройства.
/// </summary>
namespace InverseTest.Grbl.Models
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
        /// Устройство содержит ось X.
        /// </summary>
        public bool IsX { get; set; }

        /// <summary>
        /// Устройство содержит ось Y.
        /// </summary>
        public bool IsY { get; set; }

        /// <summary>
        /// Устройство содержит ось Z.
        /// </summary>
        public bool IsZ { get; set; }

        /// <summary>
        /// Устройство содержит ось A.
        /// </summary>
        public bool IsA { get; set; }

        /// <summary>
        /// Устройство содержит ось B.
        /// </summary>
        public bool IsB { get; set; }

        /// <summary>
        /// Устройство содержит ось C.
        /// </summary>
        public bool IsC { get; set; }

        /// <summary>
        /// Устройство содержит ось D.
        /// </summary>
        public bool IsD { get; set; }

        /// <summary>
        /// Устройство содержит ось E.
        /// </summary>
        public bool IsE { get; set; }

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
            IsX = true;
            IsY = true;
            IsZ = true;
            IsA = true;
            IsB = true;
            IsC = true;
            IsD = true;
            IsE = true;
        }
    }
}

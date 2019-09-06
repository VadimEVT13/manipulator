using NLog;

namespace Roentgen.Devices.Models
{
    /// <summary>
    /// Манипулятор
    /// </summary>
    public class GManipulator
    {
        #region Службы
        /// <summary>
        /// Логгирование
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Свойства
        /// <summary>
        /// Настройки устройства
        /// </summary>
        private static readonly GDevice settings = new GDevice
        {
            Name = "XManipulator",
            PortName = "192.168.0.12",
            IsX = true,
            IsY = true,
            IsZ = false,
            IsA = true,
            IsB = true,
            IsC = false,
            IsD = false,
            IsE = false
        };
        /// <summary>
        /// Порт устройства
        /// </summary>
        public GPort Port { get; set; }
        #endregion

        #region Синглетон
        private static GManipulator instance;
        public static GManipulator getInstance()
        {
            if (instance == null)
                instance = new GManipulator();
            return instance;
        }
        #endregion

        #region Конструкторы
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        private GManipulator()
        {
            Port = new GPort(settings);
        }
        #endregion

        #region Методы
        private static double GetLimitValue(double value, double minValue, double maxValue)
        {
            if (value < minValue)
            {
                return minValue;
            }
            else if (value > maxValue)
            {
                return maxValue;
            }
            else
            {
                return value;
            }
        }

        public static GPoint GlobalLimits(GPoint target)
        {
            var x = GetLimitValue(target.X, 5, 150) + (234.250 - 90);
            var y = GetLimitValue(target.Y, 0, 150) + (136.670 - 90);
            var z = GetLimitValue(target.Z, 0, 180);
            var a = GetLimitValue(target.A, -179, 179) + 32.959;
            var b = -GetLimitValue(target.B, -90, 85) + (185.493);

            return new GPoint
            {
                X = x,
                Y = y,
                Z = z,
                A = a,
                B = b
            };
        }
        #endregion
    }
}

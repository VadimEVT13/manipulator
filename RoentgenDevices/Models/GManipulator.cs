namespace Roentgen.Devices.Models
{
    /// <summary>
    /// Манипулятор
    /// </summary>
    public class GManipulator
    {
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
            return new GPoint
            {
                X = GetLimitValue(target.X, 5, 150) + (234.250 - 90),
                Y = GetLimitValue(target.Y, 0, 150) + (263.870 - 90), //263.870
                Z = GetLimitValue(target.Z, 0, 180),
                A = GetLimitValue(target.A, -179, 179) + 32.959,
                B = -GetLimitValue(target.B, -90, 85) + (185.493)
            };
        }
        #endregion
    }
}

/// <summary>
/// Модель данных - сотояние устройства.
/// </summary>
namespace Manipulator.GRBL.Models
{
    public class GState
    {
        /// <summary>
        /// Локальное положение.
        /// </summary>
        public GPoint Local { get; set; }

        /// <summary>
        /// Глобальное положение.
        /// </summary>
        public GPoint Global { get; set; }

        /// <summary>
        /// Состояние устройства.
        /// </summary>
        public GStatus Status { get; set; }

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public GState()
        {
            Local = new GPoint();
            Global = new GPoint();
            Status = GStatus.DISCONNECT;
        }
    }
}

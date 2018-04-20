/// <summary>
/// Список состояний GRBL - контроллера.
/// </summary>
namespace Manipulator.GRBL.Models
{
    public enum GStatus
    {
        /// <summary>
        /// Простой.
        /// </summary>
        IDLE,
        /// <summary>
        /// Движение.
        /// </summary>
        RUN,
        /// <summary>
        /// Удержание.
        /// </summary>
        HOLD,
        /// <summary>
        /// Ожидание закрытия защиты.
        /// </summary>
        DOOR,
        /// <summary>
        /// Самонаведение.
        /// </summary>
        HOME,
        /// <summary>
        /// Тревога.
        /// </summary>
        ALARM,
        /// <summary>
        /// Проверка G-кода без исполнения.
        /// </summary>
        CHECK,
        /// <summary>
        /// Нет подключения.
        /// </summary>
        DISCONNECT
    };
}

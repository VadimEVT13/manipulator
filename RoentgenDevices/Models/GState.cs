/// <summary>
/// Список состояний GRBL - контроллера.
/// </summary>
namespace Roentgen.Devices.Models
{
    public enum GState
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

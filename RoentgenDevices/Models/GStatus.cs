using Newtonsoft.Json;
using Roentgen.Devices.Converters;
using System.ComponentModel;

/// <summary>
/// Модель данных - сотояние устройства.
/// </summary>
namespace Roentgen.Devices.Models
{
    public class GStatus : INotifyPropertyChanged
    {
        #region Поля
        /// <summary>
        /// Состояние устройства
        /// </summary>
        [JsonIgnore]
        private GState state;
        #endregion

        #region Свойства
        /// <summary>
        /// Положение
        /// </summary>
        [JsonProperty("MPos")]
        public GPoint Position { get; set; }
        /// <summary>
        /// Состояние устройства
        /// </summary>
        [JsonProperty("State", Required = Required.Always)]
        [JsonConverter(typeof(GStateToStringConverter))]
        public GState State
        {
            get => state;
            set
            {
                state = value;
                RaiseProperty("Status");
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaiseProperty(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Конструкторы
        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public GStatus()
        {
            Position = new GPoint();
            State = GState.DISCONNECT;
        }
        #endregion
    }
}

using InverseTest.Grbl.Converters;
using Newtonsoft.Json;
using System.ComponentModel;
/// <summary>
/// Модель данных - сотояние устройства.
/// </summary>
namespace InverseTest.Grbl.Models
{
    public class GStatus : INotifyPropertyChanged
    {
        /// <summary>
        /// Положение
        /// </summary>
        [JsonProperty("MPos")]
        public GPoint Position { get; set; }

        /// <summary>
        /// Состояние устройства.
        /// </summary>
        [JsonIgnore]
        private GState _status;
        [JsonProperty("State", Required = Required.Always)]
        [JsonConverter(typeof(GStatusToStringConverter))]
        public GState Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaiseProperty("Status");
            }
        }

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public GStatus()
        {
            Position = new GPoint();
            Status = GState.DISCONNECT;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaiseProperty(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

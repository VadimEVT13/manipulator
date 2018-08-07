using System.ComponentModel;
/// <summary>
/// Модель данных - сотояние устройства.
/// </summary>
namespace InverseTest.Grbl.Models
{
    public class GState : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaiseProperty(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

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
        private GStatus _status;
        public GStatus Status
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
        public GState()
        {
            Local = new GPoint();
            Global = new GPoint();
            Status = GStatus.DISCONNECT;
        }
    }
}

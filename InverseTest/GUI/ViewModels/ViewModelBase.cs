using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using NLog;

namespace InverseTest.GUI.ViewModels
{
    /// <summary>
    /// Implements INotifyPropertyChanged for all ViewModel
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Логгирование
        /// </summary>
        protected static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected bool SetValue<T>(ref T backingField, T value, [CallerMemberName]string propertyName = "")
        {
            if (object.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            this.NotifyPropertyChanged(propertyName);
            return true;
        }

        #region Commands
        protected bool AlwaysTrue() { return true; }
        protected bool AlwaysTrue(object obj) { return true; }

        protected bool AlwaysFalse() { return false; }
        #endregion
    }
}

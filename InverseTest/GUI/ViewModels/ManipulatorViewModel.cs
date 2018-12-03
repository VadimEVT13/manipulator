using System.Windows.Input;
using System.Threading.Tasks;
using InverseTest.GUI.Utils;
using FontAwesome.WPF;
using System.Windows.Media;
using InverseTest.Grbl.Models;

/// <summary>
/// Модель представления драйвера управления манипулятором.
/// </summary>
namespace InverseTest.GUI.ViewModels
{
    public delegate void AxisChanged(double xValue);

    public class ManipulatorViewModel : ViewModelBase
    {
        /// <summary>
        /// Событие вызываемое при изменении X.
        /// </summary>
        public event AxisChanged OnXChanged;
        private double _x = 10;
        public double X
        {
            get => _x;
            set
            {
                OnXChanged?.Invoke(value);
                SetValue(ref _x, value, "X");
            }
        }

        /// <summary>
        /// Событие вызываемое при изменении Y.
        /// </summary>
        public event AxisChanged OnYChanged;
        private double _y = 10;
        public double Y
        {
            get => _y;
            set
            {
                OnYChanged?.Invoke(value);
                SetValue(ref _y, value, "Y");
            }
        }

        /// <summary>
        /// Событие вызываемое при изменении Z.
        /// </summary>
        public event AxisChanged OnZChanged;
        private double _z = 10;
        public double Z
        {
            get => _z;
            set
            {
                OnZChanged?.Invoke(value);
                SetValue(ref _z, value, "Z");
            }
        }

        /// <summary>
        /// Событие вызываемое при изменении A.
        /// </summary>
        public event AxisChanged OnAChanged;
        private double _a = 10;
        public double A
        {
            get => _a;
            set
            {
                OnAChanged?.Invoke(value);
                SetValue(ref _a, value, "A");
            }
        }

        /// <summary>
        /// Событие вызываемое при изменении B.
        /// </summary>
        public event AxisChanged OnBChanged;
        private double _b = 0;
        public double B
        {
            get => _b;
            set
            {
                OnBChanged?.Invoke(value);
                SetValue(ref _b, value, "B");
            }
        }

        /// <summary>
        /// Последовательный порт
        /// </summary>
        public GPort Port { get; set; }

        #region Images
        public ImageSource PlugImage => ImageAwesome.CreateImageSource(FontAwesomeIcon.Plug, Brushes.Green);
        public ImageSource UnPlugImage => ImageAwesome.CreateImageSource(FontAwesomeIcon.Circle, Brushes.Red);
        public ImageSource PlayImage => ImageAwesome.CreateImageSource(FontAwesomeIcon.Play, Brushes.Green);
        public ImageSource PauseImage => ImageAwesome.CreateImageSource(FontAwesomeIcon.Pause, Brushes.Orange);
        public ImageSource HomeImage => ImageAwesome.CreateImageSource(FontAwesomeIcon.Home, Brushes.Green);
        public ImageSource UnlockImage => ImageAwesome.CreateImageSource(FontAwesomeIcon.Unlock, Brushes.Red);
        #endregion

        public ManipulatorViewModel()
        {
            Port = GManipulator.getInstance().Port;
        }

        #region Commands
        public ICommand PlugCmd { get { return new RelayCommand(OnPlug, AlwaysTrue); } }
        public ICommand UnPlugCmd { get { return new RelayCommand(OnUnPlug, AlwaysTrue); } }
        public ICommand PlayCmd { get { return new RelayCommand(OnPlay, AlwaysTrue); } }
        public ICommand PauseCmd { get { return new RelayCommand(OnPause, AlwaysTrue); } }
        public ICommand HomeCmd { get { return new RelayCommand(OnHome, AlwaysTrue); } }
        public ICommand UnLockCmd { get { return new RelayCommand(OnUnLock, AlwaysTrue); } }
        #endregion

        #region Events
        public void OnPlug()
        {
            logger.Info("Plug port manipulator");
            Port.Open();
        }

        public void OnUnPlug()
        {
            logger.Info("Unplug port manipulator");
            Port.Close();
        }

        public void OnPlay()
        {
            logger.Info("Play manipulator");
            Port.Start();
        }

        public void OnPause()
        {
            logger.Info("Pause manipulator");
            Port.Pause();
        }

        public void OnHome()
        {
            logger.Info("Home manipulator");
            Port.Home();
        }

        public void OnUnLock()
        {
            logger.Info("Unlock manipulator");
            Port.Unlock();
        }
        #endregion


        /// <summary>
        /// Команда отправления данных обработана.
        /// </summary>
        private bool _isCommand;

        /// <summary>
        /// Команда отправления данных в порт.
        /// </summary>
        private AsyncRelayCommand _GlobalCommand;

        /// <summary>
        /// Свойство команда отправления данных в порт.
        /// </summary>
        public ICommand Global
        {
            get
            {
                return _GlobalCommand
                  ?? (_GlobalCommand = new AsyncRelayCommand(o =>
                    Task.Run(() =>
                    {
                        if (_isCommand)
                        {
                            return;
                        }
                        _isCommand = true;
                        GPoint target = new GPoint()
                        {
                            X = X,
                            Y = Y,
                            Z = Z,
                            A = A,
                            B = B
                        };
                        Port.Global(GManipulator.GlobalLimits(target));
                        _isCommand = false;
                    }),
                    o => !_isCommand));
            }
        }       
    }
}

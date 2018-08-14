using System;
using System.Windows.Input;
using System.Threading;
using System.Threading.Tasks;
using InverseTest.GUI.Utils;
using System.Windows.Media;
using FontAwesome.WPF;
using log4net;
using InverseTest.Grbl.Models;

namespace InverseTest.GUI.ViewModels
{
    /// <summary>
    /// Модель представления драйвера управления детектором.
    /// </summary>
    public class DetectorViewModel : ViewModelBase
    {
        /// <summary>
        /// Логгирование
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Событие вызываемое при изменении X.
        /// </summary>
        public event AxisChanged OnXChanged;
        private double x = 10;
        public double X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
                OnXChanged(value);
                NotifyPropertyChanged("X");
            }
        }

        /// <summary>
        /// Событие вызываемое при изменении Y.
        /// </summary>
        public event AxisChanged OnYChanged;
        private double y = 10;
        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
                OnYChanged(value);
                NotifyPropertyChanged("Y");
            }
        }

        /// <summary>
        /// Событие вызываемое при изменении Z.
        /// </summary>
        public event AxisChanged OnZChanged;
        private double z = 10;
        public double Z
        {
            get
            {
                return z;
            }
            set
            {
                z = value;
                OnZChanged(value);
                NotifyPropertyChanged("Z");
            }
        }

        /// <summary>
        /// Событие вызываемое при изменении A.
        /// </summary>
        public event AxisChanged OnAChanged;
        private double a = 50;
        public double A
        {
            get
            {
                return a;
            }
            set
            {
                a = value;
                OnAChanged(value);
                NotifyPropertyChanged("A");
            }
        }

        /// <summary>
        /// Событие вызываемое при изменении B.
        /// </summary>
        public event AxisChanged OnBChanged;
        private double b = 50;
        public double B
        {
            get
            {
                return b;
            }
            set
            {
                b = value;
                OnBChanged(value);
                NotifyPropertyChanged("B");
            }
        }

        /// <summary>
        /// Последовательный порт
        /// </summary>
        public GPort Port { get; set; }


        public DetectorViewModel()
        {
            var portFactory = new GPortFactory();
            Port = portFactory.CreateGPort(GPortFactory.GPortType.DETECTOR);

            PlugImage = ImageAwesome.CreateImageSource(FontAwesomeIcon.Plug, Brushes.Green);
            UnPlugImage = ImageAwesome.CreateImageSource(FontAwesomeIcon.Circle, Brushes.Red);
            PlayImage = ImageAwesome.CreateImageSource(FontAwesomeIcon.Play, Brushes.Green);
            PauseImage = ImageAwesome.CreateImageSource(FontAwesomeIcon.Pause, Brushes.Orange);
            HomeImage = ImageAwesome.CreateImageSource(FontAwesomeIcon.Home, Brushes.Green);
            UnlockImage = ImageAwesome.CreateImageSource(FontAwesomeIcon.Unlock, Brushes.Red);
        }

        public ImageSource PlugImage { get; }
        private RelayCommand _plugCommand;
        public RelayCommand PlugCommand
        {
            get
            {
                return _plugCommand
                  ?? (_plugCommand = new RelayCommand(
                    () =>
                    {
                        Port.Open();
                    }));
            }
        }
        
        public ImageSource UnPlugImage { get; }
        private RelayCommand _unPlugCommand;
        public RelayCommand UnPlugCommand
        {
            get
            {
                return _unPlugCommand
                  ?? (_unPlugCommand = new RelayCommand(
                    () =>
                    {
                        Port.Close();
                    }));
            }
        }
        
        public ImageSource PlayImage { get; }
        private RelayCommand _playCommand;
        public RelayCommand PlayCommand
        {
            get
            {
                return _playCommand
                  ?? (_playCommand = new RelayCommand(
                    () =>
                    {
                        Port.Start();
                    }));
            }
        }

        public ImageSource PauseImage { get; }
        private RelayCommand _pauseCommand;
        public RelayCommand PauseCommand
        {
            get
            {
                return _pauseCommand
                  ?? (_pauseCommand = new RelayCommand(
                    () =>
                    {
                        Port.Pause();
                    }));
            }
        }

        public ImageSource HomeImage { get; }
        private RelayCommand _homeCommand;
        public ICommand HomeCommand
        {
            get
            {
                return _homeCommand
                  ?? (_homeCommand = new RelayCommand(
                    () =>
                    {
                        Port.Home();
                    }));
            }
        }

        public ImageSource UnlockImage { get; }
        private RelayCommand _unlockCommand;
        public ICommand UnlockCommand
        {
            get
            {
                return _unlockCommand
                  ?? (_unlockCommand = new RelayCommand(
                    () =>
                    {
                        Port.Unlock();
                    }));
            }
        }
        
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
                        Port.Global(GDetector.GlobalLimits(target));
                        _isCommand = false;
                    }),
                    o => !_isCommand));
            }
        }

        /// <summary>
        /// Команда отправления данных в порт.
        /// </summary>
        private AsyncRelayCommand _LocalCommand;

        /// <summary>
        /// Свойство команда отправления данных в порт.
        /// </summary>
        public ICommand Local
        {
            get
            {
                return _LocalCommand
                  ?? (_LocalCommand = new AsyncRelayCommand(o =>
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
                        Port.Local(GDetector.LocalLimits(Port.Status.Position, target));
                        _isCommand = false;
                    }),
                    o => !_isCommand));
            }
        }
    }
}

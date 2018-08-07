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
    class DetectorViewModel : ViewModelBase
    {
        /// <summary>
        /// Логгирование
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public GPoint Target { get; set; }//TODO

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
                NotifyPropertyChanged("X");
            }
        }

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
                NotifyPropertyChanged("Y");
            }
        }

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
                NotifyPropertyChanged("Z");
            }
        }

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
                NotifyPropertyChanged("A");
            }
        }

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
                NotifyPropertyChanged("B");
            }
        }

        private GState state;
        public GState State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
                NotifyPropertyChanged("State");
            }
        }

        /// <summary>
        /// Класс для отправки комманд в последовательный порт
        /// </summary>
        private GPort port;


        public DetectorViewModel()
        {
            var portFactory = new GPortFactory();
            this.port = portFactory.CreateGPort(GPortFactory.GPortType.DETECTOR);
            this.port.OnDataReceived += OnDataReceived;

            PlugImage = ImageAwesome.CreateImageSource(FontAwesomeIcon.Plug, Brushes.Green);
            UnPlugImage = ImageAwesome.CreateImageSource(FontAwesomeIcon.Circle, Brushes.Red);
            PlayImage = ImageAwesome.CreateImageSource(FontAwesomeIcon.Play, Brushes.Green);
            PauseImage = ImageAwesome.CreateImageSource(FontAwesomeIcon.Pause, Brushes.Orange);
            HomeImage = ImageAwesome.CreateImageSource(FontAwesomeIcon.Home, Brushes.Green);
            UnlockImage = ImageAwesome.CreateImageSource(FontAwesomeIcon.Unlock, Brushes.Red);

            State = new GState();
            Target = new GPoint()
            {
                X = 10,
                Y = 10,
                Z = 10,
                A = 50,
                B = 50
            };
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
                        port.Open();
                        StateCommand();
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
                        port.Close();
                        StateCommand();
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
                        port.Start();
                        StateCommand();
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
                        port.Pause();
                        StateCommand();
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
                        port.Home();
                        StateCommand();
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
                        port.Unlock();
                        StateCommand();
                    }));
            }
        }

        public void StateCommand()
        {
            if (port.IsOpen)
            {
                port.State();
            }
            else
            {
                State.Status = GStatus.DISCONNECT;
            }
        }
        
        /// <summary>
        /// Вызывается при получии данных от детектора
        /// </summary>
        /// <param name="data"></param>
        private void OnDataReceived(GState data)
        {
            Console.WriteLine("State: " + data.Status);
            this.State = data;
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
                        //Status = "Update";
                        GPoint target = new GPoint()
                        {
                            X = X,
                            Y = Y,
                            Z = Z,
                            A = A,
                            B = B
                        };
                        port.Global(GDetector.GlobalLimits(target));
                        StateCommand();
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
                        if (_isCommand || State == null)
                        {
                            return;
                        }
                        _isCommand = true;
                        //Status = "Update";
                        GPoint target = new GPoint()
                        {
                            X = X,
                            Y = Y,
                            Z = Z,
                            A = A,
                            B = B
                        };
                        port.Local(GDetector.LocalLimits(State.Global, target));
                        StateCommand();
                        _isCommand = false;
                    }),
                    o => !_isCommand));
            }
        }
    }
}

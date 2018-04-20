using System;
using System.Windows.Input;
using System.Threading;
using System.Threading.Tasks;
using Manipulator.GRBL.Models;
using Manipulator.GRBL.Utils;
using InverseTest.GUI.Utils;

namespace InverseTest.GUI.ViewModels
{
    /// <summary>
    /// Модель представления драйвера управления порталом.
    /// </summary>
    class DetectorViewModel : ViewModelBase
    {
        /// <summary>
        /// Минимальное значение по оси X.
        /// </summary>
        private static int LIMIT_X_MIN = 1;
        /// <summary>
        /// Максимальное значение по оси X.
        /// </summary>
        private static int LIMIT_X_MAX = 450;
        /// <summary>
        /// Минимальное значение по оси Y.
        /// </summary>
        private static int LIMIT_Y_MIN = 1;
        /// <summary>
        /// Максимальное значение по оси Y.
        /// </summary>
        private static int LIMIT_Y_MAX = 760;
        /// <summary>
        /// Минимальное значение по оси Z.
        /// </summary>
        private static int LIMIT_Z_MIN = 1;
        /// <summary>
        /// Максимальное значение по оси Z.
        /// </summary>
        private static int LIMIT_Z_MAX = 760;
        /// <summary>
        /// Минимальное значение по оси A.
        /// </summary>
        private static int LIMIT_A_MIN = 1;
        /// <summary>
        /// Максимальное значение по оси A.
        /// </summary>
        private static int LIMIT_A_MAX = 70;
        /// <summary>
        /// Минимальное значение по оси B.
        /// </summary>
        private static int LIMIT_B_MIN = 1;
        /// <summary>
        /// Максимальное значение по оси B.
        /// </summary>
        private static int LIMIT_B_MAX = 180;

        private String status = GConvert.ToString(GStatus.DISCONNECT);
        public String Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                NotifyPropertyChanged("Status");
            }
        }

        private String x = "50,0";
        public String X
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

        private String y = "50,0";
        public String Y
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

        private String z = "50,0";
        public String Z
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

        private String a = "50,0";
        public String A
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

        private String b = "50,0";
        public String B
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

        private String speed = "10";
        public String Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
                NotifyPropertyChanged("Speed");
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
                if (state != null)
                {

                    Status = GConvert.ToString(state.Status);

                }
                NotifyPropertyChanged("State");
            }
        }


        /// <summary>
        /// Команда отправления данных обработана.
        /// </summary>
        private bool _isOpen;
        /// <summary>
        /// Команда отправления данных в порт.
        /// </summary>
        private AsyncRelayCommand _openCommand;

        /// <summary>
        /// Свойство команда отправления данных в порт.
        /// </summary>
        public ICommand Open
        {
            get
            {
                return _openCommand
                  ?? (_openCommand = new AsyncRelayCommand(o =>
                      Task.Run(() =>
                      {
                          if (_isOpen)
                          {
                              return;
                          }
                          _isOpen = true;
                          Status = "Update";
                          GPort.GetInstance().Settings = new GDevice
                          {
                              Name = "Grbl X-ray portal"
                          };
                          GPort.GetInstance().Open();
                          State = GPort.GetInstance().State();
                          _isOpen = false;
                      })
                  ,
                  o => !_isOpen));
            }
        }

        /// <summary>
        /// Команда отправления данных обработана.
        /// </summary>
        private bool _isClose;
        /// <summary>
        /// Команда отправления данных в порт.
        /// </summary>
        private AsyncRelayCommand _CloseCommand;

        /// <summary>
        /// Свойство команда отправления данных в порт.
        /// </summary>
        public ICommand Close
        {
            get
            {
                return _CloseCommand
                  ?? (_CloseCommand = new AsyncRelayCommand(o =>
                    Task.Run(() =>
                    {
                        if (_isClose)
                        {
                            return;
                        }
                        _isClose = true;
                        Status = "Update";
                        GPort.GetInstance().Close();
                        State = GPort.GetInstance().State();
                        _isClose = false;
                    }),
                    o => !_isClose));
            }
        }



        /// <summary>
        /// Команда отправления данных обработана.
        /// </summary>
        private bool _isCommand;

        /// <summary>
        /// Команда отправления паузы.
        /// </summary>
        private bool _isStartCommand;

        /// <summary>
        /// Команда отправления данных в порт.
        /// </summary>
        private AsyncRelayCommand _startCommand;

        /// <summary>
        /// Свойство команда отправления данных в порт.
        /// </summary>
        public ICommand Start
        {
            get
            {
                return _startCommand
                  ?? (_startCommand = new AsyncRelayCommand(o =>
                    Task.Run(() =>
                    {
                        if (_isStartCommand)
                        {
                            return;
                        }
                        _isStartCommand = true;
                        Status = "Update";
                        GPort.GetInstance().Start();
                        State = GPort.GetInstance().State();
                        _isStartCommand = false;
                    }),
                    o => !_isStartCommand));
            }
        }



        /// <summary>
        /// Команда отправления паузы.
        /// </summary>
        private bool _isPauseCommand;

        /// <summary>
        /// Команда отправления данных в порт.
        /// </summary>
        private AsyncRelayCommand _PauseCommand;

        /// <summary>
        /// Свойство команда отправления данных в порт.
        /// </summary>
        public ICommand Pause
        {
            get
            {
                return _PauseCommand
                  ?? (_PauseCommand = new AsyncRelayCommand(o =>
                    Task.Run(() =>
                    {
                        if (_isPauseCommand)
                        {
                            return;
                        }
                        _isPauseCommand = true;
                        Status = "Update";
                        GPort.GetInstance().Pause();
                        State = GPort.GetInstance().State();
                        _isPauseCommand = false;
                    }),
                    o => !_isPauseCommand));
            }
        }

        /// <summary>
        /// Команда отправления данных в порт.
        /// </summary>
        private AsyncRelayCommand _HomeCommand;

        /// <summary>
        /// Свойство команда отправления данных в порт.
        /// </summary>
        public ICommand Home
        {
            get
            {
                return _HomeCommand
                  ?? (_HomeCommand = new AsyncRelayCommand(o =>
                    Task.Run(() =>
                    {
                        if (_isCommand)
                        {
                            return;
                        }
                        _isCommand = true;
                        Status = "Update";
                        GPort.GetInstance().Home();
                        State = GPort.GetInstance().State();
                        _isCommand = false;
                    }),
                    o => !_isCommand));
            }
        }

        /// <summary>
        /// Команда отправления данных в порт.
        /// </summary>
        private AsyncRelayCommand _UnlockCommand;

        /// <summary>
        /// Свойство команда отправления данных в порт.
        /// </summary>
        public ICommand Unlock
        {
            get
            {
                return _UnlockCommand
                  ?? (_UnlockCommand = new AsyncRelayCommand(o =>
                    Task.Run(() =>
                    {
                        if (_isCommand)
                        {
                            return;
                        }
                        _isCommand = true;
                        Status = "Update";
                        GPort.GetInstance().Unlock();
                        State = GPort.GetInstance().State();
                        _isCommand = false;
                    }),
                    o => !_isCommand));
            }
        }

        private double GetSpeed()
        {
            double value = 0;
            if (Double.TryParse(Speed, out double speed))
            {
                value = speed;
            }
            Speed = Convert.ToString(speed);
            return speed;
        }




        private double GetLimitValue(double value, double state, double minValue, double maxValue)
        {
            if (state + value < minValue)
            {
                return minValue - state;
            }
            else if (state + value > maxValue)
            {
                return maxValue - state;
            }
            else
            {
                return value;
            }
        }

        private GPoint GetPoint(GPoint position)
        {
            GPoint point = new GPoint();
            if (Double.TryParse(X, out double valueX))
            {
                point.X = GetLimitValue(valueX, position.X, LIMIT_X_MIN, LIMIT_X_MAX);
            }
            if (Double.TryParse(Y, out double valueY))
            {
                point.Y = GetLimitValue(valueY, position.Y, LIMIT_Y_MIN, LIMIT_Y_MAX);
            }
            if (Double.TryParse(Z, out double valueZ))
            {
                point.Z = GetLimitValue(valueZ, position.Z, LIMIT_Z_MIN, LIMIT_Z_MAX);
            }
            if (Double.TryParse(A, out double valueA))
            {
                point.A = GetLimitValue(valueA, position.A, LIMIT_A_MIN, LIMIT_A_MAX);
            }
            if (Double.TryParse(B, out double valueB))
            {
                point.B = GetLimitValue(valueB, position.B, LIMIT_B_MIN, LIMIT_B_MAX);
            }
            X = Convert.ToString(point.X);
            Y = Convert.ToString(point.Y);
            Z = Convert.ToString(point.Z);
            A = Convert.ToString(point.A);
            B = Convert.ToString(point.B);
            return point;
        }

        public GPoint SetPoint(GPoint point)
        {
            X = Convert.ToString(point.X);
            Y = Convert.ToString(point.Y);
            Z = Convert.ToString(point.Z);
            A = Convert.ToString(point.A);
            B = Convert.ToString(point.B);
            return point;
        }

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
                        Status = "Update";
                        GPort.GetInstance().Global(GetPoint(new GPoint()));
                        State = GPort.GetInstance().State();
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
                        Status = "Update";
                        GPort.GetInstance().Local(GetPoint(State.Global));
                        State = GPort.GetInstance().State();
                        _isCommand = false;
                    }),
                    o => !_isCommand));
            }
        }

        /// <summary>
        /// Команда отправления данных в порт.
        /// </summary>
        private AsyncRelayCommand _PXCommand;

        /// <summary>
        /// Свойство команда отправления данных в порт.
        /// </summary>
        public ICommand PX
        {
            get
            {
                return _PXCommand
                  ?? (_PXCommand = new AsyncRelayCommand(o =>
                    Task.Run(() =>
                    {
                        if (_isCommand)
                        {
                            return;
                        }
                        _isCommand = true;
                        Status = "Update";
                        GPoint point = new GPoint
                        {
                            X = GetSpeed()
                        };
                        GPort.GetInstance().Local(point);
                        State = GPort.GetInstance().State();
                        _isCommand = false;
                    }),
                    o => !_isCommand));
            }
        }

        /// <summary>
        /// Команда отправления данных в порт.
        /// </summary>
        private AsyncRelayCommand _MXCommand;

        /// <summary>
        /// Свойство команда отправления данных в порт.
        /// </summary>
        public ICommand MX
        {
            get
            {
                return _MXCommand
                  ?? (_MXCommand = new AsyncRelayCommand(o =>
                    Task.Run(() =>
                    {
                        if (_isCommand)
                        {
                            return;
                        }
                        _isCommand = true;
                        Status = "Update";
                        Thread.Sleep(100);
                        GPoint point = new GPoint
                        {
                            X = -GetSpeed()
                        };
                        GPort.GetInstance().Local(point);
                        State = GPort.GetInstance().State();
                        _isCommand = false;
                    }),
                    o => !_isCommand));
            }
        }

        /// <summary>
        /// Команда отправления данных в порт.
        /// </summary>
        private AsyncRelayCommand _PYCommand;

        /// <summary>
        /// Свойство команда отправления данных в порт.
        /// </summary>
        public ICommand PY
        {
            get
            {
                return _PYCommand
                  ?? (_PYCommand = new AsyncRelayCommand(o =>
                    Task.Run(() =>
                    {
                        if (_isCommand)
                        {
                            return;
                        }
                        _isCommand = true;
                        Status = "Update";
                        GPoint point = new GPoint
                        {
                            Y = GetSpeed()
                        };
                        GPort.GetInstance().Local(point);
                        State = GPort.GetInstance().State();
                        _isCommand = false;
                    }),
                    o => !_isCommand));
            }
        }

        /// <summary>
        /// Команда отправления данных в порт.
        /// </summary>
        private AsyncRelayCommand _MYCommand;

        /// <summary>
        /// Свойство команда отправления данных в порт.
        /// </summary>
        public ICommand MY
        {
            get
            {
                return _MYCommand
                  ?? (_MYCommand = new AsyncRelayCommand(o =>
                    Task.Run(() =>
                    {
                        if (_isCommand)
                        {
                            return;
                        }
                        _isCommand = true;
                        Status = "Update";
                        GPoint point = new GPoint
                        {
                            Y = -GetSpeed()
                        };
                        GPort.GetInstance().Local(point);
                        State = GPort.GetInstance().State();
                        _isCommand = false;
                    }),
                    o => !_isCommand));
            }
        }

        /// <summary>
        /// Команда отправления данных в порт.
        /// </summary>
        private AsyncRelayCommand _PZCommand;

        /// <summary>
        /// Свойство команда отправления данных в порт.
        /// </summary>
        public ICommand PZ
        {
            get
            {
                return _PZCommand
                  ?? (_PZCommand = new AsyncRelayCommand(o =>
                    Task.Run(() =>
                    {
                        if (_isCommand)
                        {
                            return;
                        }
                        _isCommand = true;
                        Status = "Update";
                        GPoint point = new GPoint
                        {
                            Z = GetSpeed()
                        };
                        GPort.GetInstance().Local(point);
                        State = GPort.GetInstance().State();
                        _isCommand = false;
                    }),
                    o => !_isCommand));
            }
        }

        /// <summary>
        /// Команда отправления данных в порт.
        /// </summary>
        private AsyncRelayCommand _MZCommand;

        /// <summary>
        /// Свойство команда отправления данных в порт.
        /// </summary>
        public ICommand MZ
        {
            get
            {
                return _MZCommand
                  ?? (_MZCommand = new AsyncRelayCommand(o =>
                    Task.Run(() =>
                    {
                        if (_isCommand)
                        {
                            return;
                        }
                        _isCommand = true;
                        Status = "Update";
                        GPoint point = new GPoint
                        {
                            Z = -GetSpeed()
                        };
                        GPort.GetInstance().Local(point);
                        State = GPort.GetInstance().State();
                        _isCommand = false;
                    }),
                    o => !_isCommand));
            }
        }

        /// <summary>
        /// Команда отправления данных в порт.
        /// </summary>
        private AsyncRelayCommand _PACommand;

        /// <summary>
        /// Свойство команда отправления данных в порт.
        /// </summary>
        public ICommand PA
        {
            get
            {
                return _PACommand
                  ?? (_PACommand = new AsyncRelayCommand(o =>
                    Task.Run(() =>
                    {
                        if (_isCommand)
                        {
                            return;
                        }
                        _isCommand = true;
                        Status = "Update";
                        GPoint point = new GPoint
                        {
                            A = GetSpeed()
                        };
                        GPort.GetInstance().Local(point);
                        State = GPort.GetInstance().State();
                        _isCommand = false;
                    }),
                    o => !_isCommand));
            }
        }

        /// <summary>
        /// Команда отправления данных в порт.
        /// </summary>
        private AsyncRelayCommand _MACommand;

        /// <summary>
        /// Свойство команда отправления данных в порт.
        /// </summary>
        public ICommand MA
        {
            get
            {
                return _MACommand
                  ?? (_MACommand = new AsyncRelayCommand(o =>
                    Task.Run(() =>
                    {
                        if (_isCommand)
                        {
                            return;
                        }
                        _isCommand = true;
                        Status = "Update";
                        GPoint point = new GPoint
                        {
                            A = -GetSpeed()
                        };
                        GPort.GetInstance().Local(point);
                        State = GPort.GetInstance().State();
                        _isCommand = false;
                    }),
                    o => !_isCommand));
            }
        }

        /// <summary>
        /// Команда отправления данных в порт.
        /// </summary>
        private AsyncRelayCommand _PBCommand;

        /// <summary>
        /// Свойство команда отправления данных в порт.
        /// </summary>
        public ICommand PB
        {
            get
            {
                return _PBCommand
                  ?? (_PBCommand = new AsyncRelayCommand(o =>
                    Task.Run(() =>
                    {
                        if (_isCommand)
                        {
                            return;
                        }
                        _isCommand = true;
                        Status = "Update";
                        GPoint point = new GPoint
                        {
                            B = GetSpeed()
                        };
                        GPort.GetInstance().Local(point);
                        State = GPort.GetInstance().State();
                        _isCommand = false;
                    }),
                    o => !_isCommand));
            }
        }

        /// <summary>
        /// Команда отправления данных в порт.
        /// </summary>
        private AsyncRelayCommand _MBCommand;

        /// <summary>
        /// Свойство команда отправления данных в порт.
        /// </summary>
        public ICommand MB
        {
            get
            {
                return _MBCommand
                  ?? (_MBCommand = new AsyncRelayCommand(o =>
                    Task.Run(() =>
                    {
                        if (_isCommand)
                        {
                            return;
                        }
                        _isCommand = true;
                        Status = "Update";
                        GPoint point = new GPoint
                        {
                            B = -GetSpeed()
                        };
                        GPort.GetInstance().Local(point);
                        State = GPort.GetInstance().State();
                        _isCommand = false;
                    }),
                    o => !_isCommand));
            }
        }
    }
}

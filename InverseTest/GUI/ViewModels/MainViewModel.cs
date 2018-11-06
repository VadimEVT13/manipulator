using InverseTest.Frame;
using InverseTest.GUI.Model;
using InverseTest.GUI.Readers;
using InverseTest.GUI.Utils;
using InverseTest.GUI.Views;
using InverseTest.GUI.Writers;
using InverseTest.Manipulator;
using InverseTest.Path;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using MvvmDialogs.FrameworkDialogs.SaveFile;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace InverseTest.GUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Parameters

        private readonly IDialogService DialogService;

        /// <summary>
        /// Title of the application, as displayed in the top bar of the window
        /// </summary>
        public string Title
        {
            get { return "MainWindow"; }
        }

        public ManipulatorViewModel ManipulatorVM { get; set; }

        public DetectorViewModel DetectorVM { get; set; }

        public PathViewModel PathVM { get; set; }

        public DetailViewModel DetailVM { get; set; }


        public ManipulatorV2 Manipulator { get; set; }
        public DetectorFrame Detector { get; set; }

        public MovementPoint ManipulatorCamPoint { get; set; }


        private double _targetX = 0;
        public double TargetX
        {
            get => _targetX;
            set
            {
                SetValue(ref _targetX, Math.Round(value, 3), "TargetX");
                ManipulatorChanged();
            }
        }

        private double _targetY = 0;
        public double TargetY
        {
            get => _targetY;
            set
            {
                SetValue(ref _targetY, Math.Round(value, 3), "TargetY");
                ManipulatorChanged();
            }
        }

        private double _targetZ = 0;
        public double TargetZ
        {
            get => _targetZ;
            set
            {
                SetValue(ref _targetZ, Math.Round(value, 3), "TargetZ");
                ManipulatorChanged();
            }
        }

        private double _manipulatorX = 0;
        public double ManipulatorX
        {
            get => _manipulatorX;
            set
            {
                SetValue(ref _manipulatorX, Math.Round(value, 3), "ManipulatorX");
                ManipulatorChanged();
            }
        }

        private double _manipulatorY = 0;
        public double ManipulatorY
        {
            get => _manipulatorY;
            set
            {
                SetValue(ref _manipulatorY, Math.Round(value, 3), "ManipulatorY");
                ManipulatorChanged();
            }
        }

        private double _manipulatorZ = 0;
        public double ManipulatorZ
        {
            get => _manipulatorZ;
            set
            {
                SetValue(ref _manipulatorZ, Math.Round(value, 3), "ManipulatorZ");
                ManipulatorChanged();
            }
        }


        private double _rotateDetail = 0;
        public double RotateDetail
        {
            get => _rotateDetail;
            set
            {
                DetailVM.Transform(value, RiseDetail);
                SetValue(ref _rotateDetail, value, "RotateDetail");
            }
        }

        /// <summary>
        /// Подъем детали 
        /// </summary>
        private double _riseDetail = 0;
        public double RiseDetail
        {
            get => _riseDetail;
            set
            {
                DetailVM.Transform(RotateDetail, value);
                SetValue(ref _riseDetail, value, "RiseDetail");
            }
        }

        private bool _animate = false;
        public bool Animate
        {
            get => _animate;
            set
            {
                SetValue(ref _animate, value, "Animate");
            }
        }

        private double _focus = 50;
        public double Focus
        {
            get => _focus;
            set
            {
                SetValue(ref _focus, value, "Focus");
            }
        }

        private double _focuseEnlagment = 1d;
        public double FocusEnlagment
        {
            get => _focuseEnlagment;
            set
            {
                SetValue(ref _focuseEnlagment, value, "FocusEnlagment");
            }
        }

        #endregion

        #region Constructors
        public MainViewModel()
        {
            // DialogService is used to handle dialogs
            this.DialogService = new MvvmDialogs.DialogService();

            ManipulatorVM = new ManipulatorViewModel();
            ManipulatorVM.OnXChanged += ManipulatorXChanged;
            ManipulatorVM.OnYChanged += ManipulatorYChanged;
            ManipulatorVM.OnZChanged += ManipulatorZChanged;
            ManipulatorVM.OnAChanged += ManipulatorAChanged;
            ManipulatorVM.OnBChanged += ManipulatorBChanged;

            DetectorVM = new DetectorViewModel();
            DetectorVM.OnXChanged += DetectorXChanged;
            DetectorVM.OnYChanged += DetectorYChanged;
            DetectorVM.OnZChanged += DetectorZChanged;
            DetectorVM.OnAChanged += DetectorAChanged;
            DetectorVM.OnBChanged += DetectorBChanged;

            PathVM = new PathViewModel();

            DetailVM = new DetailViewModel();
        }
        #endregion

        #region Methods

        #endregion

        #region Commands
        public RelayCommand<object> SampleCmdWithArgument { get { return new RelayCommand<object>(OnSampleCmdWithArgument); } }

        public ICommand SaveAsCmd { get { return new RelayCommand(OnSaveAsTest, AlwaysTrue); } }
        public ICommand SaveCmd { get { return new RelayCommand(OnSaveTest, AlwaysFalse); } }
        public ICommand NewCmd { get { return new RelayCommand(OnNewTest, AlwaysFalse); } }
        public ICommand OpenCmd { get { return new RelayCommand(OnOpenTest, AlwaysTrue); } }
        public ICommand ShowAboutDialogCmd { get { return new RelayCommand(OnShowAboutDialog, AlwaysTrue); } }
        public ICommand ExitCmd { get { return new RelayCommand(OnExitApp, AlwaysTrue); } }

        private bool AlwaysTrue() { return true; }
        private bool AlwaysFalse() { return false; }

        private void OnSampleCmdWithArgument(object obj)
        {
            // TODO
        }

        private void OnSaveAsTest()
        {
            var settings = new SaveFileDialogSettings
            {
                Title = "Сохранить как",
                Filter = "Маршрут движения (*.json)|*.json",
                CheckFileExists = false,
                OverwritePrompt = true
            };

            bool? success = DialogService.ShowSaveFileDialog(this, settings);
            if (success == true)
            {
                logger.Info("Saving file: " + settings.FileName);
                string ext = System.IO.Path.GetExtension(settings.FileName).ToLower();
                switch (ext)
                {
                    case ".json":
                        {
                            List<ScanPoint> points = new List<ScanPoint>();
                            JsonWriter.Write(points, settings.FileName);
                            break;
                        }
                    default:
                        {
                            logger.Info("File format not supported.");
                            break;
                        }
                }
            }
        }
        private void OnSaveTest()
        {
            // TODO
        }
        private void OnNewTest()
        {
            // TODO
        }
        private void OnOpenTest()
        {
            var settings = new OpenFileDialogSettings
            {
                Title = "Открыть",
                Filter = "Маршрут движения (*.json)|*.json",
                CheckFileExists = true,
                Multiselect = false
            };

            bool? success = DialogService.ShowOpenFileDialog(this, settings);
            if (success == true)
            {
                logger.Info("Opening file: " + settings.FileName);
                string ext = System.IO.Path.GetExtension(settings.FileName).ToLower();
                switch (ext)
                {
                    case ".json":
                        {
                            List<ScanPoint> points = JsonReader.Read(settings.FileName);
                            break;
                        }
                    default:
                        {
                            logger.Info("File format not supported.");
                            break;
                        }
                }
            }
        }
        private void OnShowAboutDialog()
        {
            logger.Info("Opening About dialog");
            AboutViewModel dialog = new AboutViewModel();
            var result = DialogService.ShowDialog<About>(this, dialog);
        }
        private void OnExitApp()
        {
            System.Windows.Application.Current.MainWindow.Close();
        }
        #endregion

        #region Events

        public void ManipulatorChanged()
        {
            Point3D position = new Point3D()
            {
                X = ManipulatorX,
                Y = ManipulatorY,
                Z = ManipulatorZ
            };
            ManipulatorCamPoint.Move(position);
        }

        /// <summary>
        /// Обработчик изменения пложения манипулятора при ручном управлении
        /// </summary>
        public void ManipulatorPositionChanged()
        {
            ManipulatorCamPoint.Move(Manipulator.GetCameraPosition());
        }

        /// <summary>
        /// Обработчик изменения положения детектора
        /// </summary>
        public void DetectorPositionChanged()
        {
            DetectorVM.X = DetectorPositionController.XGlobalToLocal(Detector.VerticalFramePosition);
            DetectorVM.Y = DetectorPositionController.ZGlobalToLocal(Detector.ScreenHolderPosition);
            DetectorVM.Z = DetectorPositionController.YGlobalToLocal(Detector.HorizontalBarPosition);
            DetectorVM.A = DetectorPositionController.BGlobalToLocal(Detector.HorizontalAngle);
            DetectorVM.B = DetectorPositionController.AGlobalToLocal(Detector.VerticalAngle);
        }


        /// <summary>
        /// Обработка изменения положения первого колена
        /// </summary>
        /// <param name="x">положение колена</param>
        private void ManipulatorXChanged(double x)
        {
            if (Manipulator != null)
            {
                double value = ManipulatorPositionController.T2LocalToGlobal(x);
                if (Math.Abs(Manipulator.MiddleEdgePosition - value) > 1e-2)
                {
                    Manipulator.MiddleEdgePosition = value;
                }
            }
            
        }

        /// <summary>
        /// Обработка изменения положения второго колена
        /// </summary>
        /// <param name="y">положение колена</param>
        private void ManipulatorYChanged(double y)
        {
            if (Manipulator != null)
            {
                double value = ManipulatorPositionController.T3LocalToGlobal(y);
                if (Math.Abs(Manipulator.TopEdgePosition - value) > 1e-2)
                {
                    Manipulator.TopEdgePosition = value;
                }
            }
        }

        /// <summary>
        /// Обработка изменения положения третьего колена
        /// </summary>
        /// <param name="z">положение колена</param>
        public void ManipulatorZChanged(double z)
        {
            if (Manipulator != null)
            {
                double value = ManipulatorPositionController.T1LocalToGlobal(z);
                if (Math.Abs(Manipulator.TablePosition - value) > 1e-2)
                {
                    Manipulator.TablePosition = value;
                }
            }
        }

        /// <summary>
        /// Обработка изменения положения четвертого колена
        /// </summary>
        /// <param name="a">положение колена</param>
        private void ManipulatorAChanged(double a)
        {
            if (Manipulator != null)
            {
                double value = ManipulatorPositionController.T4LocalToGlobal(a);
                if (Math.Abs(Manipulator.CameraBasePosition - value) > 1e-2)
                {
                    Manipulator.CameraBasePosition = value;
                }
            }
        }

        /// <summary>
        /// Обработка изменения положения пятого колена
        /// </summary>
        /// <param name="b">положение колена</param>
        private void ManipulatorBChanged(double b)
        {
            if (Manipulator != null)
            {
                double value = ManipulatorPositionController.T5LocalToGlobal(b);
                if (Math.Abs(Manipulator.CameraPosition - value) > 1e-2)
                {
                    Manipulator.CameraPosition = value;
                }
            }
        }

        /// <summary>
        /// Обработка изменения положения вертикальной рамки
        /// </summary>
        /// <param name="x">положение на оси</param>
        private void DetectorXChanged(double x)
        {
            if (Detector != null)
            {
                double value = DetectorPositionController.XLocalToGlobal(x);
                if (Math.Abs(Detector.VerticalFramePosition - value) > 1e-2)
                {
                    Detector.VerticalFramePosition = value;
                }
            }
        }

        /// <summary>
        /// Обработка изменения положения держателя экрана
        /// </summary>
        /// <param name="y">положение на оси</param>
        private void DetectorYChanged(double y)
        {
            if (Detector != null)
            {
                double value = DetectorPositionController.ZLocalToGlobal(y);
                if (Math.Abs(Detector.ScreenHolderPosition - value) > 1e-2)
                {
                    Detector.ScreenHolderPosition = value;
                }
            }
        }

        /// <summary>
        /// Обработка изменения положения горизонтальной планки
        /// </summary>
        /// <param name="z">положение на оси</param>
        private void DetectorZChanged(double z)
        {
            if (Detector != null)
            {
                double value = DetectorPositionController.YLocalToGlobal(z);
                if (Math.Abs(Detector.HorizontalBarPosition - value) > 1e-2)
                {
                    Detector.HorizontalBarPosition = value;
                }
            }
        }

        /// <summary>
        /// Обработка изменения положения экрана детектора по горизонтали
        /// </summary>
        /// <param name="a">положение на оси</param>
        private void DetectorAChanged(double a)
        {
            if (Detector != null)
            {
                double value = DetectorPositionController.BLocalToGlobal(a);
                if (Math.Abs(Detector.HorizontalAngle - value) > 1e-2)
                {
                    Detector.HorizontalAngle = value;
                }
            }
        }

        /// <summary>
        /// Обработка изменения положения экрана детектора по вертикали
        /// </summary>
        /// <param name="b">положение на оси</param>
        private void DetectorBChanged(double b)
        {
            if (Detector != null)
            {
                double value = DetectorPositionController.ALocalToGlobal(b);
                if (Math.Abs(Detector.VerticalAngle - value) > 1e-2)
                {
                    Detector.VerticalAngle = value;
                }
            }
        }

        #endregion
    }
}

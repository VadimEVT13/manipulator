using InverseTest.GUI.Readers;
using InverseTest.GUI.Utils;
using InverseTest.GUI.Writers;
using InverseTest.Manipulator;
using InverseTest.Path;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using MvvmDialogs.FrameworkDialogs.SaveFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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



        public ManipulatorV2 Manipulator { get; set; }


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
                Log.Info("Saving file: " + settings.FileName);
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
                            Log.Info("File format not supported.");
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
                Log.Info("Opening file: " + settings.FileName);
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
                            Log.Info("File format not supported.");
                            break;
                        }
                }
            }
        }
        private void OnShowAboutDialog()
        {
            Log.Info("Opening About dialog");
            //AboutViewModel dialog = new AboutViewModel();
            //var result = DialogService.ShowDialog<About>(this, dialog);
        }
        private void OnExitApp()
        {
            System.Windows.Application.Current.MainWindow.Close();
        }
        #endregion

        #region Events

        /// <summary>
        /// Обработка изменения положения первого колена
        /// </summary>
        /// <param name="x">положение колена</param>
        public void ManipulatorXChanged(double x)
        {
            if (Manipulator != null)
            {
                double value = ManipulatorPositionController.T1LocalToGlobal(x);
                if (Math.Abs(Manipulator.TablePosition - value) > 1e-2)
                {
                    Manipulator.TablePosition = value;
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
                double value = ManipulatorPositionController.T2LocalToGlobal(y);
                if (Math.Abs(Manipulator.MiddleEdgePosition - value) > 1e-2)
                {
                    Manipulator.MiddleEdgePosition = value;
                }
            }
        }

        /// <summary>
        /// Обработка изменения положения третьего колена
        /// </summary>
        /// <param name="z">положение колена</param>
        private void ManipulatorZChanged(double z)
        {
            if (Manipulator != null)
            {
                double value = ManipulatorPositionController.T3LocalToGlobal(z);
                if (Math.Abs(Manipulator.TopEdgePosition - value) > 1e-2)
                {
                    Manipulator.TopEdgePosition = value;
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

        #endregion
    }
}

using InverseTest.Frame;
using InverseTest.GUI.ViewModels;
using InverseTest.GUI.Views;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace InverseTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static MainWindow app;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Log.Info("Application Startup");

            // For catching Global uncaught exception
            AppDomain currentDomain = AppDomain.CurrentDomain;
            //currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionOccured);

            Log.Info("Starting App");
            LogMachineDetails();
            var context = new MainViewModel();
            app = new MainWindow(context);
            app.DataContext = context;
            app.Show();
            
            if (e.Args.Length == 1) //make sure an argument is passed
            {
                Log.Info("File type association: " + e.Args[0]);
                FileInfo file = new FileInfo(e.Args[0]);
                if (file.Exists) //make sure it's actually a file
                {
                    // Here, add you own code
                    // ((MainViewModel)app.DataContext).OpenFile(file.FullName);
                }
            }
        }


        private void LogMachineDetails()
        {
            var computer = new Microsoft.VisualBasic.Devices.ComputerInfo();

            string text = "OS: " + computer.OSPlatform + " v" + computer.OSVersion + Environment.NewLine +
                          computer.OSFullName + Environment.NewLine +
                          "RAM: " + computer.TotalPhysicalMemory.ToString() + Environment.NewLine +
                          "Language: " + computer.InstalledUICulture.EnglishName;
            Log.Info(text);
        }
    }
}

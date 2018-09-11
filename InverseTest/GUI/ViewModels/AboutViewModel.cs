using MvvmDialogs;
using System;
using System.Deployment.Application;

namespace InverseTest.GUI.ViewModels
{
    class AboutViewModel : ViewModelBase, IModalDialogViewModel
    {
        public bool? DialogResult { get { return false; } }

        public string Content
        {
            get
            {
                return "Roentgen" + Environment.NewLine +
                        "Создано ПНИПУ" + Environment.NewLine +
                        "Россия, г.Пермь, Комсомольский просп., 29." + Environment.NewLine +
                        "2018";
            }
        }

        public string VersionText
        {
            get
            {
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    var version = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                    return string.Concat("Roentgen v", version);
                }
                else
                {
                    var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                    return "Roentgen v" + version.ToString();
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InverseTest.GUI.Views
{
    /// <summary>
    /// Логика взаимодействия для DetectorView.xaml
    /// </summary>
    public partial class DetectorView : UserControl
    {
        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public DetectorView()
        {
            InitializeComponent();
            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }
    }
}

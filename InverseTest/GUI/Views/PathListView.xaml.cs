using InverseTest.Path;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InverseTest.GUI.Views
{

    public delegate void OnPointSelected(ScanPoint p);


    /// <summary>
    /// Логика взаимодействия для PathListView.xaml
    /// </summary>
    public partial class PathListView : UserControl
    {

        public event OnPointSelected OnSelectedPoint;


        public PathListView()
        {
            InitializeComponent();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void TargetPointsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lv = e.OriginalSource as ListView;
            var selected = lv.SelectedItem as ScanPoint;
            this.OnSelectedPoint?.Invoke(selected);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {

        }
        
    }
}

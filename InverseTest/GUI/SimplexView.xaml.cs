using HelixToolkit.Wpf;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace InverseTest.GUI
{
    /// <summary>
    /// Логика взаимодействия для SimplexView.xaml
    /// </summary>
    public partial class SimplexView : Window
    {
        public SimplexView()
        {
            InitializeComponent();
        }

        public List<List<Vector3D>> frames = new List<List<Vector3D>>();

        public void AddSimplex(List<Vector3D> points)
        {
            this.Dispatcher.Invoke(new Action(() => { frames.Add(points); }));

        }



        public void AddModel(List<Vector3D> points)
        {

            SimplexViewport.Children.Clear();

            MeshBuilder builder = new MeshBuilder();
            foreach (Point3D p in points)
            {
                builder.AddSphere(p, 1);

                TextVisual3D text = new TextVisual3D() { Text = p.ToString(), FontSize = 12 ,Position = p };
                SimplexViewport.Children.Add(text);
            }

            GeometryModel3D m1 = new GeometryModel3D(builder.ToMesh(), Materials.Blue);
            ModelVisual3D mv1 = new ModelVisual3D() { Content = m1 };

            SimplexViewport.Children.Add(mv1);

        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            List<Vector3D> p = frames[(int)e.NewValue];
            AddModel(p);
        }
    }
}

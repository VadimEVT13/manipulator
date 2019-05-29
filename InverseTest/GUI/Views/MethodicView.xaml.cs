﻿using InverseTest.Path;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InverseTest.GUI.Views
{
    /// <summary>
    /// Логика взаимодействия для MethodicView.xaml
    /// </summary>
    public partial class MethodicView : UserControl
    {
        public MethodicView()
        {
            InitializeComponent();
        }

        private void GetPointsButton_Click(object sender, RoutedEventArgs e)
        {
            if (Method.SelectedItem == null)
                return;
            
            switch ((Method.SelectedItem as TextBlock).Text)
            {
                case "Шпангоут":
                    ShpangoutMethodic();
                    break;
                case "Лопатка":
                    break;
            }                        
        }

        void ShpangoutMethodic()
        {
            double x0, y0, z0;
            double r, n;

            if (!double.TryParse(X.Text, out x0) || !double.TryParse(Y.Text, out y0) || !double.TryParse(Z.Text, out z0)
                || !double.TryParse(Radius.Text, out r) || !double.TryParse(NumberOfPoints.Text, out n))
                return;

            ScanPath.getInstance.AddPoint(new ScanPoint(new Point3D(x0, y0, z0)));

            for (double i = 0; i <= Math.PI; i+= Math.PI / n)
            {       
                MethodPath.getInstance.AddPoint(new ScanPoint(new Point3D()
                {
                    X = x0 - Math.Sin(i) * r,
                    Y = y0 + Math.Cos(i) * r,
                    Z = z0
                }));
            }
        }
    }
}

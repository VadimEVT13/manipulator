using HelixToolkit.Wpf;
using ISC_Rentgen.GUI.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ISC_Rentgen.GUI.ModelView
{
    public class Emitter_and_scan_point_controller : INotifyPropertyChanged
    {
        private static Emitter_and_scan_point_controller instance;
        public static Emitter_and_scan_point_controller getInstance {
            get {
                if(instance == null)
                     instance = new Emitter_and_scan_point_controller();
                return instance;
            }
        }

        private Key_Point emitter_and_scan_point = new Key_Point(new Point3D(-10, 0, 80), new Point3D(0, 0, 80)); // начальное положение
        public  Key_Point Emitter_and_scan_point { get { return emitter_and_scan_point; }
            set
            {
                emitter_and_scan_point = value;
            }
        }
        
        private double em_x = -10;
        public double EM_X { get { return em_x; }
            set
            {
                em_x = value;
                emitter_and_scan_point.Emitter_point = new Point3D(em_x, em_y, em_z);
                ModelAndDetal_EmitterAdder();
                NotifyPropertyChanged(nameof(em_x));
            }
        }
        private double em_y = 0;
        public double EM_Y { get { return em_y; }
            set
            {
                em_y = value;
                emitter_and_scan_point.Emitter_point = new Point3D(em_x, em_y, em_z);
                ModelAndDetal_EmitterAdder();
                NotifyPropertyChanged(nameof(em_y));
            }
        }
        private double em_z = 80;
        public double EM_Z { get { return em_z; }
            set
            {
                em_z = value;
                emitter_and_scan_point.Emitter_point = new Point3D(em_x, em_y, em_z);
                ModelAndDetal_EmitterAdder();
                NotifyPropertyChanged(nameof(em_z));
            }
        }

        private double scan_x = 0;
        public double Scan_X
        {
            get { return scan_x; }
            set
            {
                scan_x = value;
                emitter_and_scan_point.Scan_point = new Point3D(scan_x, scan_y, scan_z);
                ModelAndDetal_ScanAdder();
                NotifyPropertyChanged(nameof(scan_x));
            }
        }
        private double scan_y = 0;
        public double Scan_Y
        {
            get { return scan_y; }
            set
            {
                scan_y = value;
                emitter_and_scan_point.Scan_point = new Point3D(scan_x, scan_y, scan_z);
                ModelAndDetal_ScanAdder();
                NotifyPropertyChanged(nameof(scan_y));
            }
        }
        private double scan_z = 80;
        public double Scan_Z
        {
            get { return scan_z; }
            set
            {
                scan_z = value;
                emitter_and_scan_point.Scan_point = new Point3D(scan_x, scan_y, scan_z);
                ModelAndDetal_ScanAdder();
                NotifyPropertyChanged(nameof(scan_z));
            }
        }

        public Model3DGroup Model { get; set; }
        public Model3DGroup Detal { get; set; }
        private string emitter_point_name { get { return "emitter_point"; } }
        private string scan_point_name { get { return "scan_point"; } }

        private void ModelAndDetal_EmitterAdder()
        {
            if (Model != null)
            {
                // Если уже есть такая точка, то удалить её
                while (Model.Children.Where(x => x.GetName() == emitter_point_name).Count() >= 1)
                {
                    Model.Children.Remove(Model.Children.Where(x => x.GetName() == emitter_point_name).First());
                }

                MeshBuilder mb = new MeshBuilder(true, true);
                mb.AddSphere(Emitter_and_scan_point.Emitter_point, 1.1);
                GeometryModel3D gm = new GeometryModel3D() { Geometry = mb.ToMesh(), Material = Materials.Violet };
                gm.SetName(emitter_point_name);
                Model.Children.Add(gm);
            }
            if (Detal != null)
            {
                // Если уже есть такая точка, то удалить её
                while (Detal.Children.Where(x => x.GetName() == emitter_point_name).Count() >= 1)
                {
                    Detal.Children.Remove(Detal.Children.Where(x => x.GetName() == emitter_point_name).First());
                }

                MeshBuilder mb = new MeshBuilder(true, true);
                mb.AddSphere(Emitter_and_scan_point.Emitter_point, 1.1);
                GeometryModel3D gm = new GeometryModel3D() { Geometry = mb.ToMesh(), Material = Materials.Violet };
                gm.SetName(emitter_point_name);
                Detal.Children.Add(gm);
            }

        }

        private void ModelAndDetal_ScanAdder()
        {
            if (Model != null)
            {
                // Если уже есть такая точка, то удалить её
                while (Model.Children.Where(x => x.GetName() == scan_point_name).Count() >= 1)
                {
                    Model.Children.Remove(Model.Children.Where(x => x.GetName() == scan_point_name).First());
                }

                MeshBuilder mb = new MeshBuilder(true, true);
                mb.AddSphere(Emitter_and_scan_point.Scan_point, 1.1);
                GeometryModel3D gm = new GeometryModel3D() { Geometry = mb.ToMesh(), Material = Materials.Violet };
                gm.SetName(scan_point_name);
                Model.Children.Add(gm);
            }
            if (Detal != null)
            {
                // Если уже есть такая точка, то удалить её
                while (Detal.Children.Where(x => x.GetName() == scan_point_name).Count() >= 1)
                {
                    Detal.Children.Remove(Detal.Children.Where(x => x.GetName() == scan_point_name).First());
                }

                MeshBuilder mb = new MeshBuilder(true, true);
                mb.AddSphere(Emitter_and_scan_point.Scan_point, 1.1);
                GeometryModel3D gm = new GeometryModel3D() { Geometry = mb.ToMesh(), Material = Materials.Violet };
                gm.SetName(scan_point_name);
                Detal.Children.Add(gm);
            }

        }

        public void AddEmitter(Point3D Emitter_point)
        {
            EM_X = Emitter_point.X;
            EM_Y = Emitter_point.Y;
            EM_Z = Emitter_point.Z;
        }

        public void AddScan(Point3D Scan_point)
        {
            Scan_X = Scan_point.X;
            Scan_Y = Scan_point.Y;
            Scan_Z = Scan_point.Z;
        }

        public void Clear()
        {
            while (Model.Children.Where(x => x.GetName() == emitter_point_name).Count() >= 1)
            {
                Model.Children.Remove(Model.Children.Where(x => x.GetName() == emitter_point_name).First());
            }
            while (Model.Children.Where(x => x.GetName() == scan_point_name).Count() >= 1)
            {
                Model.Children.Remove(Model.Children.Where(x => x.GetName() == scan_point_name).First());
            }
            while (Detal.Children.Where(x => x.GetName() == emitter_point_name).Count() >= 1)
            {
                Detal.Children.Remove(Detal.Children.Where(x => x.GetName() == emitter_point_name).First());
            }
            while (Detal.Children.Where(x => x.GetName() == scan_point_name).Count() >= 1)
            {
                Detal.Children.Remove(Detal.Children.Where(x => x.GetName() == scan_point_name).First());
            }
        }

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

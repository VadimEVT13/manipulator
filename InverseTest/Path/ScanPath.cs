using InverseTest.GUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Path
{
    public delegate void OnPointAdd(ScanPoint p);
    public delegate void OnPointRemove(ScanPoint p);
    public delegate void OnPointTransformed(Transform3D t);

    public class ScanPath : INotifyPropertyChanged
    {
        private static ScanPath instance;

        public event OnPointAdd PointAdd;
        public event OnPointRemove PointRemove;
        public event OnPointTransformed PointTransformed;

        public ObservableCollection<ScanPoint> points { get; set; }

        public ObservableCollection<ScanPoint> PointsList
        {
            get { return points; }
            set { points = value; }
        }

        public static ScanPath Instance
        {
            get { return instance; }
        }

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private static object syncRoot = new Object();

        public event PropertyChangedEventHandler PropertyChanged;

        public ScanPath()
        {
            this.points = new ObservableCollection<ScanPoint>();
        }

        public static ScanPath getInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ScanPath();
                    }
                }
                return instance;
            }
        }

        public void AddPoint(ScanPoint point)
        {
            this.points.Add(point);
            NotifyPropertyChanged("Add");
            this.PointAdd?.Invoke(point);
        }

        public void RemovePoint(ScanPoint point)
        {
            var removing = this.points.Remove(point);
            NotifyPropertyChanged("Remove");

            if (removing)
                this.PointRemove?.Invoke(point);
        }

        public void TransformPoint(Transform3D transform)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].Transform(transform);
            }
            NotifyPropertyChanged("Transformed");
            this.PointTransformed?.Invoke(transform);

        }
    }
}

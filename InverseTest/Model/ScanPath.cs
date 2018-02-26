using InverseTest.GUI.Model;
using InverseTest.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest
{
    public class ScanPath : INotifyPropertyChanged
    {
        private static ScanPath instance;

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
            NotifyPropertyChanged("Remove");
        }

        public void RemovePoint(ScanPoint point)
        {
            this.points.Remove(point);
            NotifyPropertyChanged("Remove");
        }
    }
}

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
    public delegate void OnMethodPointAdd(ScanPoint p);
    public delegate void OnMethodPointRemove(ScanPoint p);
    public delegate void OnMethodPointTransformed(Transform3D t);

    /// <summary>
    /// Класс для хранения списка точек сканирования
    /// </summary>
    public class MethodPath : INotifyPropertyChanged
    {
        private static MethodPath instance;

        public event OnMethodPointAdd PointAdd;
        public event OnMethodPointRemove PointRemove;
        public event OnMethodPointTransformed PointTransformed;

        public ObservableCollection<ScanPoint> points { get; set; }

        public ObservableCollection<ScanPoint> PointsList
        {
            get { return points; }
            set { points = value; }
        }

        public static MethodPath Instance
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

        public MethodPath()
        {
            this.points = new ObservableCollection<ScanPoint>();
        }

        public static MethodPath getInstance
        {
            get
            {
                if (instance == null)
                {
                    //Потокобезопасность 
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new MethodPath();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Добавление точки в список
        /// </summary>
        /// <param name="point"></param>
        public void AddPoint(ScanPoint point)
        {
            this.points.Add(point);
            NotifyPropertyChanged("Add");
            this.PointAdd?.Invoke(point);
        }

        /// <summary>
        /// Удаление точки из списка
        /// </summary>
        /// <param name="point"></param>
        public void RemovePoint(ScanPoint point)
        {
            var removing = this.points.Remove(point);
            NotifyPropertyChanged("Remove");

            if (removing)
                this.PointRemove?.Invoke(point);
        }

        /// <summary>
        /// Преобразование всех точек в списке
        /// </summary>
        /// <param name="transform"></param>
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

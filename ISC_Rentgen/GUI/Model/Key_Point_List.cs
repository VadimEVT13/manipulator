using ISC_Rentgen.Rentgen_Parts.Manipulator_Components.Model;
using ISC_Rentgen.Rentgen_Parts.Portal_Components.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISC_Rentgen.GUI.Model
{
    public delegate void OnPointAdd(Key_Point p);
    public delegate void OnPointRemove(Key_Point p);
    public delegate void OnClear();
    public delegate void OnModifAngle(Key_Point p);

    public class Key_Point_List : INotifyPropertyChanged
    {
        public OnPointAdd PointAdd;
        public OnPointRemove PointRemove;
        public OnClear PointsClear;
        public OnModifAngle ModifAngle;

        private static Key_Point_List instance;
        public static Key_Point_List getInstance
        {
            get {
                if (instance == null)
                {
                    instance = new Key_Point_List();
                }
                return instance;
            }
        }

        private ObservableCollection<Key_Point> points = new ObservableCollection<Key_Point>();
        public ObservableCollection<Key_Point> Points_List
        {
            get { return points; }
            set { points = value; }
        }

        public void AddPoint(Key_Point p)
        {
            this.Points_List.Add(p);
            NotifyPropertyChanged("Add");
            this.PointAdd?.Invoke(p);
        }

        public void RemovePoint(Key_Point p)
        {
            if (Points_List.Contains(p))
            {
                Points_List.Remove(p);
                NotifyPropertyChanged("Remove");
                this.PointRemove?.Invoke(p);
            }                
        }

        public void ModifAngles(Key_Point p, Angles_Manipulator AM, Angles_Portal AP)
        {
            if (Points_List.Contains(p))
            {
                int index = Points_List.IndexOf(p);

                Points_List[index].Manipulator_Angle = AM;
                Points_List[index].Portal_Angle = AP;

                if (AM != null & AP != null)
                    Points_List[index].IsCorrect = true;
                else
                    Points_List[index].IsCorrect = false;

                NotifyPropertyChanged("ModifAngles");
                this.ModifAngle?.Invoke(Points_List[index]);
            }
        }

        public void Clear()
        {
            while(points.Count > 0)
            {
                RemovePoint(points.First());
            }
            NotifyPropertyChanged("Clear");
            PointsClear?.Invoke();
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

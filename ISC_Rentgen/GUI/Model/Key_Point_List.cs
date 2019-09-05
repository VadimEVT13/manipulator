﻿using System;
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

    public class Key_Point_List : INotifyPropertyChanged
    {
        public OnPointAdd PointAdd;
        public OnPointRemove PointRemove;

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
            var removed = this.points.Remove(p);
            NotifyPropertyChanged("Remove");
            if (removed)
                this.PointRemove?.Invoke(p);
        }

        public void Clear()
        {
            while(points.Count > 0)
            {
                RemovePoint(points.First());
            }
            NotifyPropertyChanged("Clear");
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

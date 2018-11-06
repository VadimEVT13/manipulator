using InverseTest.GUI;
using InverseTest.GUI.Views;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using InverseTest.Path;


namespace InverseTest.GUI.ViewModels
{
    /// <summary>
    /// Модель отображения маршрута
    /// </summary>
    public class PathViewModel : ViewModelBase
    {
        /*public ObservableCollection<Point3D> Points { get; set; }
        public ManipulatorVisualizer Scene { get; set; }

        public PathViewModel()
        {
            Points = new ObservableCollection<Point3D>();
        }//*/


        public List<ScanPointVisual> Points { get; set; }
        public ManipulatorVisualizer Scene { get; set; }

        public PathViewModel()
        {
            Points = new List<ScanPointVisual>();
            ScanPath.getInstance.PointAdd += OnPointAdd;
            ScanPath.getInstance.PointRemove += OnPointRemove;
            ScanPath.getInstance.PointTransformed += OnPointsTransformed;
        }

        public void OnPointAdd(ScanPoint p)
        {
            var pointVis = new ScanPointVisual(p);
            Points.Add(pointVis);
            if (Scene != null)
            {
                Scene.AddVisual(pointVis.pointVisual);
            }
        }

        public void OnPointRemove(ScanPoint p)
        {
            var pointVis = Points.Find(x => x.Point.Equals(p));
            if (Scene != null)
            {
                Scene.RemoveVisual(pointVis.pointVisual);
            }
            Points.RemoveAll(x => x.Point.Equals(p));
        }

        public void OnPointsTransformed(Transform3D t)
        {
            Points.ForEach(x => x.pointVisual.Transform(t));
        }

        public void OnPointSelected(ScanPoint p)
        {
            foreach (ScanPointVisual pv in Points)
            {
                pv.SetSelected(pv.Point.Equals(p));
            }
        }
    }
}

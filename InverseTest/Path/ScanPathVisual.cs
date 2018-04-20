using InverseTest.GUI;
using InverseTest.GUI.Views;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace InverseTest.Path
{
    public class ScanPathVisualController
    {
        List<ScanPointVisual> points;
        ManipulatorVisualizer scene;

        public ScanPathVisualController(ManipulatorVisualizer scene)
        {
            this.scene = scene;
            this.points = new List<ScanPointVisual>();
            ScanPath.getInstance.PointAdd += OnPointAdd;
            ScanPath.getInstance.PointRemove += OnPointRemove;
            ScanPath.getInstance.PointTransformed += OnPointsTransformed;
        }

        public void OnPointAdd(ScanPoint p)
        {
            var pointVis = new ScanPointVisual(p);
            points.Add(pointVis);
            scene.AddVisual(pointVis.pointVisual);
        }

        public void OnPointRemove(ScanPoint p)
        {
            var pointVis = this.points.Find(x => x.Point.Equals(p));
            scene.RemoveVisual(pointVis.pointVisual);
            this.points.RemoveAll(x => x.Point.Equals(p));
        }

        public void OnPointsTransformed(Transform3D t)
        {
            this.points.ForEach(x => x.pointVisual.Transform(t));
        }

        public void OnPointSelected(ScanPoint p)
        {
            foreach (ScanPointVisual pv in points)
            {
                pv.SetSelected(pv.Point.Equals(p));
            }
        }
    }
}
 
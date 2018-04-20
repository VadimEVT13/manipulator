using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using InverseTest.GUI.Model;

namespace InverseTest.GUI.Models
{
    class ModelMoverAboveSurf : IModelMover
    {
        private MovementPoint point;
        private Model3D surf;

        private bool onMousePressed = false;
        private bool onModelHit = false;
        public Model3D ModelToDetect { get; set; }

        public ModelMoverAboveSurf(MovementPoint setPoint, Model3D setSurf)
        {
            point = setPoint;
            surf = setSurf;
        }

        public void OnMouseDown(object sender, MouseEventArgs e)
        {
            onMousePressed = true;
            HelixViewport3D viewPort = sender as HelixViewport3D;
            Point mousePos = e.GetPosition(viewPort);
            PointHitTestParameters hitParam = new PointHitTestParameters(mousePos);
            VisualTreeHelper.HitTest(viewPort, null, ResultCallback, hitParam);
        }

        public HitTestResultBehavior ResultCallback(HitTestResult result)
        {
            if (result is RayHitTestResult rayResult && rayResult.ModelHit.Equals(ModelToDetect))
            {
                onModelHit = true;
                point.ChangeSize(PointState.ENLAGE);
            }
            return HitTestResultBehavior.Continue;
        }

        public void OnMouseUp(object sender, MouseEventArgs e)
        {
            onMousePressed = false;
            onModelHit = false;
            point.ChangeSize(PointState.DEFAULT);
            HelixViewport3D viewPort = sender as HelixViewport3D;
            HitTestResult result = VisualTreeHelper.HitTest(viewPort.Viewport, e.GetPosition(viewPort));
            if (result is RayMeshGeometry3DHitTestResult mesh_result && mesh_result.ModelHit.Equals(ModelToDetect))
            {
                point.ChangeSize(PointState.DEFAULT);
            }
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            HelixViewport3D viewPort = sender as HelixViewport3D;
            Point mousePos = e.GetPosition(viewPort);
            PointHitTestParameters pointHitTestParams = new PointHitTestParameters(mousePos);
            VisualTreeHelper.HitTest(viewPort, null, DetailResultCallback, pointHitTestParams);
        }

        public HitTestResultBehavior DetailResultCallback(HitTestResult result)
        {
            if (result is RayHitTestResult hitResult && hitResult.ModelHit.Equals(surf)
                && onMousePressed && onModelHit)
            {
                point.MoveAndNotify(hitResult.PointHit);
            }
            return HitTestResultBehavior.Continue;
        }
    }
}


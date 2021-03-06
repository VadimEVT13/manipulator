﻿using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using InverseTest.GUI.Model;

namespace InverseTest.GUI.Models
{
    public class ModelMover:IModelMover
    {
        private MovementPoint point;
        private bool onMousePressed = false;
        private bool onModelHit = false;
        private Point3D lastPointPosition;
        public Model3D ModelToDetect { get; set; }

        public ModelMover(MovementPoint point)
        {
            this.point = point;
        }
        
        public void OnMouseDown(object sender, MouseEventArgs e)
        {
            onMousePressed = true;
            HelixViewport3D viewPort = sender as HelixViewport3D;
            Point mousePos = e.GetPosition(viewPort);
            PointHitTestParameters hitParam = new PointHitTestParameters(mousePos);
            VisualTreeHelper.HitTest(viewPort, null, ResultCallback, hitParam);
            Matrix3D matrix = Viewport3DHelper.GetProjectionMatrix(viewPort.Viewport);
        }

        public HitTestResultBehavior ResultCallback(HitTestResult result)
        {
            if (result is RayHitTestResult rayResult)
            {
                if (rayResult.ModelHit.Equals(ModelToDetect))
                {
                    onModelHit = true;
                    lastPointPosition = point.GetTargetPoint();
                    point.ChangeSize(PointState.ENLAGE);
                }
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

            if (result is RayMeshGeometry3DHitTestResult mesh_result)
            {
                if (mesh_result.ModelHit.Equals(ModelToDetect))
                {
                    point.ChangeSize(PointState.DEFAULT);
                }
            }
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            HelixViewport3D viewPort = sender as HelixViewport3D;
            Point mousePos = e.GetPosition(viewPort);
            Point3D pointFar;
            Viewport3DHelper.Point2DtoPoint3D(viewPort.Viewport, mousePos, out Point3D pointNear, out pointFar);
            
            Point3D? point = Viewport3DHelper.UnProject(viewPort.Viewport, mousePos, lastPointPosition, viewPort.Camera.LookDirection);
            Point3D newPoint = point.GetValueOrDefault();

            if (onMousePressed && onModelHit)
            {
                this.point.MoveAndNotify(newPoint);
            }
        }
    }
}

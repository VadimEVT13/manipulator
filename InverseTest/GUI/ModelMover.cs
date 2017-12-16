﻿using HelixToolkit.Wpf;
using InverseTest.GUI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace InverseTest.GUI
{
    public class ModelMover
    {
        private IMovementPoint point;
        private bool onMousePressed = false;
        private bool onModelHit = false;
        private Point3D lastPointPosition;
        public Model3D modelToDetect { get; set; }

        public ModelMover(IMovementPoint point)
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
            Console.WriteLine("Camera:" + viewPort.Camera.LookDirection.ToString());
            Matrix3D matrix = Viewport3DHelper.GetProjectionMatrix(viewPort.Viewport);
            Console.WriteLine("Matrix:" + matrix.ToString());
        }

        public HitTestResultBehavior ResultCallback(HitTestResult result)
        {
            RayHitTestResult rayResult = result as RayHitTestResult;
            if (rayResult != null)
            {
                if (rayResult.ModelHit.Equals(modelToDetect))
                {
                    onModelHit = true;
                    lastPointPosition = point.GetTargetPoint();
                    point.ChangeSize(2d);
                }
            }
            return HitTestResultBehavior.Continue;
        }

        public void OnMouseUp(object sender, MouseEventArgs e)
        {
            onMousePressed = false;
            onModelHit = false;
            point.ChangeSize(1d);

            HelixViewport3D viewPort = sender as HelixViewport3D;
            HitTestResult result = VisualTreeHelper.HitTest(viewPort.Viewport, e.GetPosition(viewPort));
            RayMeshGeometry3DHitTestResult mesh_result = result as RayMeshGeometry3DHitTestResult;

            if (mesh_result != null)
            {
                if (mesh_result.ModelHit.Equals(modelToDetect))
                {
                    point.ChangeSize(0.5d);
                }
            }
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            HelixViewport3D viewPort = sender as HelixViewport3D;
            Point mousePos = e.GetPosition(viewPort);
            Point3D pointNear;
            Point3D pointFar;
            Viewport3DHelper.Point2DtoPoint3D(viewPort.Viewport, mousePos, out pointNear, out pointFar);
            
            Point3D? point = Viewport3DHelper.UnProject(viewPort.Viewport, mousePos, lastPointPosition, viewPort.Camera.LookDirection);
            Point3D newPoint = point.GetValueOrDefault();

            if (onMousePressed && onModelHit)
            {
                Console.WriteLine("PointNear: " + newPoint.ToString());
                this.point.MoveToPositoin(newPoint);
            }
        }
    }
}
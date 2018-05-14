using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace InverseTest.Detail
{
    public delegate void AddNewPoint(Point3D point);
    public delegate void RemovePoint(Visual3D visual);

    /// <summary>
    /// Отвечает за создание и удаление точек на детали. 
    /// </summary>
    class DetailPointsCreator : IDetailMouseListener
    {
        private DetailModel detail;

        public event AddNewPoint AddNewPointCallback;
        public event RemovePoint RemovePointCallback;

        public DetailPointsCreator(DetailModel detail)
        {
            this.detail = detail;
        }

        public void OnMouseLeftDown(object sender, MouseEventArgs e)
        {
            HelixViewport3D viewPort = sender as HelixViewport3D;
            Point mousePos = e.GetPosition(viewPort);
            PointHitTestParameters hitParam = new PointHitTestParameters(mousePos);
            VisualTreeHelper.HitTest(viewPort, null, ResultCallbackLeft, hitParam);
        }

        public HitTestResultBehavior ResultCallbackLeft(HitTestResult result)
        {
            if (result is RayHitTestResult rayResult)
            {
                if (rayResult.ModelHit.Equals(detail.GetModel()))
                {
                    AddNewPointCallback(rayResult.PointHit);
                }
            }
            return HitTestResultBehavior.Stop;
        }

        public void OnMouseRightDown(object sender, MouseEventArgs e)
        {
            HelixViewport3D viewPort = sender as HelixViewport3D;
            Point mousePos = e.GetPosition(viewPort);
            PointHitTestParameters hitParam = new PointHitTestParameters(mousePos);
            VisualTreeHelper.HitTest(viewPort, null, ResultCallbackRight, hitParam);
        }

        public HitTestResultBehavior ResultCallbackRight(HitTestResult result)
        {
            if (result is RayHitTestResult rayResult){
                RemovePointCallback(rayResult.VisualHit);
            }
            return HitTestResultBehavior.Stop;
        }

    }
}

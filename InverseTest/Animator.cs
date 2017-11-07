using InverseTest.GUI;
using InverseTest.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace InverseTest
{
    class Animator
    {

        ManipulatorV2 manipulator;
        List<Point3D> points;
        IEnumerator<Point3D> iter;
        int animationCounter = 0;
       public bool animating  { get; set; }

        Dictionary<ManipulatorV2.ManipulatorParts, double> lastAngleOnPart = new Dictionary<ManipulatorV2.ManipulatorParts, double>()
        {
            {   ManipulatorV2.ManipulatorParts.Table, 0.0 },
            {   ManipulatorV2.ManipulatorParts.TopEdge, 0.0 },
            {   ManipulatorV2.ManipulatorParts.TopEdgeBase, 0.0 },
            {   ManipulatorV2.ManipulatorParts.MiddleEdge, 0.0 },
            {   ManipulatorV2.ManipulatorParts.CameraBase, 0.0 },
            {   ManipulatorV2.ManipulatorParts.Camera, 0.0 },

        };

        private UserControl manipVisual;

        public Animator(ManipulatorV2 manipulator)
        {
            animating = false;
            this.manipulator = manipulator;


            
        }



        private void animate()
        {
           


        }


        private void clearLastValues()
        {
           
        }

        private void waitAnimate()
        {
            TimeSpan duration = TimeSpan.FromSeconds(1);
            TranslateTransform3D translate = new TranslateTransform3D();
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = duration;
            animation.Completed += Empty_Aimation_Completed;
            translate.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
        }


        private void Empty_Aimation_Completed(object sender, EventArgs e)
        {
            animate();
        }

            private void Animation_Completed(object sender, EventArgs e)
        {
            if ((++animationCounter) >=5)
            {
                waitAnimate();
                animationCounter = 0;
            }

        }

        public  void startAnimation(List<Point3D> points)
        {
            clearLastValues();
            animating = true;

            this.points = new List<Point3D>();
            this.points.AddRange(points.ToArray());

            iter = this.points.GetEnumerator();
            animate();
           
        }

        public void stopAnimation()
        {
            animating = false;
            clearLastValues();
        }
    }
}

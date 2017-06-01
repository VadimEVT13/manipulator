using InverseTest.GUI;
using InverseTest.InverseAlgorithm;
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
            if (iter.MoveNext() && animating)
            {
                Point3D point = iter.Current;
                JointsChain resultedChain = Algorithm.Solve(manipulator.ManipMathModel, point);
                Console.WriteLine($"Animate point: {1} { point.ToString()}");
                Dictionary<ManipulatorV2.ManipulatorParts, double> currentRotateAngle = new Dictionary<ManipulatorV2.ManipulatorParts, double>()
       {
            {   ManipulatorV2.ManipulatorParts.Table, resultedChain.Joints[0].JointAxises.RotationAngle},
            {   ManipulatorV2.ManipulatorParts.MiddleEdge, resultedChain.Joints[0].JointAxises.TurnAngle},
            {   ManipulatorV2.ManipulatorParts.TopEdgeBase,  resultedChain.Joints[1].JointAxises.TurnAngle},
            {   ManipulatorV2.ManipulatorParts.TopEdge,resultedChain.Joints[2].JointAxises.RotationAngle },
            {   ManipulatorV2.ManipulatorParts.CameraBase, resultedChain.Joints[2].JointAxises.TurnAngle },
            {   ManipulatorV2.ManipulatorParts.Camera, 0.0 },

       };

                Point3D rotatePoint;
                Model3D modelToRotate;
                Vector3D rotationAxis;



                var allValues = (ManipulatorV2.ManipulatorParts[])Enum.GetValues(typeof(ManipulatorV2.ManipulatorParts));
                foreach (ManipulatorV2.ManipulatorParts part in allValues)
                {
                    RotateTransform3D rotate = this.manipulator.getRotateTransfofm(part,
                        currentRotateAngle[part], out rotatePoint, out modelToRotate, out rotationAxis);

                    TimeSpan duration = TimeSpan.FromSeconds(2);

                    DoubleAnimation animation = new DoubleAnimation();
                    animation.From = lastAngleOnPart[part];
                    animation.To = currentRotateAngle[part];
                    animation.Duration = duration;
                    animation.Completed += Animation_Completed;
                    animation.GetHashCode();
                    

                    modelToRotate.Transform = rotate;

                    lastAngleOnPart[part] = currentRotateAngle[part];

                    rotate.CenterX = rotatePoint.X;
                    rotate.CenterY = rotatePoint.Y;
                    rotate.CenterZ = rotatePoint.Z;


                    rotate.Rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);

                }
            }
            else
            {
                clearLastValues();
                MessageBox.Show("Симуляция заверешена!");
            }



        }


        private void clearLastValues()
        {
             var allValues = (ManipulatorV2.ManipulatorParts[])Enum.GetValues(typeof(ManipulatorV2.ManipulatorParts));

            foreach (ManipulatorV2.ManipulatorParts part in allValues)
            {
                lastAngleOnPart[part] = 0.0;
            }
            if (iter != null)
                iter.Dispose();

            manipulator.ResetMathModel();
            animationCounter = 0;
            animating = false;


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

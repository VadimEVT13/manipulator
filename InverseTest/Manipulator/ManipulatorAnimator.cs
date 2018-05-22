using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace InverseTest.Manipulator
{
    class ManipulatorAnimator
    {

        /// <summary>
        /// Интервал таймера по которому происодит движение
        /// </summary>
        private static readonly int TIMER_INTERVAL = 1000;





        private Dictionary<ManipulatorParts, double> partDeltasToRotate = new Dictionary<ManipulatorParts, double>();


        private ManipulatorV2 manipulator;


        DispatcherTimer timer;
        bool isAnimated = false;
        public bool IsAnimated{
            get
            {
                return isAnimated;
            }
        }

        private ManipulatorAngles anglesToSet;

        public ManipulatorAnimator(ManipulatorV2 manipulator)
        {
            this.manipulator = manipulator;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromTicks(TIMER_INTERVAL);
            timer.Tick += animation_tick;
        }

        public void StartAnimation(ManipulatorAngles angles)
        {
            this.anglesToSet = angles;
            calculateDeltas(angles);
            isAnimated = true;
            timer.Start();
        }

        private void calculateDeltas(ManipulatorAngles angles)
        {
            foreach (ManipulatorParts part in Enum.GetValues(typeof(ManipulatorParts)))
                partDeltasToRotate[part] = (angles.partAngles[part] - getPartCurrentAngle(part)) / 1000;
        }

        private double getPartCurrentAngle(ManipulatorParts part)
        {
            switch (part)
            {
                case ManipulatorParts.Camera: return manipulator.CameraPosition;
                case ManipulatorParts.CameraBase: return manipulator.CameraBasePosition;
                case ManipulatorParts.MiddleEdge: return manipulator.MiddleEdgePosition;
                case ManipulatorParts.Table: return manipulator.TablePosition;
                case ManipulatorParts.TopEdge:  return manipulator.TopEdgePosition;
                default: return 0;
            }
        }

        private void setPartCurrentAngle(ManipulatorParts part, double angle)
        {
            switch (part)
            {
                case ManipulatorParts.Camera: manipulator.CameraPosition = angle; break;
                case ManipulatorParts.CameraBase: manipulator.CameraBasePosition = angle; break; ;
                case ManipulatorParts.MiddleEdge: manipulator.MiddleEdgePosition = angle; break; ;
                case ManipulatorParts.Table:  manipulator.TablePosition = angle; break; ;
                case ManipulatorParts.TopEdge: manipulator.TopEdgePosition = angle; break; ;
                default: return;
            }
        }

        public void StopAnimation()
        {
            timer.Stop();
        }

        void animation_tick(object sender, EventArgs arg)
        {
            List<bool> partOnRightPos = new List<bool>();
            bool onRight = false;
            //var cameraAngle = checkedAngle();



            if (partOnRightPos.TrueForAll(b => b))
            {
                timer.Stop();
                isAnimated = false;
            }
        }

        private double checkedAngle(ManipulatorParts part, out bool onRightPosition)
        {
            double angle = getPartCurrentAngle(part) + partDeltasToRotate[part];
            onRightPosition = false;

            if (Math.Abs(anglesToSet.partAngles[part]) - Math.Abs(angle) <= 2 * Math.Abs(partDeltasToRotate[part]))
            {
                angle = anglesToSet.partAngles[part];
                onRightPosition = true;
            }
            return angle;
        }
    }
}

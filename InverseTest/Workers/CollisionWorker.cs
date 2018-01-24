using InverseTest.Collision;
using InverseTest.Detail;
using InverseTest.Manipulator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using static InverseTest.Collision.AABB;

namespace InverseTest.Workers
{
    public delegate void OnCollisionDetected(List<Except> collisoins);

    public class DummyCrutch {
        public IManipulatorModel Manipulator { get; }
        public IDetectorFrame DetectorFrame { get; }
        public DetailModel Detail { get; }
        public Model3DGroup Platform { get; }

        public DummyCrutch(IManipulatorModel manipulator, IDetectorFrame frame, DetailModel detail, Model3DGroup platform)
        {
            this.Manipulator = manipulator;
            this.DetectorFrame = frame;
            this.Detail = detail;
            this.Platform = platform;
        }
    }


    class CollisionWorker<T> : BackroundCalculations<T>
    {
        public event OnCollisionDetected onCollision;
        private AABB aabb;



        public CollisionWorker(IManipulatorModel manipulator, IDetectorFrame frame, DetailModel detail, Model3DGroup platform) : base()
        {
            this.aabb = new AABB();
            this.aabb.MakeListExcept(manipulator, frame, detail, platform);
        }

        public void findCollision(T elem)
        {
          queue.Enqueue(elem);
        }

        protected override void DoWork(T elem, DoWorkEventArgs arg)
        {

            DummyCrutch d = elem as DummyCrutch;

            List<Except> collisions = aabb.Find(d.Manipulator, d.DetectorFrame, d.Detail, d.Platform);
            worker.ReportProgress(0, collisions);
            arg.Result = collisions;
        }

        protected override void workerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
           //onCollision?.Invoke(e.Result as List<Except>);
        }

        protected override void workerProgressUpdate(object sender, ProgressChangedEventArgs e)
        {
            onCollision((List<Except>)e.UserState);
        }

    }
}

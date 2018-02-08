using InverseTest.Collision;
using InverseTest.Collision.Model;
using InverseTest.Detail;
using InverseTest.Manipulator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using static InverseTest.Collision.AABB;

namespace InverseTest.Workers
{

    class GJKWorker<T> : BackroundCalculations<T>
    {
        public event OnCollisionDetected onCollision;
        private GJKSolver solver;
        private Dispatcher dispatcher;

        public GJKWorker(GJKSolver solver, Dispatcher dispatcher) : base()
        {
            this.solver = solver;
            this.dispatcher = dispatcher;
        }

        public void findCollision(T elem)
        {
            queue.Enqueue(elem);
        }

        protected override void DoWork(T elem, DoWorkEventArgs arg)
        {
            CollisionPair c = elem as CollisionPair;
            dispatcher.InvokeAsync(new Action(() => 
            {
                if (solver.IntersectGJK(c))
                    worker.ReportProgress(0, c);
            }), DispatcherPriority.Background);
}

        protected override void workerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        protected override void workerProgressUpdate(object sender, ProgressChangedEventArgs e)
        {
            onCollision?.Invoke((CollisionPair)e.UserState);
        }

    }
}

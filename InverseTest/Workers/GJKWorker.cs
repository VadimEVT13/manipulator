using InverseTest.Collision;
using InverseTest.Collision.Model;
using InverseTest.Detail;
using InverseTest.Manipulator;
using System;
using System.Collections.Concurrent;
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
        private AABB aabb;

        public GJKWorker(AABB aabb, GJKSolver solver) : base()
        {
            this.solver = solver;
            this.aabb = aabb;
        }

        public void findCollision(T elem)
        {
            if (!queue.IsEmpty)
            {
                clearQueue(queue);
            }
            queue.Enqueue(elem);


            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync();
            }
        }

        protected override void DoWork(T elem, DoWorkEventArgs arg)
        {
            SceneSnapshot scs = elem as SceneSnapshot;

            Queue<CollisionPair> pairs = new Queue<CollisionPair> (aabb.Find(scs));
            List<CollisionPair> collisions = new List<CollisionPair>();

            if (pairs.Count > 0)
            {
                while(pairs.Count>0)
                {
                    CollisionPair pair = pairs.Dequeue();
                    if (solver.IntersectGJK(pair))
                    {
                        collisions.Add(pair);
                    }
                }
            }
            else
            {
                worker.ReportProgress(0, null);
            }

            worker.ReportProgress(0, collisions);

        }

        protected override void workerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.Error.WriteLine(e.Error);
        }

        protected override void workerProgressUpdate(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState == null)
            {
                onCollision?.Invoke(null);
            }
            else
            {
                onCollision?.Invoke((List<CollisionPair>)e.UserState);
            }
        }

      
    }
}

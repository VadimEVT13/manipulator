﻿using InverseTest.Collision;
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

            List<CollisionPair> pairs = aabb.Find(scs);

            if (pairs.Count > 0)
            {
                foreach (CollisionPair pair in pairs)
                {
                    if (solver.IntersectGJK(pair))
                    {
                        worker.ReportProgress(0, pair);
                    }
                }
            }
            else
            {
                worker.ReportProgress(0, null);
            }

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
                onCollision?.Invoke((CollisionPair)e.UserState);
            }
        }

        private void clearQueue(ConcurrentQueue<T> queue)
        {
            T res;
            while (queue.TryDequeue(out res)) { }
        }
    }
}

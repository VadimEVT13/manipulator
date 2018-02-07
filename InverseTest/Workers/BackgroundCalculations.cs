using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest
{
    abstract class BackroundCalculations<T>
    {
        public BackgroundWorker worker { get; }
        public ConcurrentQueue<T> queue;

        public BackroundCalculations()
        {
            this.worker = new BackgroundWorker() { WorkerReportsProgress = true };
            this.queue = new ConcurrentQueue<T>();
            this.worker.RunWorkerCompleted += workerCompleted;
            this.worker.DoWork += workerStarted;
            this.worker.ProgressChanged += workerProgressUpdate;
            this.worker.RunWorkerAsync();
        }

        protected abstract void workerCompleted(object sender, RunWorkerCompletedEventArgs e);

        protected abstract void workerProgressUpdate(object sender, ProgressChangedEventArgs e);

        protected void workerStarted(object sender, DoWorkEventArgs e)
        {

            Console.WriteLine("QueueCount: " + queue.Count);
            while (queue.Count > 0)
            {
                T elem;
                if (queue.TryDequeue(out elem))
                {
                    DoWork(elem, e);
                }
            }

        }

        protected abstract void DoWork(T elem, DoWorkEventArgs arg);

    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest
{
    abstract class BackroundCalculations<T, L> where L:class
    {
        public delegate void BackroundCalculationComplete(L complete);


        public event BackroundCalculationComplete OnComplete;

        public BackgroundWorker worker { get; }
        public volatile Queue<T> queue;
        public int queueMaxsize;

        public BackroundCalculations(int queueMaxSize = 10)
        {
            this.queueMaxsize = queueMaxSize;
            this.worker = new BackgroundWorker() { WorkerReportsProgress = true };
            this.queue = new Queue<T>();
            this.worker.DoWork += DoWork;
            this.worker.ProgressChanged += workerProgressUpdate;
            this.worker.RunWorkerAsync();
        }

        public void Solve(T elem)
        {
            if (queue.Count >= this.queueMaxsize)
            {
                Console.WriteLine("WORKER: " + "Clearing queue:" + queue.Count);
                queue.Clear();
            }
            queue.Enqueue(elem);
            Console.WriteLine("WORKER: " + "Add to queue:" + queue.Count);

            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync();
            }
        }

        protected void workerProgressUpdate(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is L res)
            {
                OnComplete?.Invoke(res);
            }
        }

        protected  void DoWork(object sender, DoWorkEventArgs arg)
        {
            while (queue.Count > 0)
            {
                T elem = queue.Dequeue();
                if (elem != null)
                {
                    L res = Calculate(elem);
                    worker.ReportProgress(0, res);
                }
            }
        }

        protected abstract L Calculate(T elem);
       
    }
}

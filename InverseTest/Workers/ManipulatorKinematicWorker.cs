using InverseTest.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media.Media3D;
using System.Threading;

namespace InverseTest.Workers
{
    public delegate void ManipulatorKinematicSolved(ManipulatorAngles angles);
    public delegate void ManipulatorKinematicError();

    class ManipulatorKinematicWorker<T> : BackroundCalculations<T>
    {
        Kinematic kinematic;

        public event ManipulatorKinematicSolved kinematicSolved;
        public event ManipulatorKinematicError kinematicError;


        public ManipulatorKinematicWorker(Kinematic kinematic):base()
        {
            this.kinematic = kinematic;
        }

        public void solve(T elem)
        {
            queue.Enqueue(elem);
        }

        protected override void workerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("Thread: " + Thread.CurrentThread.ManagedThreadId);

            if (e.Result == null)
                return;

            

            kinematicSolved?.Invoke((ManipulatorAngles)e.Result);
        }
        
        protected override void workerProgressUpdate(object sender, ProgressChangedEventArgs e)
        {
            ManipulatorAngles pos = e.UserState as ManipulatorAngles;
            kinematicSolved?.Invoke(pos);
        }

        protected override void DoWork(T elem, DoWorkEventArgs arg)
        {
            SystemPosition sp = elem as SystemPosition;
            Stack<double[]> rezults;



             rezults = this.kinematic.InverseNab(sp.manipPoint.X, sp.manipPoint.Z, sp.manipPoint.Y, sp.targetPoint.X, sp.manipPoint.Z, sp.manipPoint.Y);

            //TODO Перенести проверку ограничений в библиотеку кинематики, добавить функцию для задания ограничений
            // по умолчанию сделать все ограничения int.MaxValue. Если позиция не достижима то выкидывать исключение
            // PositionUnattainableException


            if (rezults.Count > 0)

            {
                Stack<double[]> satisfied = new Stack<double[]>();

                foreach (double[] one in rezults)
                {

                    if (
                       (MathUtils.RadiansToAngle(one[0]) < 90 && MathUtils.RadiansToAngle(one[0]) > -90) &
                       (MathUtils.RadiansToAngle(one[1]) < 90 && MathUtils.RadiansToAngle(one[1]) > -90) &
                       (MathUtils.RadiansToAngle(one[2]) < 70 && MathUtils.RadiansToAngle(one[2]) > -70) &
                       (MathUtils.RadiansToAngle(one[3]) < 220 && MathUtils.RadiansToAngle(one[3]) > -220) &
                       (MathUtils.RadiansToAngle(one[4]) < 170 && MathUtils.RadiansToAngle(one[4]) > 0)
                       )
                    {
                        satisfied.Push(one);
                    }
                  
                }

                if (satisfied.Count > 0)
                {
                    double[] rez = satisfied.Pop();
                    ManipulatorAngles angles = new ManipulatorAngles(
                        MathUtils.RadiansToAngle(rez[0]),
                        MathUtils.RadiansToAngle(rez[1]),
                        MathUtils.RadiansToAngle(rez[2]),
                        MathUtils.RadiansToAngle(rez[3]),
                        MathUtils.RadiansToAngle(rez[4])
                        );

                    worker.ReportProgress(0, angles);
                }
                else
                {
                    double[] rez = rezults.Pop();
                    ManipulatorAngles angles = new ManipulatorAngles(
                        MathUtils.RadiansToAngle(rez[0]),
                        MathUtils.RadiansToAngle(rez[1]),
                        MathUtils.RadiansToAngle(rez[2]),
                        MathUtils.RadiansToAngle(rez[3]),
                        MathUtils.RadiansToAngle(rez[4])
                        );

                    worker.ReportProgress(0, angles);
                }
            }
        }

    }
}




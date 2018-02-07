using InverseTest.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media.Media3D;
using System.Threading;
using InverseTest.Manipulator.Models;

namespace InverseTest.Workers
{
    public delegate void ManipulatorKinematicSolved(ManipulatorAngles angles);
    public delegate void ManipulatorKinematicError();

    class ManipulatorKinematicWorker<T> : BackroundCalculations<T>
    {
        Kinematic kinematic;

        public event ManipulatorKinematicSolved kinematicSolved;
        public event ManipulatorKinematicError kinematicError;


        public ManipulatorKinematicWorker(Kinematic kinematic) : base()
        {
            this.kinematic = kinematic;
        }

        public void solve(T elem)
        {
            queue.Enqueue(elem);
            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync();
            }
        }

        protected override void workerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
           
        }

        protected override void workerProgressUpdate(object sender, ProgressChangedEventArgs e)
        {
            ManipulatorAngles pos = e.UserState as ManipulatorAngles;
            kinematicSolved?.Invoke(pos);
        }

        protected override void DoWork(T elem, DoWorkEventArgs arg)
        {
            SystemPosition sp = elem as SystemPosition;
            Stack<Angle3D> rezults;
            
            rezults = this.kinematic.InverseNab(sp.manipPoint.X, sp.manipPoint.Z, sp.manipPoint.Y, sp.targetPoint.X, sp.manipPoint.Z, sp.manipPoint.Y);

            //TODO Перенести проверку ограничений в библиотеку кинематики, добавить функцию для задания ограничений
            // по умолчанию сделать все ограничения int.MaxValue. Если позиция не достижима то выкидывать исключение
            // PositionUnattainableException
            if (rezults.Count > 0)
            {
                Stack<Angle3D> satisfied = new Stack<Angle3D>();
                Stack<Angle3D> unsatisfied = new Stack<Angle3D>();
                foreach (Angle3D one in rezults)
                {

                    if (
                       (MathUtils.RadiansToAngle(one.O1) < 90 && MathUtils.RadiansToAngle(one.O1) > -90) &
                       (MathUtils.RadiansToAngle(one.O2) < 90 && MathUtils.RadiansToAngle(one.O2) > -90) &
                       (MathUtils.RadiansToAngle(one.O3) < 70 && MathUtils.RadiansToAngle(one.O3) > -70) &
                       (MathUtils.RadiansToAngle(one.O4) < 220 && MathUtils.RadiansToAngle(one.O4) > -220) &
                       (MathUtils.RadiansToAngle(one.O5) < 170 && MathUtils.RadiansToAngle(one.O5) > 0)
                       )
                    {
                        satisfied.Push(one);
                    }
                    else
                    {
                        unsatisfied.Push(one);
                    }
                }

                ManipulatorAngles angles;
                if (satisfied.Count > 0)
                {
                    Angle3D rez = satisfied.Pop();
                    angles = new ManipulatorAngles(
                        MathUtils.RadiansToAngle(rez.O1),
                        MathUtils.RadiansToAngle(rez.O2),
                        MathUtils.RadiansToAngle(rez.O3),
                        MathUtils.RadiansToAngle(rez.O4),
                        MathUtils.RadiansToAngle(rez.O5)
                        );
                }
                else {
                    Angle3D rez = unsatisfied.Pop();
                    angles = new ManipulatorAngles(
                        MathUtils.RadiansToAngle(rez.O1),
                        MathUtils.RadiansToAngle(rez.O2),
                        MathUtils.RadiansToAngle(rez.O3),
                        MathUtils.RadiansToAngle(rez.O4),
                        MathUtils.RadiansToAngle(rez.O5),
                        false
                        );
                }

                worker.ReportProgress(0, angles);
            }
        }

    }
}




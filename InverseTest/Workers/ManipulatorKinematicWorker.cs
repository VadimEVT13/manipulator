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
            Stack<Vector5D> rezults;
            
            rezults = this.kinematic.InverseNab(sp.manipPoint.X, sp.manipPoint.Z, sp.manipPoint.Y, sp.targetPoint.X, sp.targetPoint.Z, sp.targetPoint.Y);

            //TODO Перенести проверку ограничений в библиотеку кинематики, добавить функцию для задания ограничений
            // по умолчанию сделать все ограничения int.MaxValue. Если позиция не достижима то выкидывать исключение
            // PositionUnattainableException
            if (rezults.Count > 0)
            {
                Stack<Vector5D> satisfied = new Stack<Vector5D>();
                Stack<Vector5D> unsatisfied = new Stack<Vector5D>();
                foreach (Vector5D one in rezults)
                {

                    if (
                       (MathUtils.RadiansToAngle(one.K1) < 90 && MathUtils.RadiansToAngle(one.K1) > -90) &
                       (MathUtils.RadiansToAngle(one.K2) < 90 && MathUtils.RadiansToAngle(one.K2) > -90) &
                       (MathUtils.RadiansToAngle(one.K3) < 70 && MathUtils.RadiansToAngle(one.K3) > -70) &
                       (MathUtils.RadiansToAngle(one.K4) < 220 && MathUtils.RadiansToAngle(one.K4) > -220) &
                       (MathUtils.RadiansToAngle(one.K5) < 170 && MathUtils.RadiansToAngle(one.K5) > 0)
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
                    Vector5D rez = satisfied.Pop();
                    angles = new ManipulatorAngles(
                        MathUtils.RadiansToAngle(rez.K1),
                        MathUtils.RadiansToAngle(rez.K2),
                        MathUtils.RadiansToAngle(rez.K3),
                        MathUtils.RadiansToAngle(rez.K4),
                        MathUtils.RadiansToAngle(rez.K5)
                        );
                }
                else {
                    Vector5D rez = unsatisfied.Pop();
                    angles = new ManipulatorAngles(
                        MathUtils.RadiansToAngle(rez.K1),
                        MathUtils.RadiansToAngle(rez.K2),
                        MathUtils.RadiansToAngle(rez.K3),
                        MathUtils.RadiansToAngle(rez.K4),
                        MathUtils.RadiansToAngle(rez.K5),
                        false
                        );
                }

                worker.ReportProgress(0, angles);
            }
        }

    }
}




﻿using InverseTest.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media.Media3D;
using System.Threading;
using InverseTest.Manipulator.Models;
using InverseTest.Frame;
using InverseTest.Frame.Kinematic;
using InverseTest.Model;
using InverseTest.Bound;

namespace InverseTest.Workers
{

    class KinematicWorker<T, L> : BackroundCalculations<T, L> where L : class
    {
        Kinematic kinematic;
        PortalKinematic portal;
        PortalBoundController portalBounds;
        ManipulatorAnglesBounds manipulatorBounds;

        public KinematicWorker(Kinematic kinematic, 
            PortalKinematic portal, 
            PortalBoundController portalBounds,
            ManipulatorAnglesBounds manipulatorBounds) : base()
        {
            this.kinematic = kinematic;
            this.portal = portal;
            this.portalBounds = portalBounds;
            this.manipulatorBounds = manipulatorBounds;
        }

        protected override L Calculate(T elem)
        {
            SystemPosition sp = elem as SystemPosition;
            ManipulatorAngles manipAngles = SolveManipulator(sp);
            DetectorFramePosition portalPos = SolvePortal(sp);
            return new SystemState(manipAngles, portalPos) as L;
        }


        private ManipulatorAngles SolveManipulator(SystemPosition sp)
        {

            Stack<Angle3D> rezults;

            rezults = this.kinematic.InverseNab(sp.ManipPoint.X, sp.ManipPoint.Z, sp.ManipPoint.Y, sp.TargetPoint.X, sp.TargetPoint.Z, sp.TargetPoint.Y);

            //TODO Перенести проверку ограничений в библиотеку кинематики, добавить функцию для задания ограничений
            // по умолчанию сделать все ограничения int.MaxValue. Если позиция не достижима то выкидывать исключение
            // PositionUnattainableException
            if (rezults.Count > 0)
            {
                Stack<Angle3D> satisfied = new Stack<Angle3D>();
                Stack<Angle3D> unsatisfied = new Stack<Angle3D>();
                foreach (Angle3D one in rezults)
                {
                    if (manipulatorBounds.CheckAngles(one))
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
                else
                {
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

                return angles;
            }
            else return null;
        }

        private DetectorFramePosition SolvePortal(SystemPosition sp)
        {
            portal.setPointManipAndNab(sp.ManipPoint.X, sp.ManipPoint.Z, sp.ManipPoint.Y, sp.TargetPoint.X, sp.TargetPoint.Z, sp.TargetPoint.Y);

            double[] rez = portal.portalPoint(sp.DistanceManipulatorToScanPoint, sp.FocusEnlagment);
            if (rez != null)
            {
                ///ХЗ почему со знаком -
                DetectorFramePosition detectp = new DetectorFramePosition(new Point3D(rez[5], rez[7], rez[6]), -rez[4], rez[3]);
                detectp = portalBounds.CheckDetectroFramePosition(detectp);
                return detectp;
            }
            else
            {
                return null;
            }
        }

    }
}



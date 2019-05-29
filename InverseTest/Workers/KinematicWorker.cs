using InverseTest.Manipulator;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using InverseTest.Manipulator.Models;
using InverseTest.Frame;
using InverseTest.Frame.Kinematic;
using InverseTest.Model;
using InverseTest.Bound;
using System;

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
            ManipulatorAnglesBounds manipulatorBounds, int queueSize = 10) : base(queueSize)
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

            if (rezults.Count > 0)
            {
                ManipulatorAngles angles;
               
                    Angle3D rez = rezults.Pop();
                    angles = new ManipulatorAngles(
                        MathUtils.RadiansToAngle(rez.O1),
                        MathUtils.RadiansToAngle(rez.O2),
                        MathUtils.RadiansToAngle(rez.O3),
                        MathUtils.RadiansToAngle(rez.O4),
                        MathUtils.RadiansToAngle(rez.O5)
                        );
              
                return angles;
            }
            else return null;
        }

        private DetectorFramePosition SolvePortal(SystemPosition sp)
        {
            portal.SetPointManipAndNab(sp.ManipPoint.X, sp.ManipPoint.Z, sp.ManipPoint.Y, sp.TargetPoint.X, sp.TargetPoint.Z, sp.TargetPoint.Y);
            
            double[] rez = portal.PortalPoint(sp.DistanceManipulatorToScanPoint, sp.FocusEnlagment);
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




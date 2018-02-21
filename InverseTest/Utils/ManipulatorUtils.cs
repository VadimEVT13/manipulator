using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Manipulator
{
    public class ManipulatorUtils
    {
        public static double[] CalculateManipulatorLength(ManipulatorV2 manipulator) {
            double edge1 = manipulator.GetPointJoint(ManipulatorV2.ManipulatorRotatePoints.POINT_ON_TABLE).Y;

            double edge2 = manipulator.GetPointJoint(ManipulatorV2.ManipulatorRotatePoints.POINT_ON_TABLE)
                .DistanceTo(manipulator.GetPointJoint(ManipulatorV2.ManipulatorRotatePoints.POINT_ON_MAIN_EDGE));

            Point3D pointOnMainEdge = manipulator.GetPointJoint(ManipulatorV2.ManipulatorRotatePoints.POINT_ON_MAIN_EDGE);
            Point3D pointBelowCam = manipulator.GetPointJoint(ManipulatorV2.ManipulatorRotatePoints.POINT_BELOW_CAMERA);

            //Точка над основным ребром на уровне точки под камерой
            Point3D point1 = new Point3D(pointOnMainEdge.X, pointOnMainEdge.Y, pointBelowCam.Z);
            double edge3 = point1.DistanceTo(manipulator.GetPointJoint(ManipulatorV2.ManipulatorRotatePoints.POINT_BELOW_CAMERA));

            Point3D pointCamera = manipulator.GetPointJoint(ManipulatorV2.ManipulatorRotatePoints.POINT_ON_CAMERA);
            //Точка на уровне камеры, 
            Point3D point = new Point3D(pointBelowCam.X, pointCamera.Y, pointCamera.Z);
            double edge4 = pointBelowCam.DistanceTo(point);
            double edge5 = point.DistanceTo(pointCamera);

            return new double[] { edge1, edge2, edge3, edge4, edge5 };
        }


        //Что за Det???? 
        public static double CalculateManipulatorDet(ManipulatorV2 manip)
        {
            Point3D rotatePointOnMidleEdge = manip.GetPointJoint(ManipulatorV2.ManipulatorRotatePoints.POINT_ON_MAIN_EDGE);
            Point3D pointBelowCamera = manip.GetPointJoint(ManipulatorV2.ManipulatorRotatePoints.POINT_BELOW_CAMERA);
            Point3D pointOnTopEdge = new Point3D(rotatePointOnMidleEdge.X, pointBelowCamera.Y, rotatePointOnMidleEdge.Z);

            return rotatePointOnMidleEdge.DistanceTo(pointOnTopEdge);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using InverseTest.Manipulator;
using InverseTest.Frame.Kinematic;

namespace InverseTest
{
    class DetectFrameKinematic
    {

        PortalKinematic kinematic;
        private IManipulatorModel manipulatorModel;
        private IDetectorFrame detectorFrameModel;


        public DetectFrameKinematic(IManipulatorModel manipulatorModel, IDetectorFrame detectorFrameModel)
        {
            this.manipulatorModel = manipulatorModel;
            this.detectorFrameModel = detectorFrameModel;
        }


        /// <summary>
        /// Вычисляет позицию в которую необходимо поставить рамку
        /// </summary>
        /// <param name="point">Точка которую сканирует манипулятор</param>
        public DetectorFramePosition solve(Point3D pointToDetect)
        {
            Model3D frameModel = detectorFrameModel.GetDetectorFrameModel();

            Point3D maxOffset = new Point3D(frameModel.Bounds.SizeX, frameModel.Bounds.SizeY, frameModel.Bounds.SizeZ);
            Console.WriteLine("Максимальное откланение: " + maxOffset);

            Model3D cameraModel = manipulatorModel.GetManipulatorPart(ManipulatorV2.ManipulatorParts.Camera);
            Point3D pointManip = new Point3D(cameraModel.Bounds.X + cameraModel.Bounds.SizeX, cameraModel.Bounds.Y + cameraModel.Bounds.SizeY / 2, cameraModel.Bounds.Z + cameraModel.Bounds.SizeZ / 2);
            Console.WriteLine("Точка в которой стоит схват манипупялторая: " + pointManip);
           
            Model3D screenModel = detectorFrameModel.GetDetectorFramePart(DetectorFrame.Parts.Screen);
            Model3D screenHolderModel = detectorFrameModel.GetDetectorFramePart(DetectorFrame.Parts.ScreenHolder);

            Console.WriteLine("Длина звеньев: " + screenModel.Bounds.SizeX + " " +  screenHolderModel.Bounds.SizeX);

            kinematic = new PortalKinematic(maxOffset.X, maxOffset.Y, maxOffset.Z);
            kinematic.setPortal(frameModel.Bounds.X, frameModel.Bounds.Y,
           frameModel.Bounds.Z);
            kinematic.setPortalLen(screenModel.Bounds.SizeX, screenHolderModel.Bounds.SizeX);
            kinematic.setPointManip(pointManip.X, pointManip.Y, pointManip.Z);
            kinematic.setPointNab(pointToDetect.X, pointToDetect.Y, pointToDetect.Z);
           

            Console.WriteLine("Точка наблюдения: " + pointToDetect);
            double[] alphaAndBetha = kinematic.getAlfAndBet();

            double[] point1 = kinematic.portalPoint(1);

            double[] point2 = kinematic.ustPoint();

            try
            {
                Console.WriteLine("PortalPoint: " + new Point3D(point1[0], point1[1], point1[2]));
                Console.WriteLine("ustPoint: " + new Point3D(point2[0], point2[1], point2[2]), alphaAndBetha[0]);
            }catch(Exception ex)
            { }




            return new DetectorFramePosition(new Point3D(point1[0],point1[1], point1[2]), alphaAndBetha[0], alphaAndBetha[1]);

        }

    }
}

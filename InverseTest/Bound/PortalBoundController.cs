using InverseTest.Frame.Kinematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Bound
{
    public class PortalBoundController
    {
        private Bound VerticalFrameBound;
        private Bound HorizontalBarBound;
        private Bound ScreenHolderBound;
        private Bound VerticalAngleBound;
        private Bound HorizontalAngleBound;

        private DetectorFrame portal;


        public PortalBoundController()
        {
            this.VerticalAngleBound = new Bound();
            this.HorizontalAngleBound = new Bound();
            this.ScreenHolderBound = new Bound();
            this.HorizontalBarBound = new Bound();
            this.VerticalFrameBound = new Bound();
        }

        /// <summary>
        /// Непонятная
        /// </summary>
        /// <param name="portal"></param>
        public void CalculateBounds(DetectorFrame portal)
        {
            var verticalFrBottom = portal.GetDetectorFrameModel().Bounds.X;
            var verticalFrUpper = portal.GetDetectorFrameModel().Bounds.X 
                + portal.GetDetectorFrameModel().Bounds.SizeX
                - portal.GetDetectorFramePart(DetectorFrame.Parts.VerticalFrame).Bounds.SizeX;
            this.VerticalFrameBound = new Bound(verticalFrBottom, verticalFrUpper);

            var horizBottom = portal.GetDetectorFramePart(DetectorFrame.Parts.VerticalFrame).Bounds.Y + portal.GetDetectorFramePart(DetectorFrame.Parts.Platform).Bounds.SizeY;
            var horizUpper = portal.GetDetectorFrameModel().Bounds.SizeY
                - portal.GetDetectorFramePart(DetectorFrame.Parts.HorizontalBar).Bounds.SizeY/2;
            this.HorizontalBarBound = new Bound(horizBottom, horizUpper);

            var screenBottom = portal.GetDetectorFrameModel().Bounds.Z;
            var screenUpper = portal.GetDetectorFrameModel().Bounds.Z 
                + portal.GetDetectorFrameModel().Bounds.SizeZ
                - portal.GetDetectorFramePart(DetectorFrame.Parts.ScreenHolder).Bounds.Z;
            this.ScreenHolderBound = new Bound(screenBottom, screenUpper);

            this.HorizontalAngleBound = new Bound(-60, 60);
            this.VerticalAngleBound = new Bound(-60, 60);
        }

        public double CheckVerticalFrame(double val)
        {
            return this.VerticalFrameBound.GetBoundary(val);
        }

        public double CheckHorizontalBar(double val)
        {
            return this.HorizontalBarBound.GetBoundary(val);
        }

        public double CheckScreen(double val)
        {
            return this.ScreenHolderBound.GetBoundary(val);
        }

        public double CheckHorizAngle(double val)
        {
            return this.HorizontalAngleBound.GetBoundary(val);
        }

        public double CheckVerticalAngle(double val)
        {
            return this.VerticalAngleBound.GetBoundary(val);
        }

        public DetectorFramePosition CheckDetectroFramePosition(DetectorFramePosition p)
        {
            var newVert = CheckVerticalFrame(p.pointScreen.X);
            var newHoriz = CheckHorizontalBar(p.pointScreen.Y);
            var newScreen = CheckScreen(p.pointScreen.Z);
            var horAngle = CheckHorizAngle(p.horizontalAngle);
            var vertAngle = CheckVerticalAngle(p.verticalAngle);
            return new DetectorFramePosition(new Point3D(newVert, newHoriz,newScreen), horAngle, vertAngle);
        }
    }
}

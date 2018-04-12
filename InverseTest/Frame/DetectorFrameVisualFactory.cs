using InverseTest.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Frame
{
    public class DetectorFrameVisualFactory
    {
        public static DetectorFrameVisual CreateDetectorFrameVisual(DetectorFrame detectorFrame)
        {
            Dictionary<DetectorFrame.Parts, MainVisual> parts = new Dictionary<DetectorFrame.Parts, MainVisual>();
            MainVisual horiz = new MainVisual(detectorFrame.GetDetectorFramePart(DetectorFrame.Parts.HorizontalBar));
            MainVisual platf = new MainVisual(detectorFrame.GetDetectorFramePart(DetectorFrame.Parts.PortalPlatform)); ;
            MainVisual screen = new MainVisual(detectorFrame.GetDetectorFramePart(DetectorFrame.Parts.Screen)); ;
            MainVisual screnHolder = new MainVisual(detectorFrame.GetDetectorFramePart(DetectorFrame.Parts.ScreenHolder)); ;
            MainVisual rotator = new MainVisual(detectorFrame.GetDetectorFramePart(DetectorFrame.Parts.ScreenRotator)); ;
            MainVisual vertical= new MainVisual(detectorFrame.GetDetectorFramePart(DetectorFrame.Parts.VerticalFrame));

            parts.Add(DetectorFrame.Parts.HorizontalBar, horiz);
            parts.Add(DetectorFrame.Parts.PortalPlatform, platf);
            parts.Add(DetectorFrame.Parts.Screen, screen);
            parts.Add(DetectorFrame.Parts.ScreenHolder, screnHolder);
            parts.Add(DetectorFrame.Parts.ScreenRotator, rotator);
            parts.Add(DetectorFrame.Parts.VerticalFrame, vertical);

            return new DetectorFrameVisual(parts);
        }
    }
}

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
        public static DetectorFrameVisual CreateDetectorFrameVisual(IDetectorFrame detectorFrame)
        {
            Dictionary<DetectorFrame.Parts, VisualModel> parts = new Dictionary<DetectorFrame.Parts, VisualModel>();
            VisualModel horiz = new VisualModel(detectorFrame.GetDetectorFramePart(DetectorFrame.Parts.HorizontalBar));
            VisualModel platf = new VisualModel(detectorFrame.GetDetectorFramePart(DetectorFrame.Parts.Platform)); ;
            VisualModel screen = new VisualModel(detectorFrame.GetDetectorFramePart(DetectorFrame.Parts.Screen)); ;
            VisualModel screnHolder = new VisualModel(detectorFrame.GetDetectorFramePart(DetectorFrame.Parts.ScreenHolder)); ;
            VisualModel rotator = new VisualModel(detectorFrame.GetDetectorFramePart(DetectorFrame.Parts.ScreenRotator)); ;
            VisualModel vertical= new VisualModel(detectorFrame.GetDetectorFramePart(DetectorFrame.Parts.VerticalFrame));

            parts.Add(DetectorFrame.Parts.HorizontalBar, horiz);
            parts.Add(DetectorFrame.Parts.Platform, platf);
            parts.Add(DetectorFrame.Parts.Screen, screen);
            parts.Add(DetectorFrame.Parts.ScreenHolder, screnHolder);
            parts.Add(DetectorFrame.Parts.ScreenRotator, rotator);
            parts.Add(DetectorFrame.Parts.VerticalFrame, vertical);

            return new DetectorFrameVisual(parts);
        }
    }
}

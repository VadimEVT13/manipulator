using InverseTest.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Frame
{
    public class DetectorFrameVisual: IVisualController
    {
        public List<VisualModel> Visuals => portalVisuals.Values.ToList();

        public Dictionary<DetectorFrame.Parts, VisualModel> portalVisuals;

        public DetectorFrameVisual(Dictionary<DetectorFrame.Parts, VisualModel> parts)
        {
            this.portalVisuals = parts;
        }

        public void ChangePartsColor(List<DetectorFrame.Parts> parts)
        {
            foreach (KeyValuePair<DetectorFrame.Parts, VisualModel> pair in portalVisuals)
            {
                if (parts.Contains(pair.Key))
                    pair.Value.SetCollisionCollor();
                else pair.Value.SetDefaultColor();
            }
        }
    }
}

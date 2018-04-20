using InverseTest.GUI;
using InverseTest.GUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Frame
{
    public class DetectorFrameVisual : IVisualController
    {
        public List<MainVisual> Visuals => portalVisuals.Values.ToList();

        public Dictionary<DetectorFrame.Parts, MainVisual> portalVisuals;

        public DetectorFrameVisual(Dictionary<DetectorFrame.Parts, MainVisual> parts)
        {
            this.portalVisuals = parts;
        }

        public void ChangePartsColor(List<DetectorFrame.Parts> parts)
        {
            foreach (KeyValuePair<DetectorFrame.Parts, MainVisual> pair in portalVisuals)
            {
                if (parts.Contains(pair.Key))
                    pair.Value.SetCollisionCollor();
                else pair.Value.SetDefaultColor();
            }
        }
    }
}

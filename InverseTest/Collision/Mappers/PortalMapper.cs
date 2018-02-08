using InverseTest.Collision.Model;
using InverseTest.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Collision.Mappers
{
    public class PortalMapper
    {
        public static PortalSnapshot PortalToSnapshot(DetectorFrame frame)
        {
            var shapes = new Dictionary<DetectorFrame.Parts, PartShape>();

            var frameParts = frame.parts;

            foreach (KeyValuePair<DetectorFrame.Parts, IDetectorFramePart> part in frameParts) 
            {
                shapes.Add(part.Key, Utils.ExtractShapeFromModel(part.Value.GetModelPart()));
            }
            
            return new PortalSnapshot(shapes);
        } 

    }
}

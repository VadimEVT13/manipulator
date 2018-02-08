using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Collision.Model
{
    public class PortalSnapshot
    {
        public Dictionary<DetectorFrame.Parts, PartShape> bounds { get;}

        public PortalSnapshot(Dictionary<DetectorFrame.Parts, PartShape> bounds)
        {
            this.bounds = bounds;
        }

    }
}

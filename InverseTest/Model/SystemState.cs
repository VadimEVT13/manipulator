using InverseTest.Frame.Kinematic;
using InverseTest.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Model
{
    public class SystemState
    {
        public ManipulatorAngles Angles { get; }
        public DetectorFramePosition PortalPosition { get; }

        public SystemState(ManipulatorAngles angles, DetectorFramePosition portalPos)
        {
            this.Angles = angles;
            this.PortalPosition = portalPos;
        }
    }
}

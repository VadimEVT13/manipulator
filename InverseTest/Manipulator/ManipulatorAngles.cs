using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InverseTest.ManipulatorV2;

namespace InverseTest.Manipulator
{
    public class ManipulatorAngles
    {

        public readonly Dictionary<ManipulatorParts, double> partAngles;      

        public ManipulatorAngles(double angle1, double angle2, double angle3, double angle4, double angle5, double angle6)
        {
            partAngles = new Dictionary<ManipulatorParts, double>();
            partAngles[ManipulatorParts.Table] = -angle1;
            partAngles[ManipulatorParts.MiddleEdge] = -angle2;
            partAngles[ManipulatorParts.TopEdgeBase] = -angle3;
            partAngles[ManipulatorParts.TopEdge] = -angle4;
            partAngles[ManipulatorParts.CameraBase] = -angle5;
            partAngles[ManipulatorParts.Camera] = -angle6;
        }
      
    }
}

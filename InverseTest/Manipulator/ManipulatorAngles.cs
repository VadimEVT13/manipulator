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
        public bool isValid { get; }

        public ManipulatorAngles(double angle1, double angle2, double angle3, double angle4, double angle5, bool isValid = true)
        {
            partAngles = new Dictionary<ManipulatorParts, double>();
            partAngles[ManipulatorParts.Table] = -angle1;
            partAngles[ManipulatorParts.MiddleEdge] = -angle2;
            partAngles[ManipulatorParts.TopEdge] = -angle3;
            partAngles[ManipulatorParts.CameraBase] = -angle4;
            partAngles[ManipulatorParts.Camera] = -angle5;
            
            ///Костыль
            partAngles[ManipulatorParts.Platform] = 0;
            this.isValid = isValid;
        }

        public override string ToString()
        {
            String resString = "Angles: ";

            foreach (ManipulatorParts part in Enum.GetValues(typeof(ManipulatorParts)))
            {
                resString += part.ToString() + " :  " + partAngles[part].ToString();
            }

            return resString;
        }
    }
}

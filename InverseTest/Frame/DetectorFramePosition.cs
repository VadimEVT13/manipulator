using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Frame.Kinematic
{
    public class DetectorFramePosition
    {
        public readonly Point3D pointScreen;
        public double horizontalAngle;
        public double verticalAngle;

        public DetectorFramePosition(Point3D pointScreen, double horizontalAngle, double verticalAngle)
        {
            this.pointScreen = pointScreen;
            this.horizontalAngle = horizontalAngle;
            this.verticalAngle = verticalAngle;
        }
        
        public override string ToString()
        {
            return "DetectFrame. Position:" + pointScreen.ToString() + "  HorizontalAngle: " + horizontalAngle + "  VerticalAngle: " + verticalAngle;
        }



    }
}

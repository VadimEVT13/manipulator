using InverseTest.Manipulator;
using InverseTest.Manipulator.Models;

namespace InverseTest.Bound
{
    public class ManipulatorAnglesBounds
    {
        const int L1_TOP = 90;
        const int L1_BOTTOM = -90;

        const int L2_TOP = 90;
        const int L2_BOTTOM = -90;

        const int L3_TOP = 70;
        const int L3_BOTTOM = -70;

        const int L4_TOP = 220;
        const int L4_BOTTOM = -220;

        const int L5_TOP = 170;
        const int L5_BOTTOM = 0;

        Bound L1;
        Bound L2;
        Bound L3;
        Bound L4;
        Bound L5;

        public ManipulatorAnglesBounds()
        {
            this.L1 = new Bound(L1_BOTTOM, L1_TOP);
            this.L2 = new Bound(L2_BOTTOM, L2_TOP);
            this.L3 = new Bound(L3_BOTTOM, L3_TOP);
            this.L4 = new Bound(L4_BOTTOM, L4_TOP);
            this.L5 = new Bound(L5_BOTTOM, L5_TOP);
        }

        public bool CheckAngles(double[] angles)
        {
            return L1.Check(angles[0]) &&
                   L2.Check(angles[1]) &&
                   L3.Check(angles[2]) &&
                   L4.Check(angles[3]) &&
                   L5.Check(angles[4]);
        }

        public bool CheckAngles(Angle3D angles)
        {
            return this.CheckAngles(new double[] {
                  MathUtils.RadiansToAngle(angles.O1),
                  MathUtils.RadiansToAngle(angles.O2),
                  MathUtils.RadiansToAngle(angles.O3),
                  MathUtils.RadiansToAngle(angles.O4),
                  MathUtils.RadiansToAngle(angles.O5)});
        }        
    }
}

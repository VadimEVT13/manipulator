using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using static InverseTest.GUI.Model.MovementPoint;

namespace InverseTest.GUI.Model
{
    public delegate void PositoinHandler(Point3D position);

    public enum PointState
    {
        ENLAGE,
        DEFAULT
    }
}

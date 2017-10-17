using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Manipulator
{
    public interface IManipulatorPart
    {
        void RotateTransform3D(Transform3D transform);

        Model3D GetModel();

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Manipulator
{
    public class JointsChain
    {
        public NewJoint[] Joints { get; set; }

        public JointsChain(NewJoint[] joints)
        {
            Joints = joints;
        }
    }
}

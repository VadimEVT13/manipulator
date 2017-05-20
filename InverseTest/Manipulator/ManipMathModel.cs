using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Manipulator
{
    public class ManipMathModel
    {
        public List<Joint> Joints { get; set; }

        public ManipMathModel(Joint[] joints)
        {
            Joints = new List<Joint>();
            foreach (Joint joint in joints)
            {
                Joints.Add(joint);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace InverseTest.Manipulator
{
    class ManipulatorPart : IManipulatorPart
    {
        Model3D part;
        private Transform3DGroup transforms;
        
        public ManipulatorPart(Model3D part)
        {
            this.part = part;
        }

        public Model3D GetModel()
        {
            return part;
        }

        public void RotateTransform3D(Transform3D transform)
        {           
            this.part.Transform = transform;                 
        }        
    }
}

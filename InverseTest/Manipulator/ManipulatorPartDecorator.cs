using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace InverseTest.Manipulator
{
    class ManipulatorPartDecorator : IManipulatorPart
    {
        Model3D part;
        IManipulatorPart dependentPart;
        private Transform3DGroup transforms;
        private RotateTransform3D rotate;
        
        public ManipulatorPartDecorator(Model3D part, IManipulatorPart dependentPart)
        {
            this.part = part;
            this.dependentPart = dependentPart;
            transforms = new Transform3DGroup();
            rotate = new RotateTransform3D();
            transforms.Children.Add(rotate);
            this.part.Transform = transforms;
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

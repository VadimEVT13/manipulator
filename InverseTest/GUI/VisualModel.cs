using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.GUI
{
    public class VisualModel
    {
        public Visual3D camManip;
        public Visual3D camPortal;
        public Visual3D top;
        public Visual3D right;
        public Visual3D front;
        public Visual3D _3d;

        public VisualModel(Model3D model, bool camDisplayed = true)
        {
            if (camDisplayed)
            {
                camManip = new ModelVisual3D() { Content = model };
                camPortal = new ModelVisual3D() { Content = model };
            }

            this.top = new ModelVisual3D() { Content = model };
            this.right= new ModelVisual3D() { Content = model };
            this.front = new ModelVisual3D() { Content = model };
            this._3d = new ModelVisual3D() { Content = model };

        }


    }
}

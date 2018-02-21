using InverseTest.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Manipulator
{
    public class ManipulatorVisual:IVisualController
    {
        Dictionary<ManipulatorParts, VisualModel> visualParts;

        public ManipulatorVisual(ManipulatorV2 manipulator)
        {



        }

        public List<VisualModel> GetVisuals()
        {
            return visualParts.Values.ToList();
        }
    }
}

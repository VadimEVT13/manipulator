using InverseTest.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace InverseTest.Manipulator
{
    public class ManipulatorVisual : IVisualController
    {
        Dictionary<ManipulatorParts, VisualModel> visualParts;

        public ManipulatorVisual(Dictionary<ManipulatorParts, VisualModel> visual)
        {
            this.visualParts = visual;
        }

        public List<VisualModel> Visuals => visualParts.Values.ToList();

        public void ChangePartsColor(List<ManipulatorParts> parts)
        {
            foreach (KeyValuePair<ManipulatorParts, VisualModel> pair in visualParts)
            {
                if (parts.Contains(pair.Key))
                    pair.Value.SetCollisionCollor();
                else pair.Value.SetDefaultColor();
            }
        }


    }
}

using InverseTest.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using InverseTest.GUI.Models;

namespace InverseTest.Manipulator
{
    public class ManipulatorVisual : IVisualController
    {
        Dictionary<ManipulatorParts, MainVisual> visualParts;

        public ManipulatorVisual(Dictionary<ManipulatorParts, MainVisual> visual)
        {
            this.visualParts = visual;
        }

        public List<MainVisual> Visuals => visualParts.Values.ToList();

        public void ChangePartsColor(List<ManipulatorParts> parts)
        {
            foreach (KeyValuePair<ManipulatorParts, MainVisual> pair in visualParts)
            {
                if (parts.Contains(pair.Key))
                    pair.Value.SetCollisionCollor();
                else pair.Value.SetDefaultColor();
            }
        }

        public void BuildShell(List<ManipulatorParts> parts) {
            foreach (KeyValuePair<ManipulatorParts, MainVisual> pair in visualParts)
            {
                if (parts.Contains(pair.Key))
                    pair.Value.BuildShell();
                else pair.Value.removeShell();
            }
        }

    }
}

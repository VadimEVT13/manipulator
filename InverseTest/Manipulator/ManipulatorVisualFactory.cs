using InverseTest.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Manipulator
{
    public class ManipulatorVisualFactory
    {
        public static ManipulatorVisual CreateManipulator(IManipulatorModel manipulator)
        {
            Dictionary<ManipulatorParts, MainVisual> parts = new Dictionary<ManipulatorParts, MainVisual>();
            MainVisual cam = new MainVisual( manipulator.GetManipulatorPart(ManipulatorParts.Camera));
            MainVisual camBase = new MainVisual(manipulator.GetManipulatorPart(ManipulatorParts.CameraBase));
            MainVisual top = new MainVisual(manipulator.GetManipulatorPart(ManipulatorParts.TopEdge));
            MainVisual mid = new MainVisual(manipulator.GetManipulatorPart(ManipulatorParts.MiddleEdge));
            MainVisual table = new MainVisual(manipulator.GetManipulatorPart(ManipulatorParts.Table));
            MainVisual platf = new MainVisual(manipulator.GetManipulatorPart(ManipulatorParts.Platform));
            parts.Add(ManipulatorParts.Camera, cam);
            parts.Add(ManipulatorParts.CameraBase, camBase);

            parts.Add(ManipulatorParts.MiddleEdge, mid);
            parts.Add(ManipulatorParts.TopEdge, top);
            parts.Add(ManipulatorParts.Table, table);
            parts.Add(ManipulatorParts.Platform, platf);

            return new ManipulatorVisual(parts);
        }
    }
}

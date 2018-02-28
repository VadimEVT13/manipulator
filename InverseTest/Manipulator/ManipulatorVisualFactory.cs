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
            Dictionary<ManipulatorParts, VisualModel> parts = new Dictionary<ManipulatorParts, VisualModel>();
            VisualModel cam = new VisualModel( manipulator.GetManipulatorPart(ManipulatorParts.Camera));
            VisualModel camBase = new VisualModel(manipulator.GetManipulatorPart(ManipulatorParts.CameraBase));
            VisualModel top = new VisualModel(manipulator.GetManipulatorPart(ManipulatorParts.TopEdge));
            VisualModel mid = new VisualModel(manipulator.GetManipulatorPart(ManipulatorParts.MiddleEdge));
            VisualModel table = new VisualModel(manipulator.GetManipulatorPart(ManipulatorParts.Table));
            VisualModel platf = new VisualModel(manipulator.GetManipulatorPart(ManipulatorParts.Platform));
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

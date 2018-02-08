using InverseTest.Collision.Model;
using InverseTest.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Collision.Mappers
{
    class ManipulatorMapper
    {
        public static ManipulatorSnapshot ManipulatorToSnapshot(ManipulatorV2 manip) {
            var shapes = new Dictionary<ManipulatorV2.ManipulatorParts, PartShape>();

            var manipupParts = manip.parts;

            foreach(KeyValuePair<ManipulatorV2.ManipulatorParts, IManipulatorPart> part in manipupParts)
            {
                shapes.Add(part.Key, Utils.ExtractShapeFromModel(part.Value.GetModel()));
            }
            
            return new ManipulatorSnapshot(shapes);
        }

    }
}

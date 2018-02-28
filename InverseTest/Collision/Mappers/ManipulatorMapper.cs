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
            var shapes = new List<PartShape>();

            var manipupParts = manip.parts;

            foreach(KeyValuePair<ManipulatorParts, IManipulatorPart> part in manipupParts)
            {
                shapes.Add(Utils.ExtractShapeFromModel(part.Key,part.Value.GetModel()));
            }
            
            return new ManipulatorSnapshot(shapes);
        }

    }
}

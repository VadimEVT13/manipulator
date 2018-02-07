using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Collision.Model
{
   public class ManipulatorSnapshot
    {
        public Dictionary<ManipulatorV2.ManipulatorParts, PartShape> bounds { get; }

        public ManipulatorSnapshot(Dictionary<ManipulatorV2.ManipulatorParts, PartShape> bounds)
        {
            this.bounds = bounds;
        }
    }
}

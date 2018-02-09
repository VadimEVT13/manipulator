using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Collision.Model
{
    public class PortalSnapshot
    {
        public List<PartShape> parts { get;}

        public PortalSnapshot(List<PartShape> bounds)
        {
            this.parts = bounds;
        }
    }
}

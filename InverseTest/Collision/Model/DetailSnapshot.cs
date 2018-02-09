using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Collision.Model
{
    public class DetailSnapshot
    {
        public PartShape detailShape;
        public string name { get { return "Detail"; } }

        public DetailSnapshot(PartShape shape)
        {
            this.detailShape = shape;
        }
    }
}

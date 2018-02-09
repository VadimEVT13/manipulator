using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Collision.Model
{
    public class DetailPlatformSnapshot
    {
        public PartShape detailPlatformShape;
        public string name { get { return "DetailPlatform"; } }

        public DetailPlatformSnapshot(PartShape shape)
        {
            this.detailPlatformShape = shape;
        }
    }
}

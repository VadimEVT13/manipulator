using InverseTest.Collision.Model;
using InverseTest.Detail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Collision.Mappers
{
    class DetailPlatformMapper
    {
        public static DetailPlatformSnapshot DetailPlatformToSnapshot(Model3D detailPlatform)
        {
            return new DetailPlatformSnapshot(Utils.ExtractShapeFromModel(detailPlatform));
        }
    }
}

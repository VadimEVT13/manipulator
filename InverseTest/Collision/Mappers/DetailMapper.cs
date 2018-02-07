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
    public class DetailMapper
    {
        public static DetailSnapshot DetailToSnapshot(DetailModel detail)
        {
            return new DetailSnapshot(Utils.ExtractShapeFromModel(detail.GetModel()));
        }
    }
}

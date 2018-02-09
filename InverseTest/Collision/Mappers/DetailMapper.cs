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
        public const string detailName = "Detail";

        public static DetailSnapshot DetailToSnapshot(DetailModel detail)
        {
            return new DetailSnapshot(Utils.ExtractShapeFromModel(detailName, detail.GetModel()));
        }
    }
}

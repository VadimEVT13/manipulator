using InverseTest.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Detail
{
    public class DetailVisualFactory
    {
            public static DetailVisualCollisionController CreateDetailVisual(DetailModel detail, Model3D platform)
            {
                MainVisual detailVis = detail.detailVisual;
                MainVisual platformVis = new MainVisual(platform);

                return new DetailVisualCollisionController(detailVis, platformVis);
            }
        }
    
}

using InverseTest.Collision.Model;
using InverseTest.Detail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Collision.Mappers
{
    class SceneMapper
    {
        public static SceneSnapshot CreateSceneSnapshot(ManipulatorV2 manip, DetectorFrame portal, DetailModel detail)
        {
            ManipulatorSnapshot manipSnapshot = ManipulatorMapper.ManipulatorToSnapshot(manip);
            PortalSnapshot portalSnapshot = PortalMapper.PortalToSnapshot(portal);
            DetailSnapshot detailSnapshot = DetailMapper.DetailToSnapshot(detail);

            return new SceneSnapshot(manipSnapshot, portalSnapshot, detailSnapshot);
        }
    }
}

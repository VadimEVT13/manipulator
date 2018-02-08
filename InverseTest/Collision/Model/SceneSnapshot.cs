using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Collision.Model
{
   public class SceneSnapshot
    {
        public ManipulatorSnapshot manipSnapshot { get; }
        public PortalSnapshot portalSnapshot { get; }
        public DetailSnapshot detailSnapshot { get; }

        public SceneSnapshot(ManipulatorSnapshot manipSnapshot, PortalSnapshot portalSnapshot, DetailSnapshot detail)
        {
            this.manipSnapshot = manipSnapshot;
            this.portalSnapshot = portalSnapshot;
            this.detailSnapshot = detail;
        }

    }
}

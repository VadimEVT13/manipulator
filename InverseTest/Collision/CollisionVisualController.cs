using InverseTest.Collision.Model;
using InverseTest.Frame;
using InverseTest.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Collision
{
    class CollisionVisualController
    {
        private ManipulatorVisual manipulator;
        private DetectorFrameVisual portal;


        public CollisionVisualController(ManipulatorVisual manip, DetectorFrameVisual portal)
        {
            this.manipulator = manip;
            this.portal = portal;
        }

        public void Collisions(List<CollisionPair> collisions)
        {
            List<ManipulatorParts> manipParts = new List<ManipulatorParts>();
            List<DetectorFrame.Parts> portalParts = new List<DetectorFrame.Parts>();
            foreach (CollisionPair c in collisions)
            {
                if(c.modelCollision1.type is ManipulatorParts mt)
                {
                    manipParts.Add(mt);
                }else if( c.modelCollision1.type is DetectorFrame.Parts dt)
                {
                    portalParts.Add(dt);
                }

                if (c.modelCollision2.type is ManipulatorParts mt2)
                {
                    manipParts.Add(mt2);
                }
                else if (c.modelCollision2.type is DetectorFrame.Parts dt2)
                {
                    portalParts.Add(dt2);
                }

            }


            this.manipulator.ChangePartsColor(manipParts);
            this.portal.ChangePartsColor(portalParts);
        }



    }
}

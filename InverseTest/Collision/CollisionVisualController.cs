using InverseTest.Collision.Model;
using InverseTest.Detail;
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
        public ManipulatorVisual manipulator;
        public DetectorFrameVisual portal;
        public DetailVisualCollisionController Detail { get; set; } 

        public CollisionVisualController(ManipulatorVisual manip, DetectorFrameVisual portal, DetailVisualCollisionController detail)
        {
            this.manipulator = manip;
            this.portal = portal;
            this.Detail = detail;
        }

        public void Collisions(List<CollisionPair> collisions)
        {
            List<ManipulatorParts> manipParts = new List<ManipulatorParts>();
            List<DetectorFrame.Parts> portalParts = new List<DetectorFrame.Parts>();
            List<ExtraPartsEnum> detailParts = new List<ExtraPartsEnum>();

            foreach (CollisionPair c in collisions)
            {
                if(c.ModelCollision1.type is ManipulatorParts mt)
                {
                    manipParts.Add(mt);
                }else if( c.ModelCollision1.type is DetectorFrame.Parts dt)
                {
                    portalParts.Add(dt);
                }
                else if (c.ModelCollision1.type is ExtraPartsEnum dp)
                {
                    detailParts.Add(dp);
                }

                if (c.ModelCollision2.type is ManipulatorParts mt2)
                {
                    manipParts.Add(mt2);
                }
                else if (c.ModelCollision2.type is DetectorFrame.Parts dt2)
                {
                    portalParts.Add(dt2);
                }
                else if (c.ModelCollision2.type is ExtraPartsEnum dp2)
                {
                    detailParts.Add(dp2);
                }

               
            }

            this.Detail.ChangePartsColor(detailParts);
            this.manipulator.ChangePartsColor(manipParts);
            this.portal.ChangePartsColor(portalParts);

            //this.Detail.BuildShell(detailParts);
            //this.manipulator.BuildShell(manipParts);
            //this.portal.BuildShell(portalParts);

        }



    }
}

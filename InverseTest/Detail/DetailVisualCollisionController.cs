using InverseTest.Collision.Model;
using InverseTest.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Detail
{
    public class DetailVisualCollisionController:IVisualController
    {
        MainVisual detail;
        MainVisual platform;

        public DetailVisualCollisionController(MainVisual det, MainVisual plat)
        {
            this.detail = det;
            this.platform = plat;
        }

        public List<MainVisual> Visuals => new List<MainVisual>() { detail, platform };

        public void ChangePartsColor(List<ExtraPartsEnum> parts)
        {
            resetColors();
            foreach(ExtraPartsEnum en in parts)
            {
                if (en == ExtraPartsEnum.DETAIL)
                    this.detail.SetCollisionCollor();
                else if (en == ExtraPartsEnum.DETAIL_PLATFORM)
                    this.platform.SetCollisionCollor();
                else resetColors();
            }
        }

        private void resetColors()
        {
            this.detail.SetDefaultColor();
            this.platform.SetDefaultColor();
        }
    }
}

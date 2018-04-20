using InverseTest.Detail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest
{
    class ModelParser
    {
        
        public ManipulatorV2 Manipulator { get; set; }
        public DetailModel LopatkaDetail { get; set; }
        public DetailModel ShpangoutDetail { get; set; }
        public DetectorFrame Frame { get; set; }
        public Model3DGroup Others { get; set; }
        public Model3DGroup DetailPlatform { get; set;}

        private Model3DGroup scene;

        public ModelParser(Model3DGroup scene)
        {
            this.scene = scene;
        }
        
        public void Parse()
        {
            Model3DGroup allModels = scene;
            Model3DGroup portal = new Model3DGroup();
            portal.Children = new Model3DCollection(allModels.Children.ToList().GetRange(17,16));
            portal.Children.Add(allModels.Children[36]);
            portal.Children.Add(allModels.Children[37]);

            Frame = new DetectorFrame(portal);

            //Определяем модельку манипулятора
            Model3DGroup manipulatorGroup = new Model3DGroup();
            Model3DCollection manipulatorChildrens = new Model3DCollection();
            manipulatorChildrens.Add(allModels.Children[2]);
            manipulatorChildrens.Add(allModels.Children[3]);
            manipulatorChildrens.Add(allModels.Children[9]);
            manipulatorChildrens.Add(allModels.Children[10]);
            manipulatorChildrens.Add(allModels.Children[11]);
            manipulatorChildrens.Add(allModels.Children[12]);
            manipulatorChildrens.Add(allModels.Children[13]);
            manipulatorChildrens.Add(allModels.Children[14]);
            manipulatorChildrens.Add(allModels.Children[15]);
            manipulatorChildrens.Add(allModels.Children[16]);
            //Точки вращения
            manipulatorChildrens.Add(allModels.Children[1]);
            manipulatorChildrens.Add(allModels.Children[33]);
            manipulatorChildrens.Add(allModels.Children[34]);
            manipulatorChildrens.Add(allModels.Children[35]);
            manipulatorGroup.Children = manipulatorChildrens;
            //Точки в узлах манипулятора. Каждая точка это кубик 1 на 1 на 1
           Manipulator = new ManipulatorV2(manipulatorGroup);

            Model3D lopatka = allModels.Children[8];
            LopatkaDetail = new DetailModel(lopatka);

            Model3D shpangout = allModels.Children[0];
            this.ShpangoutDetail = new DetailModel(shpangout);

            //Добавляем остальные мешы
            Model3DGroup others = new Model3DGroup();
            others.Children = new Model3DCollection();
            this.Others = others;

            this.DetailPlatform = new Model3DGroup();
            this.DetailPlatform.Children.Add(allModels.Children[4]);
            this.DetailPlatform.Children.Add(allModels.Children[5]);
            this.DetailPlatform.Children.Add(allModels.Children[6]);
            this.DetailPlatform.Children.Add(allModels.Children[7]);
        }
    }
}

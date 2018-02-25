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

        private const int PORTAL_START_INDEX = 29;
        private const int PORTAL_END_INDEX = 59;
        private const int MANIPULATOR_START_INDEX = 9;
        private const int MANIPULATOR_END_INDEX = 29;
        private const int LOPATKA_INDEX = 8;


        public ManipulatorV2 Manipulator { get; set; }
        public DetailModel Detail { get; set; }
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
            portal.Children = new Model3DCollection(allModels.Children.ToList().GetRange(16,16));
            portal.Children.Add(allModels.Children[37]);
            portal.Children.Add(allModels.Children[36]);

            Frame = new DetectorFrame(portal);

            //Определяем модельку манипулятора
            Model3DGroup manipulatorGroup = new Model3DGroup();
            Model3DCollection manipulatorChildrens = new Model3DCollection();
            manipulatorChildrens.Add(allModels.Children[1]);
            manipulatorChildrens.Add(allModels.Children[6]);
            manipulatorChildrens.Add(allModels.Children[7]);
            manipulatorChildrens.Add(allModels.Children[8]);
            manipulatorChildrens.Add(allModels.Children[9]);
            manipulatorChildrens.Add(allModels.Children[10]);
            manipulatorChildrens.Add(allModels.Children[11]);
            manipulatorChildrens.Add(allModels.Children[12]);
            manipulatorChildrens.Add(allModels.Children[13]);
            manipulatorChildrens.Add(allModels.Children[14]);
            manipulatorChildrens.Add(allModels.Children[15]);
            //Точки вращения
            manipulatorChildrens.Add(allModels.Children[33]);
            manipulatorChildrens.Add(allModels.Children[32]);
            manipulatorChildrens.Add(allModels.Children[0]);
            manipulatorChildrens.Add(allModels.Children[34]);




            manipulatorGroup.Children = manipulatorChildrens;
            //Точки в узлах манипулятора. Каждая точка это кубик 1 на 1 на 1
         
            Manipulator = new ManipulatorV2(manipulatorGroup);

            Model3D lopatka = allModels.Children[5];
            Detail = new DetailModel(lopatka);

            //Добавляем остальные мешы
            Model3DGroup others = new Model3DGroup();
            others.Children = new Model3DCollection();
            this.Others = others;

            this.DetailPlatform = new Model3DGroup();
            this.DetailPlatform.Children.Add(allModels.Children[2]);
            this.DetailPlatform.Children.Add(allModels.Children[3]);
            this.DetailPlatform.Children.Add(allModels.Children[4]);
            this.DetailPlatform.Children.Add(allModels.Children[35]);
        }
    }
}
;
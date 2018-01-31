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


        public ManipulatorV2 manipulator { get; set; }
        public DetailModel detail { get; set; }
        public DetectorFrame frame { get; set; }
        public Model3DGroup others { get; set; }

        private Model3DGroup scene;

        public ModelParser(Model3DGroup scene)
        {
            this.scene = scene;
        }

        public void Parse()
        {
            Model3DGroup allModels = scene;
            Model3DGroup portal = new Model3DGroup();
            portal.Children = new Model3DCollection(allModels.Children.ToList().GetRange(PORTAL_START_INDEX, PORTAL_END_INDEX - PORTAL_START_INDEX));
            portal.Children.Add(allModels.Children[62]);
            frame = new DetectorFrame(portal);

            //Определяем модельку манипулятора
            Model3DGroup manipulatorGroup = new Model3DGroup();
            manipulatorGroup.Children = new Model3DCollection(allModels.Children.ToList()
             .GetRange(MANIPULATOR_START_INDEX, MANIPULATOR_END_INDEX - MANIPULATOR_START_INDEX));

            //Точки в узлах манипулятора. Каждая точка это кубик 1 на 1 на 1
            manipulatorGroup.Children.Add(allModels.Children[0]);
            manipulatorGroup.Children.Add(allModels.Children[59]);
            manipulatorGroup.Children.Add(allModels.Children[60]);
            manipulatorGroup.Children.Add(allModels.Children[61]);
            manipulator = new ManipulatorV2(manipulatorGroup);

            Model3D lopatka = allModels.Children[LOPATKA_INDEX];
            detail = new DetailModel(lopatka);

            //Добавляем остальные мешы
            Model3DGroup others = new Model3DGroup();
            List<Model3D> othersModels = new List<Model3D>();
            othersModels.AddRange(allModels.Children.ToList().GetRange(0, LOPATKA_INDEX-1));
            othersModels.AddRange(allModels.Children.ToList().GetRange(LOPATKA_INDEX + 1, MANIPULATOR_START_INDEX - (LOPATKA_INDEX-1)));
            others.Children = new Model3DCollection(othersModels);
            this.others = others;
        }
    }
}
;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace InverseTest.Detail
{

    /// <summary>
    /// Класс для представления трехмерной детали которую будут сканировать
    /// </summary>
    class DetailModel
    {
        private readonly Model3D detailModel;

        public DetailModel(Model3D detailModel)
        {
            this.detailModel = detailModel;

        }

        public Model3D GetModel()
        {
            return detailModel;
        }

        public void SetJunctionsPoints(int[] indexes)
        {
            ((GeometryModel3D)detailModel).Material = new MaterialGroup();
            DiffuseMaterial material = new DiffuseMaterial(new RadialGradientBrush(Colors.Red, Colors.Transparent));
        }


    }
}

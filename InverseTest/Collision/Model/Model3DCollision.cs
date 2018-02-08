using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Collision.Model
{

    /// <summary>
    /// Структура для хранения точек меша. Подается на вход Алгоритма GJK
    /// </summary>
    public class Model3DCollision
    {
        public String meshName;
        public Model3DGroup model{ get; }

        public Model3DCollision(string modelName, Model3DGroup modelGroup)
        {
            this.meshName = modelName;
            this.model = modelGroup;
        }


    }
}

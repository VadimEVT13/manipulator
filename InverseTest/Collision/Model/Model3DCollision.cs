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
        public Enum type { get; }
        public PartShape shape{ get; }

        public Model3DCollision(Enum modelName, PartShape shape)
        {
            this.type = modelName;
            this.shape = shape;
        }
    }
}

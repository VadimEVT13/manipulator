using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Collision.Model
{
    public class CollisionPair
    {
        public Model3DCollision ModelCollision1 { get; }
        public Model3DCollision ModelCollision2 { get; }

        public CollisionPair(Model3DCollision m1, Model3DCollision m2)
        {
            this.ModelCollision1 = m1;
            this.ModelCollision2 = m2;
        }
    }
}

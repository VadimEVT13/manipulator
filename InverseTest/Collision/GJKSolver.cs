using InverseTest.Collision.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Collision
{
    class GJKSolver
    {

        public GJKSolver()
        {

        } 

        public bool IntersectGJK(CollisionPair pair) // отправка данных на GJK
        {
            HullsV2 hull = new HullsV2();

            hull.BuildShell(pair.modelCollision1.model);
            hull.BuildShell(pair.modelCollision2.model);

            return hull.find();  //поиск пересечений GJK
            
        }
    }
}

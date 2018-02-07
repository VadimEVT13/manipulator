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

            var shell1 = hull.BuildShell(pair.modelCollision1);
            var shell2 = hull.BuildShell(pair.modelCollision2);

            return hull.find(shell1, shell2);  //поиск пересечений GJK            
        }
    }
}

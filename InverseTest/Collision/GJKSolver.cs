using InverseTest.Collision.Model;
using InverseTest.GUI;
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

        public bool IntersectGJK(CollisionPair pair, SimplexView simplex) // отправка данных на GJK
        {
            HullsV2 hull = new HullsV2();

            var shell1 = hull.BuildShell(pair.ModelCollision1);
            var shell2 = hull.BuildShell(pair.ModelCollision2);

            var find = hull.find(shell1, shell2, simplex);  //поиск пересечений GJK            
            return find;
        }
    }
}

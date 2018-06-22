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

        public bool IntersectGJK(CollisionPair pair) // отправка данных на GJK
        {
            HullsV2 hull = new HullsV2();
            //expandedAABB exAABB = new expandedAABB();

            var shell1 = hull.BuildShell(pair.ModelCollision1);
            var shell2 = hull.BuildShell(pair.ModelCollision2);
            
            var findGJK = hull.find(shell1, shell2);  //поиск пересечений GJK     

            //var findAABBex = exAABB.find(pair.ModelCollision1.shape, pair.ModelCollision2.shape);
            return findGJK;
        }
    }
}

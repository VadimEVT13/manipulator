using InverseTest.Collision.Model;
using InverseTest.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InverseTest.Collision.AABB;

namespace InverseTest.Collision
{
    public delegate void OnCollisionDetected(CollisionPair collision);


    class CollisionDetector
    {
        private AABB aabb;
        private GJKWorker<CollisionPair> worker;

        public event OnCollisionDetected OnCollision;

        public CollisionDetector(AABB aabb, GJKWorker<CollisionPair> worker)
        {
            this.aabb = aabb;
            this.worker = worker;
            this.worker.onCollision += OnCollision;
        }

        public void FindCollisoins()
        {
            List<CollisionPair> pairs = aabb.Find();

            foreach (CollisionPair p in pairs)
            {                
                worker.findCollision(p);
            }
        }


    }
}

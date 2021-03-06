﻿using InverseTest.Collision;
using InverseTest.Collision.Model;
using InverseTest.Detail;
using InverseTest.GUI;
using InverseTest.Manipulator;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using static InverseTest.Collision.AABB;

namespace InverseTest.Workers
{

    class GJKWorker<T,L> : BackroundCalculations<T,L> where L:class
    {
        private GJKSolver solver;
        private AABB aabb;

        public GJKWorker(AABB aabb, GJKSolver solver, int queueSize = 10) : base(queueSize)
        {
            this.solver = solver;
            this.aabb = aabb;
        }
        
        protected override L Calculate(T elem)
        {
            SceneSnapshot scs = elem as SceneSnapshot;

            Queue<CollisionPair> pairs = new Queue<CollisionPair> (aabb.Find(scs));
            List<CollisionPair> collisions = new List<CollisionPair>();

            if (pairs.Count > 0)
            {
                while(pairs.Count>0)
                {
                    CollisionPair pair = pairs.Dequeue();

                  
                        if (solver.IntersectGJK(pair))
                        {
                            collisions.Add(pair);
                        }
                    }
                    
                            }
            else
            {
                return new List<CollisionPair>() as L;
            }

            
            return collisions as L;
        }
    }
}

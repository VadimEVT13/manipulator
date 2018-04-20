using InverseTest.Collision.Mappers;
using InverseTest.Collision.Model;
using InverseTest.Detail;
using InverseTest.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using static InverseTest.Collision.AABB;

namespace InverseTest.Collision
{
    public delegate void OnCollisionDetected(List<CollisionPair> collision);

    class CollisionDetector
    {

        ManipulatorV2 Manipulator;
        DetectorFrame Portal;
        public DetailModel Detail { get; set; }
        Model3D Platform;

        private GJKWorker<SceneSnapshot, List<CollisionPair>> worker;

        public event OnCollisionDetected OnCollision;

        public CollisionDetector(ManipulatorV2 Manipulator,
            DetectorFrame DetectorFrame,
            DetailModel Detail,
            Model3D Platform,
            GJKWorker<SceneSnapshot, List<CollisionPair>> worker)
        {
            this.Manipulator = Manipulator;
            this.Portal = DetectorFrame;
            this.Detail = Detail;
            this.Platform = Platform;

            this.worker = worker;
        }

        public void FindCollisoins()
        {
            SceneSnapshot sc = SceneMapper.CreateSceneSnapshot(this.Manipulator, this.Portal, this.Detail, this.Platform);
            worker.Solve(sc);
        }
    }
}

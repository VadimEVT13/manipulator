﻿using InverseTest.Collision.Model;
using InverseTest.Detail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Collision.Mappers
{
    class SceneMapper
    {
        public static SceneSnapshot CreateSceneSnapshot(ManipulatorV2 manip, DetectorFrame portal, DetailModel detail, Model3D detailPlatform)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();

            ManipulatorSnapshot manipSnapshot = ManipulatorMapper.ManipulatorToSnapshot(manip);
            PortalSnapshot portalSnapshot = PortalMapper.PortalToSnapshot(portal);
            DetailSnapshot detailSnapshot = DetailMapper.DetailToSnapshot(detail);
            DetailPlatformSnapshot detailPlatformSnapshot = DetailPlatformMapper.DetailPlatformToSnapshot(detailPlatform);
            watch.Stop();
            Console.WriteLine("Mapping:" + watch.ElapsedMilliseconds);

            return new SceneSnapshot(manipSnapshot, portalSnapshot, detailSnapshot, detailPlatformSnapshot);
        }
    }
}

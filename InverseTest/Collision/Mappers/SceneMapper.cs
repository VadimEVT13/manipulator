using InverseTest.Collision.Model;
using InverseTest.Detail;
using NLog;
using System.Windows.Media.Media3D;

namespace InverseTest.Collision.Mappers
{
    class SceneMapper
    {
        /// <summary>
        /// Логгирование
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static SceneSnapshot CreateSceneSnapshot(ManipulatorV2 manip, DetectorFrame portal, DetailModel detail, Model3D detailPlatform)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();

            ManipulatorSnapshot manipSnapshot = ManipulatorMapper.ManipulatorToSnapshot(manip);
            PortalSnapshot portalSnapshot = PortalMapper.PortalToSnapshot(portal);
            DetailSnapshot detailSnapshot = DetailMapper.DetailToSnapshot(detail);
            DetailPlatformSnapshot detailPlatformSnapshot = DetailPlatformMapper.DetailPlatformToSnapshot(detailPlatform);
            watch.Stop();
            logger.Debug("Mapping:" + watch.ElapsedMilliseconds);

            return new SceneSnapshot(manipSnapshot, portalSnapshot, detailSnapshot, detailPlatformSnapshot);
        }
    }
}

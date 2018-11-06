using InverseTest.Detail;
using InverseTest.Frame;
using InverseTest.Path;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.GUI.ViewModels
{
    public class DetailViewModel
    {
        /// <summary>
        /// Модель сканируемой детали
        /// </summary>
        public DetailModel Detail { get; set; }

        /// <summary>
        /// Путь сканирования состоящий из точек
        /// </summary>
        public ScanPath Path { get; set; }

        /// <summary>
        /// Поворот детали и перемещение детали
        /// </summary>
        /// <param name="angle">угол поворота в градусах</param>
        /// <param name="height">высота в мм</param>
        public void Transform(double angle, double height)
        {
            Transform3DGroup transforms = new Transform3DGroup();
            transforms.Children = new Transform3DCollection()
            {
                new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), angle)),
                new TranslateTransform3D(0, DetectorPositionController.MmToSm(height), 0)
            };
            Path.TransformPoint(transforms);
            Detail.Transform(transforms);
        }
    }
}

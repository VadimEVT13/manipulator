using InverseTest.Detail;
using InverseTest.Path;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest
{
    public class DetailPathController
    {

        /// <summary>
        /// Модель сканируемой детали
        /// </summary>
        public DetailModel Detail { get; set; }

        /// <summary>
        /// Путь сконирования состоящий из точек
        /// </summary>
        public ScanPath Path;

        public DetailPathController(DetailModel detail, ScanPath path)
        {
            this.Detail = detail;
            this.Path = path;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle">Угол поворота в градусах</param>
        public void Rotate(double angle)
        {
            RotateTransform3D transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), angle));
            Path.TransformPoint(transform);
            Detail.Transform(transform);
        }

        /// <summary>
        /// Перемещает деталь и точки пути на ней.
        /// </summary>
        /// <param name="transform"></param>
        public void Transform(Transform3D transform)
        {
            Path.TransformPoint(transform);
            Detail.Transform(transform);
        }
    }
}

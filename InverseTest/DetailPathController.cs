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
        public DetailModel detail;
        public ScanPath path;

        public DetailPathController(DetailModel detail, ScanPath path)
        {
            this.detail = detail;
            this.path = path;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle">Угол поворота в градусах</param>
        public void Rotate(double angle)
        {
            RotateTransform3D transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), angle));
            path.TransformPoint(transform);
            detail.Transform(transform);
        }
    }
}

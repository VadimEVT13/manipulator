using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.GUI.Model
{
    interface IConeModel:IModel
    {
        void ChangePosition(Point3D origin, Vector3D directoin);
    }
}

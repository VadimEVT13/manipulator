using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using static InverseTest.GUI.Model.ScanPoint;

namespace InverseTest.GUI.Model
{
    public delegate void PositoinHandler(Point3D position);
    
    public interface IScanPoint:IModel
    {
        event PositoinHandler PositoinChanged;

        void MoveToPositoin(Point3D newPositoin);

        Point3D GetTargetPoint();
        
        void ChangeSize(double scale);
    }
}

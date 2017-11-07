using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using static InverseTest.ManipulatorV2;

namespace InverseTest.Manipulator
{
    
    public interface IManipulatorModel
    {
        /// <summary>
        /// Возвращает 3D модель манипулятора
        /// </summary>
        /// <returns>3D модель манипулятора</returns>
        /// 
               

        Model3D GetManipulatorModel();

        Model3D GetManipulatorPart(ManipulatorParts part);

        void RotatePart(ManipulatorParts part, double angle);

        Vector3D GetCameraDirection();

        Point3D GetCameraPosition();

        void MoveManipulator(ManipulatorAngles angles, bool animate);
    }
}

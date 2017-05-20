using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Manipulator
{
    
    public interface IManipulatorModel
    {
        /// <summary>
        /// Возвращает 3D модель манипулятора
        /// </summary>
        /// <returns>3D модель манипулятора</returns>
        Model3D GetManipulatorModel();
    }
}

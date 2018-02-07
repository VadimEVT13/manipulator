using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Manipulator.Models
{
    /// <summary>
    /// Перечисление всех подвижных ребер манипулятора
    /// </summary>
    public enum ManipulatorParts
    {
        /// <summary>
        /// Камера манипулятора
        /// </summary>
        Camera,

        /// <summary>
        /// Стойка, на которой закреплена камера
        /// </summary>
        CameraBase,

        /// <summary>
        /// Вращающаяся часть верхнего ребра
        /// </summary>
        TopEdge,

        /// <summary>
        /// Поворотная часть стойки
        /// </summary>
        MiddleEdge,

        /// <summary>
        /// Вращающаяся часть нижнего ребра (т.е. столик)
        /// </summary>
        Table
    }
}

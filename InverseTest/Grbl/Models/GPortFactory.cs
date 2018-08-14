using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.Grbl.Models
{
    /// <summary>
    /// Фабрика для создания подключений к последовательному порту
    /// </summary>
    public class GPortFactory
    {
        public enum GPortType
        {
            MANIPULATOR,
            DETECTOR
        }

        private static GDevice detectorSettings = new GDevice
        {
            Name = "Grbl X-ray portal",
            IsC = false,
            IsD = false,
            IsE = false

        };

        private static GDevice manipulatorSettings = new GDevice
        {
            Name = "XManipulator",
            IsY = false,
            IsA = false,
            IsB = false,
            IsC = false,
            IsD = false,
            IsE = false
        };

        /// <summary>
        /// Создает подчклюение к последовательному порту в зависимости от типа
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public GPort CreateGPort(GPortType type)
        {
            if (type == GPortType.MANIPULATOR)
            {
                return createManipulatorGPort();
            }
            else if (type == GPortType.DETECTOR)
            {
                return createDetectorGPort();
            }
            else throw new ArgumentException("No such type of GPort");
        }

        /// <summary>
        /// Создает подключение к порту манипулятора
        /// </summary>
        /// <returns></returns>
        private GPort createManipulatorGPort()
        {
            return new GPort(manipulatorSettings);
        }


        /// <summary>
        /// Создает подключение к порту детектора
        /// </summary>
        /// <returns></returns>
        private GPort createDetectorGPort()
        {
            return new GPort(detectorSettings);
        }

    }
}

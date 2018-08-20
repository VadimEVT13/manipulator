using InverseTest.Path;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.GUI.Readers
{
    public class JsonReader
    {
        /// <summary>
        /// Логгирование
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static List<ScanPoint> Read(string fileName)
        {
            return new List<ScanPoint>();
        }
    }
}

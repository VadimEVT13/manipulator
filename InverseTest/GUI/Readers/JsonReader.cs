using InverseTest.Path;
using log4net;
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
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<ScanPoint> Read(string fileName)
        {
            return new List<ScanPoint>();
        }
    }
}

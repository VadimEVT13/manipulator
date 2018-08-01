using InverseTest.Path;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest.GUI.Writers
{
    public class JsonWriter
    {
        /// <summary>
        /// Логгирование
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Write(List<ScanPoint> pointsList, string fileName)
        {

        }
    }
}

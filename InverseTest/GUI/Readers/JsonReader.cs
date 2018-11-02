using InverseTest.Path;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
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
            return JsonConvert.DeserializeObject<List<ScanPoint>>(fileName);
        }
    }
}

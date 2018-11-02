using InverseTest.Path;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static void Write(List<ScanPoint> pointsList, string fileName)
        {
            string json = JsonConvert.SerializeObject(pointsList, Formatting.Indented);

            // Без дозаписи?
            using (StreamWriter sw = new StreamWriter(fileName, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(json);
            }
        }
    }
}

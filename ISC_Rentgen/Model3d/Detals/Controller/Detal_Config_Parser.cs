using ISC_Rentgen.Model3d.Detals.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISC_Rentgen.Model3d.Detals.Controller
{
    public static class Detal_Config_Parser
    {
        public static void Save_Config(string path)
        {
            string json = JsonConvert.SerializeObject(Detal_Config.getInstance);
            System.IO.File.WriteAllText(path, json);
        }

        public static void Load_Config(string path)
        {
            string json = System.IO.File.ReadAllText(path);            
            Detal_Config.getInstance = JsonConvert.DeserializeObject<Detal_Config>(json);
        }
    }
}

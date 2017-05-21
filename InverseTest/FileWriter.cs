using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace InverseTest
{
    class FileWriter
    {


      public  static void WriteAnglesToFile(String anglesToOut, String filename)
        {
            File.WriteAllText(filename, anglesToOut);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;



namespace InverseTest
{
    class IOFile
    {


      public  static void WriteAnglesToFile(String anglesToOut, String filename)
        {
            File.WriteAllText(filename, anglesToOut);
        }

        public static Model3D loadObjModel(string filename)
        {
            Model3D machine3DModel = new ModelImporter().Load(filename);

            return machine3DModel;
        }


    }
}

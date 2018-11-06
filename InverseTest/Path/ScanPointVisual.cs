using HelixToolkit.Wpf;
using InverseTest.GUI;
using InverseTest.GUI.Models;
using InverseTest.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Path
{
    public class ScanPointVisual : BaseScanPointVisual
    {

        private static Material DEFAULT_MATERIAL = Materials.Green;
        private static Material SELECTED_MATERIAL = Materials.Orange;


        public MainVisual pointVisual { get; }
        
        public ScanPointVisual(ScanPoint point):base(point)
        {
            this.pointVisual = new MainVisual(model);
        }      
    }
}

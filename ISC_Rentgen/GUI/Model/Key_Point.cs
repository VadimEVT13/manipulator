using ISC_Rentgen.Rentgen_Parts.Manipulator_Components.Model;
using ISC_Rentgen.Rentgen_Parts.Portal_Components.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ISC_Rentgen.GUI.Model
{
    public class Key_Point : INotifyPropertyChanged
    {
        private static int static_index = 0;
        public int Index { get { return index; } }
        private int index = 0;

        public Point3D Emitter_point { get; set; }
        public Point3D Scan_point { get; set; }

        public Angles_Manipulator Manipulator_Angle { get; set; }
        public Angles_Portal Portal_Angle { get; set; }

        public bool IsCorrect { get { return iscorrect; } set { iscorrect = value; NotifyPropertyChanged(nameof(iscorrect)); } }
        private bool iscorrect = false;

        public Key_Point(Point3D Emitter_point, Point3D Scan_point)
        {
            this.Emitter_point  = Emitter_point;
            this.Scan_point     = Scan_point;
            //индекс и его инкриментация
            index = static_index;
            static_index++;
        }

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

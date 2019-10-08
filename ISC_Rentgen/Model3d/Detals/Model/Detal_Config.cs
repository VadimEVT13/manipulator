using ISC_Rentgen.GUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ISC_Rentgen.Model3d.Detals.Model
{
    public class Detal_Config
    {
        private static Detal_Config instance;
        public static Detal_Config getInstance { get { if (instance == null) instance = new Detal_Config(); return instance; } set { instance = value; } }

        private Point3D detal_base = new Point3D();
        public Point3D Detal_Base { get { return detal_base; } set { detal_base = value; } }

        private double radius = double.NaN;
        public double Radius { get { return radius; } set { if(value > 0) radius = value; } }

        private string methodic_name = "";
        public string Methodic_name { get { return methodic_name; } set { methodic_name = value; } }

        private List<Position> positions = new List<Position>();
        public List<Position> Positions { get { return positions; } set { positions = value; } }
        
        public List<Position> Key_Point_ListToPosition_List(Key_Point_List kpl)
        {
            List<Position> result = new List<Position>();

            foreach (Key_Point kp in kpl.Points_List)
            {
                result.Add(new Position() { Emitter = kp.Emitter_point, Scan = kp.Scan_point });
            }

            return result;
        }

        public ObservableCollection<Key_Point> Position_ListToKey_Point_List(List<Position> pl)
        {
            ObservableCollection<Key_Point> result = new ObservableCollection<Key_Point>();

            foreach (Position p in pl)
            {
                result.Add(new Key_Point(p.Emitter, p.Scan));
            }

            return result;
        }
    }

    public class Position
    {
        private Point3D emitter = new Point3D();
        public Point3D Emitter { get { return emitter; } set { emitter = value; } }

        private Point3D scan = new Point3D();
        public Point3D Scan { get { return scan; } set { scan = value; } }
    }
}

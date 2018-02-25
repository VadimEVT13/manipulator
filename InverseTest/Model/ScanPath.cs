using InverseTest.GUI.Model;
using InverseTest.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest
{
    public class ScanPath
    {
        private static ScanPath instance;

        private List<ScanPoint> points;
        private static object syncRoot = new Object();

        public ScanPath()
        {
            this.points = new List<ScanPoint>();
        }

        public static ScanPath getInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ScanPath();
                    }
                }
                return instance;
            }
        }

        public void AddPoint(ScanPoint point)
        {
            this.points.Add(point);
        }

        public void RemovePoint(ScanPoint point)
        {
            this.points.Remove(point);
        }
    }
}

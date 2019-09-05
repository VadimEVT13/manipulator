using ISC_Rentgen.Rentgen_Parts.Manipulator_Components.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ISC_Rentgen.Test
{
    public delegate void OnPositionChanged(Angles_Manipulator angle);

    public static class Controller
    {
        public static event OnPositionChanged PositionChanged;
        private static Thread t;

        public static void StartThread()
        {
            //Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate() { DoWork(); });
            DoWork();
            //t = new Thread(DoWork);
            //t.Start();
        }

        public static void DoWork()
        {
            for (int i = 0; i < 10000; i++)
            {
                Random rnd = new Random();
                PositionChanged?.Invoke(new Angles_Manipulator()
                {
                    O1 = rnd.NextDouble() * 180,
                    O2 = rnd.NextDouble() * 180,
                    O3 = rnd.NextDouble() * 180,
                    O4 = rnd.NextDouble() * 180,
                    O5 = rnd.NextDouble() * 180
                });
            }
        }
    }
}

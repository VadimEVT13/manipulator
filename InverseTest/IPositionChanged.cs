using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseTest
{
    public delegate void PositionHandler();

    public delegate void ManualPositionHandler();

   public  interface IPositionChanged
    {
        event PositionHandler onPositionChanged;

        event ManualPositionHandler onManulaPositionChanged;
    }
}

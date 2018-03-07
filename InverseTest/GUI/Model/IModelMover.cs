using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InverseTest.GUI
{
   public interface IModelMover
    {
        void OnMouseDown(object sender, MouseEventArgs e);

        void OnMouseUp(object sender, MouseEventArgs e);

        void OnMouseMove(object sender, MouseEventArgs e);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InverseTest.Detail
{
    interface IDetailMouseListener
    {
        void OnMouseLeftDown(object sender, MouseEventArgs e);
        
        void OnMouseRightDown(object sender, MouseEventArgs e);
    }
}

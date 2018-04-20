using System.Windows.Input;

namespace InverseTest.GUI.Models
{
   public interface IModelMover
    {
        void OnMouseDown(object sender, MouseEventArgs e);

        void OnMouseUp(object sender, MouseEventArgs e);

        void OnMouseMove(object sender, MouseEventArgs e);
    }
}

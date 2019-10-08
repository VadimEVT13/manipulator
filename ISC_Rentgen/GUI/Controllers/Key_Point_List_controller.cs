using HelixToolkit.Wpf;
using ISC_Rentgen.GUI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ISC_Rentgen.GUI.Controllers
{
    public class Key_Point_List_controller
    {
        public Model3DGroup Model { get; set; }
        public Model3DGroup Detal { get; set; }
        private static string Key_Point_Model_Name = "Key_Point_From_Table+";

        public void PointAdd(Key_Point p)
        {
            if (Model != null)
            {
                MeshBuilder mb = new MeshBuilder(true, true);
                mb.AddSphere(p.Emitter_point, 1);
                GeometryModel3D gm = new GeometryModel3D() { Geometry = mb.ToMesh(), Material = Materials.Red };
                gm.SetName(Key_Point_Model_Name+p.Index);
                Model.Children.Add(gm);
            }

            if (Detal != null)
            {
                MeshBuilder mb = new MeshBuilder(true, true);
                mb.AddSphere(p.Emitter_point, 1);
                GeometryModel3D gm = new GeometryModel3D() { Geometry = mb.ToMesh(), Material = Materials.Red };
                gm.SetName(Key_Point_Model_Name+p.Index);
                Detal.Children.Add(gm);
            }
        }

        public void PointRemove(Key_Point p)
        {
            if (Model != null)
            {
                Model.Children.Remove(Model.Children.Where(x => x.GetName() == Key_Point_Model_Name + p.Index).First());
            }
            if (Detal != null)
            {
                Detal.Children.Remove(Detal.Children.Where(x => x.GetName() == Key_Point_Model_Name + p.Index).First());
            }
        }

        public void PointsClear()
        {
            if (Model != null)
            {
                while (Model.Children.Where(x => x.GetName() == Key_Point_Model_Name).Count() >= 1)
                {
                    Model.Children.Remove(Model.Children.Where(x => x.GetName() == Key_Point_Model_Name).First());
                }
            }

            if (Detal != null)
            {
                while (Detal.Children.Where(x => x.GetName() == Key_Point_Model_Name).Count() >= 1)
                {
                    Detal.Children.Remove(Detal.Children.Where(x => x.GetName() == Key_Point_Model_Name).First());
                }
            }
        }

        public void AngleModif(Key_Point p)
        {
            if (Model != null)
            {
                Model3D m = Model.Children.Where(x => x.GetName() == Key_Point_Model_Name + p.Index).First();
                if (m != null)
                {
                    if (p.IsCorrectManip & p.IsCorrectPort)
                        (m as GeometryModel3D).Material = Materials.Green;
                    else
                        (m as GeometryModel3D).Material = Materials.Red;
                }
            }
            if (Detal != null)
            {
                Model3D m = Detal.Children.Where(x => x.GetName() == Key_Point_Model_Name + p.Index).First();
                if (m != null)
                {
                    if (p.IsCorrectManip & p.IsCorrectPort)
                        (m as GeometryModel3D).Material = Materials.Green;
                    else
                        (m as GeometryModel3D).Material = Materials.Red;
                }
            }
        }
    }
}

using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace InverseTest.GUI.Model
{
    class ConeModel:IConeModel
    {
        TruncatedConeVisual3D cone;
        Material material;

        public ConeModel()
        {
            material = MaterialHelper.CreateMaterial(Colors.Aqua, opacity:0.4d);

            cone = new TruncatedConeVisual3D()
            {
                Material = material,
                Height = 40,
                BaseRadius = 4,
                TopRadius=0,
                
            }; 
        }

        public void ChangePosition(Point3D origin, Vector3D directoin)
        {
            cone.Normal = directoin;
            cone.Origin = origin;
            cone.UpdateModel();
        }

        public Model3D GetModel()
        {
            return cone.Model;
        }
    }
}

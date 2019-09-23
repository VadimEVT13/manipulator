using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ISC_Rentgen.GUI.Model
{
    public delegate void Addition_Sphere_Deleted(Model3D sphere);
    public delegate void Addition_Sphere_Added(Model3D sphere);

    public class Addition_Sphere
    {
        public event Addition_Sphere_Deleted    Sphere_Deleted;
        public event Addition_Sphere_Added      Sphere_Added;

        private static Addition_Sphere instance;
        public static Addition_Sphere getInstance {
            get {
                if (instance == null) { instance = new Addition_Sphere(); }
                return instance;
            }
        }

        public Model3D Sphere {
            get { return CreateSphere(); }
        }
        private Model3D sphere;

        private Color color = Colors.Red;    // Цвет сферы
        private double opacity = 0.5;        // Прозрачность сферы

        public Point3D Position { get { return position; } set { position = value; CreateSphere(); } }
        private Point3D position = new Point3D();

        public double Radius { get { return radius; } set { if (value > 0) radius = value; CreateSphere(); } }
        private double radius = 1;

        public string SphereName { get { return "Sphere"; } }
        public bool If_Sphere_Exist { get { return if_sphere_exist; } set { if_sphere_exist = value; CreateSphere(); } }
        private bool if_sphere_exist = false;

        // Создание сферы
        private Model3D CreateSphere()
        {
            MeshBuilder mb = new MeshBuilder();
            mb.AddSphere(Position, Radius);

            double current_opacity = opacity;
            if (If_Sphere_Exist == false) { current_opacity = 0; }

            sphere = new GeometryModel3D()
            {
                Geometry = mb.ToMesh(),
                Material = MaterialHelper.CreateMaterial(color, current_opacity)                             
            };
            sphere.SetName(SphereName);
            Sphere_Added?.Invoke(sphere);

            return sphere;
        }

        // Сокрытие сферы
        public void DeleteSphere()
        {
            if (sphere != null)
            {
                If_Sphere_Exist = false;
                Sphere_Deleted?.Invoke(sphere);
            }
        }
    }
}

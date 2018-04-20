using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.GUI
{
    public class MainVisual
    {
        public ModelVisual3D camManip;
        public ModelVisual3D camPortal;
        public ModelVisual3D top;
        public ModelVisual3D right;
        public ModelVisual3D front;
        public ModelVisual3D _3d;

        private Model3D model;

        private Material[] DefaultMaterials;
        private Material CollisionMaterial = Materials.Red;

        public MainVisual(Model3D model, bool camDisplayed = true)
        {

            this.model = model;
            StoreDefaultMaterials(model);
            if (camDisplayed)
            {
                camManip = new ModelVisual3D() { Content = model };
                camPortal = new ModelVisual3D() { Content = model };
            }

            this.top = new ModelVisual3D() { Content = model };
            this.right = new ModelVisual3D() { Content = model };
            this.front = new ModelVisual3D() { Content = model };
            this._3d = new ModelVisual3D() { Content = model };
        }

        public void Transform(Transform3D transform)
        {
            this.model.Transform = transform;
        }
        private void StoreDefaultMaterials(Model3D model) {

            List<Material> materials = new List<Material>();
            if (model is Model3DGroup modelGroup)
            {
                foreach (Model3D m in modelGroup.Children)
                {
                    Material newMaterial = CreateNewMaterial((m as GeometryModel3D).Material);
                    materials.Add(newMaterial);
                }
            }
            else
            {
                Material newMaterial = CreateNewMaterial((model as GeometryModel3D).Material);
                materials.Add(newMaterial);
            }
                this.DefaultMaterials = materials.ToArray();
        }

        public void SetCollisionCollor()
        {
            if (model is Model3DGroup modelGroup)
            {
                foreach (GeometryModel3D m in modelGroup.Children)
                {
                    m.Material = CollisionMaterial;
                }
            }
            else (model as GeometryModel3D).Material = CollisionMaterial;
        }

        public void SetDefaultColor()
        {
            if (model is Model3DGroup modelGroup)
            {
                for(int i= 0; i< modelGroup.Children.Count; i++)
                {
                    (modelGroup.Children[i] as GeometryModel3D).Material = DefaultMaterials[i];
                }
            }
            else (model as GeometryModel3D).Material = DefaultMaterials[0];
        }

        private Material CreateNewMaterial(Material material)
        {
            MaterialGroup newMaterialGroup = new MaterialGroup();
            if(material is MaterialGroup mg)
            {
                foreach(Material mt in mg.Children)
                {
                    if (mt is DiffuseMaterial dfm)
                    {
                        newMaterialGroup.Children.Add(new DiffuseMaterial(dfm.Brush));
                    }
                    else if (mt is SpecularMaterial spm) {
                        newMaterialGroup.Children.Add(new SpecularMaterial(spm.Brush, spm.SpecularPower));
                    }
                }
            }

            return newMaterialGroup;
        }

    }
}

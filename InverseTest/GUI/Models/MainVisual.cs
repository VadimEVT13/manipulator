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
        public ModelVisual3D CamManip;
        public ModelVisual3D CamPortal;
        public ModelVisual3D Top;
        public ModelVisual3D Right;
        public ModelVisual3D Front;
        public ModelVisual3D _3D;

        private Model3D model;

        private Material[] DefaultMaterials;
        private Material CollisionMaterial = Materials.Red;

        public MainVisual(Model3D model, bool camDisplayed = true)
        {

            this.model = model;
            StoreDefaultMaterials(model);
            if (camDisplayed)
            {
                CamManip = new ModelVisual3D() { Content = model };
                CamPortal = new ModelVisual3D() { Content = model };
            }

            this.Top = new ModelVisual3D() { Content = model };
            this.Right = new ModelVisual3D() { Content = model };
            this.Front = new ModelVisual3D() { Content = model };
            this._3D = new ModelVisual3D() { Content = model };
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

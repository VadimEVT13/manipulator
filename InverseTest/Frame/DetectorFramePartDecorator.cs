using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Frame
{
    class DetectorFramePartDecorator:DetectorFramePart
    {

        private Model3DGroup part;
        private DetectorFramePart priviusPart;
        private Transform3DGroup transforms;
        private Transform3D translateTrans;
        private Transform3D RotateTransform;
        
        public DetectorFramePartDecorator(Model3DGroup model, DetectorFramePartDecorator decorator)
        {
            this.part = model;
            this.priviusPart = decorator;
            this.transforms = new Transform3DGroup();
            this.translateTrans = new TranslateTransform3D();
            this.RotateTransform = new RotateTransform3D();

            this.transforms.Children.Add(translateTrans);
            this.transforms.Children.Add(RotateTransform);
            
            if (part != null)
                part.Transform = transforms;
        }

        public override void TranslateTransform3D(Transform3D transform)
        {
            part.Transform = new TranslateTransform3D();
            part.Transform = transform;
        }

        public override void RotateTransform3D(Transform3D rotate)
        {
            Transform3D transform = HelixToolkit.Wpf.Transform3DHelper.CombineTransform(part.Transform, rotate);
            part.Transform = transform;
        }

        public override Model3DGroup GetAllModelGroup()
        {
            Model3DGroup modelGroup = new Model3DGroup();
            List<Model3D> list = new List<Model3D>();
            if (part != null)
                list.AddRange(part.Children.ToList());

            if (priviusPart != null)
                list.AddRange(priviusPart.GetAllModelGroup().Children.ToList());

            modelGroup.Children = new Model3DCollection(list);
            return modelGroup;
        }

        public override Model3D GetModelPart()
        {
            return part;
        }

        public override Rect3D Bounds()
        {
            return part.Bounds;
        }

        public override void ResetTransforms()
        {
        }

        public override Point3D GetCameraPosition()
        {
            if (priviusPart != null)
                return priviusPart.GetCameraPosition();
            else return part.Bounds.Location;
        }

    }
}

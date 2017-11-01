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
        private TranslateTransform3D lastTranslateTrans;
        private RotateTransform3D ZAxisRotateTransform;
        private RotateTransform3D YAxisRotateTransform;

        public DetectorFramePartDecorator(Model3DGroup model, DetectorFramePartDecorator decorator)
        {
            this.part = model;
            this.priviusPart = decorator;
            this.transforms = new Transform3DGroup();
            this.lastTranslateTrans = new TranslateTransform3D(0,0,0);
            this.ZAxisRotateTransform = new RotateTransform3D();
            this.YAxisRotateTransform = new RotateTransform3D();

            this.transforms.Children.Add(lastTranslateTrans);
            this.transforms.Children.Add(ZAxisRotateTransform);
            this.transforms.Children.Add(YAxisRotateTransform);
            
            if (part != null)
                part.Transform = transforms;
        }

        public override void TranslateTransform3D(Transform3D transform)
        {
            part.Transform = transform;
        }

        public override void RotateTransform3D(Transform3D rotate)
        {

            part.Transform = rotate;
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
            transforms = new Transform3DGroup();
            lastTranslateTrans = new TranslateTransform3D();
            ZAxisRotateTransform = new RotateTransform3D();
            transforms.Children.Add(lastTranslateTrans);
            transforms.Children.Add(ZAxisRotateTransform);
            transforms.Children.Add(YAxisRotateTransform);

            if (part != null)
                part.Transform = transforms;

            if (priviusPart != null)
                priviusPart.ResetTransforms();
        }

        public override Point3D GetCameraPosition()
        {
            if (priviusPart != null)
                return priviusPart.GetCameraPosition();
            else return part.Bounds.Location;
        }

    }
}

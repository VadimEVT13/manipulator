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
        private RotateTransform3D rotateTransform;

        public DetectorFramePartDecorator(Model3DGroup model, DetectorFramePartDecorator decorator)
        {
            this.part = model;
            this.priviusPart = decorator;
            this.transforms = new Transform3DGroup();
            this.lastTranslateTrans = new TranslateTransform3D(0,0,0);
            this.rotateTransform = new RotateTransform3D();

            this.transforms.Children.Add(lastTranslateTrans);
            this.transforms.Children.Add(rotateTransform);
            
            if (part != null)
                part.Transform = transforms;
        }

        public override void TranslateTransform3D(TranslateTransform3D transform)
        {

            double offsetX = transform.OffsetX != 0 ? transform.OffsetX : lastTranslateTrans.OffsetX;
            double offsetY = transform.OffsetY != 0 ? transform.OffsetY : lastTranslateTrans.OffsetY;
            double offsetZ = transform.OffsetZ != 0 ? transform.OffsetZ : lastTranslateTrans.OffsetZ;

            lastTranslateTrans.OffsetX = offsetX;
            lastTranslateTrans.OffsetY = offsetY;
            lastTranslateTrans.OffsetZ = offsetZ;


            if (priviusPart != null)
                priviusPart.TranslateTransform3D(transform);
        }

        public override void RotateTransform3D(RotateTransform3D rotate)
        {
            this.rotateTransform.CenterX = rotate.CenterX;
            this.rotateTransform.CenterY = rotate.CenterY;
            this.rotateTransform.CenterZ = rotate.CenterZ;

            this.rotateTransform.Rotation = rotate.Rotation;
            

            //TODO Эта фигня ниже вроде как не правильно работает
            if (priviusPart != null)
                priviusPart.RotateTransform3D(rotate);
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
            rotateTransform = new RotateTransform3D();
            transforms.Children.Add(lastTranslateTrans);
            transforms.Children.Add(rotateTransform);

            if (part != null)
                part.Transform = transforms;

            if (priviusPart != null)
                priviusPart.ResetTransforms();
        }

    }
}

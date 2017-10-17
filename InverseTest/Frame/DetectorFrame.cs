
using System;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using InverseTest.Frame.Kinematic;
using InverseTest.Frame;

namespace InverseTest
{
    public class DetectorFrame: IDetectorFrame, IDebugModels
    {
        public static Vector3D ZRotateAxis = new Vector3D(0, 0, 1);
        public static Vector3D XRotateAxis = new Vector3D(1, 0, 0);
        public static Vector3D YRotateAxis = new Vector3D(0, 1, 0);

        private Model3DGroup detectorFrameGraph = new Model3DGroup();
        private Model3DGroup portalModel;
        private int numberMesh = 0;


        /// Части 
        private  Dictionary<Parts, DetectorFramePart> parts = new Dictionary<Parts, DetectorFramePart>();
        private Dictionary<Parts, Model3DGroup> partsCoordinates = new Dictionary<Parts, Model3DGroup>();
        private Model3DCollection partsCollectoin = new Model3DCollection();


        private static Vector3D defaultScreenDirection = new Vector3D(-1,0,0);
        private Vector3D currentScreenDirection = new Vector3D(-1, 0, 0);

       public enum Parts
        {
            Platform,
            VerticalFrame,
            HorizontalBar,
            ScreenHolder,
            Screen
        }


        public DetectorFrame(Model3DGroup portal)
        {

            portalModel = portal;
            //Экран
            Model3DGroup screen = new Model3DGroup();
            screen.Children.Add(portalModel.Children[34]);
            screen.Children.Add(portalModel.Children[35]);
            screen.Children.Add(portalModel.Children[36]);
            screen.Children.Add(portalModel.Children[37]);

            //Платформа на которой стоит вся конструкция по идее не двигается но пусть.
            Model3DGroup platform = new Model3DGroup();
            platform.Children.Add(portalModel.Children[0]);
            platform.Children.Add(portalModel.Children[1]);
            platform.Children.Add(portalModel.Children[2]);
            platform.Children.Add(portalModel.Children[3]);
            platform.Children.Add(portalModel.Children[4]);
            platform.Children.Add(portalModel.Children[5]);
            platform.Children.Add(portalModel.Children[6]);
            platform.Children.Add(portalModel.Children[7]);
            platform.Children.Add(portalModel.Children[8]);
            platform.Children.Add(portalModel.Children[9]);



            ///Вертикальная рамка
            Model3DGroup verticalFrame = new Model3DGroup();
            verticalFrame.Children.Add(portalModel.Children[10]);
            verticalFrame.Children.Add(portalModel.Children[11]);
            verticalFrame.Children.Add(portalModel.Children[12]);
            verticalFrame.Children.Add(portalModel.Children[13]);
            verticalFrame.Children.Add(portalModel.Children[14]);
            verticalFrame.Children.Add(portalModel.Children[15]);
            verticalFrame.Children.Add(portalModel.Children[16]);
            verticalFrame.Children.Add(portalModel.Children[17]);
            verticalFrame.Children.Add(portalModel.Children[18]);
            verticalFrame.Children.Add(portalModel.Children[19]);

            ///Горизонтальная платка на которой крепится держатель для экрана
            Model3DGroup horizontalBar = new Model3DGroup();
            horizontalBar.Children.Add(portalModel.Children[20]);
            horizontalBar.Children.Add(portalModel.Children[21]);
            horizontalBar.Children.Add(portalModel.Children[22]);
            horizontalBar.Children.Add(portalModel.Children[23]);
            horizontalBar.Children.Add(portalModel.Children[24]);
            horizontalBar.Children.Add(portalModel.Children[25]);
            horizontalBar.Children.Add(portalModel.Children[26]);
            horizontalBar.Children.Add(portalModel.Children[27]);
            horizontalBar.Children.Add(portalModel.Children[28]);



            //Держатель для экрана, относительно него вращается экран
            Model3DGroup screenHolder = new Model3DGroup();
            screenHolder.Children.Add(portalModel.Children[29]);
            screenHolder.Children.Add(portalModel.Children[30]);
            screenHolder.Children.Add(portalModel.Children[31]);
            screenHolder.Children.Add(portalModel.Children[32]);
            screenHolder.Children.Add(portalModel.Children[33]);



            DetectorFramePartDecorator screenPart = new DetectorFramePartDecorator(screen, null);
            DetectorFramePartDecorator screenHolderPart = new DetectorFramePartDecorator(screenHolder, screenPart);
            DetectorFramePartDecorator horizontalBarPart = new DetectorFramePartDecorator(horizontalBar, screenHolderPart);
            DetectorFramePartDecorator verticalFramePart = new DetectorFramePartDecorator(verticalFrame, horizontalBarPart);
            DetectorFramePartDecorator platformPart = new DetectorFramePartDecorator(platform, verticalFramePart);


            parts.Add(Parts.Screen,screenPart);
            parts.Add(Parts.ScreenHolder,screenHolderPart);
            parts.Add(Parts.VerticalFrame,verticalFramePart);
            parts.Add(Parts.HorizontalBar,horizontalBarPart);
            parts.Add(Parts.Platform, platformPart);

            partsCollectoin.Add(screenPart.GetModelPart());
            partsCollectoin.Add(screenHolderPart.GetModelPart());
            partsCollectoin.Add(horizontalBarPart.GetModelPart());
            partsCollectoin.Add(verticalFramePart.GetModelPart());
            partsCollectoin.Add(platformPart.GetModelPart());


            detectorFrameGraph.Children = partsCollectoin;
        }

        private Model3DCollection copyCollection(Model3DCollection collection)
        {
            Model3DCollection retCollection = new Model3DCollection();

            foreach (Model3D model in collection)
                retCollection.Add(model);

            return retCollection;
        }

        public virtual Vector3D GetScreenDirection()
        {
            return currentScreenDirection;
        }

        public virtual void MoveDetectFrame(DetectorFramePosition p)
        {
            ResetTransforms();

            Model3D verticalFrame = parts[Parts.VerticalFrame].GetModelPart();
            double offsetX = p.pointScreen.X - verticalFrame.Bounds.X + (verticalFrame.Bounds.X-parts[Parts.Screen].GetModelPart().Bounds.X);

            Model3D horizontalBar = parts[Parts.HorizontalBar].GetModelPart();
            double centerOfHorizontalBar = horizontalBar.Bounds.Y + (horizontalBar.Bounds.SizeY / 2);
            double offsetY = p.pointScreen.Y - centerOfHorizontalBar;

            Model3D screenHolder = parts[Parts.ScreenHolder].GetModelPart();
            double centerOfScreenHoldear = screenHolder.Bounds.Z + (screenHolder.Bounds.SizeZ / 2);
            double offsetZ = p.pointScreen.Z - centerOfScreenHoldear;
            
            Console.WriteLine("DetectorFramePosition:" + p.ToString());
            MovePart(Parts.VerticalFrame, offsetX);
            MovePart(Parts.HorizontalBar, offsetY);
            MovePart(Parts.ScreenHolder, offsetZ);
            RotatePart(Parts.Screen, p.horizontalAngle, YRotateAxis);
            RotatePart(Parts.Screen, p.verticalAngle, ZRotateAxis);
            calculateScreenDirection(p);
        }
         
        /// <summary>
        /// Вычисляет текущее направление экрана относительно положения по умолчанию
        /// </summary>
        /// <param name="p">Структура содержащая положение всего портала в том числе и углы поворота экрана</param>
        private void calculateScreenDirection(DetectorFramePosition p)
        {
            double horizontalAngle = (p.horizontalAngle * 180) / Math.PI;
            double verticalAngle = (p.verticalAngle * 180) / Math.PI;
            Matrix3D m = Matrix3D.Identity;
            Quaternion horizQuaternion = new Quaternion(ZRotateAxis, horizontalAngle);
            m.Rotate(horizQuaternion);
            Quaternion verticalQuaternion = new Quaternion(YRotateAxis, verticalAngle);
            m.Rotate(verticalQuaternion);
            currentScreenDirection = m.Transform(defaultScreenDirection);
        }
        
        public virtual void MovePart(Parts partToMove, double offsetToMove)
        {
            TranslateTransform3D moveTransform;
            DetectorFramePart modelToMove = parts[partToMove];

            switch(partToMove)  
            {
                case Parts.ScreenHolder:
                    moveTransform = new TranslateTransform3D(0, 0, offsetToMove);
                    break;
                case Parts.HorizontalBar:
                    moveTransform = new TranslateTransform3D(0, offsetToMove, 0);
                    break;
                case Parts.VerticalFrame:
                    moveTransform = new TranslateTransform3D(offsetToMove, 0, 0);
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }
            

            modelToMove.TranslateTransform3D(moveTransform);
        }

        public virtual void RotatePart(Parts partToRotate, double angle, Vector3D rotateAxis)
        {
            RotateTransform3D rotateTransform;
            DetectorFramePart modelToRotate = parts[partToRotate];
            
            double angleInDegrees = (angle * 180) / Math.PI;

            switch (partToRotate)
            {
                case Parts.Screen:
                    Point3D point = parts[Parts.ScreenHolder].Bounds().Location;
                    Size3D size = parts[Parts.ScreenHolder].Bounds().Size;
                    Point3D centerRotate = new Point3D(point.X, point.Y+size.Y/2, point.Z + size.Z/2);
                    rotateTransform = new RotateTransform3D(new AxisAngleRotation3D(rotateAxis, angleInDegrees),centerRotate);
                    break;
                default: throw new InvalidEnumArgumentException();
            }
            modelToRotate.RotateTransform3D(rotateTransform);
        }

        public virtual void transformModel(Double x)
        {
            portalModel.Children[numberMesh].Transform = new TranslateTransform3D(0, x, 0);
        }

        public virtual void addNumberMesh(int number)
        {
            this.numberMesh = number;
        }
        public virtual Model3D GetDetectorFrameModel()
        {
            return detectorFrameGraph;
        }

       public virtual Model3D GetDetectorFramePart(Parts part)
        { 
            return parts[part].GetModelPart();
        }

        public virtual void ResetTransforms()
        {
            parts[Parts.Platform].ResetTransforms();
        }

    }



}

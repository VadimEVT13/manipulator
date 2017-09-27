
using System;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using InverseTest.Frame.Kinematic;

namespace InverseTest
{

    


    public class DetectorFrame: IDetectorFrame
    {
        private static Vector3D ZRotateAxis = new Vector3D(0, 0, 1);
        private static Vector3D XRotateAxis = new Vector3D(1, 0, 0);
        private static Vector3D YRotateAxis = new Vector3D(0, 1, 0);

        private readonly Model3DGroup detectorFrameModel = new Model3DGroup();
        private int numberMesh = 0;

        /// Части 
        private readonly Dictionary<Parts, Model3DGroup> parts = new Dictionary<Parts, Model3DGroup>();
        private readonly Dictionary<Parts, Model3DGroup> partsCoordinates = new Dictionary<Parts, Model3DGroup>();


       public enum Parts
        {
            Platform,
            VerticalFrame,
            HorizontalBar,
            ScreenHolder,
            Screen
        }


        public DetectorFrame(string file)
        {
            Model3DGroup portalModel = new ModelImporter().Load(file);


            //Экран
            Model3DGroup screen = new Model3DGroup();
            screen.Children.Add(portalModel.Children[26]);
            screen.Children.Add(portalModel.Children[27]);
            screen.Children.Add(portalModel.Children[28]);

            //Платформа на которой стоит вся конструкция по идее не двигается но пусть.
            Model3DGroup platform = new Model3DGroup();
            platform.Children.Add(portalModel.Children[20]);
            platform.Children.Add(portalModel.Children[21]);
            platform.Children.Add(portalModel.Children[22]);
            platform.Children.Add(portalModel.Children[23]);
            platform.Children.Add(portalModel.Children[24]);


            ///Вертикальная рамка
            Model3DGroup verticalFrame = new Model3DGroup();
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
            horizontalBar.Children.Add(portalModel.Children[5]);
            horizontalBar.Children.Add(portalModel.Children[6]);
            horizontalBar.Children.Add(portalModel.Children[7]);
            horizontalBar.Children.Add(portalModel.Children[8]);
            horizontalBar.Children.Add(portalModel.Children[9]);
            horizontalBar.Children.Add(portalModel.Children[10]);

            //Держатель для экрана, относительно него вращается экран
            Model3DGroup screenHolder = new Model3DGroup();
            screenHolder.Children.Add(portalModel.Children[0]);
            screenHolder.Children.Add(portalModel.Children[1]);
            screenHolder.Children.Add(portalModel.Children[2]);
            screenHolder.Children.Add(portalModel.Children[3]);
            screenHolder.Children.Add(portalModel.Children[4]);

            Model3DGroup screenCoordinates = new Model3DGroup();
            screenCoordinates.Children = copyCollection(screen.Children);
            partsCoordinates.Add(Parts.Screen, screenCoordinates);
            


            Model3DGroup screenHolderCoordinates = new Model3DGroup();
            screenHolderCoordinates.Children = copyCollection(screenHolder.Children);
            partsCoordinates.Add(Parts.ScreenHolder, screenHolderCoordinates);

            Model3DGroup verticalFrameCoordinates= new Model3DGroup();
            verticalFrameCoordinates.Children = copyCollection(verticalFrame.Children);
            partsCoordinates.Add(Parts.VerticalFrame, verticalFrameCoordinates);
            
            Model3DGroup horizontalBarCoordinates = new Model3DGroup();
            horizontalBarCoordinates.Children = copyCollection(horizontalBar.Children);
            partsCoordinates.Add(Parts.HorizontalBar, horizontalBarCoordinates);
            
            Model3DGroup platformCoordinates = new Model3DGroup();
            platformCoordinates.Children = copyCollection(platform.Children);
            partsCoordinates.Add(Parts.Platform, platformCoordinates);
            
            screenHolder.Children.Add(screen);
            horizontalBar.Children.Add(screenHolder);
            verticalFrame.Children.Add(horizontalBar);
            platform.Children.Add(verticalFrame);

            detectorFrameModel.Children.Add(platform);
            
            parts.Add(Parts.Screen,screen);
            parts.Add(Parts.ScreenHolder,screenHolder);
            parts.Add(Parts.VerticalFrame,verticalFrame);
            parts.Add(Parts.HorizontalBar,horizontalBar);
            parts.Add(Parts.Platform, platform);
        }

        private Model3DCollection copyCollection(Model3DCollection collection)
        {
            Model3DCollection retCollection = new Model3DCollection();

            foreach (Model3D model in collection)
                retCollection.Add(model);

            return retCollection;
        }

        public void MoveDetectFrame(DetectorFramePosition p)
        {

            double centerOfVerticalBar = partsCoordinates[Parts.VerticalFrame].Bounds.X + (partsCoordinates[Parts.VerticalFrame].Bounds.SizeX / 2);
            double offsetX = partsCoordinates[Parts.Platform].Bounds.X + partsCoordinates[Parts.Platform].Bounds.SizeX - p.pointScreen.X+ centerOfVerticalBar;
          
           // offsetX = offsetX - partsCoordinates[Parts.VerticalFrame].Bounds.X;
            double centerOfHorizontalBar = partsCoordinates[Parts.HorizontalBar].Bounds.Y + (partsCoordinates[Parts.HorizontalBar].Bounds.SizeY / 2);
            double offsetY = p.pointScreen.Y - centerOfHorizontalBar;
            double centerOfScreenHoldear = partsCoordinates[Parts.ScreenHolder].Bounds.Z + (partsCoordinates[Parts.ScreenHolder].Bounds.SizeZ / 2);
            double offsetZ = p.pointScreen.Z - centerOfScreenHoldear;
            
            Console.WriteLine("DetectorFramePosition:" + p.ToString());
            movePart(Parts.VerticalFrame, offsetX);
            movePart(Parts.HorizontalBar, offsetY);
            movePart(Parts.ScreenHolder, offsetZ);
            rotatePart(Parts.Screen, p.horizontalAngle, YRotateAxis);
            rotatePart(Parts.Screen, p.verticalAngle, ZRotateAxis);
        } 
        
        private void movePart(Parts partToMove, double offsetToMove)
        {
            Transform3D moveTransform;
            Model3DGroup modelToMove = parts[partToMove];

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

            modelToMove.Transform = moveTransform;
        }

        private void rotatePart(Parts partToRotate, double angle, Vector3D rotateAxis)
        {
            Transform3D rotateTransform;
            Model3DGroup modelToRotate = parts[partToRotate];
            angle = (angle * 180) / Math.PI;

            switch (partToRotate)
            {
                case Parts.Screen:
                    Point3D point = partsCoordinates[Parts.ScreenHolder].Bounds.Location;
                    Size3D size = partsCoordinates[Parts.ScreenHolder].Bounds.Size;
                    Point3D centerRotate = new Point3D(point.X, point.Y+size.Y/2, point.Z + size.Z/2);
                    rotateTransform = new RotateTransform3D(new AxisAngleRotation3D(rotateAxis, angle),centerRotate);
                    break;
                default: throw new InvalidEnumArgumentException();
            }

            modelToRotate.Transform = rotateTransform;

        }




        public void transformModel(Double x)
        {

           Model3D horizontalBar = parts[Parts.Screen];
           // horizontalBar.Transform = new TranslateTransform3D(x, 0, 0);
            Console.WriteLine("VerticalFrame: " + horizontalBar.Bounds.Location);
            movePart(Parts.HorizontalBar, x);
        }

        public void addNumberMesh(int number)
        {
            this.numberMesh = number;
        }
        Model3D IDetectorFrame.GetDetectorFrameModel()
        {
            return detectorFrameModel;
        }

        Model3D IDetectorFrame.GetDetectorFramePart(Parts part)
        {
            return partsCoordinates[part];
        }



        




    }



}

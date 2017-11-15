
using System;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using InverseTest.Frame.Kinematic;
using InverseTest.Frame;
using System.Windows.Threading;
using InverseTest.Manipulator;

namespace InverseTest
{
    public class DetectorFrame : IDetectorFrame, IDebugModels
    {
        public static Vector3D ZRotateAxis = new Vector3D(0, 0, 1);
        public static Vector3D XRotateAxis = new Vector3D(1, 0, 0);
        public static Vector3D YRotateAxis = new Vector3D(0, 1, 0);

        private Model3DGroup detectorFrameGraph = new Model3DGroup();
        private Model3DGroup portalModel;
        private Model3D camPositionCube;
        private int numberMesh = 0;



        /// Части 
        private Dictionary<Parts, DetectorFramePart> parts = new Dictionary<Parts, DetectorFramePart>();
        private Dictionary<Parts, double> partOffset = new Dictionary<Parts, double>();
        private Dictionary<Parts, double> partDeltas = new Dictionary<Parts, double>();
        private Dictionary<Parts, Point3D> partStartPosition = new Dictionary<Parts, Point3D>();
        private Dictionary<Parts, double> positionsToSet = new Dictionary<Parts, double>();


        private DetectorFramePosition position;
        private double verticalAngle;
        private double horizontalAngle;
        private double verticalAngleDelta;
        private double horizontalAngleDelta;
        private Model3DCollection partsCollectoin = new Model3DCollection();
        private Parts[] movedParts = { Parts.VerticalFrame, Parts.HorizontalBar, Parts.ScreenHolder };



        private static Vector3D defaultScreenDirection = new Vector3D(-1, 0, 0);
        private Vector3D currentScreenDirection = new Vector3D(-1, 0, 0);

        public enum Parts
        {
            Platform,
            VerticalFrame,
            HorizontalBar,
            ScreenHolder,
            Screen,
            ScreenCameraPos
        }

        DispatcherTimer timer;
        bool isAnimated = false;

        public event PositionHandler onPositionChanged;

        public DetectorFrame(Model3DGroup portal)
        {

            portalModel = portal;
            //Экран
            Model3DGroup screen = new Model3DGroup();
            screen.Children.Add(portalModel.Children[34]);
            screen.Children.Add(portalModel.Children[35]);
            screen.Children.Add(portalModel.Children[36]);
            screen.Children.Add(portalModel.Children[37]);
            screen.Children.Add(portalModel.Children[38]);

            Model3DGroup screenCameraPos = new Model3DGroup();
            screenCameraPos.Children.Add(portalModel.Children[39]);

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



            DetectorFramePartDecorator screenCameraPart = new DetectorFramePartDecorator(screenCameraPos, null);
            DetectorFramePartDecorator screenPart = new DetectorFramePartDecorator(screen, screenCameraPart);
            DetectorFramePartDecorator screenHolderPart = new DetectorFramePartDecorator(screenHolder, screenPart);
            DetectorFramePartDecorator horizontalBarPart = new DetectorFramePartDecorator(horizontalBar, screenHolderPart);
            DetectorFramePartDecorator verticalFramePart = new DetectorFramePartDecorator(verticalFrame, horizontalBarPart);
            DetectorFramePartDecorator platformPart = new DetectorFramePartDecorator(platform, verticalFramePart);

            parts.Add(Parts.ScreenCameraPos, screenCameraPart);
            parts.Add(Parts.Screen, screenPart);
            parts.Add(Parts.ScreenHolder, screenHolderPart);
            parts.Add(Parts.VerticalFrame, verticalFramePart);
            parts.Add(Parts.HorizontalBar, horizontalBarPart);
            parts.Add(Parts.Platform, platformPart);

            partsCollectoin.Add(screenCameraPart.GetModelPart());
            partsCollectoin.Add(screenPart.GetModelPart());
            partsCollectoin.Add(screenHolderPart.GetModelPart());
            partsCollectoin.Add(horizontalBarPart.GetModelPart());
            partsCollectoin.Add(verticalFramePart.GetModelPart());
            partsCollectoin.Add(platformPart.GetModelPart());

            partStartPosition.Add(Parts.ScreenCameraPos, parts[Parts.ScreenCameraPos].GetModelPart().Bounds.Location);
            partStartPosition.Add(Parts.Screen, parts[Parts.Screen].GetModelPart().Bounds.Location);
            partStartPosition.Add(Parts.ScreenHolder, parts[Parts.ScreenHolder].GetModelPart().Bounds.Location);
            partStartPosition.Add(Parts.VerticalFrame, parts[Parts.VerticalFrame].GetModelPart().Bounds.Location);
            partStartPosition.Add(Parts.HorizontalBar, parts[Parts.HorizontalBar].GetModelPart().Bounds.Location);
            partStartPosition.Add(Parts.Platform, parts[Parts.Platform].GetModelPart().Bounds.Location);

            partOffset.Add(Parts.ScreenHolder, 0);
            partOffset.Add(Parts.VerticalFrame, 0);
            partOffset.Add(Parts.HorizontalBar, 0);


            detectorFrameGraph.Children = partsCollectoin;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromTicks(1000);
            timer.Tick += animation_tick;
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

        public virtual Point3D GetCameraPosition()
        {
            Point3D point = parts[Parts.ScreenCameraPos].GetCameraPosition();
            return new Point3D(point.X, point.Y, point.Z);
        }

        public virtual void MoveDetectFrame(DetectorFramePosition p, bool animate)
        {
            positionsToSet[Parts.VerticalFrame] = (p.pointScreen.X - partStartPosition[Parts.VerticalFrame].X);
            positionsToSet[Parts.HorizontalBar] = (p.pointScreen.Y - partStartPosition[Parts.ScreenCameraPos].Y);
            positionsToSet[Parts.ScreenHolder] = (p.pointScreen.Z - partStartPosition[Parts.ScreenCameraPos].Z);
            position = p;

            if (animate)
                startAnimation(p);
            else
                setPosition(p);
        }


        private void startAnimation(DetectorFramePosition p)
        {
            if (!isAnimated)
            {
                partDeltas[Parts.VerticalFrame] = (positionsToSet[Parts.VerticalFrame] - partOffset[Parts.VerticalFrame]) / 1000;
                partDeltas[Parts.HorizontalBar] = (positionsToSet[Parts.HorizontalBar] - partOffset[Parts.HorizontalBar]) / 1000;
                partDeltas[Parts.ScreenHolder] = (positionsToSet[Parts.ScreenHolder] - partOffset[Parts.ScreenHolder])/1000;
                verticalAngleDelta = p.verticalAngle - verticalAngle;
                horizontalAngle = p.horizontalAngle - horizontalAngle;

                timer.Start();
                isAnimated = true;
            }
        }
        
        void animation_tick(object sender, EventArgs arg)
        {

            List<bool> partOnRightPos = new List<bool>();
            foreach (Parts part in movedParts)
            {
                bool onRightPos;
                partOffset[part] = checkedPositoin(part, out onRightPos);
                partOnRightPos.Add(onRightPos);
            }

            bool onRightAngle;
            verticalAngle = checkedAngleVertical(out onRightAngle);
            partOnRightPos.Add(onRightAngle);

            horizontalAngle= checkedAngleHorizontal(out onRightAngle);
            partOnRightPos.Add(onRightAngle);

            ConfirmPosition();
            if (partOnRightPos.TrueForAll(b => b))
            {
                timer.Stop();
                isAnimated = false;
            }

        }
        private double checkedAngleVertical(out bool onRightAngle)
        {
            double angle = verticalAngle + verticalAngleDelta;
            onRightAngle = false;

            if (Math.Abs(position.verticalAngle) - Math.Abs(angle) <= 2 * Math.Abs(verticalAngleDelta))
            {
                angle = position.verticalAngle;
                onRightAngle = true;
            }
            return angle;
        }
        

        private double checkedAngleHorizontal(out bool onRightAngle)
        {
            double angle = horizontalAngle + horizontalAngleDelta;
            onRightAngle = false;

            if (Math.Abs(position.horizontalAngle) - Math.Abs(angle) <= 2 * Math.Abs(horizontalAngleDelta))
            {
                angle = position.horizontalAngle;
                onRightAngle = true;
            }
            return angle;
        }

        private double checkedPositoin(Parts part, out bool onRightPosition)
        {
            double position = partOffset[part] + partDeltas[part];
            onRightPosition = false;

            if (Math.Abs(positionsToSet[part]) - Math.Abs(position) <= 2 * Math.Abs(partDeltas[part]))
            {
                position = positionsToSet[part];
                onRightPosition = true;
            }
            return position;
        }

        private void setPosition(DetectorFramePosition p)
        {
            double offsetX = p.pointScreen.X - partStartPosition[Parts.VerticalFrame].X;
            partOffset[Parts.VerticalFrame] = offsetX;

            double offsetY = p.pointScreen.Y - partStartPosition[Parts.ScreenCameraPos].Y;
            partOffset[Parts.HorizontalBar] = offsetY;

            double offsetZ = p.pointScreen.Z - partStartPosition[Parts.ScreenCameraPos].Z;
            partOffset[Parts.ScreenHolder] = offsetZ;

            verticalAngle = p.verticalAngle;
            horizontalAngle = p.horizontalAngle;

            ConfirmPosition();

            Console.WriteLine("DetectorFramePosition:" + p.ToString());
        }


        private void ConfirmPosition()
        {
            Transform3DGroup verticalFrameGroup = new Transform3DGroup();
            Transform3DGroup horizontalBarGroup = new Transform3DGroup();
            Transform3DGroup screenHolderGroup = new Transform3DGroup();
            Transform3DGroup screenGroup = new Transform3DGroup();
            Transform3DGroup camPositionCubeGroup = new Transform3DGroup();


            TranslateTransform3D T = new TranslateTransform3D(partOffset[Parts.VerticalFrame], 0, 0);
            verticalFrameGroup.Children.Add(T);

            T = new TranslateTransform3D(0, partOffset[Parts.HorizontalBar], 0);
            horizontalBarGroup.Children.Add(T);
            horizontalBarGroup.Children.Add(verticalFrameGroup);

            T = new TranslateTransform3D(0, 0, partOffset[Parts.ScreenHolder]);
            screenHolderGroup.Children.Add(T);
            screenHolderGroup.Children.Add(horizontalBarGroup);




            Transform3DGroup rotateGroup = new Transform3DGroup();

            Point3D point = parts[Parts.ScreenHolder].Bounds().Location;
            Size3D size = parts[Parts.ScreenHolder].Bounds().Size;
            Point3D centerRotate = new Point3D(point.X, point.Y + size.Y / 2, point.Z + size.Z / 2);
            RotateTransform3D R = new RotateTransform3D(new AxisAngleRotation3D(ZRotateAxis, MathUtils.RadiansToAngle(verticalAngle)), centerRotate);
            rotateGroup.Children.Add(R);
            R = new RotateTransform3D(new AxisAngleRotation3D(YRotateAxis, MathUtils.RadiansToAngle(horizontalAngle)), centerRotate);
            rotateGroup.Children.Add(R);

            screenGroup.Children.Add(screenHolderGroup);

            camPositionCubeGroup.Children.Add(screenGroup);

            parts[Parts.Screen].TranslateTransform3D(screenGroup);
            parts[Parts.Screen].RotateTransform3D(rotateGroup);

            parts[Parts.VerticalFrame].TranslateTransform3D(verticalFrameGroup);
            parts[Parts.HorizontalBar].TranslateTransform3D(horizontalBarGroup);
            parts[Parts.ScreenHolder].TranslateTransform3D(screenHolderGroup);
            parts[Parts.ScreenCameraPos].TranslateTransform3D(camPositionCubeGroup);

            onPositionChanged();

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
            partOffset[partToMove] = offsetToMove;
            ConfirmPosition();


        }

        public virtual void RotatePart(Parts partToRotate, double angle, Vector3D rotateAxis)
        {
            
            switch (partToRotate)
            {
                case Parts.Screen:
                    if (rotateAxis.Equals(ZRotateAxis))
                        verticalAngle = angle;
                    else if (rotateAxis.Equals(YRotateAxis))
                        horizontalAngle = angle;
                    break;
                default: throw new InvalidEnumArgumentException();
            }

            ConfirmPosition();
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
            currentScreenDirection = defaultScreenDirection;
        }

    }



}


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
using System.Linq;
using InverseTest.Bound;

namespace InverseTest
{
    public class DetectorFrame : IPositionChanged
    {
        private static Vector3D ZRotateAxis = new Vector3D(0, 0, -1);
        private static Vector3D XRotateAxis = new Vector3D(1, 0, 0);
        private static Vector3D YRotateAxis = new Vector3D(0, 1, 0);

        private Model3DGroup detectorFrameGraph = new Model3DGroup();
        private Model3DGroup portalModel;
        private Model3DGroup screenCameraPos;

        private int numberMesh = 0;
        
        /// Части 
        public readonly Dictionary<Parts, IDetectorFramePart> parts = new Dictionary<Parts, IDetectorFramePart>();
        private Dictionary<Parts, double> partOffset = new Dictionary<Parts, double>();
        private Dictionary<Parts, double> partDeltas = new Dictionary<Parts, double>();
        private Dictionary<Parts, Point3D> partStartPosition = new Dictionary<Parts, Point3D>();
        private Dictionary<Parts, double> positionsToSet = new Dictionary<Parts, double>();


        private DetectorFramePosition position;

        /// <summary>
        /// Положение вертикальной рамки.
        /// </summary>
        public double VerticalFramePosition
        {
            get
            {
                return partOffset[Parts.VerticalFrame];
            }
            set
            {
                partOffset[Parts.VerticalFrame] = value;
                ConfirmPosition();
                onManulaPositionChanged?.Invoke();
            }
        }

        /// <summary>
        /// Положение горизонтальной планки.
        /// </summary>
        public double HorizontalBarPosition
        {
            get
            {
                return partOffset[Parts.HorizontalBar];
            }
            set
            {
                partOffset[Parts.HorizontalBar] = value;
                ConfirmPosition();
                onManulaPositionChanged?.Invoke();
            }
        }

        /// <summary>
        /// Положение держателя экрана.
        /// </summary>
        public double ScreenHolderPosition
        {
            get
            {
                return partOffset[Parts.ScreenHolder];
            }
            set
            {
                partOffset[Parts.ScreenHolder] = value;
                ConfirmPosition();
                onManulaPositionChanged?.Invoke();
            }
        }

        /// <summary>
        /// Положение экрана по вертикали.
        /// </summary>
        private double verticalAngle;

        /// <summary>
        /// Положение экрана по вертикали.
        /// </summary>
        public double VerticalAngle
        {
            get
            {
                return verticalAngle;
            }
            set
            {
                verticalAngle = value;
                ConfirmPosition();
                onManulaPositionChanged?.Invoke();
            }
        }

        /// <summary>
        /// Положение экрана по горизонтали.
        /// </summary>
        private double horizontalAngle;

        /// <summary>
        /// Положение экрана по горизонтали.
        /// </summary>
        public double HorizontalAngle
        {
            get
            {
                return horizontalAngle;
            }
            set
            {
                horizontalAngle = value;
                ConfirmPosition();
                onManulaPositionChanged?.Invoke();
            }
        }


        private double verticalAngleDelta;
        private double horizontalAngleDelta;
        private Model3DCollection partsCollectoin = new Model3DCollection();
        private Parts[] movedParts = { Parts.VerticalFrame, Parts.HorizontalBar, Parts.ScreenHolder };

        private Vector3D currentScreenDirection = new Vector3D(-1, 0, 0);

        public enum Parts
        {
            PortalPlatform,
            VerticalFrame,
            HorizontalBar,
            ScreenHolder,
            ScreenRotator,
            ScreenRotatePoint,
            Screen,
            ScreenCameraPos
        }

        DispatcherTimer timer;
        bool isAnimated = false;

        public event PositionHandler onPositionChanged;
        public event ManualPositionHandler onManulaPositionChanged;

        public DetectorFrame(Model3DGroup portal)
        {
            portalModel = portal;
            //Экран
            Model3DGroup screen = new Model3DGroup();
            screen.Children.Add(portal.Children[13]);
            screen.Children.Add(portal.Children[14]);
            screen.Children.Add(portal.Children[15]);

            screenCameraPos = new Model3DGroup();
            screenCameraPos.Children.Add(portal.Children[16]);

            //Платформа на которой стоит вся конструкция по идее не двигается но пусть.
            Model3DGroup platform = new Model3DGroup();
            platform.Children.Add(portal.Children[0]);
            platform.Children.Add(portal.Children[1]);
            
            ///Вертикальная рамка
            Model3DGroup verticalFrame = new Model3DGroup();
            verticalFrame.Children.Add(portal.Children[2]);
            verticalFrame.Children.Add(portal.Children[3]);
            verticalFrame.Children.Add(portal.Children[4]);


            ///Горизонтальная платка на которой крепится держатель для экрана
            Model3DGroup horizontalBar = new Model3DGroup();
            horizontalBar.Children.Add(portal.Children[5]);
            horizontalBar.Children.Add(portal.Children[6]);
            horizontalBar.Children.Add(portal.Children[7]);

            //Держатель для экрана, относительно него вращается экран
            Model3DGroup screenHolder = new Model3DGroup();
            screenHolder.Children.Add(portal.Children[8]);
            screenHolder.Children.Add(portal.Children[9]);
            screenHolder.Children.Add(portal.Children[10]);
            
            Model3DGroup screenRotator = new Model3DGroup();
            screenRotator.Children.Add(portal.Children[11]);
            screenRotator.Children.Add(portal.Children[12]);

            Model3DGroup rotateScreenPoint = new Model3DGroup();
            rotateScreenPoint.Children.Add(portal.Children[17]);
            
            DetectorFramePart screenCameraPart = new DetectorFramePart(screenCameraPos);
            DetectorFramePart screenPart = new DetectorFramePart(screen);
            DetectorFramePart rotatePoint = new DetectorFramePart(rotateScreenPoint);
            DetectorFramePart screenRotatorPart = new DetectorFramePart(screenRotator);
            DetectorFramePart screenHolderPart = new DetectorFramePart(screenHolder);
            DetectorFramePart horizontalBarPart = new DetectorFramePart(horizontalBar);
            DetectorFramePart verticalFramePart = new DetectorFramePart(verticalFrame);
            DetectorFramePart platformPart = new DetectorFramePart(platform);

            parts.Add(Parts.ScreenCameraPos, screenCameraPart);
            parts.Add(Parts.Screen, screenPart);
            parts.Add(Parts.ScreenRotatePoint, rotatePoint);
            parts.Add(Parts.ScreenRotator, screenRotatorPart);
            parts.Add(Parts.ScreenHolder, screenHolderPart);
            parts.Add(Parts.VerticalFrame, verticalFramePart);
            parts.Add(Parts.HorizontalBar, horizontalBarPart);
            parts.Add(Parts.PortalPlatform, platformPart);

            partsCollectoin.Add(screenCameraPart.GetModelPart());
            partsCollectoin.Add(screenPart.GetModelPart());
            partsCollectoin.Add(rotatePoint.GetModelPart());
            partsCollectoin.Add(screenRotatorPart.GetModelPart());
            partsCollectoin.Add(screenHolderPart.GetModelPart());
            partsCollectoin.Add(horizontalBarPart.GetModelPart());
            partsCollectoin.Add(verticalFramePart.GetModelPart());
            partsCollectoin.Add(platformPart.GetModelPart());

            partStartPosition.Add(Parts.ScreenCameraPos, parts[Parts.ScreenCameraPos].GetModelPart().Bounds.Location);
            partStartPosition.Add(Parts.Screen, parts[Parts.Screen].GetModelPart().Bounds.Location);
            partStartPosition.Add(Parts.ScreenRotator, parts[Parts.ScreenRotator].GetModelPart().Bounds.Location);
            partStartPosition.Add(Parts.ScreenHolder, parts[Parts.ScreenHolder].GetModelPart().Bounds.Location);
            partStartPosition.Add(Parts.VerticalFrame, parts[Parts.VerticalFrame].GetModelPart().Bounds.Location);
            partStartPosition.Add(Parts.HorizontalBar, parts[Parts.HorizontalBar].GetModelPart().Bounds.Location);
            partStartPosition.Add(Parts.PortalPlatform, parts[Parts.PortalPlatform].GetModelPart().Bounds.Location);

            partOffset.Add(Parts.ScreenHolder, 0);
            partOffset.Add(Parts.VerticalFrame, 0);
            partOffset.Add(Parts.HorizontalBar, 0);

            detectorFrameGraph.Children = partsCollectoin;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromTicks(5000);
            timer.Tick += animation_tick;
        }

        private Model3DCollection copyCollection(Model3DCollection collection)
        {
            Model3DCollection retCollection = new Model3DCollection();

            foreach (Model3D model in collection)
                retCollection.Add(model);

            return retCollection;
        }

        public Vector3D GetScreenDirection()
        {
            return currentScreenDirection;
        }

        public Point3D GetCameraPosition()
        {
            Point3D point = parts[Parts.ScreenCameraPos].GetCameraPosition();
            return new Point3D(point.X, point.Y, point.Z);
        }

        public void MoveDetectFrame(DetectorFramePosition p, bool animate)
        {
            //positionsToSet[Parts.VerticalFrame] = (p.pointScreen.X - partStartPosition[Parts.VerticalFrame].X);
            //positionsToSet[Parts.HorizontalBar] = (p.pointScreen.Y - partStartPosition[Parts.ScreenCameraPos].Y);
            //positionsToSet[Parts.ScreenHolder] = (p.pointScreen.Z - partStartPosition[Parts.ScreenCameraPos].Z);
            //positionsToSet[Parts.VerticalFrame] = p.pointScreen.X;
            //positionsToSet[Parts.HorizontalBar] = p.pointScreen.Y;
            //positionsToSet[Parts.ScreenHolder]  = p.pointScreen.Z;
            //position = p;

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
                partDeltas[Parts.ScreenHolder] = (positionsToSet[Parts.ScreenHolder] - partOffset[Parts.ScreenHolder]) / 1000;
                verticalAngleDelta = p.verticalAngle - verticalAngle;
                horizontalAngle = p.horizontalAngle - horizontalAngle;

                timer.Start();
                isAnimated = true;
            }
        }

        private void animation_tick(object sender, EventArgs arg)
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

            horizontalAngle = checkedAngleHorizontal(out onRightAngle);
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
            //double offsetX = p.pointScreen.X - partStartPosition[Parts.VerticalFrame].X;
            partOffset[Parts.VerticalFrame] = p.pointScreen.X;

            //double offsetY = p.pointScreen.Y - (partStartPosition[Parts.ScreenCameraPos].Y + parts[Parts.ScreenCameraPos].Bounds().SizeY / 2);
            partOffset[Parts.HorizontalBar] = p.pointScreen.Y - parts[Parts.ScreenCameraPos].Bounds().SizeY / 2;

            //double offsetZ = p.pointScreen.Z - (partStartPosition[Parts.ScreenCameraPos].Z + parts[Parts.ScreenCameraPos].Bounds().SizeZ / 2);
            partOffset[Parts.ScreenHolder] = p.pointScreen.Z - parts[Parts.ScreenCameraPos].Bounds().SizeZ / 2;

            verticalAngle = p.verticalAngle;
            horizontalAngle = p.horizontalAngle;

            ConfirmPosition();
        }


        private void ConfirmPosition()
        {
            Transform3DGroup verticalFrameGroup = new Transform3DGroup();
            Transform3DGroup horizontalBarGroup = new Transform3DGroup();
            Transform3DGroup screenHolderGroup = new Transform3DGroup();
            Transform3DGroup screenRotatorGroup = new Transform3DGroup();
            Transform3DGroup screenGroup = new Transform3DGroup();
            Transform3DGroup camPositionCubeGroup = new Transform3DGroup();
            
            TranslateTransform3D T = new TranslateTransform3D(partOffset[Parts.VerticalFrame], 0, 0);
            verticalFrameGroup.Children.Add(T);
            parts[Parts.VerticalFrame].TranslateTransform3D(verticalFrameGroup);

            T = new TranslateTransform3D(0, partOffset[Parts.HorizontalBar], 0);
            horizontalBarGroup.Children.Add(T);
            horizontalBarGroup.Children.Add(verticalFrameGroup);
            parts[Parts.HorizontalBar].TranslateTransform3D(horizontalBarGroup);

            T = new TranslateTransform3D(0, 0, partOffset[Parts.ScreenHolder]);
            screenHolderGroup.Children.Add(T);
            screenHolderGroup.Children.Add(horizontalBarGroup);
            parts[Parts.ScreenHolder].TranslateTransform3D(screenHolderGroup);

            screenRotatorGroup.Children.Add(screenHolderGroup);
            parts[Parts.ScreenRotator].TranslateTransform3D(screenRotatorGroup);

            screenGroup.Children.Add(screenRotatorGroup);
            parts[Parts.Screen].TranslateTransform3D(screenGroup);
            parts[Parts.ScreenRotatePoint].TranslateTransform3D(screenHolderGroup);

            var horizRotate = GetHorizontalScreenRotate();
            var vertRotate = GetVerticalScreenRotate();
            parts[Parts.Screen].RotateTransform3D(horizRotate);
            parts[Parts.ScreenRotator].RotateTransform3D(vertRotate);
            parts[Parts.Screen].RotateTransform3D(vertRotate);


            camPositionCubeGroup.Children.Add(screenGroup);
            camPositionCubeGroup.Children.Add(horizRotate);
            camPositionCubeGroup.Children.Add(vertRotate);
            parts[Parts.ScreenCameraPos].TranslateTransform3D(camPositionCubeGroup);

            onPositionChanged?.Invoke();
        }

        public RotateTransform3D GetVerticalScreenRotate()
        {
            Point3D center = MathUtils.GetRectCenter(parts[Parts.ScreenRotatePoint].Bounds());
            RotateTransform3D R = new RotateTransform3D(new AxisAngleRotation3D(ZRotateAxis, MathUtils.RadiansToAngle(verticalAngle)), center);
            return R;
        }

        public RotateTransform3D GetHorizontalScreenRotate()
        {
            Point3D center = MathUtils.GetRectCenter(parts[Parts.ScreenRotatePoint].Bounds());
            RotateTransform3D R = new RotateTransform3D(new AxisAngleRotation3D(YRotateAxis, MathUtils.RadiansToAngle(horizontalAngle)), center);
            return R;
        }

        public void transformModel(Double x)
        {
            portalModel.Children[numberMesh].Transform = new TranslateTransform3D(0, x, 0);
        }

        public void addNumberMesh(int number)
        {
            this.numberMesh = number;
        }
        public Model3D GetDetectorFrameModel()
        {
            return detectorFrameGraph;
        }

        public Model3D GetDetectorFramePart(Parts part)
        {
            return parts[part].GetModelPart();
        }

        public void ResetTransforms()
        {
            foreach (Parts part in Enum.GetValues(typeof(Parts)))
            {
                partOffset[part] = 0;
            }
            verticalAngle = 0;
            horizontalAngle = 0;

            ConfirmPosition();
        }

        public Dictionary<Parts, double> GetCurrentOffsets()
        {
            return partOffset;
        }
    }
}

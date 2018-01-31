using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using InverseTest.Manipulator;
using InverseTest.Detail;
using InverseTest.Collision.Model;

namespace InverseTest.Collision
{
    public class AABB
    {
        IManipulatorModel manipulator;
        IDetectorFrame detectorFrame;
        DetailModel detail;
        Model3DGroup Det_platform;
        bool CollisionGJK = false;
        bool rdy = false;

        private List<Except> _ListExcept = new List<Except>(); //лист исключений
        private List<CollisionPair> _ListCollisions = new List<CollisionPair>(); //колизии ААВВ
        private Except _exc;

        public AABB(IManipulatorModel Manipulator, IDetectorFrame DetectorFrame, DetailModel Detail, Model3DGroup Platform)
        {
            this.manipulator = Manipulator;
            this.detectorFrame = DetectorFrame;
            this.detail = Detail;
            this.Det_platform = Platform;
        }

        public struct Except //структура для хранения исключений
        {
            public string S1;
            public string S2;

            public Except(string s1, string s2)
            {
                this.S1 = s1;
                this.S2 = s2;
            }
        }

        //(IManipulatorModel manipulator, IDetectorFrame detectorFrame, DetailModel detail, Model3DGroup platform)  
        public void MakeListExcept()//создаем список исключений самопересечений манипулятора и детектора
        {

            for (int i = 0; i < Enum.GetValues(typeof(ManipulatorV2.ManipulatorParts)).Length; i++) //самопересечение манипулятора
            {
                for (int j = i + 1; j < Enum.GetValues(typeof(ManipulatorV2.ManipulatorParts)).Length; j++)
                {
                    if (manipulator.GetManipulatorPart((ManipulatorV2.ManipulatorParts)(Enum.GetValues(typeof(ManipulatorV2.ManipulatorParts)).GetValue(i))).Bounds.
                        IntersectsWith(manipulator.GetManipulatorPart((ManipulatorV2.ManipulatorParts)(Enum.GetValues(typeof(ManipulatorV2.ManipulatorParts)).GetValue(j))).Bounds))
                    {
                        _exc = new Except(Enum.GetName(typeof(ManipulatorV2.ManipulatorParts), i), Enum.GetName(typeof(ManipulatorV2.ManipulatorParts), j));
                        _ListExcept.Add(_exc);
                    }
                }
            }

            for (int i = 0; i < Enum.GetValues(typeof(DetectorFrame.Parts)).Length; i++) //самопересечение детектора
            {
                for (int j = i + 1; j < Enum.GetValues(typeof(DetectorFrame.Parts)).Length; j++)
                {
                    if (detectorFrame.GetDetectorFramePart((DetectorFrame.Parts)(Enum.GetValues(typeof(DetectorFrame.Parts)).GetValue(i))).Bounds.
                        IntersectsWith(detectorFrame.GetDetectorFramePart((DetectorFrame.Parts)(Enum.GetValues(typeof(DetectorFrame.Parts)).GetValue(j))).Bounds))
                    {
                        _exc = new Except(Enum.GetName(typeof(DetectorFrame.Parts), i), Enum.GetName(typeof(DetectorFrame.Parts), j));
                        _ListExcept.Add(_exc);
                    }
                }

            }
            _exc = new Except("Platform", "Det_platform"); //дополнительные исключений найденые при тесте
            _ListExcept.Add(_exc);
            _exc = new Except("Screen", "ScreenCameraPos");
            _ListExcept.Add(_exc);
            rdy = true;
        }

        private bool CompareExcept(Except e) //проверка со списком исключений
        {
            bool IsExcec = false;
            for (int i = 0; i < _ListExcept.Count; i++)
            {
                if ((_ListExcept[i].S1 == e.S1 && _ListExcept[i].S2 == e.S2) || (_ListExcept[i].S1 == e.S2 && _ListExcept[i].S2 == e.S1) || (e.S1 == e.S2))
                { IsExcec = true; break; }
            }

            return IsExcec;
        }
        
        public List<CollisionPair> Find() //поиск коллизий ААВВ
        {
            if (rdy)
            {

                List<CollisionPair> collisoins = new List<CollisionPair>();
                Model3D part1;
                Model3D part2;
                foreach (ManipulatorV2.ManipulatorParts Manip_part in Enum.GetValues(typeof(ManipulatorV2.ManipulatorParts))) //все части манипулятора...
                {

                    foreach (ManipulatorV2.ManipulatorParts Manip_part2 in Enum.GetValues(typeof(ManipulatorV2.ManipulatorParts))) //...со всеми частями манипулятора
                    {
                        part1 = manipulator.GetManipulatorPart(Manip_part);
                        part2 = manipulator.GetManipulatorPart(Manip_part2);
                        if (Intersects(part1,part2))
                        {
                            _exc = new Except(Manip_part.ToString(), Manip_part2.ToString());
                            if (!CompareExcept(_exc))
                            {
                                CollisionPair pair = new CollisionPair(
                                    new Model3DCollision(Manip_part.ToString(), part1 as Model3DGroup),
                                    new Model3DCollision(Manip_part2.ToString(), part2 as Model3DGroup)
                                    );
                                _ListCollisions.Add(pair);
                            }

                        }
                    }
                    
                    part1 = manipulator.GetManipulatorPart(Manip_part);
                    part2 = detail.GetModel();
                        if (Intersects(part1, part2)) //манипулятор с моделью
                        {
                            _exc = new Except(Manip_part.ToString(), detail.ToString());
                            if (!CompareExcept(_exc))
                            {
                                CollisionPair pair = new CollisionPair(
                                    new Model3DCollision(Manip_part.ToString(), part1 as Model3DGroup),
                                    new Model3DCollision(detail.ToString(), part2 as Model3DGroup)
                                    );
                                _ListCollisions.Add(pair);
                            }
                        }

                        
                    part1 = manipulator.GetManipulatorPart(Manip_part);
                    part2 = Det_platform;
                    if (Intersects(part1, part2)) //манипулятор с платформой
                    {
                        _exc = new Except(Manip_part.ToString(), "Det_platform");
                        if (!CompareExcept(_exc))
                        {
                            CollisionPair pair = new CollisionPair(
                            new Model3DCollision(Manip_part.ToString(), part1 as Model3DGroup),
                            new Model3DCollision("Platform", part2 as Model3DGroup)
                            );

                            _ListCollisions.Add(pair);
                        }
                    }
                }

                foreach (DetectorFrame.Parts part_frame in Enum.GetValues(typeof(DetectorFrame.Parts)))
                {
                    foreach (DetectorFrame.Parts part_frame2 in Enum.GetValues(typeof(DetectorFrame.Parts))) //детектор со всеми частями детектора
                    {
                        part1 = detectorFrame.GetDetectorFramePart(part_frame);
                        part2 = detectorFrame.GetDetectorFramePart(part_frame2);

                        if (Intersects(part1,part2))
                        {
                            _exc = new Except(part_frame.ToString(), part_frame2.ToString());
                            if (!CompareExcept(_exc))
                            {
                                CollisionPair pair = new CollisionPair(
                                new Model3DCollision(part_frame.ToString(), part1 as Model3DGroup),
                                new Model3DCollision(part_frame2.ToString(), part2 as Model3DGroup)
                                );
                                _ListCollisions.Add(pair);
                            }
                        }


                    }


                    part1 = detectorFrame.GetDetectorFramePart(part_frame);
                    part2 = detail.GetModel();
                    if (Intersects(part1, part2)) //детектор с деталью
                    {
                        _exc = new Except(part_frame.ToString(), detail.ToString());
                        if (!CompareExcept(_exc))
                        {
                            CollisionPair pair = new CollisionPair(
                            new Model3DCollision(part_frame.ToString(), part1 as Model3DGroup),
                            new Model3DCollision("Detail", part2 as Model3DGroup)
                            );
                            _ListCollisions.Add(pair);
                        }
                    }


                    part1 = detectorFrame.GetDetectorFramePart(part_frame);
                    part2 = Det_platform;
                    if (Intersects(part1, part2)) //детектор с платформой 
                    {
                        _exc = new Except(part_frame.ToString(), "Det_platform");
                        if (!CompareExcept(_exc))
                        {
                            CollisionPair pair = new CollisionPair(
                            new Model3DCollision(part_frame.ToString(), part1 as Model3DGroup),
                            new Model3DCollision("Platform", part2 as Model3DGroup)
                            );
                            _ListCollisions.Add(pair);
                        }
                    }
                }

                foreach (ManipulatorV2.ManipulatorParts Manip_part in Enum.GetValues(typeof(ManipulatorV2.ManipulatorParts))) //все части манипулятора...
                {
                    foreach (DetectorFrame.Parts part_frame in Enum.GetValues(typeof(DetectorFrame.Parts))) //со всеми частями детектора
                    {

                        part1 = manipulator.GetManipulatorPart(Manip_part);
                        part2 = detectorFrame.GetDetectorFramePart(part_frame);
                        if (Intersects(part1, part2))
                        {
                            _exc = new Except(Manip_part.ToString(), part_frame.ToString());
                            if (!CompareExcept(_exc))
                            {
                                CollisionPair pair = new CollisionPair(
                                new Model3DCollision(part_frame.ToString(), part1 as Model3DGroup),
                                new Model3DCollision("Platform", part2 as Model3DGroup)
                                );
                                _ListCollisions.Add(pair);
                            }
                        }
                    }
                }
              
            }

            return _ListCollisions;
        }
        
        private bool Intersects(Model3D model1, Model3D model2)
        {
            return model1.Bounds.IntersectsWith(model2.Bounds);
        }

    }
}

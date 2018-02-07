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
        bool rdy = true;

        private List<Except> _ListExcept = new List<Except>(); //лист исключений
        private Except _exc;


        public AABB()
        {
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

        
        public void MakeListExcept(IManipulatorModel manipulator, IDetectorFrame detectorFrame, DetailModel detail, Model3DGroup platform)//создаем список исключений самопересечений манипулятора и детектора
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

        public List<CollisionPair> Find(SceneSnapshot sceneSnapshot) //поиск коллизий ААВВ
        {
            if (rdy)
            {
                
                DetailSnapshot detail = sceneSnapshot.detailSnapshot;
                ManipulatorSnapshot manipulator = sceneSnapshot.manipSnapshot;
                PortalSnapshot portal = sceneSnapshot.portalSnapshot;

                List<CollisionPair> collisoins = new List<CollisionPair>();


                foreach (KeyValuePair<ManipulatorV2.ManipulatorParts, PartShape> part1 in manipulator.bounds) //все части манипулятора...
                {

                    foreach (KeyValuePair<ManipulatorV2.ManipulatorParts, PartShape> part2 in manipulator.bounds) //...со всеми частями манипулятора
                    {

                        if (Intersects(part1.Value,part2.Value))
                        {
                            _exc = new Except(part1.Key.ToString(), part2.Key.ToString());
                            if (!CompareExcept(_exc))
                            {
                                CollisionPair pair = new CollisionPair(
                                    new Model3DCollision(part1.Key.ToString(), part1.Value),
                                    new Model3DCollision(part2.Key.ToString(), part2.Value)
                                    );

                                collisoins.Add(pair);
                            }

                        }
                    }

                    if (Intersects(part1.Value,detail.detailShape)) //манипулятор с моделью
                    {
                        _exc = new Except(part1.ToString(), detail.ToString());
                        if (!CompareExcept(_exc))
                        {
                            CollisionPair pair = new CollisionPair(
                                new Model3DCollision(part1.Key.ToString(), part1.Value),
                                new Model3DCollision("Detail", detail.detailShape)
                                );

                            collisoins.Add(pair);
                        }
                    }
                }


                //TODO МАНИПУЛЯТОР И ПЛАТФОРМА 

                //    part2 = Det_platform;
                //    if (Intersects(part1, part2)) //манипулятор с платформой
                //    {
                //        _exc = new Except(Manip_part.ToString(), "Det_platform");
                //        if (!CompareExcept(_exc))
                //        {
                //            CollisionPair pair = new CollisionPair(
                //            new Model3DCollision(Manip_part.ToString(), part1 as Model3DGroup),
                //            new Model3DCollision("Platform", part2 as Model3DGroup)
                //            );

                //            _ListCollisions.Add(pair);
                //        }
                //    }
                //}

                foreach (KeyValuePair<DetectorFrame.Parts, PartShape> portalPart1 in portal.bounds)
                {
                    foreach (KeyValuePair<DetectorFrame.Parts, PartShape> portalPart2 in portal.bounds) //детектор со всеми частями детектора
                    {
                        if (Intersects(portalPart1.Value, portalPart2.Value))
                        {
                            _exc = new Except(portalPart1.Key.ToString(), portalPart2.Key.ToString());
                            if (!CompareExcept(_exc))
                            {
                                CollisionPair pair = new CollisionPair(
                                new Model3DCollision(portalPart1.Key.ToString(), portalPart1.Value),
                                new Model3DCollision(portalPart2.Key.ToString(), portalPart2.Value)
                                );
                                collisoins.Add(pair);
                            }
                        }


                    }

                                     
                    if (Intersects(portalPart1.Value, detail.detailShape)) //детектор с деталью
                    {
                        _exc = new Except(portalPart1.ToString(), detail.ToString());
                        if (!CompareExcept(_exc))
                        {
                            CollisionPair pair = new CollisionPair(
                            new Model3DCollision(portalPart1.Key.ToString(), portalPart1.Value),
                            new Model3DCollision("Detail", detail.detailShape)
                            );
                            collisoins.Add(pair);
                        }
                    }

                    //TODO ДЕТЕКТОР С ПЛАТФОРМОЙ
                    //    if (Intersects(part1, part2)) //детектор с платформой 
                    //    {
                    //        _exc = new Except(part_frame.ToString(), "Det_platform");
                    //        if (!CompareExcept(_exc))
                    //        {
                    //            CollisionPair pair = new CollisionPair(
                    //            new Model3DCollision(part_frame.ToString(), part1 as Model3DGroup),
                    //            new Model3DCollision("Det_Platform", part2 as Model3DGroup)
                    //            );
                    //            _ListCollisions.Add(pair);
                    //        }
                    //    }
                    //}

                    foreach (KeyValuePair<ManipulatorV2.ManipulatorParts, PartShape> part in manipulator.bounds) //все части манипулятора...
                    {
                        foreach (KeyValuePair<DetectorFrame.Parts, PartShape> portalPart in portal.bounds) //со всеми частями детектора
                        {                           
                            if (Intersects(part.Value,portalPart.Value))
                            {
                                _exc = new Except(part.Key.ToString(), portalPart.Key.ToString());
                                if (!CompareExcept(_exc))
                                {
                                    CollisionPair pair = new CollisionPair(
                                    new Model3DCollision(part.Key.ToString(), part.Value),
                                    new Model3DCollision(portalPart.Key.ToString(), portalPart.Value)
                                    );
                                    collisoins.Add(pair);
                                }
                            }
                        }
                    }

                }
                return collisoins;

            }
            else return new List<CollisionPair>();
        }

        private bool Intersects(PartShape s1, PartShape s2)
        {
            return s1.bounds.IntersectsWith(s1.bounds);
        }


    }
}

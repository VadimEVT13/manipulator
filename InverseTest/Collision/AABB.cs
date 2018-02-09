using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using InverseTest.Manipulator;
using InverseTest.Detail;
using InverseTest.Collision.Model;
using InverseTest.Collision.Mappers;

namespace InverseTest.Collision
{
    public class AABB
    {
        bool rdy = false;

        //private List<Except> _ListExcept = new List<Except>(); //лист исключений
        private HashSet<Except> _ListExcept = new HashSet<Except>();
        private Except _exc;


        public AABB()
        {
        }

        public struct Except : IEquatable<Except> //структура для хранения исключений
        {
            public string S1;
            public string S2;


            public Except(string s1, string s2)
            {
                this.S1 = s1;
                this.S2 = s2;
            }

            public override int GetHashCode()
            {
                return 31 * this.S1.GetHashCode() + 31 * this.S2.GetHashCode();
            }
            
            public bool Equals(Except exept)
            {
                return (this.S1.Equals(exept.S1) && this.S2.Equals(exept.S2)) || (this.S2.Equals(exept.S1) && (this.S1.Equals(exept.S2)));
            }
        }


        public void MakeListExcept(IManipulatorModel manipulator, IDetectorFrame detectorFrame, DetailModel detail, Model3D platform)//создаем список исключений самопересечений манипулятора и детектора
        {

            for (int i = 0; i < Enum.GetValues(typeof(ManipulatorV2.ManipulatorParts)).Length; i++) //самопересечение манипулятора
            {
                for (int j = i; j < Enum.GetValues(typeof(ManipulatorV2.ManipulatorParts)).Length; j++)
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
                for (int j = i; j < Enum.GetValues(typeof(DetectorFrame.Parts)).Length; j++)
                {
                    if (detectorFrame.GetDetectorFramePart((DetectorFrame.Parts)(Enum.GetValues(typeof(DetectorFrame.Parts)).GetValue(i))).Bounds.
                        IntersectsWith(detectorFrame.GetDetectorFramePart((DetectorFrame.Parts)(Enum.GetValues(typeof(DetectorFrame.Parts)).GetValue(j))).Bounds))
                    {
                        _exc = new Except(Enum.GetName(typeof(DetectorFrame.Parts), i), Enum.GetName(typeof(DetectorFrame.Parts), j));
                        _ListExcept.Add(_exc);
                    }
                }

            }
            _exc = new Except(DetectorFrame.Parts.Platform.ToString(), DetailPlatformMapper.platformName); //дополнительные исключений найденые при тесте
            _ListExcept.Add(_exc);
            _exc = new Except(DetectorFrame.Parts.Screen.ToString(), DetectorFrame.Parts.ScreenCameraPos.ToString());
            _ListExcept.Add(_exc);
            rdy = true;
        }

        private bool CompareExcept(Except e) //проверка со списком исключений
        {
            return _ListExcept.Contains(e);
        }

        public List<CollisionPair> Find(SceneSnapshot sceneSnapshot) //поиск коллизий ААВВ
        {
            if (rdy)
            {

                DetailSnapshot detail = sceneSnapshot.detailSnapshot;
                ManipulatorSnapshot manipulator = sceneSnapshot.manipSnapshot;
                PortalSnapshot portal = sceneSnapshot.portalSnapshot;
                DetailPlatformSnapshot detailPlatform = sceneSnapshot.detailPlatformSnapshot;

                List<CollisionPair> collisoins = new List<CollisionPair>();

                /*foreach (KeyValuePair<ManipulatorV2.ManipulatorParts, PartShape> part1 in manipulator.bounds) //все части манипулятора...
                {
                    foreach (KeyValuePair<ManipulatorV2.ManipulatorParts, PartShape> part2 in manipulator.bounds) //...со всеми частями манипулятора*/
                for (int i = 0; i < manipulator.parts.Count; i++) //самопересечение манипулятора
                {
                    for (int j = i; j < manipulator.parts.Count; j++)
                    {

                        if (Intersects(manipulator.parts[i], manipulator.parts[j]))
                        {
                            _exc = new Except(manipulator.parts[i].partName, manipulator.parts[j].partName);
                            if (!CompareExcept(_exc))
                            {
                                CollisionPair pair = new CollisionPair(
                                    new Model3DCollision(manipulator.parts[i].partName, manipulator.parts[i]),
                                    new Model3DCollision(manipulator.parts[j].partName, manipulator.parts[j])
                                    );

                                collisoins.Add(pair);
                            }

                        }
                    }

                    if (Intersects(manipulator.parts[i], detail.detailShape)) //манипулятор с моделью
                    {
                        _exc = new Except(manipulator.parts[i].partName, detail.detailShape.partName);
                        if (!CompareExcept(_exc))
                        {
                            CollisionPair pair = new CollisionPair(
                                new Model3DCollision(manipulator.parts[i].partName, manipulator.parts[i]),
                                new Model3DCollision(detail.detailShape.partName, detail.detailShape)
                                );

                            collisoins.Add(pair);
                        }
                    }
                    //}


                    //TODO МАНИПУЛЯТОР И ПЛАТФОРМА

                    //part2 = Det_platform;
                    if (Intersects(manipulator.parts[i], detailPlatform.detailPlatformShape)) //манипулятор с платформой
                    {
                        _exc = new Except(manipulator.parts[i].partName, detailPlatform.detailPlatformShape.partName);
                        if (!CompareExcept(_exc))
                        {
                            CollisionPair pair = new CollisionPair(
                            new Model3DCollision(manipulator.parts[i].partName, manipulator.parts[i]),
                            new Model3DCollision(detailPlatform.detailPlatformShape.partName, detailPlatform.detailPlatformShape)
                            );

                            collisoins.Add(pair);
                        }
                    }
                }

                for (int i = 0; i < portal.parts.Count; i++) //детектор
                {
                    for (int j = i; j < portal.parts.Count; j++) //с детектором
                    {
                        if (Intersects(portal.parts[i], portal.parts[j]))
                        {
                            _exc = new Except(portal.parts[i].partName, portal.parts[j].partName);
                            if (!CompareExcept(_exc))
                            {
                                CollisionPair pair = new CollisionPair(
                                new Model3DCollision(portal.parts[i].partName, portal.parts[i]),
                                new Model3DCollision(portal.parts[j].partName, portal.parts[j])
                                );
                                collisoins.Add(pair);
                            }
                        }


                    }


                    if (Intersects(portal.parts[i], detail.detailShape)) //детектор с деталью
                    {
                        _exc = new Except(portal.parts[i].partName, detail.detailShape.partName);
                        if (!CompareExcept(_exc))
                        {
                            CollisionPair pair = new CollisionPair(
                            new Model3DCollision(portal.parts[i].partName, portal.parts[i]),
                            new Model3DCollision(detail.detailShape.partName, detail.detailShape)
                            );
                            collisoins.Add(pair);
                        }
                    }

                    // ДЕТЕКТОР С ПЛАТФОРМОЙ
                    if (Intersects(portal.parts[i], detailPlatform.detailPlatformShape)) //детектор с платформой 
                    {
                        _exc = new Except(portal.parts[i].partName, detailPlatform.detailPlatformShape.partName);
                        if (!CompareExcept(_exc))
                        {
                            CollisionPair pair = new CollisionPair(
                            new Model3DCollision(portal.parts[i].partName, portal.parts[i]),
                            new Model3DCollision(detailPlatform.detailPlatformShape.partName, detailPlatform.detailPlatformShape)
                            );
                            collisoins.Add(pair);
                        }
                    }
                }


                for (int i = 0; i < manipulator.parts.Count; i++) //манипулятор
                {
                    for (int j = 0; j < portal.parts.Count; j++) //с детектором
                    {
                        if (Intersects(manipulator.parts[i], portal.parts[j]))
                        {
                            _exc = new Except(manipulator.parts[i].partName, portal.parts[j].partName);
                            if (!CompareExcept(_exc))
                            {
                                CollisionPair pair = new CollisionPair(
                                new Model3DCollision(manipulator.parts[i].partName, manipulator.parts[i]),
                                new Model3DCollision(portal.parts[j].partName, portal.parts[j])
                                );
                                collisoins.Add(pair);
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
            return s1.bounds.IntersectsWith(s2.bounds);
        }


    }
}

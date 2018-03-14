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

        private HashSet<Except> _ListExcept = new HashSet<Except>();
        private Except _exc;


        public AABB()
        {
        }

        public struct Except : IEquatable<Except> //структура для хранения исключений
        {
            public Enum S1;
            public Enum S2;


            public Except(Enum s1, Enum s2)
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

            foreach (ManipulatorParts i in Enum.GetValues(typeof(ManipulatorParts))) //самопересечение манипулятора
            {
                
                foreach (ManipulatorParts j in Enum.GetValues(typeof(ManipulatorParts)))
                {
                    if (manipulator.GetManipulatorPart(i).Bounds.
                        IntersectsWith(manipulator.GetManipulatorPart(j).Bounds))
                    {
                        _exc = new Except(i,j);
                        _ListExcept.Add(_exc);
                    }
                }
            }

            foreach (DetectorFrame.Parts p1 in Enum.GetValues(typeof(DetectorFrame.Parts))) //самопересечение детектора
            {
                foreach (DetectorFrame.Parts p2 in Enum.GetValues(typeof(DetectorFrame.Parts)))
                {
                    if (detectorFrame.GetDetectorFramePart(p1).Bounds.
                        IntersectsWith(detectorFrame.GetDetectorFramePart(p2).Bounds))
                    {
                        _exc = new Except(p1, p2);
                        _ListExcept.Add(_exc);
                    }
                }

            }
              //дополнительные исключений найденые при тесте
            _ListExcept.Add(new Except(ExtraPartsEnum.DETAIL, ExtraPartsEnum.DETAIL_PLATFORM));
            _ListExcept.Add(new Except(ExtraPartsEnum.DETAIL_PLATFORM, ManipulatorParts.Platform));
            _ListExcept.Add(new Except(DetectorFrame.Parts.Screen, DetectorFrame.Parts.ScreenCameraPos));
            _ListExcept.Add(new Except(ExtraPartsEnum.DETAIL_PLATFORM, DetectorFrame.Parts.PortalPlatform));
            _ListExcept.Add(new Except(ManipulatorParts.Platform, DetectorFrame.Parts.PortalPlatform));
            _ListExcept.Add(new Except(ExtraPartsEnum.DETAIL, ManipulatorParts.Platform));
            _ListExcept.Add(new Except(ExtraPartsEnum.DETAIL, DetectorFrame.Parts.PortalPlatform));

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

                for (int i = 0; i < manipulator.parts.Count; i++) //самопересечение манипулятора
                {
                    for (int j = i; j < manipulator.parts.Count; j++)
                    {
                        if (Intersects(manipulator.parts[i], manipulator.parts[j]))
                        {
                            _exc = new Except(manipulator.parts[i].PartType, manipulator.parts[j].PartType);
                            if (!CompareExcept(_exc))
                            {
                                CollisionPair pair = new CollisionPair(
                                    new Model3DCollision(manipulator.parts[i].PartType, manipulator.parts[i]),
                                    new Model3DCollision(manipulator.parts[j].PartType, manipulator.parts[j])
                                    );

                                collisoins.Add(pair);
                            }

                        }
                    }

                    if (Intersects(manipulator.parts[i], detail.detailShape)) //манипулятор с моделью
                    {
                        _exc = new Except(manipulator.parts[i].PartType, detail.detailShape.PartType);
                        if (!CompareExcept(_exc))
                        {
                            CollisionPair pair = new CollisionPair(
                                new Model3DCollision(manipulator.parts[i].PartType, manipulator.parts[i]),
                                new Model3DCollision(detail.detailShape.PartType, detail.detailShape)
                                );

                            collisoins.Add(pair);
                        }
                    }
                    //}


                    //TODO МАНИПУЛЯТОР И ПЛАТФОРМА

                    //part2 = Det_platform;
                    if (Intersects(manipulator.parts[i], detailPlatform.detailPlatformShape)) //манипулятор с платформой
                    {
                        _exc = new Except(manipulator.parts[i].PartType, detailPlatform.detailPlatformShape.PartType);
                        if (!CompareExcept(_exc))
                        {
                            CollisionPair pair = new CollisionPair(
                            new Model3DCollision(manipulator.parts[i].PartType, manipulator.parts[i]),
                            new Model3DCollision(detailPlatform.detailPlatformShape.PartType, detailPlatform.detailPlatformShape)
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
                            _exc = new Except(portal.parts[i].PartType, portal.parts[j].PartType);
                            if (!CompareExcept(_exc))
                            {
                                CollisionPair pair = new CollisionPair(
                                new Model3DCollision(portal.parts[i].PartType, portal.parts[i]),
                                new Model3DCollision(portal.parts[j].PartType, portal.parts[j])
                                );
                                collisoins.Add(pair);
                            }
                        }


                    }


                    if (Intersects(portal.parts[i], detail.detailShape)) //детектор с деталью
                    {
                        _exc = new Except(portal.parts[i].PartType, detail.detailShape.PartType);
                        if (!CompareExcept(_exc))
                        {
                            CollisionPair pair = new CollisionPair(
                            new Model3DCollision(portal.parts[i].PartType, portal.parts[i]),
                            new Model3DCollision(detail.detailShape.PartType, detail.detailShape)
                            );
                            collisoins.Add(pair);
                        }
                    }

                    // ДЕТЕКТОР С ПЛАТФОРМОЙ
                    if (Intersects(portal.parts[i], detailPlatform.detailPlatformShape)) //детектор с платформой 
                    {
                        _exc = new Except(portal.parts[i].PartType, detailPlatform.detailPlatformShape.PartType);
                        if (!CompareExcept(_exc))
                        {
                            CollisionPair pair = new CollisionPair(
                            new Model3DCollision(portal.parts[i].PartType, portal.parts[i]),
                            new Model3DCollision(detailPlatform.detailPlatformShape.PartType, detailPlatform.detailPlatformShape)
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
                            _exc = new Except(manipulator.parts[i].PartType, portal.parts[j].PartType);
                            if (!CompareExcept(_exc))
                            {
                                CollisionPair pair = new CollisionPair(
                                new Model3DCollision(manipulator.parts[i].PartType, manipulator.parts[i]),
                                new Model3DCollision(portal.parts[j].PartType, portal.parts[j])
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
            return s1.Bounds.IntersectsWith(s2.Bounds);
        }


    }
}

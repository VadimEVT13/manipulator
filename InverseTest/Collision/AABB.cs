using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using InverseTest.Manipulator;
using InverseTest.Detail;


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
        private List<Except> _ListCollisions = new List<Except>(); //колизии ААВВ
        private List<Except> _ListCollisionsGJK = new List<Except>(); //коллизии GJK
        private Except _exc;

        public AABB()
        {

        }

        public struct Except //структура для хранения исключений
        {
            public string S1;
            public string S2;

            public Except (string s1, string s2)
            {
                this.S1 = s1;
                this.S2 = s2;
            }
        }

        //(IManipulatorModel manipulator, IDetectorFrame detectorFrame, DetailModel detail, Model3DGroup platform)  
        public void MakeListExcept(IManipulatorModel Manipulator, IDetectorFrame DetectorFrame, DetailModel Detail, Model3DGroup Platform)//создаем список исключений самопересечений манипулятора и детектора
        {
            this.manipulator = Manipulator;
            this.detectorFrame = DetectorFrame;
            this.detail = Detail;
            this.Det_platform = Platform;

            for (int i=0;i< Enum.GetValues(typeof(ManipulatorV2.ManipulatorParts)).Length; i++) //самопересечение манипулятора
            {
                for (int j = i+1; j < Enum.GetValues(typeof(ManipulatorV2.ManipulatorParts)).Length; j++)
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

        private bool CompareExcept (Except e) //проверка со списком исключений
        {
            bool IsExcec = false;
            for (int i = 0; i < _ListExcept.Count; i++)
            {
                if ((_ListExcept[i].S1 == e.S1 && _ListExcept[i].S2 == e.S2) || (_ListExcept[i].S1 == e.S2 && _ListExcept[i].S2 == e.S1) || (e.S1 == e.S2))
                { IsExcec = true; break; }
            }

            return IsExcec;
        }

        private bool DataForGJK (object _Obj_1, object _Obj_2) // отправка данных на GJK
        {
            
            CollisionGJK = false;
            HullsV2 hull = new HullsV2();
            object[] objects = new object[] { _Obj_1, _Obj_2 };

            for (int i=0; i<2; i++)
            {
                if (objects[i] is ManipulatorV2.ManipulatorParts)
                {
                    hull.BuildShell((Model3DGroup)manipulator.GetManipulatorPart((ManipulatorV2.ManipulatorParts)objects[i]));
                }

                if (objects[i] is DetectorFrame.Parts)
                {
                    hull.BuildShell((Model3DGroup)detectorFrame.GetDetectorFramePart((DetectorFrame.Parts)objects[i]));
                }

                if (objects[i] is DetailModel)
                {
                    Model3DGroup tmp_group = new Model3DGroup();
                    tmp_group.Children.Add(detail.GetModel());
                    hull.BuildShell(tmp_group);
                }

                if (objects[i] is Model3DGroup)
                {
                    hull.BuildShell((Model3DGroup)objects[i]);
                }
            }
            CollisionGJK = hull.find();  //поиск пересечений GJK
            if (hull.find())
            {
                CollisionGJK = true;
                hull.DisplayConvexHull(); //показать оболочки столкнувшихся моделей (знать бы еще как потом убирать их)
            }
            //hull.clear();
            return CollisionGJK;
        }

        public List<Except> Find(IManipulatorModel Manipulator, IDetectorFrame DetectorFrame, DetailModel Detail, Model3DGroup Platform) //поиск коллизий ААВВ
        {
            if (rdy)
            {
                _ListCollisions.Clear();
                _ListCollisionsGJK.Clear();
                this.manipulator = Manipulator;
                this.detectorFrame = DetectorFrame;
                this.detail = Detail;
                this.Det_platform = Platform;

                foreach (ManipulatorV2.ManipulatorParts Manip_part in Enum.GetValues(typeof(ManipulatorV2.ManipulatorParts))) //все части манипулятора...
                {
                    foreach (ManipulatorV2.ManipulatorParts Manip_part2 in Enum.GetValues(typeof(ManipulatorV2.ManipulatorParts))) //...со всеми частями манипулятора
                    {
                        if (manipulator.GetManipulatorPart(Manip_part).Bounds.IntersectsWith(manipulator.GetManipulatorPart(Manip_part2).Bounds))
                        {
                            _exc = new Except(Manip_part.ToString(), Manip_part2.ToString());
                            if (!CompareExcept(_exc))
                            {
                                _ListCollisions.Add(_exc);

                                if (DataForGJK(Manip_part, Manip_part2)) _ListCollisionsGJK.Add(_exc);
                            }

                        }
                    }

                    if (manipulator.GetManipulatorPart(Manip_part).Bounds.IntersectsWith(detail.GetModel().Bounds)) //манипулятор с моделью
                    {
                        _exc = new Except(Manip_part.ToString(), detail.ToString());
                        if (!CompareExcept(_exc))
                        {
                            _ListCollisions.Add(_exc);

                            if (DataForGJK(Manip_part, detail)) _ListCollisionsGJK.Add(_exc);
                        }
                    }

                    if (manipulator.GetManipulatorPart(Manip_part).Bounds.IntersectsWith(Det_platform.Bounds)) //манипулятор с платформой
                    {
                        _exc = new Except(Manip_part.ToString(), "Det_platform");
                        if (!CompareExcept(_exc))
                        {
                            _ListCollisions.Add(_exc);

                            if (DataForGJK(Manip_part, Det_platform)) _ListCollisionsGJK.Add(_exc);
                        }
                    }
                }


                foreach (DetectorFrame.Parts part_frame in Enum.GetValues(typeof(DetectorFrame.Parts)))
                {
                    foreach (DetectorFrame.Parts part_frame2 in Enum.GetValues(typeof(DetectorFrame.Parts))) //детектор со всеми частями детектора
                    {
                        if (detectorFrame.GetDetectorFramePart(part_frame).Bounds.IntersectsWith(detectorFrame.GetDetectorFramePart(part_frame2).Bounds))
                        {
                            _exc = new Except(part_frame.ToString(), part_frame2.ToString());
                            if (!CompareExcept(_exc))
                            {
                                _ListCollisions.Add(_exc);

                                if (DataForGJK(part_frame, part_frame2)) _ListCollisionsGJK.Add(_exc);
                            }
                        }


                    }

                    if (detectorFrame.GetDetectorFramePart(part_frame).Bounds.IntersectsWith(detail.GetModel().Bounds)) //детектор с деталью
                    {
                        _exc = new Except(part_frame.ToString(), detail.ToString());
                        if (!CompareExcept(_exc))
                        {
                            _ListCollisions.Add(_exc);

                            if (DataForGJK(part_frame, detail)) _ListCollisionsGJK.Add(_exc);
                        }
                    }

                    if (detectorFrame.GetDetectorFramePart(part_frame).Bounds.IntersectsWith(Det_platform.Bounds)) //детектор с платформой 
                    {
                        _exc = new Except(part_frame.ToString(), "Det_platform");
                        if (!CompareExcept(_exc))
                        {
                            _ListCollisions.Add(_exc);

                            if (DataForGJK(part_frame, Det_platform)) _ListCollisionsGJK.Add(_exc);
                        }
                    }
                }

                foreach (ManipulatorV2.ManipulatorParts Manip_part in Enum.GetValues(typeof(ManipulatorV2.ManipulatorParts))) //все части манипулятора...
                {
                    foreach (DetectorFrame.Parts part_frame in Enum.GetValues(typeof(DetectorFrame.Parts))) //со всеми частями детектора
                    {
                        if (manipulator.GetManipulatorPart(Manip_part).Bounds.IntersectsWith(detectorFrame.GetDetectorFramePart(part_frame).Bounds))
                        {
                            _exc = new Except(Manip_part.ToString(), part_frame.ToString());
                            if (!CompareExcept(_exc))
                            {
                                _ListCollisions.Add(_exc);

                                if (DataForGJK(Manip_part, part_frame)) _ListCollisionsGJK.Add(_exc);
                            }
                        }
                    }
                }
                return _ListCollisionsGJK;
            }
            else return new List<Except>();
            
        }
    }
}

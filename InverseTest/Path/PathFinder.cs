using InverseTest.Frame;
using InverseTest.Manipulator;
using InverseTest.Manipulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Roentgen.Path
{
    public class PortalAngle
    {
        public double O1
        {
            get { return _o1; }
            set { _o1 = value; }
        }
        private double _o1 = 0;

        public double O2
        {
            get { return _o2; }
            set { _o2 = value; }
        }
        private double _o2 = 0;

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }
        private double _x = 0;

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }
        private double _y = 0;

        public double Z
        {
            get { return _z; }
            set { _z = value; }
        }
        private double _z = 0;

        public PortalAngle(PortalAngle A)
        {
            this.O1 = A.O1;
            this.O2 = A.O2;
            this.X = A.X;
            this.Y = A.Y;
            this.Z = A.Z;
        }

        public PortalAngle()
        {
            O1 = 0;
            O2 = 0;
            X = 0;
            Y = 0;
            Z = 0;
        }

        public override bool Equals(object obj)
        {
            bool flag =
                (O1 == ((PortalAngle)obj).O1) &
                (O2 == ((PortalAngle)obj).O2) &
                (X == ((PortalAngle)obj).X) &
                (Y == ((PortalAngle)obj).Y) &
                (Z == ((PortalAngle)obj).Z);
            return flag;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    class Element : IEquatable<Element>
    {
        public Point3D current_point { get; set; }              // Текущая точка
        public Angle3D angle
        {
            get { return _angle; }
            set { _angle = value; }
        }                     // Текущие углы
        private Angle3D _angle = null;
        public Element parrent { get; set; }                    // Родительский элемент
        public double cost                                      // Стоимость точки
        {
            get { return cost_G + cost_H; }
            set { cost = value; }
        }
        public double cost_G { get; set; }                      // Стоимость по хопам
        public double cost_H { get; set; }                      // Стоимость по дистанции

        bool IEquatable<Element>.Equals(Element other)          // Реализация интерфейса сравнения
        {
            return Equals(current_point, other.current_point);
        }
    }

    class Element_Comparer : IEqualityComparer<Element>
    {
        public bool Equals(Element x, Element y)
        {
            return x.current_point.Equals(y.current_point);
        }

        public int GetHashCode(Element obj)
        {
            return obj.GetHashCode();
        }
    }

    class Element_5DOF : IEquatable<Element>
    {
        public Angle3D angle
        {
            get { return _angle; }
            set { _angle = value; }
        }                     // Текущие углы
        private Angle3D _angle = null;
        public Element_5DOF parrent { get; set; }                    // Родительский элемент
        public double cost                                      // Стоимость точки
        {
            get { return cost_G + cost_H; }
            set { cost = value; }
        }
        public double cost_G { get; set; }                      // Стоимость по хопам
        public double cost_H { get; set; }                      // Стоимость по дистанции

        bool IEquatable<Element>.Equals(Element other)          // Реализация интерфейса сравнения
        {
            return Equals(angle, other.angle);
        }
    }

    class Element_5DOF_Comparer : IEqualityComparer<Element_5DOF>
    {
        public bool Equals(Element_5DOF x, Element_5DOF y)
        {
            return x.angle.Equals(y.angle);
        }

        public int GetHashCode(Element_5DOF obj)
        {
            return obj.GetHashCode();
        }
    }

    class Element_Portal : IEquatable<Element_Portal>
    {
        public PortalAngle portal
        {
            get { return _portal; }
            set { _portal = value; }
        }                     // Текущие углы
        private PortalAngle _portal = null;
        public Element_Portal parrent { get; set; }                     // Родительский элемент
        public double cost                                              // Стоимость точки
        {
            get { return cost_G + cost_H; }
            set { cost = value; }
        }
        public double cost_G { get; set; }                              // Стоимость по хопам
        public double cost_H { get; set; }                              // Стоимость по дистанции

        bool IEquatable<Element_Portal>.Equals(Element_Portal other)    // Реализация интерфейса сравнения
        {
            return Equals(_portal, other._portal);
        }
    }

    class Element_Portal_Comparer : IEqualityComparer<Element_Portal>
    {
        public bool Equals(Element_Portal x, Element_Portal y)
        {
            return x.portal.Equals(y.portal);
        }

        public int GetHashCode(Element_Portal obj)
        {
            return obj.GetHashCode();
        }
    }

    public class Out_doc
    {
        public Angle3D Angle { get; set; }
        public Point3D Point { get; set; }

        public Out_doc(Angle3D angle, Point3D point)
        {
            Angle = angle;
            Point = point;
        }
    }

    public enum Mode
    {
        Manipulator,
        Portal
    }

    class PathFinder
    {
        public delegate bool Check_Manipulator_Delegate(Point3D point, Point3D scan_point, Kinematic kin);          // Делегат для проверок
        Check_Manipulator_Delegate check_manipulator = null;                                                        // Ссылка на метод
        public delegate bool Check_Portal_Delegate(Point3D point, Point3D scan_point, PortalKinematic kin);         // Делегат для проверок
        Check_Portal_Delegate check_portal = null;                                                                  // Ссылка на метод
        public delegate bool Check_Manipulator_5DOF_Delegate(Ogranichenie ogranich, Angle3D current);               // Делегат для проверок
        Check_Manipulator_5DOF_Delegate check_manipulator_5DOF = null;                                              // Ссылка на метод
        public delegate bool Check_Portal_5DOF_Delegate(PortalAngle position);
        Check_Portal_5DOF_Delegate check_portal_5DOF = null;

        double shag = 1.0;                      // Шаг сетки
        double[] shag_dof5;
        double G = 1 / 3.0;                     // Стоимость передвижения
        Kinematic kin = null;       // Киниматика манипулятора
        PortalKinematic pkin = null;           // Кинематика портала
        Point3D scan_point;                     // Точка сканирования
        Ogranichenie ogranich;                  // Ограничения для пятимерного поиска пути

        int razradnost_shaga = 1;                   // Набегает ошибка, для этого использую эту переменную для округления

        public PathFinder()
        {
            shag = 1;
            G = shag / 10.0;
        }

        public PathFinder(double shag = 1)
        {
            this.shag = shag;
            G = shag / 10.0;
        }

        public PathFinder(Kinematic kin, Point3D scan_point, double shag = 1)
        {
            this.shag = shag;
            G = shag / 10.0;

            this.kin = kin;
            this.scan_point = scan_point;
        }

        public PathFinder(Kinematic kin, Ogranichenie ogr, double shag = 1)
        {
            this.shag = shag;
            G = shag / 10.0;

            this.kin = kin;
            this.ogranich = ogr;
        }

        public PathFinder(PortalKinematic pkin, Point3D scan_point, double shag = 1)
        {
            this.shag = shag;
            G = shag / 10.0;

            this.pkin = pkin;
            this.scan_point = scan_point;
        }

        /// <summary>
        /// Получение точек окружения уже со стоймостью
        /// </summary>
        /// <param name="current_element"></param>
        /// <returns></returns>
        private List<Element> Get_Okrug(Element current_element, Point3D end)
        {
            razradnost_shaga = 1;
            if (shag < 0)
            {
                throw new Exception("Шаг не может быть отрицательным");
            }
            else
            {
                if (shag > 1)
                {
                    while ((int)(shag / (10 * razradnost_shaga)) != 0)
                        razradnost_shaga++;
                }
                else
                {
                    if (shag > 0)
                    {
                        while ((int)(shag * Math.Pow(10, razradnost_shaga)) < 1)
                        {
                            razradnost_shaga++;
                        }
                    }
                    else
                        throw new Exception("Шаг не может быть равен 0");
                }
            }

            List<Element> Okrug_Element_List = new List<Element>();

            List<double> list_perem = new List<double> { -shag, 0, shag };
            foreach (double x in list_perem)
            {
                foreach (double y in list_perem)
                {
                    foreach (double z in list_perem)
                    {
                        if (!(x == 0 & y == 0 & z == 0))
                        {
                            double X = Math.Round(current_element.current_point.X + x, razradnost_shaga);
                            double Y = Math.Round(current_element.current_point.Y + y, razradnost_shaga);
                            double Z = Math.Round(current_element.current_point.Z + z, razradnost_shaga);
                            Point3D new_point = new Point3D(X, Y, Z);

                            if (!IsFree(new_point))
                                continue;

                            Okrug_Element_List.Add(
                            new Element
                            {
                                current_point = new_point,
                                parrent = current_element,
                                cost_G = current_element.cost_G + G,
                                cost_H = Distance(new_point, end)
                            });
                        }
                    }
                }
            }

            return Okrug_Element_List;
        }

        private List<Element_5DOF> Get_Okrug(Element_5DOF current_element, Angle3D end)
        {
            razradnost_shaga = 6;

            List<Element_5DOF> Okrug_Element_List = new List<Element_5DOF>();

            List<double> list_perem1 = new List<double> { -shag_dof5[0], 0, shag_dof5[0] };
            List<double> list_perem2 = new List<double> { -shag_dof5[1], 0, shag_dof5[1] };
            List<double> list_perem3 = new List<double> { -shag_dof5[2], 0, shag_dof5[2] };
            List<double> list_perem4 = new List<double> { -shag_dof5[3], 0, shag_dof5[3] };
            List<double> list_perem5 = new List<double> { -shag_dof5[4], 0, shag_dof5[4] };

            foreach (double o1 in list_perem1)
            {
                foreach (double o2 in list_perem2)
                {
                    foreach (double o3 in list_perem3)
                    {
                        foreach (double o4 in list_perem4)
                        {
                            foreach (double o5 in list_perem5)
                            {
                                if (!(o1 == 0 & o2 == 0 & o3 == 0 & o4 == 0 & o5 == 0))
                                {
                                    Angle3D curr = current_element.angle;
                                    Angle3D new_angle = new Angle3D();

                                    new_angle.O1 = Math.Round(curr.O1 + o1, razradnost_shaga);
                                    new_angle.O2 = Math.Round(curr.O2 + o2, razradnost_shaga);
                                    new_angle.O3 = Math.Round(curr.O3 + o3, razradnost_shaga);
                                    new_angle.O4 = Math.Round(curr.O4 + o4, razradnost_shaga);
                                    new_angle.O5 = Math.Round(curr.O5 + o5, razradnost_shaga);

                                    if (!IsFree(new_angle))
                                        continue;

                                    Okrug_Element_List.Add(new Element_5DOF
                                    {
                                        angle = new_angle,
                                        parrent = current_element,
                                        cost_G = current_element.cost_G + G,
                                        cost_H = Distance(new_angle, end)
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return Okrug_Element_List;
        }

        private List<Element_Portal> Get_Okrug(Element_Portal current_element, PortalAngle end)
        {
            razradnost_shaga = 6;

            List<Element_Portal> Okrug_Element_List = new List<Element_Portal>();

            List<double> list_perem1 = new List<double> { -shag_dof5[0], 0, shag_dof5[0] };
            List<double> list_perem2 = new List<double> { -shag_dof5[1], 0, shag_dof5[1] };
            List<double> list_perem3 = new List<double> { -shag_dof5[2], 0, shag_dof5[2] };
            List<double> list_perem4 = new List<double> { -shag_dof5[3], 0, shag_dof5[3] };
            List<double> list_perem5 = new List<double> { -shag_dof5[4], 0, shag_dof5[4] };

            foreach (double o1 in list_perem1)
            {
                foreach (double o2 in list_perem2)
                {
                    foreach (double o3 in list_perem3)
                    {
                        foreach (double o4 in list_perem4)
                        {
                            foreach (double o5 in list_perem5)
                            {
                                if (!(o1 == 0 & o2 == 0 & o3 == 0 & o4 == 0 & o5 == 0))
                                {
                                    PortalAngle curr = current_element.portal;
                                    PortalAngle position = new PortalAngle()
                                    {

                                    };

                                    position.O1 = Math.Round(curr.O1 + o1, razradnost_shaga);
                                    position.O2 = Math.Round(curr.O2 + o2, razradnost_shaga);
                                    position.X = Math.Round(curr.X + o3, razradnost_shaga);
                                    position.Y = Math.Round(curr.Y + o4, razradnost_shaga);
                                    position.Z = Math.Round(curr.Z + o5, razradnost_shaga);

                                    if (!IsFree(position))
                                        continue;

                                    Okrug_Element_List.Add(new Element_Portal
                                    {
                                        portal = position,
                                        parrent = current_element,
                                        cost_G = current_element.cost_G + G,
                                        cost_H = Distance(position, end)
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return Okrug_Element_List;
        }

        public void SetManipulatorChecker(Check_Manipulator_Delegate method)
        {
            check_manipulator = method;
        }

        public void SetPortalChecker(Check_Portal_Delegate method)
        {
            check_portal = method;
        }

        public void SetManipulatorChecker(Check_Manipulator_5DOF_Delegate method)
        {
            check_manipulator_5DOF = method;
        }

        public void SetPortalChecker(Check_Portal_5DOF_Delegate method)
        {
            check_portal_5DOF = method;
        }

        /// <summary>
        /// Проверка на доступность точки
        /// </summary>
        /// <param name="new_point"></param>
        /// <returns></returns>
        public bool IsFree(Point3D new_point)
        {
            if ((kin == null) & (pkin == null)) { }

            if (check_manipulator != null)
            {
                if (check_manipulator.Invoke(new_point, scan_point, kin))
                {

                }
                else
                    return false;
            }
            if (check_portal != null)
            {
                if (check_portal.Invoke(new_point, scan_point, pkin))
                { }
                else
                    return false;
            }

            return true;
        }

        private bool IsFree(Angle3D new_angle)
        {
            if (check_manipulator_5DOF != null)
            {
                return check_manipulator_5DOF.Invoke(ogranich, new_angle);
            }
            else
            {
                throw new Exception("Не определён checker");
            }
        }

        private bool IsFree(PortalAngle position)
        {
            if (check_portal_5DOF != null)
            {
                return check_portal_5DOF.Invoke(position);
            }
            else
            {
                throw new Exception("Не определён checker");
            }
        }

        public static bool Checker(Angle3D current, Ogranichenie ogranich)
        {
            return Ogranichenie.IsOK(ogranich, current);
        }

        /// <summary>
        /// Самописный поиск минимума
        /// </summary>
        /// <param name="list_element"></param>
        /// <returns></returns>
        private Element MinElement(List<Element> list_element)
        {
            if (list_element.Count == 1)
            {
                return list_element[0];
            }
            if (list_element.Count > 1)
            {
                Element min = list_element[0];

                for (int i = 1; i < list_element.Count; i++)
                {
                    if (list_element[i].cost < min.cost)
                        min = list_element[i];
                }

                return min;
            }

            throw new Exception("list_element.Count < 1");
        }

        private Element_5DOF MinElement(List<Element_5DOF> list_element)
        {
            if (list_element.Count == 1)
            {
                return list_element[0];
            }
            if (list_element.Count > 1)
            {
                Element_5DOF min = list_element[0];

                for (int i = 1; i < list_element.Count; i++)
                {
                    if (list_element[i].cost < min.cost)
                        min = list_element[i];
                }

                return min;
            }

            throw new Exception("list_element.Count < 1");
        }

        private Element_Portal MinElement(List<Element_Portal> list_element)
        {
            if (list_element.Count == 1)
            {
                return list_element[0];
            }
            if (list_element.Count > 1)
            {
                Element_Portal min = list_element[0];

                for (int i = 1; i < list_element.Count; i++)
                {
                    if (list_element[i].cost < min.cost)
                        min = list_element[i];
                }

                return min;
            }

            throw new Exception("list_element.Count < 1");
        }

        /// <summary>
        /// Возвращает путь от начальной точки до конечной
        /// </summary>
        /// <param name="element"></param>
        /// <param name="begin_point"></param>
        /// <returns></returns>
        private Stack<Out_doc> GetAllParrents(Element element, Point3D begin_point, Mode mode)
        {
            Stack<Out_doc> output = new Stack<Out_doc>();
            Element current_element = element;                                                  // Текущий элемент

            output.Push(new Out_doc(current_element.angle, current_element.current_point));     // Запись текущего элемента в стек

            while (!Equals(current_element.parrent.current_point, begin_point))                 // Пока родительская точка не начальная
            {
                output.Push(new Out_doc(null, current_element.parrent.current_point));          // Пишем точку родителя
                current_element = current_element.parrent;                                      // Текущий элемент = родительскому
            }
            output.Push(new Out_doc(null, begin_point));                                        // Запись начальной точки


            // Если есть кинематика манипулятора то считаем
            if (kin != null & mode == Mode.Manipulator)
            {
                foreach (var v in output)
                {
                    Point3D currPoint = v.Point;
                    Stack<Angle3D> ang_stack = kin.InverseNab(currPoint.X, currPoint.Y, currPoint.Z,
                        scan_point.X, scan_point.Y, scan_point.Z);

                    if (ang_stack.Count >= 1)
                        v.Angle = ang_stack.First();
                    else
                    {
                        throw new Exception("Непонятная ошибка");
                    }
                }
            }
            // Если есть кинематика портала то считаем
            if (pkin != null & mode == Mode.Portal)
            {
                // Что? Без точек манипулятора нельзя получить углы портала
            }

            List<Out_doc> li = output.Reverse().ToList();
            output = new Stack<Out_doc>();
            foreach (Out_doc od in li)
            {
                output.Push(od);
            }
            return output;
        }

        private Stack<Angle3D> GetAllParrents(Element_5DOF element, Angle3D begin)
        {
            Stack<Angle3D> output = new Stack<Angle3D>();
            Element_5DOF current_element = element;                                          // Текущий элемент

            output.Push(current_element.angle);     // Запись текущего элемента в стек

            while (!Equals(current_element.parrent.angle, begin))         // Пока родительская точка не начальная
            {
                output.Push(current_element.parrent.angle);                     // Пишем точку родителя
                current_element = current_element.parrent;                              // Текущий элемент = родительскому
            }
            output.Push(begin);                                                   // Запись начальной точки

            List<Angle3D> li = output.ToList();
            output = new Stack<Angle3D>();
            for (int i = 0; i <= li.Count() - 1; i++)
            {
                output.Push(li[i]);
            }

            return output;
        }

        private Stack<PortalAngle> GetAllParrents(Element_Portal element, PortalAngle begin)
        {
            Stack<PortalAngle> output = new Stack<PortalAngle>();
            Element_Portal current_element = element;                   // Текущий элемент

            output.Push(current_element.portal);                        // Запись текущего элемента в стек

            while (!Equals(current_element.parrent.portal, begin))      // Пока родительская точка не начальная
            {
                output.Push(current_element.parrent.portal);            // Пишем точку родителя
                current_element = current_element.parrent;              // Текущий элемент = родительскому
            }
            output.Push(begin);                                         // Запись начальной точки

            return output;
        }

        public Stack<Out_doc> Solve(Point3D beg, Point3D end, Mode mode)
        {
            double distance = Distance(beg, end);                               // Дистанция между начальной и конечной точкой
            List<Element> open_list = new List<Element>();                      // Открытый список
            List<Element> close_list = new List<Element>();                     // Закрытый список
            G = shag * 0.1;

            if (!(IsFree(beg) & IsFree(end)))
            {
                throw new Exception("Начальная точка: " + beg.ToString() + " или конечная точка: " + end.ToString() + " не доступны");
            }

            open_list.Add(                                                      // Добавление начальной точки в открытый список
                new Element
                {
                    current_point = beg,
                    cost_G = 0,
                    cost_H = distance
                });

            while (open_list.Count != 0) // Начало цикла пока не пуст лист open
            {
                Element current_element = MinElement(open_list);                // Текущий элемент - минимальный элемент открытого списка

                if (current_element.current_point == end)                       // Текущая точка - конечная
                {
                    return GetAllParrents(current_element, beg, mode);                // Вернуть стак, где верх - начальная точка
                }
                if (Distance(current_element.current_point, end) <= shag / 2.0)
                {
                    Element end_element = current_element;
                    end_element.current_point = end;
                    return GetAllParrents(end_element, beg, mode);
                }

                foreach (Element element in Get_Okrug(current_element, end))    // Окружение текущего элемента
                {
                    if (close_list.Contains(element, new Element_Comparer()))                           // Точка есть в закрытом списке? T: переход к следующему элементу
                        continue;
                    if (!open_list.Contains(element, new Element_Comparer()))                           // Открытый список содержит элемент? F: добавляем T: добавляем с наименьшей стоимостью
                    {
                        open_list.Add(element);
                    }
                    else
                    {
                        if (IsLowerCost(element, open_list))
                        {
                            open_list.Add(element);
                        }
                    }
                }

                //open_list.AddRange(Get_Okrug(current_element, end));            // Добавили все точки окружения в открытый список

                close_list.Add(current_element);                                // Добавляем рассмотренную точку в закрытый список
                open_list.Remove(current_element);                              // Удаление рассмотренной точки из открытого списка
            }

            throw new Exception("No way");                                      // Не найден путь
        }

        public Stack<Angle3D> SolveManipulator(Angle3D beg, Angle3D end)
        {
            int time = DateTime.Now.Minute;

            #region rub
            razradnost_shaga = 6;

            Angle3D first = new Angle3D(beg);
            Angle3D last = new Angle3D(end);

            // Если начальная и конечная позиция равны
            if (Angle3D.Equals(first, last))
            {
                Stack<Angle3D> r = new Stack<Angle3D>();
                r.Push(first);
                return r;
            }

            // Если влетает в ограничения
            if (!(Ogranichenie.IsOK(ogranich, first) && Ogranichenie.IsOK(ogranich, last)))
            {
                //throw new Exception("Начальная или конечная точка не проходит ограничения");
            }

            if (!(IsFree(first) & IsFree(last)))
            {
                throw new Exception("Пересечение в начальной или конечной точке");
            }

            beg.O1 = Math.Round(beg.O1, razradnost_shaga);
            beg.O2 = Math.Round(beg.O2, razradnost_shaga);
            beg.O3 = Math.Round(beg.O3, razradnost_shaga);
            beg.O4 = Math.Round(beg.O4, razradnost_shaga);
            beg.O5 = Math.Round(beg.O5, razradnost_shaga);

            end.O1 = Math.Round(end.O1, razradnost_shaga);
            end.O2 = Math.Round(end.O2, razradnost_shaga);
            end.O3 = Math.Round(end.O3, razradnost_shaga);
            end.O4 = Math.Round(end.O4, razradnost_shaga);
            end.O5 = Math.Round(end.O5, razradnost_shaga);
            #endregion

            shag = Distance(beg, end) / 10.0;
            G = shag * 0.1;
            //вычисляем все возможные шаги

            shag_dof5 = new double[] {
                Math.Abs(((end.O1 - beg.O1)  / 10.0)),
                Math.Abs(((end.O2 - beg.O2)  / 10.0)),
                Math.Abs(((end.O3 - beg.O3)    / 10.0)),
                Math.Abs(((end.O4 - beg.O4)    / 10.0)),
                Math.Abs(((end.O5 - beg.O5)    / 10.0))
            };

            double distance = Distance(beg, end);                               // Дистанция между начальной и конечной точкой
            List<Element_5DOF> open_list = new List<Element_5DOF>();                      // Открытый список
            List<Element_5DOF> close_list = new List<Element_5DOF>();                     // Закрытый список

            open_list.Add(                                                      // Добавление начальной точки в открытый список
                new Element_5DOF
                {
                    angle = beg,
                    cost_G = 0,
                    cost_H = distance
                });

            while (open_list.Count != 0) // Начало цикла пока не пуст лист open
            {
                Element_5DOF current_element = MinElement(open_list);                // Текущий элемент - минимальный элемент открытого списка

                if (DateTime.Now.Minute - time >= 2)
                    throw new Exception("Закончено по времени");

                if (current_element.angle == end)                       // Текущая точка - конечная
                {
                    Angle3D[] rez_mass = GetAllParrents(current_element, beg).ToArray();
                    rez_mass[0] = last;
                    rez_mass[rez_mass.Count() - 1] = first;

                    Stack<Angle3D> rezult = new Stack<Angle3D>();
                    foreach (Angle3D ang in rez_mass)
                    {
                        rezult.Push(ang);
                    }

                    return rezult;
                }

                var first_element = current_element.angle;
                var second_element = end;

                if (Math.Round(first_element.O1, 4) == Math.Round(second_element.O1, 4) &
                    Math.Round(first_element.O2, 4) == Math.Round(second_element.O2, 4) &
                    Math.Round(first_element.O3, 4) == Math.Round(second_element.O3, 4) &
                    Math.Round(first_element.O4, 4) == Math.Round(second_element.O4, 4) &
                    Math.Round(first_element.O5, 4) == Math.Round(second_element.O5, 4))
                {
                    Angle3D[] rez_mass = GetAllParrents(current_element, beg).ToArray();
                    rez_mass[0] = last;
                    rez_mass[rez_mass.Count() - 1] = first;

                    Stack<Angle3D> rezult = new Stack<Angle3D>();
                    foreach (Angle3D ang in rez_mass)
                    {
                        rezult.Push(ang);
                    }

                    return rezult;
                }

                if (Math.Sqrt(Math.Pow(Math.Round(first_element.O1, 4) - Math.Round(second_element.O1, 4), 2)) <= shag_dof5[0] &&
                   Math.Sqrt(Math.Pow(Math.Round(first_element.O2, 4) - Math.Round(second_element.O2, 4), 2)) <= shag_dof5[1] &&
                   Math.Sqrt(Math.Pow(Math.Round(first_element.O3, 4) - Math.Round(second_element.O3, 4), 2)) <= shag_dof5[2] &&
                   Math.Sqrt(Math.Pow(Math.Round(first_element.O4, 4) - Math.Round(second_element.O4, 4), 2)) <= shag_dof5[3] &&
                   Math.Sqrt(Math.Pow(Math.Round(first_element.O5, 4) - Math.Round(second_element.O5, 4), 2)) <= shag_dof5[4])
                {
                    Angle3D[] rez_mass = GetAllParrents(current_element, beg).ToArray();
                    rez_mass[0] = last;
                    rez_mass[rez_mass.Count() - 1] = first;

                    Stack<Angle3D> rezult = new Stack<Angle3D>();
                    foreach (Angle3D ang in rez_mass)
                    {
                        rezult.Push(ang);
                    }

                    return rezult;
                }

                foreach (Element_5DOF element in Get_Okrug(current_element, end))    // Окружение текущего элемента
                {
                    if (close_list.Contains(element, new Element_5DOF_Comparer()))                           // Точка есть в закрытом списке? T: переход к следующему элементу
                        continue;
                    if (!open_list.Contains(element, new Element_5DOF_Comparer()))                           // Открытый список содержит элемент? F: добавляем T: добавляем с наименьшей стоимостью
                    {
                        open_list.Add(element);
                    }
                    else
                    {
                        if (IsLowerCost(element, open_list))
                        {
                            open_list.Add(element);
                        }
                    }
                }

                //open_list.AddRange(Get_Okrug(current_element, end));            // Добавили все точки окружения в открытый список

                close_list.Add(current_element);                                // Добавляем рассмотренную точку в закрытый список
                open_list.Remove(current_element);                              // Удаление рассмотренной точки из открытого списка
            }

            throw new Exception("No way");                                      // Не найден путь

            throw new NotImplementedException();
        }

        public Stack<PortalAngle> SolvePortal(PortalAngle beg, PortalAngle end)
        {
            int time = DateTime.Now.Minute;

            #region rub
            razradnost_shaga = 6;

            PortalAngle first = new PortalAngle(beg);
            PortalAngle last = new PortalAngle(end);

            // Если начальная и конечная позиция равны
            if (PortalAngle.Equals(first, last))
            {
                Stack<PortalAngle> r = new Stack<PortalAngle>();
                r.Push(first);
                return r;
            }

            //// Если влетает в ограничения
            //if (!(Ogranichenie.IsOK(ogranich, first) && Ogranichenie.IsOK(ogranich, last)))
            //{
            //    //throw new Exception("Начальная или конечная точка не проходит ограничения");
            //}

            if (!(IsFree(first) & IsFree(last)))
            {
                throw new Exception("Пересечение в начальной или конечной точке");
            }

            beg.O1 = Math.Round(beg.O1, razradnost_shaga);
            beg.O2 = Math.Round(beg.O2, razradnost_shaga);
            beg.X = Math.Round(beg.X, razradnost_shaga);
            beg.Y = Math.Round(beg.Y, razradnost_shaga);
            beg.Z = Math.Round(beg.Z, razradnost_shaga);

            end.O1 = Math.Round(end.O1, razradnost_shaga);
            end.O2 = Math.Round(end.O2, razradnost_shaga);
            end.X = Math.Round(end.X, razradnost_shaga);
            end.Y = Math.Round(end.Y, razradnost_shaga);
            end.Z = Math.Round(end.Z, razradnost_shaga);
            #endregion

            shag = Distance(beg, end) / 10.0;
            G = shag * 0.1;
            //вычисляем все возможные шаги
            shag_dof5 = new double[] {
                Math.Abs(((end.O1 - beg.O1)  / 10.0)),
                Math.Abs(((end.O2 - beg.O2)  / 10.0)),
                Math.Abs(((end.X - beg.X)    / 10.0)),
                Math.Abs(((end.Y - beg.Y)    / 10.0)),
                Math.Abs(((end.Z - beg.Z)    / 10.0))
            };

            double distance = Distance(beg, end);                               // Дистанция между начальной и конечной точкой
            List<Element_Portal> open_list = new List<Element_Portal>();        // Открытый список
            List<Element_Portal> close_list = new List<Element_Portal>();       // Закрытый список

            open_list.Add(                                                      // Добавление начальной точки в открытый список
                new Element_Portal
                {
                    portal = beg,
                    cost_G = 0,
                    cost_H = distance
                });

            while (open_list.Count != 0) // Начало цикла пока не пуст лист open
            {
                Element_Portal current_element = MinElement(open_list);                // Текущий элемент - минимальный элемент открытого списка

                if (DateTime.Now.Minute - time >= 2)
                    throw new Exception("Закончено по времени");

                if (current_element.portal == end)                       // Текущая точка - конечная
                {
                    PortalAngle[] rez_mass = GetAllParrents(current_element, beg).ToArray();
                    rez_mass[0] = first;
                    rez_mass[rez_mass.Count() - 1] = last;

                    Stack<PortalAngle> rezult = new Stack<PortalAngle>();
                    foreach (PortalAngle position in rez_mass)
                    {
                        rezult.Push(position);
                    }

                    return rezult;
                }

                var first_element = current_element.portal;
                var second_element = end;

                if (Math.Round(first_element.O1, 4) == Math.Round(second_element.O1, 4) &
                    Math.Round(first_element.O2, 4) == Math.Round(second_element.O2, 4) &
                    Math.Round(first_element.X, 4) == Math.Round(second_element.X, 4) &
                    Math.Round(first_element.Y, 4) == Math.Round(second_element.Y, 4) &
                    Math.Round(first_element.Z, 4) == Math.Round(second_element.Z, 4))
                {
                    PortalAngle[] rez_mass = GetAllParrents(current_element, beg).ToArray();
                    rez_mass[0] = first;
                    rez_mass[rez_mass.Count() - 1] = last;

                    Stack<PortalAngle> rezult = new Stack<PortalAngle>();
                    foreach (PortalAngle position in rez_mass)
                    {
                        rezult.Push(position);
                    }

                    return rezult;
                }

                if (Math.Sqrt(Math.Pow(Math.Round(first_element.O1, 4) - Math.Round(second_element.O1, 4), 2)) <= shag_dof5[0] &&
                    Math.Sqrt(Math.Pow(Math.Round(first_element.O2, 4) - Math.Round(second_element.O2, 4), 2)) <= shag_dof5[1] &&
                    Math.Sqrt(Math.Pow(Math.Round(first_element.X, 4) - Math.Round(second_element.X, 4), 2)) <= shag_dof5[2] &&
                    Math.Sqrt(Math.Pow(Math.Round(first_element.Y, 4) - Math.Round(second_element.Y, 4), 2)) <= shag_dof5[3] &&
                    Math.Sqrt(Math.Pow(Math.Round(first_element.Z, 4) - Math.Round(second_element.Z, 4), 2)) <= shag_dof5[4])
                {
                    PortalAngle[] rez_mass = GetAllParrents(current_element, beg).ToArray();
                    rez_mass[0] = first;
                    rez_mass[rez_mass.Count() - 1] = last;

                    Stack<PortalAngle> rezult = new Stack<PortalAngle>();
                    foreach (PortalAngle position in rez_mass)
                    {
                        rezult.Push(position);
                    }

                    return rezult;
                }

                foreach (Element_Portal element in Get_Okrug(current_element, end))    // Окружение текущего элемента
                {
                    if (close_list.Contains(element, new Element_Portal_Comparer()))                           // Точка есть в закрытом списке? T: переход к следующему элементу
                        continue;
                    if (!open_list.Contains(element, new Element_Portal_Comparer()))                           // Открытый список содержит элемент? F: добавляем T: добавляем с наименьшей стоимостью
                    {
                        open_list.Add(element);
                    }
                    else
                    {
                        if (IsLowerCost(element, open_list))
                        {
                            open_list.Add(element);
                        }
                    }
                }

                //open_list.AddRange(Get_Okrug(current_element, end));            // Добавили все точки окружения в открытый список

                close_list.Add(current_element);                                // Добавляем рассмотренную точку в закрытый список
                open_list.Remove(current_element);                              // Удаление рассмотренной точки из открытого списка
            }

            throw new Exception("No way");                                      // Не найден путь

            throw new NotImplementedException();
        }

        /// <summary>
        /// Проверка стоимости элемента
        /// </summary>
        /// <param name="element"></param>
        /// <param name="open_list"></param>
        /// <returns></returns>
        private bool IsLowerCost(Element element, List<Element> open_list)
        {
            foreach (Element el in open_list)
            {
                if (Equals(el.current_point, element.current_point))
                {
                    if (element.cost < el.cost)
                    {
                        //open_list.Add(element);
                        return true;
                    }
                    else
                    {
                    }
                }
            }

            return false;

            throw new Exception("open_list not contain element");
        }

        private bool IsLowerCost(Element_5DOF element, List<Element_5DOF> open_list)
        {
            foreach (Element_5DOF el in open_list)
            {
                if (Equals(el.angle, element.angle))
                {
                    if (element.cost < el.cost)
                    {
                        return true;
                    }
                    else
                    {
                    }
                }
            }

            return false;

            throw new Exception("open_list not contain element");
        }

        private bool IsLowerCost(Element_Portal element, List<Element_Portal> open_list)
        {
            foreach (Element_Portal el in open_list)
            {
                if (Equals(el.portal, element.portal))
                {
                    if (element.cost < el.cost)
                    {
                        return true;
                    }
                }
            }

            return false;

            throw new Exception("open_list not contain element");
        }

        /// <summary>
        /// Дистанция между точкой A и B
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        private static double Distance(Point3D A, Point3D B)
        {
            return Math.Sqrt(Math.Pow((A.X - B.X), 2) + Math.Pow((A.Y - B.Y), 2) + Math.Pow((A.Z - B.Z), 2));
        }

        public static double Distance(Angle3D A, Angle3D B)
        {
            return Math.Sqrt(
                Math.Pow((B.O1 - A.O1), 2) +
                Math.Pow((B.O2 - A.O2), 2) +
                Math.Pow((B.O3 - A.O3), 2) +
                Math.Pow((B.O4 - A.O4), 2) +
                Math.Pow((B.O5 - A.O5), 2));
        }

        private static double Distance(PortalAngle A, PortalAngle B)
        {
            return Math.Sqrt(
                Math.Pow((B.O1 - A.O1), 2) +
                Math.Pow((B.O2 - A.O2), 2) +
                Math.Pow((B.X - A.X), 2) +
                Math.Pow((B.Y - A.Y), 2) +
                Math.Pow((B.Z - A.Z), 2));
        }

        private Point3D Transfer(double[] x)
        {
            Angle3D ang = new Angle3D();
            ang.O1 = x[0];
            ang.O2 = x[1];
            ang.O3 = x[2];
            ang.O4 = x[3];
            ang.O5 = x[4];

            var matrix = kin.DirectKinematic(ang);

            return new Point3D(matrix[0][3], matrix[1][3], matrix[2][3]);
        }

        #region ForIntersection

        /// <summary>
        /// Пересекаются?
        /// </summary>
        /// <param name="FirstObject"></param>
        /// <param name="SecondObject"></param>
        /// <returns></returns>
        public static bool IsIntersect(List<Point3D[]> FirstObject, List<Point3D[]> SecondObject)
        {
            foreach (Point3D[] p_o1 in FirstObject)
            {
                foreach (Point3D[] p_o2 in SecondObject)
                {
                    if (p_o1[0].X <= p_o2[1].X & p_o2[0].X <= p_o1[1].X &
                        p_o1[0].Y <= p_o2[1].Y & p_o2[0].Y <= p_o1[1].Y &
                        p_o1[0].Z <= p_o2[1].Z & p_o2[0].Z <= p_o1[1].Z)
                    {
                        return true;
                    }
                }
            }
            return false;
            //(minX1 ≤ maxX2, minX2 ≤ maxX1, minY1 ≤ maxY2, minY2 ≤ maxY1, minZ1 ≤ maxZ2, minZ2 ≤ maxZ1)

            throw new NotImplementedException();
        }

        /// <summary>
        /// Ищет минимальные и максимальные точки у оболочки манипулятора
        /// </summary>
        /// <param name="List_Matrix"></param>
        /// <param name="lj"></param>
        /// <returns></returns>
        public static List<Point3D[]> GetCover_Manipulator(List<double[][]> List_Matrix, LengthJoin lj)
        {
            List<List<Point3D>> All_Cover = new List<List<Point3D>>();
            List<Point3D[]> rezult = new List<Point3D[]>();

            //Для централизации боксов
            double[][] perem_minusy_2 = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, -lj.J4 },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_minusy = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, -lj.Det },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };
            List_Matrix[2] = MathUtils.Mul_Matrix(List_Matrix[2], perem_minusy);
            List_Matrix[4] = MathUtils.Mul_Matrix(List_Matrix[4], perem_minusy_2);

            List_Matrix[4] = new double[][] {
                new double[] { (List_Matrix[4])[0][0], (List_Matrix[4])[0][1], (List_Matrix[4])[0][2], (List_Matrix[4])[0][3] },
                new double[] { (List_Matrix[4])[1][0], (List_Matrix[4])[1][1], (List_Matrix[4])[1][2], (List_Matrix[4])[1][3] },
                new double[] { (List_Matrix[4])[2][0], (List_Matrix[4])[2][1], (List_Matrix[4])[2][2], (List_Matrix[4])[2][3] },
                new double[] { (List_Matrix[4])[3][0], (List_Matrix[4])[3][1], (List_Matrix[4])[3][2], (List_Matrix[4])[3][3] }
            };

            //Само вычисление точек            
            double del = 100.0;
            All_Cover.Add(GetPointsFromMatrix(List_Matrix[0], 20, 20, lj.J1));
            for (double i = 0; i < lj.J2; i += lj.J2 / del)
            {
                double[][] Trans = new double[][] {
                    new double[] { 1, 0, 0, 0 },
                    new double[] { 0, 1, 0, 0 },
                    new double[] { 0, 0, 1, i },
                    new double[] { 0, 0, 0, 1 }
                };
                All_Cover.Add(GetPointsFromMatrix(MathUtils.Mul_Matrix(List_Matrix[1], Trans), 5, 5, lj.J2 / del));
            }
            for (double i = 0; i < lj.J3; i += lj.J3 / del)
            {
                double[][] Trans = new double[][] {
                    new double[] { 1, 0, 0, 0 },
                    new double[] { 0, 1, 0, 0 },
                    new double[] { 0, 0, 1, i },
                    new double[] { 0, 0, 0, 1 }
                };
                All_Cover.Add(GetPointsFromMatrix(MathUtils.Mul_Matrix(List_Matrix[2], Trans), 5, 5, lj.J3 / del));
            }
            for (double i = 0; i < lj.J5; i += lj.J5 / del)
            {
                double[][] Trans = new double[][] {
                    new double[] { 1, 0, 0, 0 },
                    new double[] { 0, 1, 0, 0 },
                    new double[] { 0, 0, 1, i },
                    new double[] { 0, 0, 0, 1 }
                };
                All_Cover.Add(GetPointsFromMatrix(MathUtils.Mul_Matrix(List_Matrix[4], Trans), 5, lj.J4 - 5, lj.J5 / del));
            }
            //All_Cover.Add(GetPointsFromMatrix(List_Matrix[1], 5, 5, lj.J2));
            //All_Cover.Add(GetPointsFromMatrix(List_Matrix[2], 5, 5, lj.J3));
            //All_Cover.Add(GetPointsFromMatrix(List_Matrix[4], 5, lj.J4 - 5, lj.J5));
            //All_Cover.Add(GetPointsFromMatrix_Box(List_Matrix[4], 5, lj.J4 - 5, lj.J5));

            foreach (List<Point3D> p in All_Cover)
            {
                rezult.Add(GetMinAndMaxPoint(p));
            }

            return rezult;
        }

        public static List<Point3D[]> GetCover_Portal(List<Matrix3D> List_Matrix, double l1, double l2, double l3)
        {
            List<Point3D[]> rezult = new List<Point3D[]>();

            List<Point3D> t1 = new List<Point3D>();
            t1.Add(new Point3D(0, -10, -List_Matrix[0].M34));
            t1.Add(new Point3D(0, -10 + 15, -List_Matrix[0].M34));
            t1.Add(new Point3D(0 + List_Matrix[0].M14, -10, -List_Matrix[0].M34));
            t1.Add(new Point3D(0 + List_Matrix[0].M14, -10 + 15, -List_Matrix[0].M34));
            t1.Add(new Point3D(0, -10, -List_Matrix[0].M34 + List_Matrix[0].M34 * 2));
            t1.Add(new Point3D(0, -10 + 15, -List_Matrix[0].M34 + List_Matrix[0].M34 * 2));
            t1.Add(new Point3D(0 + List_Matrix[0].M14, -10, -List_Matrix[0].M34 + List_Matrix[0].M34 * 2));
            t1.Add(new Point3D(0 + List_Matrix[0].M14, -10 + 15, -List_Matrix[0].M34 + List_Matrix[0].M34 * 2));
            rezult.Add(GetMinAndMaxPoint(t1));

            List<Point3D> t2 = new List<Point3D>();
            t2.Add(new Point3D(List_Matrix[1].M14 - 10, 0, -60));
            t2.Add(new Point3D(List_Matrix[1].M14 - 10, 0 + 135, -60));
            t2.Add(new Point3D(List_Matrix[1].M14 - 10 + 20 + 10, 0, -60));
            t2.Add(new Point3D(List_Matrix[1].M14 - 10 + 20 + 10, 0 + 135, -60));
            t2.Add(new Point3D(List_Matrix[1].M14 - 10, 0, -60 + 120));
            t2.Add(new Point3D(List_Matrix[1].M14 - 10, 0 + 135, -60 + 120));
            t2.Add(new Point3D(List_Matrix[1].M14 - 10 + 20 + 10, 0, -60 + 120));
            t2.Add(new Point3D(List_Matrix[1].M14 - 10 + 20 + 10, 0 + 135, -60 + 120));
            rezult.Add(GetMinAndMaxPoint(t2));

            List<Point3D> t3 = new List<Point3D>();
            t3.Add(new Point3D(List_Matrix[1].M14 - 28.791469999999997, List_Matrix[1].M34 - 12, List_Matrix[1].M24 - 11));
            t3.Add(new Point3D(List_Matrix[1].M14 - 28.791469999999997, List_Matrix[1].M34 - 12 + 24, List_Matrix[1].M24 - 11));
            t3.Add(new Point3D(List_Matrix[1].M14, List_Matrix[1].M34 - 12, List_Matrix[1].M24 - 11));
            t3.Add(new Point3D(List_Matrix[1].M14, List_Matrix[1].M34 - 12 + 24, List_Matrix[1].M24 - 11));
            t3.Add(new Point3D(List_Matrix[1].M14 - 28.791469999999997, List_Matrix[1].M34 - 12, List_Matrix[1].M24 - 11 + 22));
            t3.Add(new Point3D(List_Matrix[1].M14 - 28.791469999999997, List_Matrix[1].M34 - 12 + 24, List_Matrix[1].M24 - 11 + 22));
            t3.Add(new Point3D(List_Matrix[1].M14, List_Matrix[1].M34 - 12, List_Matrix[1].M24 - 11 + 22));
            t3.Add(new Point3D(List_Matrix[1].M14, List_Matrix[1].M34 - 12 + 24, List_Matrix[1].M24 - 11 + 22));
            rezult.Add(GetMinAndMaxPoint(t3));

            Matrix3D lastmat = new Matrix3D()
            {
                M11 = List_Matrix[3].M11,
                M12 = List_Matrix[3].M12,
                M13 = List_Matrix[3].M13,
                M14 = List_Matrix[3].M14,
                M21 = List_Matrix[3].M21,
                M22 = List_Matrix[3].M22,
                M23 = List_Matrix[3].M23,
                M24 = List_Matrix[3].M24,
                M31 = List_Matrix[3].M31,
                M32 = List_Matrix[3].M32,
                M33 = List_Matrix[3].M33,
                M34 = List_Matrix[3].M34,
                M44 = 1,
            };

            Point3D[] last = GetMinAndMaxPoint(GetPointsFromMatrix(lastmat, l3 + 3, 20, 20));
            rezult.Add(new Point3D[] { new Point3D(last[0].X, last[0].Z, last[0].Y),
             new Point3D(last[1].X, last[1].Z, last[1].Y)});

            return rezult;
        }

        /// <summary>
        /// Получение угловых точек коробки
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static List<Point3D> GetPointsFromMatrix(double[][] mat, double x, double y, double z)
        {
            List<Point3D> rezult = new List<Point3D>();

            double[][] perem_x = new double[][] {
                new double[] { 1, 0, 0, x },
                new double[] { 0, 1, 0, 0 },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_y = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, y },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_z = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, 0 },
                new double[] { 0, 0, 1, z },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_minus2x = new double[][] {
                new double[] { 1, 0, 0, -2*x },
                new double[] { 0, 1, 0, 0 },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_plus2x = new double[][] {
                new double[] { 1, 0, 0, 2*x },
                new double[] { 0, 1, 0, 0 },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_minus2y = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, -2*y },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_plus2y = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, 2*y },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };

            double[][] R;
            R = MathUtils.Mul_Matrix(mat, perem_x);
            R = MathUtils.Mul_Matrix(R, perem_y);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));

            R = MathUtils.Mul_Matrix(R, perem_minus2x);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));

            R = MathUtils.Mul_Matrix(R, perem_minus2y);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));

            R = MathUtils.Mul_Matrix(R, perem_plus2x);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));

            R = MathUtils.Mul_Matrix(R, perem_z);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));

            R = MathUtils.Mul_Matrix(R, perem_plus2y);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));

            R = MathUtils.Mul_Matrix(R, perem_minus2x);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));

            R = MathUtils.Mul_Matrix(R, perem_minus2y);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));

            return rezult;
        }

        public static List<Point3D> GetPointsFromMatrix(Matrix3D Mat, double x, double y, double z)
        {
            double[][] mat = new double[][] {
                new double[] { Mat.M11, Mat.M12, Mat.M13, Mat.M14},
                new double[] { Mat.M21, Mat.M22, Mat.M23, Mat.M24},
                new double[] { Mat.M31, Mat.M32, Mat.M33, Mat.M34},
                new double[] { 0, 0, 0, Mat.M44}
            };

            List<Point3D> rezult = new List<Point3D>();

            double[][] perem_x = new double[][] {
                new double[] { 1, 0, 0, x },
                new double[] { 0, 1, 0, 0 },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_y = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, y },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_z = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, 0 },
                new double[] { 0, 0, 1, z },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_minus_x = new double[][] {
                new double[] { 1, 0, 0, -x },
                new double[] { 0, 1, 0, 0 },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_minus_y = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, -y },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_minus_z = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, 0 },
                new double[] { 0, 0, 1, -z },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_minus2x = new double[][] {
                new double[] { 1, 0, 0, -2*x },
                new double[] { 0, 1, 0, 0 },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_plus2x = new double[][] {
                new double[] { 1, 0, 0, 2*x },
                new double[] { 0, 1, 0, 0 },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_minus2y = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, -2*y },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_plus2y = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, 2*y },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };

            double[][] R;
            R = MathUtils.Mul_Matrix(mat, perem_x);
            R = MathUtils.Mul_Matrix(R, perem_y);
            R = MathUtils.Mul_Matrix(R, perem_z);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));

            R = MathUtils.Mul_Matrix(mat, perem_x);
            R = MathUtils.Mul_Matrix(R, perem_y);
            R = MathUtils.Mul_Matrix(R, perem_minus_z);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));

            R = MathUtils.Mul_Matrix(mat, perem_x);
            R = MathUtils.Mul_Matrix(R, perem_minus_y);
            R = MathUtils.Mul_Matrix(R, perem_z);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));

            R = MathUtils.Mul_Matrix(mat, perem_x);
            R = MathUtils.Mul_Matrix(R, perem_minus_y);
            R = MathUtils.Mul_Matrix(R, perem_minus_z);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));

            R = MathUtils.Mul_Matrix(mat, perem_minus_x);
            R = MathUtils.Mul_Matrix(R, perem_y);
            R = MathUtils.Mul_Matrix(R, perem_z);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));

            R = MathUtils.Mul_Matrix(mat, perem_minus_x);
            R = MathUtils.Mul_Matrix(R, perem_y);
            R = MathUtils.Mul_Matrix(R, perem_minus_z);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));

            R = MathUtils.Mul_Matrix(mat, perem_minus_x);
            R = MathUtils.Mul_Matrix(R, perem_minus_y);
            R = MathUtils.Mul_Matrix(R, perem_z);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));

            R = MathUtils.Mul_Matrix(mat, perem_minus_x);
            R = MathUtils.Mul_Matrix(R, perem_minus_y);
            R = MathUtils.Mul_Matrix(R, perem_minus_z);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));
            return rezult;
        }

        public static List<Point3D> GetPointsFromMatrix_Box(double[][] mat, double x, double y, double z)
        {
            List<Point3D> rezult = new List<Point3D>();

            double[][] perem_x = new double[][] {
                new double[] { 1, 0, 0, x },
                new double[] { 0, 1, 0, 0 },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_y = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, y },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_z = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, 0 },
                new double[] { 0, 0, 1, z },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_minus_x = new double[][] {
                new double[] { 1, 0, 0, -x },
                new double[] { 0, 1, 0, 0 },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_minus_y = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, -y },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_minus_z = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, 0 },
                new double[] { 0, 0, 1, -z },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_minus2x = new double[][] {
                new double[] { 1, 0, 0, -2*x },
                new double[] { 0, 1, 0, 0 },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_plus2x = new double[][] {
                new double[] { 1, 0, 0, 2*x },
                new double[] { 0, 1, 0, 0 },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_minus2y = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, -2*y },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };
            double[][] perem_plus2y = new double[][] {
                new double[] { 1, 0, 0, 0 },
                new double[] { 0, 1, 0, 2*y },
                new double[] { 0, 0, 1, 0 },
                new double[] { 0, 0, 0, 1 }
            };

            double[][] R;
            R = MathUtils.Mul_Matrix(mat, perem_x);
            R = MathUtils.Mul_Matrix(R, perem_y);
            R = MathUtils.Mul_Matrix(R, perem_z);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));

            R = MathUtils.Mul_Matrix(mat, perem_x);
            R = MathUtils.Mul_Matrix(R, perem_y);
            R = MathUtils.Mul_Matrix(R, perem_minus_z);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));

            R = MathUtils.Mul_Matrix(mat, perem_x);
            R = MathUtils.Mul_Matrix(R, perem_minus_y);
            R = MathUtils.Mul_Matrix(R, perem_z);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));

            R = MathUtils.Mul_Matrix(mat, perem_x);
            R = MathUtils.Mul_Matrix(R, perem_minus_y);
            R = MathUtils.Mul_Matrix(R, perem_minus_z);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));

            R = MathUtils.Mul_Matrix(mat, perem_minus_x);
            R = MathUtils.Mul_Matrix(R, perem_y);
            R = MathUtils.Mul_Matrix(R, perem_z);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));

            R = MathUtils.Mul_Matrix(mat, perem_minus_x);
            R = MathUtils.Mul_Matrix(R, perem_y);
            R = MathUtils.Mul_Matrix(R, perem_minus_z);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));

            R = MathUtils.Mul_Matrix(mat, perem_minus_x);
            R = MathUtils.Mul_Matrix(R, perem_minus_y);
            R = MathUtils.Mul_Matrix(R, perem_z);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));

            R = MathUtils.Mul_Matrix(mat, perem_minus_x);
            R = MathUtils.Mul_Matrix(R, perem_minus_y);
            R = MathUtils.Mul_Matrix(R, perem_minus_z);
            rezult.Add(new Point3D(R[0][3], R[1][3], R[2][3]));
            return rezult;
        }

        /// <summary>
        /// Ищет минимальную и максимальную точку оболочки
        /// </summary>
        /// <param name="ListOfPoints"></param>
        /// <returns>[0] - минимальные значения по осям, [1] - максимальные</returns>
        private static Point3D[] GetMinAndMaxPoint(List<Point3D> ListOfPoints)
        {
            Point3D[] rezult = new Point3D[2];

            double min_x = ListOfPoints[0].X;
            double min_y = ListOfPoints[0].Y;
            double min_z = ListOfPoints[0].Z;

            double max_x = ListOfPoints[0].X;
            double max_y = ListOfPoints[0].Y;
            double max_z = ListOfPoints[0].Z;

            foreach (Point3D p in ListOfPoints)
            {
                if (p.X < min_x)
                    min_x = p.X;
                if (p.Y < min_y)
                    min_y = p.Y;
                if (p.Z < min_z)
                    min_z = p.Z;

                if (p.X > max_x)
                    max_x = p.X;
                if (p.Y > max_y)
                    max_y = p.Y;
                if (p.Z > max_z)
                    max_z = p.Z;
            }

            rezult[0] = new Point3D(min_x, min_y, min_z);
            rezult[1] = new Point3D(max_x, max_y, max_z);

            return rezult;
        }


        #endregion

    }
}

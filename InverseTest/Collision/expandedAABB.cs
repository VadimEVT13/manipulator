using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using InverseTest.Collision;

namespace InverseTest.Collision.Model
{
     
    class expandedAABB
    {
        List<Point3D[]> left = new List<Point3D[]>();  //часть 1 разделенного бокса
        List<Point3D[]> right = new List<Point3D[]>(); //часть 2
        List<Box> boxes = new List<Box>(); //все боксы
        
        //double c = 0;

        public struct Box
        {
            public Rect3D Bounds;
            public List<Point3D[]> Triangles;
            public int lvl; //глубина разбиения
            public int axis; //бОльшая по виличине ось, по которой делим бокс
            public double coordinate_centre; //координаты центра бокса по длинейшей оси
            public int box_id; //от которого куба появился (из первоначальных двух)

            public Box(List<Point3D[]> triangles, Rect3D bounds, int lvl, int id)
            {
                this.Bounds = bounds;
                this.Triangles = triangles;
                this.lvl = lvl;
                this.axis = 0;
                this.coordinate_centre = 0;
                this.box_id = id;
            }

            public void calc_axis_and_centre() //вычисление большей оси и координат ее центра (1-х, 2-у, 3-z)
            {
                if (Bounds.SizeX > Bounds.SizeY)
                    if (Bounds.SizeX > Bounds.SizeZ) { axis = 1; coordinate_centre = Bounds.Location.X + Bounds.SizeX / 2; }
                    else { axis = 3; coordinate_centre = Bounds.Location.Z + Bounds.SizeZ / 2; }
                else if (Bounds.SizeY > Bounds.SizeZ) { axis = 2; coordinate_centre = Bounds.Location.Y + Bounds.SizeY / 2; }
                else { axis = 3; coordinate_centre = Bounds.Location.Z + Bounds.SizeZ / 2; }
                return;
            }
        }

        public Rect3D CreateRect3D(List<Point3D[]> tr) //создание бокса вокруг треугольников разбитых на 2 группы
        {

            double min_x = tr[0][0].X, min_y = tr[0][0].Y, min_z = tr[0][0].Z, max_x = tr[0][0].X, max_y = tr[0][0].Y, max_z = tr[0][0].Z;
            Rect3D rect;
            foreach (Point3D[] i in tr)
            {
                for( int j =0;j<3;j++)
                {
                    if (i[j].X > max_x) max_x = i[j].X; if (i[j].X < min_x) min_x = i[j].X;
                    if (i[j].Y > max_y) max_y = i[j].Y; if (i[j].Y < min_y) min_y = i[j].Y;
                    if (i[j].Z > max_z) max_z = i[j].Z; if (i[j].Z < min_z) min_z = i[j].Z;
                }
            }
             
            rect = new Rect3D(min_x, min_y, min_z, max_x-min_x, max_y-min_y, max_z-min_z);
            return rect;
        }

        public void Split(Box box) //разбитие бокса на 2 части с определением, какие треугольники в них входят
        {
            left.Clear();
            right.Clear();
            Rect3D rect = new Rect3D();
            for (int i = 0; i < box.Triangles.Count; i++)
            {
                if (Algebra.proection(box.axis, box.coordinate_centre, box.Triangles[i]) == 0)
                {
                    left.Add(box.Triangles[i]);
                }
                else right.Add(box.Triangles[i]);
            }
            if (left.Count != 0)
            {
                rect = CreateRect3D(left);
                boxes.Add(new Box(new List<Point3D[]>(left), rect, box.lvl + 1, box.box_id));
            }
            rect = new Rect3D();
            if (right.Count != 0)
            {
                rect = CreateRect3D(right);
                boxes.Add(new Box(new List<Point3D[]>(right), rect, box.lvl + 1, box.box_id));
            }

        }

        public bool Find(Box b1, Box b2) //основная часть поиска столкновений
        {
            int lvl = b1.lvl;
            if (b1.Bounds.IntersectsWith(b2.Bounds))
            {
                if (b1.Triangles.Count == 1 || b2.Triangles.Count == 1) return true;

                b1.calc_axis_and_centre();
                b2.calc_axis_and_centre();

                Split(b1);
                Split(b2);

                foreach (Box b in boxes)
                {
                    foreach (Box bb in boxes)
                    {
                        if (b.box_id != bb.box_id && lvl + 1 == b.lvl && lvl + 1 == bb.lvl)
                        {
                            if(Find(b, bb)) return true;
                        }
                        
                    }
                        
                    
                }
                

            }
            return false;
        }

        public static List<Point3D[]> GetTriangles(PartShape m) //применяем ко всем точкам трансформ и формируем треугольники из них
        {
            Transform3D transform = new MatrixTransform3D(m.Transform);
            List<Point3D> points = m.Points;
            List<Point3D[]> tr = new List<Point3D[]>();
            Point3D[] t = new Point3D[3];
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = transform.Value.Transform(points[i]);
                //points[i] = x;
                t[i % 3] = points[i];
                if (i % 3 == 2)
                {
                    tr.Add(new Point3D[] { t[0], t[1], t[2] });
                }
            }
            return tr;
        }

        public bool Prepare(PartShape m1, PartShape m2) //работа с изначальными двумя боксами
        {
            List<Point3D[]> pre = new List<Point3D[]>();
            pre = GetTriangles(m1);
            boxes.Add(new Box(pre, CreateRect3D(pre), 0, 1));
            pre = GetTriangles(m2);
            boxes.Add(new Box(pre, CreateRect3D(pre), 0, 2));
            return Find(boxes[0],boxes[1]);
        }
    }
}

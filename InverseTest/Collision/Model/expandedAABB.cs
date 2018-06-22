using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Collision.Model
{
    class expandedAABB
    {
        Tree tree_first, tree_second = new Tree();
        List<Rect3D> boxes_1, boxes_2 = new List<Rect3D>();

        public bool find(PartShape box_first, PartShape box_second)
        {
            tree_first.Insert(box_first.Bounds);
            tree_second.Insert(box_second.Bounds);
            boxes_1 = tree_first.Search(1);
            boxes_2 = tree_second.Search(1);

            for (int i = 0; i<boxes_1.Count; i++) //проверка столкновений кубов однрго уровня
                for (int j = 0; j< boxes_2.Count; j++)
                {
                    if(boxes_1[i].IntersectsWith(boxes_2[j]))

                    for (int k=0;k<box_first.Points.Count; k++) //проверка имеет ли бокс хоть одну точку модели
                    {
                        if (boxes_1[i].Contains(box_first.Points[k]))
                            {
                                //boxes_1.Remove(boxes_1[i]);
                                if (boxes_1[i].SizeX>boxes_1[i].SizeY && boxes_1[i].SizeX > boxes_1[i].SizeZ)
                                {
                                    Rect3D tmp = boxes_1[i];
                                    tmp.SizeX = tmp.SizeX / 2;
                                    tree_first.Insert(tmp);
                                    //tmp.Location.X = tmp.Location.X + tmp.SizeX / 2;
                                }
                                    
                                break;
                            }

                    }

                }
            return true;
        }

        /*public List<Point3D> trans (List<Point3D> points)
        {
            for (int j = 0; j < points.Count; j++) //применяем трансформ к точкам
            {
                Point3D p = trans.Value.Transform(verts[j]);
                verts[j] = p;
            }
            return points;
        }*/
    }
}

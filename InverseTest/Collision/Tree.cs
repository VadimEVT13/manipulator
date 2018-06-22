using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace InverseTest.Collision
{
    class Tree
    {
        public Rect3D bound;
        public int lvl=0;
        public Tree left;
        public Tree right;

        

        public void Insert(Rect3D value)
        {
            if (this.bound == null)
            {
                this.bound = value;
                this.lvl = lvl++;
            }
            else
            {
                if (left == null)
                { this.left = new Tree(); this.left.lvl = lvl++; }
                left.Insert(value);

                if (right == null)
                { this.right = new Tree(); this.right.lvl = lvl++; }
                right.Insert(value);
            }
            
            }
        


        public List<Rect3D> boxes = new List<Rect3D>();
        public List<Rect3D> Search(int lvl)
        {
            if (this.lvl == lvl)
            {
                boxes.Add(this.bound);
                return boxes;
            }

            if (left != null)
            { boxes.AddRange(this.left.Search(lvl)); return boxes; }

            if (right != null)
            { boxes.AddRange(this.right.Search(lvl)); return boxes; }

            return boxes;
            //throw new Exception("Искомого узла в дереве нет");
        }
        
    }
}

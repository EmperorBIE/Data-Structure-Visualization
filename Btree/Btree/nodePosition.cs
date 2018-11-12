﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Btree {
    public class NodePosition {
        public int depth;
        // public List<int> totalParentsChild = new List<int>();
        public List<int> childIndex = new List<int>();
        public int[] keys;

        public NodePosition(List<int> childIndex, int depth, int[] keys){
            this.depth = depth;
            this.childIndex = childIndex;
            this.keys = keys;
        }
    }

    public class KeyPosition{
        public int depth, key,childIndex;

        public KeyPosition(int depth,int key,int childIndex){
            this.depth = depth;
            this.key = key;
            this.childIndex = childIndex;
        }
    }

    public class linePosition {
        public int x1, y1,x2,y2;

        public linePosition(int x1,int y1, int x2, int y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }
    }

    public class NodeContainer
    {
        public int x1, x2, depth, keysCount;

        public NodeContainer(int x1,int x2,int depth, int keysCount)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.depth = depth;
            this.keysCount = keysCount;
        }
    }
}

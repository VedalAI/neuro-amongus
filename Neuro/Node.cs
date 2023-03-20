using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Neuro
{
    public class Node : IHeapItem<Node>
    {
        public bool accessible;
        public Vector2 worldPosition;

        public Node parent;

        public int gridX;
        public int gridY;

        public int gCost;
        public int hCost;
        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }

        int heapIndex;

        public int HeapIndex
        {
            get
            {
                return heapIndex;
            }
            set
            {
                heapIndex = value;
            }
        }

        public Node(bool _accessible, Vector2 _worldPosition, int _gridX, int _gridY)
        {
            accessible = _accessible;
            worldPosition = _worldPosition;
            gridX = _gridX;
            gridY = _gridY;
        }

        public int CompareTo(Node other)
        {
            int compare = fCost.CompareTo(other.fCost);
            if (compare == 0)
            {
                compare = hCost.CompareTo(other.hCost);
            }
            return -compare;
        }
    }
}

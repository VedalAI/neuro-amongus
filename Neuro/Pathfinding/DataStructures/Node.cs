using System.Collections.Generic;
using UnityEngine;

namespace Neuro.Pathfinding.DataStructures;

public class Node : IHeapItem<Node>
{
    public bool accessible;

    public int gCost;

    public readonly int gridX;
    public readonly int gridY;
    public int hCost;

    public Node parent;
    public Vector2 worldPosition;

    public int transportSelfId;
    public int transportTargetId;

    public List<Node> transportNeighborsCache;

    public Node(bool _accessible, Vector2 _worldPosition, int _gridX, int _gridY)
    {
        accessible = _accessible;
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridY = _gridY;
    }

    private int fCost => gCost + hCost;

    public int HeapIndex { get; set; }

    public int CompareTo(Node other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0) compare = hCost.CompareTo(other.hCost);
        return -compare;
    }
}

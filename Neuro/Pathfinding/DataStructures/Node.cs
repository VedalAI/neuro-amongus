using System.Collections.Generic;
using UnityEngine;

namespace Neuro.Pathfinding.DataStructures;

public class Node : IHeapItem<Node>
{
    public Vector2 WorldPosition { get; }
    public Vector2Int GridPosition { get; }
    public bool IsAccessible { get; set; }

    public virtual bool IsTransportActive => true;

    public Node(Vector2 worldPosition, Vector2Int gridPosition, bool isAccessible)
    {
        WorldPosition = worldPosition;
        GridPosition = gridPosition;
        IsAccessible = isAccessible;
    }

    public int HeapIndex { get; set; }

    public Node parent;
    public int gCost;
    public int hCost;

    public Color color = Color.red;

    public int transportSelfId;
    public int transportTargetId;
    public List<Node> transportNeighborsCache;

    private int _fCost => gCost + hCost;

    public int CompareTo(Node other)
    {
        int compare = _fCost.CompareTo(other._fCost);
        if (compare == 0) compare = hCost.CompareTo(other.hCost);
        return -compare;
    }
}

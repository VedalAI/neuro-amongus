using System.Collections.Generic;
using UnityEngine;

namespace Neuro.Pathfinding.DataStructures;

public class Node : IHeapItem<Node>
{
    public Vector2 WorldPosition { get; }
    public Vector2Int GridPosition { get; }
    public bool IsAccessible { get; set; }

    public Color Color { get; set; } = Color.red;

    public int TransportSelfId { get; init; }
    public int TransportTargetId { get; init; }
    public List<Node> TransportNeighborsCache { get; set; }
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

    private int _fCost => gCost + hCost;

    public int CompareTo(Node other)
    {
        int compare = _fCost.CompareTo(other._fCost);
        if (compare == 0) compare = hCost.CompareTo(other.hCost);
        return -compare;
    }

    public virtual Node Clone()
    {
        return new Node(WorldPosition, GridPosition, IsAccessible)
        {
            Color = Color,
            TransportSelfId = TransportSelfId,
            TransportTargetId = TransportTargetId,
            TransportNeighborsCache = new List<Node>(TransportNeighborsCache)
        };
    }
}

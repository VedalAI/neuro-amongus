using System;
using System.Collections.Generic;
using System.Linq;
using Neuro.Events;
using Neuro.Pathfinding.DataStructures;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;
using Gizmos = Neuro.Utilities.Gizmos;

namespace Neuro.Pathfinding;

[RegisterInIl2Cpp]
public sealed class PathfindingHandler : MonoBehaviour
{
    private const float GRID_DENSITY = 5f; // TODO: Fine-tune individual maps to optimize performance
    private const int GRID_BASE_WIDTH = 100;

    private const int GRID_SIZE = (int)(GRID_BASE_WIDTH * GRID_DENSITY);
    private const int GRID_LOWER_BOUNDS = GRID_SIZE / -2;
    private const int GRID_UPPER_BOUNDS = GRID_SIZE / 2;

    public static PathfindingHandler Instance { get; private set; }

    public PathfindingHandler(IntPtr ptr) : base(ptr)
    {
    }

    private Node[,] _grid;

    private void Awake()
    {
        if (Instance)
        {
            NeuroUtilities.WarnDoubleSingletonInstance();
            Destroy(this);
            return;
        }

        Instance = this;
        EventManager.RegisterHandler(this);

        GenerateNodeGrid();
        FloodFill(ShipStatus.Instance.MeetingSpawnCenter + Vector2.down * ShipStatus.Instance.SpawnRadius);
    }

    private void GenerateNodeGrid()
    {
        _grid = new Node[GRID_SIZE, GRID_SIZE];

        const float NODE_RADIUS = 1 / GRID_DENSITY;

        for (int x = GRID_LOWER_BOUNDS; x < GRID_UPPER_BOUNDS; x++)
        for (int y = GRID_LOWER_BOUNDS; y < GRID_UPPER_BOUNDS; y++)
        {
            Vector2 point = new(x / GRID_DENSITY, y / GRID_DENSITY);

            Collider2D[] cols = Physics2D.OverlapCircleAll(point, NODE_RADIUS, LayerMask.GetMask("Ship", "ShortObjects"));
            int validColsCount = cols.Count(col =>
                !col.isTrigger &&
                !col.GetComponentInParent<Vent>() &&
                !col.GetComponentInParent<SomeKindaDoor>()
            );

            // TODO: Add edge case for Airship ladders

            bool accessible = validColsCount == 0;
            _grid[x + GRID_UPPER_BOUNDS, y + GRID_UPPER_BOUNDS] = new Node(accessible, point, x + GRID_UPPER_BOUNDS, y + GRID_UPPER_BOUNDS);
        }
    }

    private void FloodFill(Vector2 accessiblePosition)
    {
        Node startingNode = NodeFromWorldPoint(accessiblePosition);

        List<Node> openSet = new();
        HashSet<Node> closedSet = new();
        openSet.Add(startingNode);

        while (openSet.Count > 0)
        {
            Node node = openSet[0];
            openSet.Remove(node);
            closedSet.Add(node);

            foreach (Node neighbour in GetNeighbours(node))
            {
                if (!neighbour.accessible || closedSet.Contains(neighbour)) continue;
                if (!openSet.Contains(neighbour)) openSet.Add(neighbour);

                neighbour.parent = node;
            }
        }

        foreach (Node node in closedSet.ToList())
        {
            Gizmos.CreateNodeVisualPoint(node.worldPosition);
        }

        // Set all nodes not in closed set to inaccessible
        foreach (Node node in _grid)
        {
            if (!closedSet.Contains(node)) node.accessible = false;
        }
    }

    private readonly Dictionary<(Vector2 start, Vector2 target), Vector2[]> _cachedPaths = new();

    public Vector2[] FindPath(Vector2 start, Vector2 target)
    {
        if (_cachedPaths.TryGetValue((start, target), out Vector2[] cachedPath))
        {
            return cachedPath.ToArray();
        }

        bool pathSuccess = false;

        Node startNode = FindClosestNode(start);
        Node targetNode = FindClosestNode(target);

        if (startNode is not { accessible: true } || targetNode is not { accessible: true }) return Array.Empty<Vector2>();

        Gizmos.CreateDestinationVisualPoint(targetNode.worldPosition);

        Heap<Node> openSet = new(GRID_SIZE * GRID_SIZE);
        HashSet<Node> closedSet = new();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();

            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                pathSuccess = true;
                break;
            }

            foreach (Node neighbour in GetNeighbours(currentNode))
            {
                if (!neighbour.accessible || closedSet.Contains(neighbour)) continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                if (newMovementCostToNeighbour >= neighbour.gCost && openSet.Contains(neighbour)) continue;

                neighbour.gCost = newMovementCostToNeighbour;
                neighbour.hCost = GetDistance(neighbour, targetNode);
                neighbour.parent = currentNode;

                if (!openSet.Contains(neighbour))
                    openSet.Add(neighbour);
                else
                    openSet.UpdateItem(neighbour);
            }
        }

        if (pathSuccess)
        {
            Vector2[] path = RetracePath(startNode, targetNode);
            _cachedPaths[(start, target)] = path;
            return path.ToArray();
        }

        Warning("Failed to find path");
        return Array.Empty<Vector2>();
    }

    public float CalculateTotalDistance(Vector2 start, Vector2 target)
    {
        Vector2[] path = FindPath(start, target);
        if (path.Length == 0) return -1;

        float distance = 0f;
        for (int i = 0; i < path.Length - 1; i++)
        {
            distance += Vector2.Distance(path[i], path[i + 1]);
        }

        return distance;
    }

    public Vector2 CalculateOffsetToFirstNode(Vector2 start, Vector2 target)
    {
        Vector2[] path = FindPath(start, target);
        if (path.Length == 0) return Vector2.zero;

        Vector2 firstNode = path[0];

        return firstNode - start;
    }

    private Node NodeFromWorldPoint(Vector2 position)
    {
        position *= GRID_DENSITY;
        float clampedX = Mathf.Clamp(position.x, GRID_LOWER_BOUNDS, GRID_UPPER_BOUNDS);
        float clampedY = Mathf.Clamp(position.y, GRID_LOWER_BOUNDS, GRID_UPPER_BOUNDS);

        int xIndex = Mathf.RoundToInt(clampedX + GRID_UPPER_BOUNDS);
        int yIndex = Mathf.RoundToInt(clampedY + GRID_UPPER_BOUNDS);

        return _grid[xIndex, yIndex];
    }

    private Node FindClosestNode(Vector2 position)
    {
        Node closestNode = NodeFromWorldPoint(position);

        Queue<Node> queue = new();
        List<Node> nodes = new();
        queue.Enqueue(closestNode);
        nodes.Add(closestNode);

        while (!closestNode.accessible)
        {
            closestNode = queue.Dequeue();
            float closestDistance = Mathf.Infinity;
            Node closestNeighbour = null;
            foreach (Node neighbour in GetNeighbours(closestNode))
            {
                if (neighbour.accessible)
                {
                    float distance = Vector2.Distance(position, neighbour.worldPosition);
                    if (distance < closestDistance)
                    {
                        closestNeighbour = neighbour;
                        closestDistance = distance;
                    }
                }
                else
                {
                    if (!nodes.Contains(neighbour))
                    {
                        queue.Enqueue(neighbour);
                        nodes.Add(neighbour);
                    }
                }
            }

            if (closestNeighbour != null) closestNode = closestNeighbour;
        }

        return closestNode;
    }

    private List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new();

        for (int x = -1; x <= 1; x++)
        for (int y = -1; y <= 1; y++)
        {
            if (x == 0 && y == 0)
                continue;

            int checkX = node.gridX + x;
            int checkY = node.gridY + y;

            if (checkX is >= 0 and < GRID_SIZE && checkY is >= 0 and < GRID_SIZE) neighbours.Add(_grid[checkX, checkY]);
        }

        return neighbours;
    }

    private Vector2[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector2[] waypoints = path.Select(p => p.worldPosition).ToArray();
        new Span<Vector2>(waypoints).Reverse();

        return waypoints;
    }

    private static int GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);

        return 14 * dstY + 10 * Math.Abs(dstX - dstY);
    }

    [EventHandler(EventTypes.GameStarted)]
    public static void OnGameStarted()
    {
        ShipStatus.Instance.gameObject.AddComponent<PathfindingHandler>();
    }
}

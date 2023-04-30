using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Neuro.Pathfinding.DataStructures;
using Neuro.Utilities;
using UnityEngine;

namespace Neuro.Pathfinding;

public sealed class PathfindingThread : NeuroThread
{
    private const int THREAD_COUNT = 3;

    private readonly ConcurrentQueue<string> _queue = new();
    private readonly ConcurrentDictionary<string, (Vector2 start, Vector2 target)> _requests = new();
    private readonly ConcurrentDictionary<string, (Vector2 start, Vector2 target, Vector2[] path, float length)> _results = new();

    private readonly Dictionary<int, Node[,]> _grid = new();
    private readonly Transform _visualPointParent;

    public PathfindingThread(Node[,] grid,  List<Vector2> accessiblePositions, Transform visualPointParent) : base(THREAD_COUNT)
    {
        _visualPointParent = visualPointParent;
        for (int i = 0; i < THREAD_COUNT; i++)
        {
            _grid[i] = CloneNodeGrid(grid);
            FloodFill(accessiblePositions, i);
        }
    }

    public void RequestPath(Vector2 start, Vector2 target, string identifier)
    {
        if (_results.TryGetValue(identifier, out (Vector2 start, Vector2 target, Vector2[], float) result) &&
            result.start == start && result.target == target)
            return;

        if (!_queue.Contains(identifier)) _queue.Enqueue(identifier);

        _requests[identifier] = (start, target);
    }

    public bool TryGetPath(string identifier, out Vector2[] path, out float length, bool removeCloseNodes = true)
    {
        bool tried = _results.TryGetValue(identifier, out (Vector2, Vector2, Vector2[] path, float length) result);
        path = result.path;
        length = result.length;
        if (removeCloseNodes && path != null)
        {
            while (path.Length > 1 && Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), path[0]) < 0.5f)
            {
                path = path[1..];
            }
            // _results[identifier] = result with { path = path };
        }

        return tried;
    }

    // TODO: Instead of pathing to the closest node to the destination, path to the closest node to the player that is close enough to the destination
    protected override void RunThread(int id)
    {
        while (true)
        {
            try
            {
                Thread.Sleep(250);

                Warning(_queue.Count + " " + _requests.Count);
                while (!_queue.IsEmpty)
                {
                    Warning("Starting");
                    if (!_queue.TryDequeue(out string identifier)) continue;
                    Warning("Dequeued " + identifier + ", new length: " + _queue.Count);
                    if (!_requests.TryGetValue(identifier, out (Vector2 start, Vector2 target) vec)) continue;
                    Warning("Found request " + vec.start + " -> " + vec.target + " | current length: " + _requests.Count);
                    _requests.TryRemove(identifier, out _);
                    Warning("New requests length: " + _requests.Count);

                    Vector2[] path = FindPath(vec.start, vec.target, id);
                    Warning("Path found, " + path.Length + " nodes");
                    _results[identifier] = (vec.start, vec.target, path, CalculateLength(path));
                    Warning("Path cached, calculated length is " + _results[identifier].length);

                    Thread.Yield();
                    Warning("Yielded");
                }
            }
            catch (ThreadInterruptedException)
            {
                System.Console.WriteLine("[PATHFINDING] Caught thread interrupted");
                return;
            }
            catch (Exception e)
            {
                System.Console.WriteLine("[PATHFINDING] Caught another exception: " + e);
                Error(e);
            }
        }
    }

    private void FloodFill(List<Vector2> accessiblePositions, int threadId)
    {
        List<Node> openSet = new();
        HashSet<Node> closedSet = new();

        foreach (Vector2 accessiblePosition in accessiblePositions)
        {
            Node startingNode = NodeFromWorldPoint(accessiblePosition, threadId);
            startingNode.Color = Color.yellow;
            openSet.Add(startingNode);

            while (openSet.Count > 0)
            {
                Node node = openSet[0];
                openSet.Remove(node);
                closedSet.Add(node);

                foreach (Node neighbour in GetNeighbours(node, false, threadId))
                {
                    if (!neighbour.IsAccessible || closedSet.Contains(neighbour)) continue;
                    if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                }
            }
        }

        if (threadId == 0)
        {
            foreach (Node node in closedSet.ToList())
            {
                CreateNodeVisualPoint(node);
            }
        }

        // Set all nodes not in closed set to inaccessible
        foreach (Node node in _grid[threadId])
        {
            if (!closedSet.Contains(node)) node.IsAccessible = false;
        }
    }

    private Vector2[] FindPath(Vector2 start, Vector2 target, int threadId)
    {
        bool pathSuccess = false;

        Node startNode = FindClosestNode(start, threadId);
        Node targetNode = FindClosestNode(target, threadId);

        if (startNode is not { IsAccessible: true } ||
            targetNode is not { IsAccessible: true } ||
            startNode == targetNode) {return Array.Empty<Vector2>();}

        Heap<Node> openSet = new(PathfindingHandler.Instance.GridSize * PathfindingHandler.Instance.GridSize);
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

            foreach (Node neighbour in GetNeighbours(currentNode, true, threadId))
            {
                if (!neighbour.IsAccessible || closedSet.Contains(neighbour)) continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistanceCost(currentNode, neighbour);

                if (newMovementCostToNeighbour >= neighbour.gCost && openSet.Contains(neighbour)) continue;

                neighbour.gCost = newMovementCostToNeighbour;
                neighbour.hCost = GetDistanceCost(neighbour, targetNode);
                neighbour.parent = currentNode;

                if (!openSet.Contains(neighbour))
                    openSet.Add(neighbour);
                else
                    openSet.UpdateItem(neighbour);
            }
        }

        if (pathSuccess)
        {
            return RetracePath(startNode, targetNode);
        }

        Warning("Failed to find path");
        return Array.Empty<Vector2>();
    }

    private float CalculateLength(Vector2[] path)
    {
        if (path.Length == 0) return -1;

        float length = 0f;
        for (int i = 0; i < path.Length - 1; i++)
        {
            length += Vector2.Distance(path[i], path[i + 1]);
        }

        return length;
    }

    private Node FindClosestNode(Vector2 position, int threadId)
    {
        Node closestNode = NodeFromWorldPoint(position, threadId);

        Queue<Node> queue = new();
        List<Node> nodes = new();
        queue.Enqueue(closestNode);
        nodes.Add(closestNode);

        while (!closestNode.IsAccessible)
        {
            closestNode = queue.Dequeue();
            float closestDistance = float.PositiveInfinity;
            Node closestNeighbour = null;
            foreach (Node neighbour in GetNeighbours(closestNode, true, threadId))
            {
                if (neighbour.IsAccessible)
                {
                    float distance = Vector2.Distance(position, neighbour.WorldPosition);
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

    private Node NodeFromWorldPoint(Vector2 position, int threadId)
    {
        position *= PathfindingHandler.Instance.GridDensity;
        float clampedX = Math.Clamp(position.x, PathfindingHandler.Instance.GridLowerBounds, PathfindingHandler.Instance.GridUpperBounds);
        float clampedY = Math.Clamp(position.y, PathfindingHandler.Instance.GridLowerBounds, PathfindingHandler.Instance.GridUpperBounds);

        int xIndex = (int) Math.Round(clampedX + PathfindingHandler.Instance.GridUpperBounds);
        int yIndex = (int) Math.Round(clampedY + PathfindingHandler.Instance.GridUpperBounds);

        return _grid[threadId][xIndex, yIndex];
    }

    private List<Node> GetNeighbours(Node node, bool includeTransport, int threadId)
    {
        List<Node> neighbours = new();

        for (int x = -1; x <= 1; x++)
        for (int y = -1; y <= 1; y++)
        {
            if (x == 0 && y == 0)
                continue;

            int checkX = node.GridPosition.x + x;
            int checkY = node.GridPosition.y + y;

            if (checkX >= 0 && checkX < PathfindingHandler.Instance.GridSize &&
                checkY >= 0 && checkY < PathfindingHandler.Instance.GridSize) neighbours.Add(_grid[threadId][checkX, checkY]);
        }

        if (!includeTransport || !node.IsTransportActive || node.TransportSelfId == 0) return neighbours;

        if (node.TransportNeighborsCache == null)
        {
            node.TransportNeighborsCache = new List<Node>();
            foreach (Node otherNode in _grid[threadId])
            {
                if (otherNode.TransportSelfId == node.TransportTargetId) node.TransportNeighborsCache.Add(otherNode);
            }
        }

        neighbours.AddRange(node.TransportNeighborsCache);

        return neighbours;
    }

    private static Vector2[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return SimplifyPath(path);
    }

    private static Vector2[] SimplifyPath(List<Node> path)
    {
        List<Vector2> waypoints = new();
        Vector2 directionOld = Vector2.zero;
        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector2 directionNew = new(path[i].WorldPosition.x - path[i + 1].WorldPosition.x, path[i].WorldPosition.y - path[i + 1].WorldPosition.y);
            if (directionNew != directionOld || path[i].TransportSelfId != 0 || path[i + 1].TransportSelfId != 0)
            {
                waypoints.Add(path[i].WorldPosition);
            }

            directionOld = directionNew;
        }

        // add last waypoint
        waypoints.Add(path[^1].WorldPosition);

        return waypoints.ToArray();
    }

    private static int GetDistanceCost(Node a, Node b)
    {
        int dstX = Math.Abs(a.GridPosition.x - b.GridPosition.x);
        int dstY = Math.Abs(a.GridPosition.y - b.GridPosition.y);

        return 14 * Math.Min(dstX, dstY) + 10 * Math.Abs(dstX - dstY);
    }

    // TODO: Create 'Gizmos' debug tab with this and paths and stuff maybe
    private void CreateNodeVisualPoint(Node node) => CreateVisualPoint(node.WorldPosition, node.Color, 0.1f);

    private void CreateVisualPoint(Vector2 position, Color color, float widthMultiplier)
    {
        if (!_visualPointParent) return;

        Vector3 calculatedPosition = new(position.x, position.y, position.y / 1000f + 0.0005f);

        GameObject nodeVisualPoint = new("Gizmo (Visual Point)")
        {
            transform =
            {
                position = calculatedPosition,
                parent = _visualPointParent
            }
        };

        LineRenderer renderer = nodeVisualPoint.AddComponent<LineRenderer>();
        renderer.SetPosition(0, calculatedPosition);
        renderer.SetPosition(1, calculatedPosition + new Vector3(0, widthMultiplier));
        renderer.widthMultiplier = widthMultiplier;
        renderer.positionCount = 2;
        renderer.material = NeuroUtilities.MaskShaderMat;
        renderer.startColor = color;
        renderer.endColor = color;
    }

    private static Node[,] CloneNodeGrid(Node[,] input)
    {
        Node[,] newGrid = new Node[PathfindingHandler.Instance.GridSize, PathfindingHandler.Instance.GridSize];

        for (int x = 0; x < PathfindingHandler.Instance.GridSize; x++)
        for (int y = 0; y < PathfindingHandler.Instance.GridSize; y++)
        {
            newGrid[x, y] = input[x, y].Clone();
        }

        return newGrid;
    }
}

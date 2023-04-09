using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Neuro.Communication.AmongUsAI.DataStructures;
using Neuro.Pathfinding.DataStructures;
using Neuro.Utilities.DataStructures;
using Reactor.Utilities.Attributes;
using UnityEngine;
using Thread = Il2CppSystem.Threading.Thread;

namespace Neuro.Pathfinding;

[RegisterInIl2Cpp]
public sealed class PathfindingThread : Il2CppSystem.Object
{
    private readonly Queue<string> _queue = new();
    private readonly Dictionary<string, (MyVector2 start, MyVector2 target)> _requests = new();
    private readonly Dictionary<string, (MyVector2 start, MyVector2 target, MyVector2[] path)> _results = new();

    private readonly Node[,] _grid;
    private readonly Thread _thread;
    private bool _stop;

    public PathfindingThread(Node[,] grid, MyVector2 accessiblePosition)
    {
        _grid = grid;
        FloodFill(accessiblePosition);

        _thread = new Thread(new Action(RunThread));
    }

    public void Start()
    {
        if (!_thread.IsAlive)
        {
            _thread.Start();
        }
    }

    public void Stop()
    {
        if (_thread.IsAlive)
        {
            _stop = true;
        }
    }

    private void CheckForStop()
    {
        if (_stop) throw new ThreadInterruptedException();
    }

    public void RequestPath(MyVector2 start, MyVector2 target, string identifier)
    {
        if (_results.TryGetValue(identifier, out (MyVector2 start, MyVector2 target, MyVector2[] path) result) &&
            result.start == start && result.target == target)
            return;

        if (!_queue.Contains(identifier)) _queue.Enqueue(identifier);

        _requests[identifier] = (start, target);
    }

    public bool TryGetPath(string identifier, out MyVector2[] path)
    {
        bool tried = _results.TryGetValue(identifier, out (MyVector2 start, MyVector2 target, MyVector2[] path) result);
        path = result.path;
        return tried;
    }

    private void RunThread()
    {
        while (true)
        {
            try
            {
                System.Console.WriteLine($"Running pathfinding with {_requests.Count} requests");
                while (_requests.Count > 0)
                {
                    string identifier = _queue.Dequeue();
                    (MyVector2 start, MyVector2 target) = _requests[identifier];
                    _results[identifier] = (start, target, FindPath(start, target));
                    _requests.Remove(identifier);

                    Thread.Yield();
                    CheckForStop();
                }

                Thread.Sleep(250);
                CheckForStop();
            }
            catch (ThreadInterruptedException)
            {
                return;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                try
                {
                    Thread.Yield();
                    CheckForStop();
                }
                catch
                {
                    return;
                }
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private void FloodFill(MyVector2 accessiblePosition)
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

        /*
        foreach (Node node in closedSet.ToList())
        {
            Gizmos.CreateNodeVisualPoint(node.worldPosition);
        }
        */

        // Set all nodes not in closed set to inaccessible
        foreach (Node node in _grid)
        {
            if (!closedSet.Contains(node)) node.accessible = false;
        }
    }

    private MyVector2[] FindPath(MyVector2 start, MyVector2 target)
    {
        bool pathSuccess = false;

        Node startNode = FindClosestNode(start);
        Node targetNode = FindClosestNode(target);


        if (startNode is not { accessible: true } || targetNode is not { accessible: true }) return Array.Empty<MyVector2>();

        Heap<Node> openSet = new(PathfindingHandler.GRID_SIZE * PathfindingHandler.GRID_SIZE);
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
            return RetracePath(startNode, targetNode);
        }

        Warning("Failed to find path");
        return Array.Empty<MyVector2>();
    }

    private Node FindClosestNode(MyVector2 position)
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
                    float distance = MyVector2.Distance(position, neighbour.worldPosition);
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

    private Node NodeFromWorldPoint(MyVector2 position)
    {
        position *= PathfindingHandler.GRID_DENSITY;
        float clampedX = Mathf.Clamp(position.x, PathfindingHandler.GRID_LOWER_BOUNDS, PathfindingHandler.GRID_UPPER_BOUNDS);
        float clampedY = Mathf.Clamp(position.y, PathfindingHandler.GRID_LOWER_BOUNDS, PathfindingHandler.GRID_UPPER_BOUNDS);

        int xIndex = Mathf.RoundToInt(clampedX + PathfindingHandler.GRID_UPPER_BOUNDS);
        int yIndex = Mathf.RoundToInt(clampedY + PathfindingHandler.GRID_UPPER_BOUNDS);

        return _grid[xIndex, yIndex];
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

            if (checkX is >= 0 and < PathfindingHandler.GRID_SIZE && checkY is >= 0 and < PathfindingHandler.GRID_SIZE) neighbours.Add(_grid[checkX, checkY]);
        }

        return neighbours;
    }

    private static MyVector2[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        MyVector2[] waypoints = path.Select(p => p.worldPosition).ToArray();
        new Span<MyVector2>(waypoints).Reverse();

        return waypoints;
    }

    private static int GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);

        return 14 * dstY + 10 * Math.Abs(dstX - dstY);
    }
}

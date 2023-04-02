using System;
using System.Collections.Generic;
using System.Linq;
using Neuro.Pathfinding.DataStructures;
using UnityEngine;

namespace Neuro.Pathfinding;

public class PathfindingHandler
{
    private const float GRID_DENSITY = 6f; // TODO: Fine-tune individual maps to optimize performance
    private const int GRID_BASE_WIDTH = 100;
    private const int GRID_SIZE = (int)(GRID_BASE_WIDTH * GRID_DENSITY);
    private const int GRID_LOWER_BOUNDS = GRID_SIZE / -2;
    private const int GRID_UPPER_BOUNDS = GRID_SIZE / 2;

    private Node[,] grid;

    public void Initialize()
    {
        GenerateNodeGrid();

        FloodFill(ShipStatus.Instance.MeetingSpawnCenter + Vector2.down * ShipStatus.Instance.SpawnRadius);
    }

    public Vector2[] FindPath(Vector2 start, Vector2 target)
    {
        bool pathSuccess = false;

        /*GameObject test = new GameObject("Test");
        //Info(test.transform);
        test.transform.position = (Vector3)start;

        LineRenderer renderer = test.AddComponent<LineRenderer>();
        renderer.SetPosition(0, start);
        renderer.SetPosition(1, (Vector3)target + new Vector3(0, 0.1f, 0));
        renderer.widthMultiplier = 0.2f;
        renderer.positionCount = 2;
        renderer.startColor = Color.blue;*/

        Info("Start Node");
        Node startNode = FindClosestNode(start);
        Info("End Node");
        Node targetNode = FindClosestNode(target);

        GameObject endNodeObj = new("Task Visual Point");

        Vector3 position = targetNode.worldPosition;
        endNodeObj.transform.position = position;

        LineRenderer renderer2 = endNodeObj.AddComponent<LineRenderer>();
        renderer2.SetPosition(0, position);
        renderer2.SetPosition(1, position + new Vector3(0f, 0.3f, 0));
        renderer2.widthMultiplier = 0.3f;
        renderer2.positionCount = 2;
        renderer2.material = new Material(Shader.Find("Unlit/MaskShader"));
        renderer2.startColor = Color.blue;
        renderer2.endColor = Color.blue;

        Info($"startNode: {startNode.worldPosition} targetNode: {targetNode.worldPosition}");

        if (startNode is not { accessible: true } || targetNode is not { accessible: true }) return Array.Empty<Vector2>();

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

                /*GameObject nodeGO = new GameObject("Test");
                //Info(test.transform);
                nodeGO.transform.position = (Vector3)currentNode.worldPosition;

                LineRenderer nodeRenderer = nodeGO.AddComponent<LineRenderer>();
                nodeRenderer.SetPosition(0, (Vector3)currentNode.worldPosition);
                nodeRenderer.SetPosition(1, (Vector3)currentNode.worldPosition + new Vector3(0, 0.1f, 0));
                nodeRenderer.widthMultiplier = 0.1f;
                nodeRenderer.positionCount = 2;
                nodeRenderer.startColor = Color.red;*/

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
            Info("Path found successfully.");
            return RetracePath(startNode, targetNode);
        }

        Warning("Failed to find path");
        return Array.Empty<Vector2>();
    }

    private void GenerateNodeGrid()
    {
        grid = new Node[GRID_SIZE, GRID_SIZE];

        const float NODE_RADIUS = 1 / GRID_DENSITY;

        for (int x = GRID_LOWER_BOUNDS; x < GRID_UPPER_BOUNDS; x++)
        for (int y = GRID_LOWER_BOUNDS; y < GRID_UPPER_BOUNDS; y++)
        {
            Vector2 point = new(x / GRID_DENSITY, y / GRID_DENSITY);

            //Info(point.ToString());
            Collider2D[] cols = Physics2D.OverlapCircleAll(point, NODE_RADIUS, LayerMask.GetMask("Ship", "ShortObjects"));
            int validColsCount = cols.Count(col =>
                !col.isTrigger
                && !col.GetComponentInParent<Vent>()
                && !col.GetComponentInParent<SomeKindaDoor>()
            );

            // TODO: Add edge case for Airship ladders

            bool accessible = validColsCount == 0;
            grid[x + GRID_UPPER_BOUNDS, y + GRID_UPPER_BOUNDS] = new Node(accessible, point, x + GRID_UPPER_BOUNDS, y + GRID_UPPER_BOUNDS);
        }
    }

    private void FloodFill(Vector2 accessiblePosition)
    {
        Node startingNode = NodeFromWorldPoint(accessiblePosition);

        // Flood fill
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

        Material nodeMaterial = new(Shader.Find("Unlit/MaskShader"));
        foreach (Node node in closedSet.ToList())
        {
            GameObject nodeVisualPoint = new("Node Visual Point");

            Vector3 position = node.worldPosition;
            nodeVisualPoint.transform.position = position;

            LineRenderer renderer = nodeVisualPoint.AddComponent<LineRenderer>();
            renderer.SetPosition(0, position);
            renderer.SetPosition(1, position + new Vector3(0, 0.1f, 0));
            renderer.widthMultiplier = 0.1f;
            renderer.positionCount = 2;
            renderer.material = nodeMaterial;
            renderer.startColor = Color.red;
            renderer.endColor = Color.red;
        }

        // Set all nodes not in closed set to inaccessible
        foreach (Node node in grid)
        {
            if (!closedSet.Contains(node)) node.accessible = false;
        }
    }

    private Node NodeFromWorldPoint(Vector2 position)
    {
        position *= GRID_DENSITY;
        float clampedX = Mathf.Clamp(position.x, GRID_LOWER_BOUNDS, GRID_UPPER_BOUNDS);
        float clampedY = Mathf.Clamp(position.y, GRID_LOWER_BOUNDS, GRID_UPPER_BOUNDS);

        int xIndex = Mathf.RoundToInt(clampedX + GRID_UPPER_BOUNDS);
        int yIndex = Mathf.RoundToInt(clampedY + GRID_UPPER_BOUNDS);

        return grid[xIndex, yIndex];
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

            if (checkX is >= 0 and < GRID_SIZE && checkY is >= 0 and < GRID_SIZE) neighbours.Add(grid[checkX, checkY]);
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

    private void DrawPath(Vector2[] path)
    {
        // TODO: Cache old path instead of destroying it by name
        GameObject.Destroy(GameObject.Find("Neuro Path"));
        GameObject test = new("Neuro Path");
        //Info(test.transform);
        test.transform.position = PlayerControl.LocalPlayer.transform.position;

        LineRenderer renderer = test.AddComponent<LineRenderer>();
        renderer.positionCount = path.Length;
        for (int i = 0; i < path.Length; i++)
        {
            Info(path[i].ToString());
            renderer.SetPosition(i, path[i]);
        }

        renderer.widthMultiplier = 0.2f;
        renderer.startColor = Color.blue;
    }

    private static int GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);

        return 14 * dstY + 10 * Math.Abs(dstX - dstY); // TODO: 14 magic number?
    }
}
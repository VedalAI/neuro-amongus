using System;
using System.Collections.Generic;
using System.Linq;
using Neuro.DependencyInjection;
using Neuro.Pathfinding.DataStructures;
using UnityEngine;

namespace Neuro.Pathfinding;

public class PathfindingHandler : IPathfindingHandler
{
    private const int GRID_SIZE = 500;
    private const int GRID_LOWER_LIMIT = GRID_SIZE / -2;
    private const int GRID_UPPER_LIMIT = GRID_SIZE / 2;

    public IContextProvider Context { get; set; }

    public void Initialize()
    {
        GenerateNodeGrid();
        FloodFill(ShipStatus.Instance.MeetingSpawnCenter + Vector2.up * ShipStatus.Instance.SpawnRadius + new Vector2(0f, 0.3636f)); // TODO: Magic number?
    }

    private Node[,] grid;

    private void GenerateNodeGrid()
    {
        grid = new Node[GRID_SIZE, GRID_SIZE];

        for (int x = GRID_LOWER_LIMIT; x < GRID_UPPER_LIMIT; x++)
        for (int y = GRID_LOWER_LIMIT; y < GRID_UPPER_LIMIT; y++)
        {
            Vector2 point = new(x / 4f, y / 4f);

            //Debug.Log(point.ToString());
            Collider2D[] cols = Physics2D.OverlapCircleAll(point, 0.25f, LayerMask.GetMask("Ship", "ShortObjects"));
            List<Collider2D> colsList = cols.Where(col => !col.isTrigger && !col.transform.name.Contains("Vent") && !col.transform.name.Contains("Door")).ToList();

            bool accessible = colsList.Count == 0;

            //Debug.Log(accessible);
            if (accessible)
            {
                /*GameObject test = new GameObject("Test");
                    //Debug.Log(test.transform);
                    test.transform.position = (Vector3)point;

                    LineRenderer renderer = test.AddComponent<LineRenderer>();
                    renderer.SetPosition(0, test.transform.position);
                    renderer.SetPosition(1, test.transform.position + new Vector3(0, 0.1f, 0));
                    renderer.widthMultiplier = 0.1f;
                    renderer.positionCount = 2;
                    renderer.startColor = Color.red;*/
            }

            grid[x + GRID_UPPER_LIMIT, y + GRID_UPPER_LIMIT] = new Node(accessible, point, x + GRID_UPPER_LIMIT, y + GRID_UPPER_LIMIT);
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

        foreach (Node node in closedSet.ToList())
        {
            GameObject test = new("Test");
            //Debug.Log(test.transform);
            test.transform.position = node.worldPosition;

            LineRenderer renderer = test.AddComponent<LineRenderer>();
            renderer.SetPosition(0, test.transform.position);
            renderer.SetPosition(1, test.transform.position + new Vector3(0, 0.1f, 0));
            renderer.widthMultiplier = 0.1f;
            renderer.positionCount = 2;
            renderer.startColor = Color.red;
        }

        // Set all nodes not in closed set to inaccessible
        foreach (Node node in grid)
        {
            if (!closedSet.Contains(node)) node.accessible = false;
        }
    }

    private Node NodeFromWorldPoint(Vector2 position)
    {
        position *= 4;
        float percentX = Mathf.Clamp(position.x, -100, 100);
        float percentY = Mathf.Clamp(position.y, -100, 100);

        int xIndex = Mathf.RoundToInt(percentX + 100);
        int yIndex = Mathf.RoundToInt(percentY + 100);

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

    private Vector2[] FindPath(Vector2 start, Vector2 target)
    {
        bool pathSuccess = false;

        /*GameObject test = new GameObject("Test");
        //Debug.Log(test.transform);
        test.transform.position = (Vector3)start;

        LineRenderer renderer = test.AddComponent<LineRenderer>();
        renderer.SetPosition(0, start);
        renderer.SetPosition(1, (Vector3)target + new Vector3(0, 0.1f, 0));
        renderer.widthMultiplier = 0.2f;
        renderer.positionCount = 2;
        renderer.startColor = Color.blue;*/

        // Debug.Log("Start Node");
        Node startNode = FindClosestNode(start);
        // Debug.Log("End Node");
        Node targetNode = FindClosestNode(target);

        GameObject endNodeObj = new("Test");
        //Debug.Log(test.transform);
        endNodeObj.transform.position = targetNode.worldPosition;

        LineRenderer renderer2 = endNodeObj.AddComponent<LineRenderer>();
        renderer2.SetPosition(0, targetNode.worldPosition);
        renderer2.SetPosition(1, (Vector3) targetNode.worldPosition + new Vector3(0f, 0.3f, 0));
        renderer2.widthMultiplier = 0.3f;
        renderer2.positionCount = 2;
        renderer2.startColor = Color.blue;

        // Debug.Log(startNode.worldPosition.ToString());
        // Debug.Log(targetNode.worldPosition.ToString());

        if (startNode is not {accessible: true} || targetNode is not {accessible: true}) return Array.Empty<Vector2>();

        Heap<Node> openSet = new(200 * 200);
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
                //Debug.Log(test.transform);
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
            Debug.Log("Path found successfully.");
            return RetracePath(startNode, targetNode);
        }

        Debug.Log("Failed to find path");
        return Array.Empty<Vector2>();
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
        Array.Reverse(waypoints);

        return waypoints;
    }

    private void DrawPath(Vector2[] path)
    {
        // TODO: Cache old path instead of destroying it by name
        GameObject.Destroy(GameObject.Find("Neuro Path"));
        GameObject test = new("Neuro Path");
        //Debug.Log(test.transform);
        test.transform.position = PlayerControl.LocalPlayer.transform.position;

        LineRenderer renderer = test.AddComponent<LineRenderer>();
        renderer.positionCount = path.Length;
        for (int i = 0; i < path.Length; i++)
        {
            Debug.Log(path[i].ToString());
            renderer.SetPosition(i, path[i]);
        }

        renderer.widthMultiplier = 0.2f;
        renderer.startColor = Color.blue;
    }

    private static int GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);

        return 14 * dstY + 10 * Math.Abs(dstX - dstY);
    }
}

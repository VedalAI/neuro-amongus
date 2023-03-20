using Reactor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Neuro
{
    public class Pathfinding
    {
        public Node[,] grid;

        public void GenerateNodeGrid()
        {
            grid = new Node[200, 200];

            for (int x = -100; x < 100; x++)
            {
                for (int y = -100; y < 100; y++)
                {
                    Vector2 point = new Vector2(x / 4f, y / 4f);

                    //Debug.Log(point.ToString());

                    Collider2D[] cols = Physics2D.OverlapCircleAll(point, 0.25f, LayerMask.GetMask(new[] { "Ship", "ShortObjects" }));
                    List<Collider2D> colsList = new List<Collider2D>();
                    foreach (Collider2D col in cols)
                    {
                        if (col.isTrigger || col.transform.name.Contains("Vent") || col.transform.name.Contains("Door")) continue;
                        colsList.Add(col);
                    }

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

                    grid[x + 100, y + 100] = new Node(accessible, point, x + 100, y + 100);
                }
            }
        }

        public void FloodFill(Vector2 accessiblePosition)
        {
            Node startingNode = NodeFromWorldPoint(accessiblePosition);

            // Flood fill
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
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
                GameObject test = new GameObject("Test");
                //Debug.Log(test.transform);
                test.transform.position = (Vector3)node.worldPosition;

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

        public Node NodeFromWorldPoint(Vector2 position)
        {
            position *= 4;
            float percentX = Mathf.Clamp(position.x, -100, 100);
            float percentY = Mathf.Clamp(position.y, -100, 100);

            int xIndex = Mathf.RoundToInt(percentX + 100);
            int yIndex = Mathf.RoundToInt(percentY + 100);

            return grid[xIndex, yIndex];
        }

        public Node FindClosestNode(Vector2 position)
        {
            Node closestNode = NodeFromWorldPoint(position);

            Queue<Node> queue = new Queue<Node>();
            List<Node> nodes = new List<Node>();
            queue.Enqueue(closestNode);
            nodes.Add(closestNode);

            while(!closestNode.accessible)
            {
                closestNode = queue.Dequeue();
                float closestDistance = Mathf.Infinity;
                Node closestNeighbour = null;
                foreach(Node neighbour in GetNeighbours(closestNode))
                {
                    if(neighbour.accessible)
                    {
                        float distance = Vector2.Distance(position, neighbour.worldPosition);
                        if(distance < closestDistance)
                        {
                            closestNeighbour = neighbour;
                            closestDistance = distance;
                        }
                    } else
                    {
                        if(!nodes.Contains(neighbour))
                        {
                            queue.Enqueue(neighbour);
                            nodes.Add(neighbour);
                        }
                    }
                }
                if(closestNeighbour != null)
                {
                    closestNode = closestNeighbour;
                }
            }
            return closestNode;
        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < 200 && checkY >= 0 && checkY < 200)
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }

            return neighbours;
        }

        public Vector2[] FindPath(Vector2 start, Vector2 target)
        {
            Vector2[] path = new Vector2[0];
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

            Debug.Log("Start Node");
            Node startNode = FindClosestNode(start);
            Debug.Log("End Node");
            Node targetNode = FindClosestNode(target);

            GameObject endNodeObj = new GameObject("Test");
            //Debug.Log(test.transform);
            endNodeObj.transform.position = (Vector3)targetNode.worldPosition;

            LineRenderer renderer2 = endNodeObj.AddComponent<LineRenderer>();
            renderer2.SetPosition(0, (Vector3)targetNode.worldPosition);
            renderer2.SetPosition(1, (Vector3)targetNode.worldPosition + new Vector3(0f, 0.3f, 0));
            renderer2.widthMultiplier = 0.3f;
            renderer2.positionCount = 2;
            renderer2.startColor = Color.blue;

            Debug.Log(startNode.worldPosition.ToString());
            Debug.Log(targetNode.worldPosition.ToString());

            if (startNode == null || targetNode == null || !startNode.accessible || !targetNode.accessible)
            {
                return path;
            }

            Heap<Node> openSet = new Heap<Node>(200 * 200);
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(startNode);

            while(openSet.Count > 0) {
                Node currentNode = openSet.RemoveFirst();

                closedSet.Add(currentNode);

                if(currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach(Node neighbour in GetNeighbours(currentNode))
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

                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                    
                }
            }

            if(pathSuccess)
            {
                Debug.Log("Path found successfully.");
                return RetracePath(startNode, targetNode);
            } else
            {
                Debug.Log("Failed to find path");
                return path;
            }
        }

        Vector2[] RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;
            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            Vector2[] waypoints = SimplifyPath(path);
            Array.Reverse(waypoints);

            return waypoints;
        }

        Vector2[] SimplifyPath(List<Node> path)
        {
            List<Vector2> waypoints = new List<Vector2>();
            Vector2 directionOld = Vector2.zero;
            for (int i = 0; i < path.Count; i++)
            {
                waypoints.Add(path[i].worldPosition);
            }
            return waypoints.ToArray();
        }

        int GetDistance(Node a, Node b)
        {
            int dstX = Mathf.Abs(a.gridX - b.gridX);
            int dstY = Mathf.Abs(a.gridY - b.gridY);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }
}

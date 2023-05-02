using System;
using System.Collections;
using System.Collections.Generic;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(Weather1Game))]
public sealed class FixWeatherNodeSolver : GeneralMinigameSolver<Weather1Game>
{
    public override IEnumerator CompleteMinigame(Weather1Game minigame, NormalPlayerTask task)
    {
        Vector3Int[] solution = SolveMaze(minigame);

        Vector3 tileBounds = minigame.fillTile.sprite.bounds.max * 2;
        Vector3 mapPos = minigame.transform.position;

        WaitForSeconds wait = new(0.04f);
        foreach (Vector3Int tile in solution)
        {
            Vector3 tileRealPosition = new Vector3((tile.x + 1) * tileBounds.x, (tile.y + 1) * tileBounds.y, 0);
            yield return InGameCursor.Instance.CoMoveTo(tileRealPosition + mapPos);

            if (!InGameCursor.Instance.IsLeftButtonPressed) InGameCursor.Instance.StartHoldingLMB(minigame);
            yield return wait;
        }

        InGameCursor.Instance.StopHoldingLMB();
    }

    private static Vector3Int[] SolveMaze(Weather1Game minigame)
    {
        BfsNode startTile = new BfsNode { Parent = null, Position = minigame.controlTilePos };
        Queue<BfsNode> searchQueue = new();
        HashSet<Vector3Int> searchedPositions = new();
        searchQueue.Enqueue(startTile);

        BfsNode endTile = startTile;
        while (searchQueue.TryDequeue(out BfsNode tile))
        {
            searchedPositions.Add(tile.Position);

            if (tile.Position is { x: 7, y: -3 })
            {
                // The exit tile's position is (7, -3)
                endTile = tile;
                break;
            }

            foreach (BfsNode neighbour in GetNeighbours(minigame, tile))
            {
                if (searchedPositions.Contains(neighbour.Position)) continue;
                searchQueue.Enqueue(neighbour);
            }
        }

        if (endTile.Position == startTile.Position) Warning("A valid path was not found!");

        return RetracePath(startTile, endTile);
    }

    private static List<BfsNode> GetNeighbours(Weather1Game minigame, BfsNode parentNode)
    {
        List<BfsNode> neighbours = new();

        for (int x = -1; x < 2; x++)
        for (int y = -1; y < 2; y++)
        {
            // x | 0 | x
            // 0 | x | 0
            // x | 0 | x
            if (Mathf.Abs(x) == Mathf.Abs(y)) continue;

            Vector3Int point = parentNode.Position + new Vector3Int(x, y, 0);
            if (!minigame.PointIsValid(point)) continue;
            if (minigame.BarrierMap.GetTile(point)) continue;

            neighbours.Add(new BfsNode { Parent = parentNode, Position = point});
        }

        return neighbours;
    }

    private static Vector3Int[] RetracePath(BfsNode startNode, BfsNode endNode)
    {
        List<Vector3Int> path = new(19); // The minimum tiles to get to the exit tile is 19
        BfsNode currentNode = endNode;
        while (currentNode.Position != startNode.Position)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }

        path.Add(startNode.Position);

        Vector3Int[] pathArray = path.ToArray();
        new Span<Vector3Int>(pathArray).Reverse();

        return pathArray;
    }
}

public record BfsNode
{
    public BfsNode Parent;
    public Vector3Int Position;
}
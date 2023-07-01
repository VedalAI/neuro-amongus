using System.Collections;
using System.Collections.Generic;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(Weather1Game))]
[MinigameSolver(typeof(WeatherSwitchGame))]
public sealed class FixWeatherNodeSolver : IMinigameSolver<Minigame>, IMinigameOpener
{
    public float CloseTimout => 10;

    public bool ShouldOpenConsole(Console console, PlayerTask task) => true;

    public IEnumerator CompleteMinigame(Minigame minigame)
    {
        if (minigame.TryCast<Weather1Game>() is { } part1)
            yield return CompleteStage1(part1);
        else
            yield return CompleteStage2(minigame.Cast<WeatherSwitchGame>());
    }

    private IEnumerator CompleteStage1(Weather1Game minigame)
    {
        IEnumerable<Vector3Int> solution = SolveMaze(minigame);

        Vector3 tileBounds = minigame.fillTile.sprite.bounds.max * 2;
        Vector3 mapPos = minigame.transform.position;

        foreach (Vector3Int tile in solution)
        {
            Vector3 tileRealPosition = new((tile.x + 1) * tileBounds.x, (tile.y + 1) * tileBounds.y, 0);
            yield return InGameCursor.Instance.CoMoveTo(tileRealPosition + mapPos, 1f);

            if (!InGameCursor.Instance.IsLeftButtonPressed) InGameCursor.Instance.StartHoldingLMB(minigame);
        }

        InGameCursor.Instance.StopHoldingLMB();
    }

    private IEnumerator CompleteStage2(WeatherSwitchGame minigame)
    {
        WeatherControl desiredSwitch = minigame.Controls[minigame.WeatherTask.NodeId];

        yield return InGameCursor.Instance.CoMoveTo(desiredSwitch.Switch);
        yield return InGameCursor.Instance.CoPressLMB(); // TODO: Sometimes doesn't work :(
    }

    private static IEnumerable<Vector3Int> SolveMaze(Weather1Game minigame)
    {
        BfsNode startTile = new() {Parent = null, Position = minigame.controlTilePos};
        HashSet<Vector3Int> searchedPositions = new();
        Queue<BfsNode> searchQueue = new();
        searchQueue.Enqueue(startTile);

        BfsNode endTile = startTile;
        while (searchQueue.TryDequeue(out BfsNode tile))
        {
            searchedPositions.Add(tile.Position);

            if (tile.Position is {x: 7, y: -3})
            {
                // The exit tile's position is (7, -3)
                endTile = tile;
                break;
            }

            foreach (BfsNode neighbour in GetValidNeighbours(tile, minigame))
            {
                if (searchedPositions.Contains(neighbour.Position)) continue;
                searchQueue.Enqueue(neighbour);
            }
        }

        if (endTile.Position == startTile.Position) Warning("A valid path was not found!");

        return RetracePath(startTile, endTile);
    }

    private static IEnumerable<BfsNode> GetValidNeighbours(BfsNode node, Weather1Game minigame)
    {
        for (int y = -1; y < 2; y++)
        for (int x = -1; x < 2; x++)
        {
            // x | 0 | x
            // 0 | x | 0
            // x | 0 | x
            if (Mathf.Abs(x) == Mathf.Abs(y)) continue;

            Vector3Int point = node.Position + new Vector3Int(x, y, 0);
            if (!minigame.PointIsValid(point)) continue;
            if (minigame.BarrierMap.GetTile(point)) continue;

            yield return new BfsNode {Parent = node, Position = point};
        }
    }

    private static IEnumerable<Vector3Int> RetracePath(BfsNode startNode, BfsNode endNode)
    {
        List<Vector3Int> path = new(23); // The minimum number of tiles inclusively between the entrance and exit tiles is 23

        BfsNode currentNode = endNode;
        while (currentNode.Position != startNode.Position)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }

        path.Add(startNode.Position);

        path.Reverse();
        return path;
    }

    private record BfsNode
    {
        public BfsNode Parent;
        public Vector3Int Position;
    }
}

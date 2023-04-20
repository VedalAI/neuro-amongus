using System.Collections;
using Il2CppSystem.Collections.Generic;
using Neuro.Cursor;
using Neuro.Minigames.Patches;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(Weather1Game))]
public sealed class FixWeatherNodeSolver : GeneralMinigameSolver<Weather1Game>
{
    public override IEnumerator CompleteMinigame(Weather1Game minigame, NormalPlayerTask task)
    {
        HashSet<Vector3Int> solution = Weather1Game_SolveMaze.MazeSolution;

        Vector3 tileBounds = minigame.fillTile.sprite.bounds.max * 2;
        Vector3 mapPos = minigame.transform.position;

        WaitForSeconds wait = new(0.04f);
        foreach (Vector3Int tile in solution)
        {
            if (!minigame.PointIsValid(tile))
            {
                Error($"{tile} was invalid when it shouldn't be!");
                continue;
            }

            Vector3 tileRealPosition = new Vector3((tile.x + 1) * tileBounds.x, (tile.y + 1) * tileBounds.y, (tile.z + 1) * tileBounds.z);
            yield return InGameCursor.Instance.CoMoveTo(tileRealPosition + mapPos);

            if (!InGameCursor.Instance.IsLeftButtonPressed) InGameCursor.Instance.StartHoldingLMB(minigame);
            yield return wait;
        }

        InGameCursor.Instance.StopHoldingLMB();
    }
}
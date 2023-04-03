using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(WaterStage))]
public sealed class ReplaceWaterJugSolver : MinigameSolver<WaterStage>
{
    public override IEnumerator CompleteMinigame(WaterStage minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.waterButton);
        minigame.Refuel();

        while (!minigame.complete) yield return new WaitForFixedUpdate();

        yield return minigame.CoStartClose(0.5f);
    }
}

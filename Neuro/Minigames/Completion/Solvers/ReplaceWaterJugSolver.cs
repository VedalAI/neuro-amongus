using Neuro.Cursor;
using System.Collections;
using UnityEngine;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(typeof(WaterStage))]
public sealed class ReplaceWaterJugSolver : MinigameSolver<WaterStage>
{
    public override IEnumerator CompleteMinigame(WaterStage minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.waterButton.transform.position);
        minigame.Refuel();
        while (!minigame.complete)
            yield return new WaitForFixedUpdate();
        minigame.Close();
    }
}



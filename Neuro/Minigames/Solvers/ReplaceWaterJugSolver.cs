using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(WaterStage))]
public sealed class ReplaceWaterJugSolver : MinigameSolver<WaterStage>
{
    protected override IEnumerator CompleteMinigame(WaterStage minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.waterButton);
        InGameCursor.Instance.StartHoldingLMB(minigame);
    }
}

using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(WaterStage))]
public sealed class ReplaceWaterJugSolver : TaskMinigameSolver<WaterStage>
{
    protected override IEnumerator CompleteMinigame(WaterStage minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.waterButton);
        InGameCursor.Instance.StartHoldingLMB(minigame);
    }
}

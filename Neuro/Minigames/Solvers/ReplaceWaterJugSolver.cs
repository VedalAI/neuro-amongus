using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(WaterStage), false)]
[MinigameOpener(typeof(MultistageMinigame))]
public sealed class ReplaceWaterJugSolver : IMinigameSolver<WaterStage>, IMinigameOpener
{
    public float CloseTimout => 20;

    public bool ShouldOpenConsole(Console console, PlayerTask task)
    {
        return task.TaskType == TaskTypes.ReplaceWaterJug;
    }

    public IEnumerator CompleteMinigame(WaterStage minigame)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.waterButton);
        InGameCursor.Instance.StartHoldingLMB(minigame);
    }
}

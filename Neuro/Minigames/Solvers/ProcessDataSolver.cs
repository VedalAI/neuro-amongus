using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(ProcessDataMinigame))]
public sealed class ProcessDataSolver : MinigameSolver<ProcessDataMinigame>
{
    protected override IEnumerator CompleteMinigame(ProcessDataMinigame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.StartButton);
        minigame.StartStopFill();
    }
}

using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(typeof(ProcessDataMinigame))]
public sealed class ProcessDataSolver : MinigameSolver<ProcessDataMinigame>
{
    public override IEnumerator CompleteMinigame(ProcessDataMinigame minigame, NormalPlayerTask task)
    {
        InGameCursor.Instance.SnapTo(minigame.StartButton);
        minigame.StartStopFill();
        yield break;
    }
}

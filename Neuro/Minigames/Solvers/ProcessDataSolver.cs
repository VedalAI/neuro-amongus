using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(ProcessDataMinigame))]
public sealed class ProcessDataSolver : GeneralMinigameSolver<ProcessDataMinigame>
{
    public override float CloseTimout => 15;

    public override IEnumerator CompleteMinigame(ProcessDataMinigame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.StartButton);
        minigame.StartStopFill();
    }
}

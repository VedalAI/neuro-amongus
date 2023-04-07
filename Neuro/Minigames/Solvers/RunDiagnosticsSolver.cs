using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(DiagnosticGame))]
public class RunDiagnosticsSolver : MinigameSolver<DiagnosticGame>
{
    protected override IEnumerator CompleteMinigame(DiagnosticGame minigame, NormalPlayerTask task)
    {
        if (task.TimerStarted == NormalPlayerTask.TimerState.NotStarted)
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.StartButton);
            minigame.StartDiagnostic();
            yield return Sleep(0.5f);
            minigame.Close();
        }
        else if (task.TimerStarted == NormalPlayerTask.TimerState.Finished)
        {
            // TODO: make the cursor move to the dot instead of the center of the sprite
            yield return InGameCursor.Instance.CoMoveTo(minigame.Targets[minigame.TargetNum]);
            minigame.PickAnomaly(minigame.TargetNum);
        }
        else
        {
            yield return Sleep(0.5f);
            minigame.Close();
        }
    }
}

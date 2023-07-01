using System.Collections;
using Neuro.Cursor;
using UnityEngine;
using static NormalPlayerTask;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(DiagnosticGame))]
public sealed class RunDiagnosticsSolver : IMinigameSolver<DiagnosticGame, NormalPlayerTask>, IMinigameOpener<NormalPlayerTask>
{
    public float CloseTimout => 6;

    public bool ShouldOpenConsole(Console console, NormalPlayerTask task)
    {
        return task.TimerStarted is TimerState.NotStarted or TimerState.Finished;
    }

    public IEnumerator CompleteMinigame(DiagnosticGame minigame, NormalPlayerTask task)
    {
        switch (task.TimerStarted)
        {
            case TimerState.NotStarted:
                yield return CompleteStep1(minigame);
                break;
            case TimerState.Finished:
                yield return CompleteStep2(minigame);
                break;
            default:
                yield return new WaitForSeconds(0.5f);
                minigame.Close();
                break;
        }
    }

    private IEnumerator CompleteStep1(DiagnosticGame minigame)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.StartButton, 0.5f);
        minigame.StartDiagnostic();
        yield return new WaitForSeconds(0.5f);
        minigame.Close();
    }

    private IEnumerator CompleteStep2(DiagnosticGame minigame)
    {
        Vector2 target = minigame.Targets[minigame.TargetNum].transform.position;
        if (minigame.TargetNum is 0 or 3) target += Vector2.left * 0.7f;
        else target += Vector2.right * 0.5f;
        yield return InGameCursor.Instance.CoMoveTo(target, 0.5f);

        minigame.PickAnomaly(minigame.TargetNum);
    }
}

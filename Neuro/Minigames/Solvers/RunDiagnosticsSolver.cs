using Neuro.Cursor;
using System.Collections;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(DiagnosticGame))]
public sealed class RunDiagnosticsSolver : IMinigameSolver<DiagnosticGame, NormalPlayerTask>, IMinigameOpener<NormalPlayerTask>
{
    public bool ShouldOpenConsole(Console console, NormalPlayerTask task)
    {
        return (task.TimerStarted != NormalPlayerTask.TimerState.Started) && !task.IsComplete;
    }

    public IEnumerator CompleteMinigame(DiagnosticGame minigame, NormalPlayerTask task)
    {
        if (task.TimerStarted == NormalPlayerTask.TimerState.NotStarted) yield return CompleteStage1(minigame);
        if (task.TimerStarted == NormalPlayerTask.TimerState.Finished) yield return CompleteStage2(minigame);
    }

    private IEnumerator CompleteStage1(DiagnosticGame minigame)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.StartButton);
        minigame.StartDiagnostic();
        yield return new WaitForSeconds(0.5f);
        minigame.Close();
    }

    private IEnumerator CompleteStage2(DiagnosticGame minigame)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.Targets[minigame.TargetNum]);
        minigame.PickAnomaly(minigame.TargetNum);
    }
}

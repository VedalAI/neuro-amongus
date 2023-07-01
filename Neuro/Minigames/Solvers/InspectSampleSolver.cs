using System.Collections;
using Neuro.Cursor;
using UnityEngine;
using static SampleMinigame;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(SampleMinigame))]
public sealed class InspectSampleSolver : IMinigameSolver<SampleMinigame>, IMinigameOpener<NormalPlayerTask>
{
    public float CloseTimout => 5;

    public bool ShouldOpenConsole(Console console, NormalPlayerTask task)
    {
        States state = (States) task.Data[0];
        return state == States.PrepareSample || (state == States.Processing && task.TaskTimer <= 0);
    }

    public IEnumerator CompleteMinigame(SampleMinigame minigame)
    {
        switch (minigame.State)
        {
            case States.PrepareSample:
                yield return CompleteStep1(minigame);
                break;
            case States.Selection:
                yield return CompleteStep2(minigame);
                break;
            default:
                minigame.CoStartClose(0.5f);
                break;
        }
    }

    private IEnumerator CompleteStep1(SampleMinigame minigame)
    {
        // wait for SampleMinigame.BringPanelUp to finish
        while (minigame.State != States.AwaitingStart) yield return null;

        yield return InGameCursor.Instance.CoMoveTo(minigame.LowerButtons[0]);
        minigame.NextStep();

        // CoStartClose doesn't work here, probably because the minigame does StopAllCoroutines(?)
        yield return new WaitForSeconds(0.75f);
        minigame.Close();
    }

    private IEnumerator CompleteStep2(SampleMinigame minigame)
    {
        yield return new WaitForSeconds(0.5f);
        yield return InGameCursor.Instance.CoMoveTo(minigame.Buttons[minigame.AnomalyId]);
        minigame.SelectTube(minigame.AnomalyId);
    }
}

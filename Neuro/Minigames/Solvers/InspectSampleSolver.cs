using System.Collections;
using System.Linq;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(SampleMinigame))]
public class InspectSampleSolver : MinigameSolver<SampleMinigame>
{
    protected override IEnumerator CompleteMinigame(SampleMinigame minigame, NormalPlayerTask task)
    {
        // when you first open the task, the state will actually be PrepareSample, not AwaitingStart
        // therefore we have to handle it here
        if (minigame.State == SampleMinigame.States.PrepareSample) yield return CompleteStage1(minigame);
        else if (minigame.State == SampleMinigame.States.Selection) yield return CompleteStage2(minigame);
        else
        {
            yield return new WaitForSeconds(0.5f);
            minigame.Close();
        }
    }

    private IEnumerator CompleteStage1(SampleMinigame minigame)
    {
        // wait a bit for SampleMinigame.BringPanelUp to finish. if the game is just starting, then the state will be corrected after this function is called
        yield return new WaitForSeconds(1f);
        if (minigame.State == SampleMinigame.States.AwaitingStart)
        {
            SpriteRenderer startButton = minigame.LowerButtons.First(l => l.name == "medBay_buttonConfirm");
            yield return InGameCursor.Instance.CoMoveTo(startButton);
            minigame.NextStep();
            yield return new WaitForSeconds(2f);
            minigame.Close();
        }
        else
        {
            // if not, we somehow closed the minigame during the selection animation
            yield return new WaitForSeconds(0.5f);
            minigame.Close();
        }
    }

    private IEnumerator CompleteStage2(SampleMinigame minigame)
    {
        yield return new WaitForSeconds(1f);
        yield return InGameCursor.Instance.CoMoveTo(minigame.Buttons[minigame.AnomalyId]);
        minigame.SelectTube(minigame.AnomalyId);
    }
}

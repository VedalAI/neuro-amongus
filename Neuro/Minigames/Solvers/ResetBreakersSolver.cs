using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(ElecLeverGame))]
public class ResetBreakersSolver : TaskMinigameSolver<ElecLeverGame>
{
    protected override IEnumerator CompleteMinigame(ElecLeverGame minigame, NormalPlayerTask task)
    {
        // TODO: programatically pathfind to the switches in order and/or remember ones we visit

        if (minigame.MyNormTask.Data[minigame.MyNormTask.taskStep] == minigame.ConsoleId)
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.Handle);
            InGameCursor.Instance.StartHoldingLMB(minigame.Handle);
            Vector3 down = minigame.transform.TransformPoint(minigame.Handle.transform.localPosition + new Vector3(0f, -3f, 0f));
            yield return InGameCursor.Instance.CoMoveTo(down);
            yield return new WaitForSeconds(0.1f);
            InGameCursor.Instance.StopHoldingLMB();
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            minigame.Close();
        }
    }
}

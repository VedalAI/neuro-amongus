using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(ToiletMinigame))]
public class CleanToiletSolver : MinigameSolver<ToiletMinigame>
{
    protected override IEnumerator CompleteMinigame(ToiletMinigame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.Plunger);
        InGameCursor.Instance.StartHoldingLMB(minigame.Plunger);

        while (!task.IsComplete)
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.Plunger.transform.position + new Vector3(0f, 1f, 0f), 2);
            yield return Sleep(0.05f);
            yield return InGameCursor.Instance.CoMoveTo(minigame.Plunger.transform.position + new Vector3(0f, -1f, 0f), 2);
            yield return Sleep(0.05f);
        }

        InGameCursor.Instance.StopHolding();
    }
}
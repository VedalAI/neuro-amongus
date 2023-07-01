using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(ToiletMinigame))]
public sealed class CleanToiletSolver : GeneralMinigameSolver<ToiletMinigame>
{
    public override float CloseTimout => 11f;

    public override IEnumerator CompleteMinigame(ToiletMinigame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.Plunger);
        InGameCursor.Instance.StartHoldingLMB(minigame);

        while (!task.IsComplete)
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.Plunger.transform.position + new Vector3(0f, 1f), 2);
            yield return new WaitForSeconds(0.05f);
            yield return InGameCursor.Instance.CoMoveTo(minigame.Plunger.transform.position + new Vector3(0f, -1f), 2);
            yield return new WaitForSeconds(0.05f);
        }

        InGameCursor.Instance.StopHoldingLMB();
    }
}

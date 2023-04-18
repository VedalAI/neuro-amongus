using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(FillCanistersGame))]
public class FillCanistersSolver : GeneralMinigameSolver<FillCanistersGame>
{
    public override IEnumerator CompleteMinigame(FillCanistersGame minigame, NormalPlayerTask task)
    {
        // this minigame also uses localPositions so convert them into world space
        while (!task.IsComplete)
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.Canister);
            InGameCursor.Instance.StartHoldingLMB(minigame.Canister);
            yield return InGameCursor.Instance.CoMoveTo(minigame.transform.TransformPoint(minigame.CanisterSnapPosition));
            InGameCursor.Instance.StopHoldingLMB();
            while (minigame.Canister.Gauge.Value < minigame.Canister.Gauge.MaxValue)
                yield return null;
            yield return new WaitForSeconds(0.2f);
            yield return InGameCursor.Instance.CoMoveTo(minigame.transform.TransformPoint(minigame.CanisterSnapPosition) + Vector3.right, 0.5f);
            // pulling away the canister only requires a click
            yield return InGameCursor.Instance.CoPressLMB();
            // wait for the next canister to appear
            yield return new WaitForSeconds(0.5f);
        }
    }
}

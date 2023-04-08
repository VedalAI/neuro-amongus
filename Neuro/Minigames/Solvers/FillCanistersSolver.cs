using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(FillCanistersGame))]
public class FillCanistersSolver : MinigameSolver<FillCanistersGame>
{
    protected override IEnumerator CompleteMinigame(FillCanistersGame minigame, NormalPlayerTask task)
    {
        // this minigame also uses localPositions so convert them into world space
        while (!task.IsComplete)
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.transform.TransformPoint(minigame.Canister.transform.localPosition));
            InGameCursor.Instance.StartHoldingLMB(minigame.Canister);
            yield return InGameCursor.Instance.CoMoveTo(minigame.transform.TransformPoint(minigame.CanisterSnapPosition));
            InGameCursor.Instance.StopHoldingLMB();
            while (minigame.Canister.Gauge.Value < minigame.Canister.Gauge.MaxValue)
                yield return null;
            // pulling away the canister only requires a click
            InGameCursor.Instance.StartHoldingLMB(minigame.Canister);
            yield return new WaitForSeconds(0.1f);
            InGameCursor.Instance.StopHoldingLMB();
            // wait for the next canister to appear
            yield return new WaitForSeconds(0.5f);
        }
    }
}

using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(FillCanistersGame))]
public sealed class FillCanistersSolver : GeneralMinigameSolver<FillCanistersGame>
{
    public override float CloseTimout => 17;

    public override IEnumerator CompleteMinigame(FillCanistersGame minigame, NormalPlayerTask task)
    {
        // this minigame also uses localPositions so convert them into world space
        while (!task.IsComplete)
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.Canister);
            InGameCursor.Instance.StartHoldingLMB(minigame);
            yield return InGameCursor.Instance.CoMoveTo(minigame.transform.TransformPoint(minigame.CanisterSnapPosition));
            InGameCursor.Instance.StopHoldingLMB();
            while (minigame.Canister.Gauge.Value < minigame.Canister.Gauge.MaxValue)
                yield return null;
            yield return new WaitForSeconds(0.2f);
            // pulling away the canister only requires a click, but we still make a movement to the right so it looks better
            yield return InGameCursor.Instance.CoPressLMB();
            yield return InGameCursor.Instance.CoMoveTo(minigame.transform.TransformPoint(minigame.CanisterSnapPosition) + Vector3.right, 0.8f);
            // wait for the next canister to appear
            yield return new WaitForSeconds(0.5f);
        }
    }
}

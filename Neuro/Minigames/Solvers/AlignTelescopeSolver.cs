using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(TelescopeGame))]
public sealed class AlignTelescopeSolver : GeneralMinigameSolver<TelescopeGame>
{
    public override IEnumerator CompleteMinigame(TelescopeGame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.Reticle);

        // Do a little square dance to be more realistic
        InGameCursor.Instance.StartHoldingLMB(minigame);
        yield return InGameCursor.Instance.CoMoveTo(minigame.Reticle.transform.position + new Vector3(0, -3f));
        yield return new WaitForSeconds(0.3f);

        Vector2 position = minigame.Reticle.transform.position + new Vector3(3, 0);
        WaitForSeconds wait = new(0.8f);
        yield return InGameCursor.Instance.CoMoveTo(position);
        yield return wait;
        position.y += 6;
        yield return InGameCursor.Instance.CoMoveTo(position);
        yield return wait;
        position.x -= 6;
        yield return InGameCursor.Instance.CoMoveTo(position);
        yield return wait;
        position.y -= 6;
        yield return InGameCursor.Instance.CoMoveTo(position);
        yield return wait;
        InGameCursor.Instance.StopHoldingLMB();

        // Move towards the target in gradual steps
        Transform target = minigame.TargetItem.transform;
        Vector3 positionDifference;
        do
        {
            positionDifference = minigame.Reticle.transform.position - target.position;
            yield return InGameCursor.Instance.CoMoveTo(minigame.Reticle);
            InGameCursor.Instance.StartHoldingLMB(minigame);

            const int DISTANCE_MULTIPLIER = 8;
            yield return InGameCursor.Instance.CoMoveTo(minigame.Reticle.transform.position + positionDifference.normalized * DISTANCE_MULTIPLIER);
            InGameCursor.Instance.StopHoldingLMB();
        } while (Mathf.Abs(positionDifference.x) + Mathf.Abs(positionDifference.y) > 2);

        // Just in case we're a bit off, move exactly the remaining distance
        yield return InGameCursor.Instance.CoMoveTo(target);
        InGameCursor.Instance.StartHoldingLMB(minigame);
        yield return InGameCursor.Instance.CoMoveTo(minigame.Reticle);
        InGameCursor.Instance.StopHoldingLMB();
    }
}
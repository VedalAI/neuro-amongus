using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(TelescopeGame))]
public sealed class AlignTelescopeSolver : MinigameSolver<TelescopeGame>
{
    protected override IEnumerator CompleteMinigame(TelescopeGame minigame, NormalPlayerTask task)
    {
        Transform target = minigame.TargetItem.transform;

        yield return InGameCursor.Instance.CoMoveTo(minigame.Reticle);

        // Do a little square dance to be more realistic
        InGameCursor.Instance.StartHoldingLMB(minigame.Reticle.transform);
        yield return InGameCursor.Instance.CoMoveTo(minigame.Reticle.transform.position + new Vector3(0, -3f));
        yield return new WaitForSeconds(0.3f);

        Vector2 position = minigame.Reticle.transform.position + new Vector3(3, 6);
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
        InGameCursor.Instance.StopHolding();

        Vector3 positionDifference;
        do
        {
            positionDifference = target.position - minigame.Reticle.transform.position;
            yield return InGameCursor.Instance.CoMoveTo(minigame.Reticle);
            InGameCursor.Instance.StartHoldingLMB(minigame.Reticle);

            const int SPEED_MULTIPLIER = -8;
            yield return InGameCursor.Instance.CoMoveTo(minigame.Reticle.transform.position + positionDifference.normalized * SPEED_MULTIPLIER);
            InGameCursor.Instance.StopHolding();
        } while (Mathf.Abs(positionDifference.x) + Mathf.Abs(positionDifference.y) > 2f);

        yield return InGameCursor.Instance.CoMoveTo(target);
        yield return InGameCursor.Instance.CoMoveTo(minigame.Reticle);
    }
}
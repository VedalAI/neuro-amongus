using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(TelescopeGame))]
public sealed class AlignTelescopeSolver : GeneralMinigameSolver<TelescopeGame>
{
    public override float CloseTimout => 20;

    public override IEnumerator CompleteMinigame(TelescopeGame minigame, NormalPlayerTask task)
    {
        const float viewDist = 3.5f;
        const float snapSpeed = 0.5f;

        yield return InGameCursor.Instance.CoMoveTo(minigame.Reticle);
        InGameCursor.Instance.StartHoldingLMB(minigame);

        // Check if target is already in view (cracked planet)
        if (Vector2.Distance(minigame.Background.transform.localPosition, -minigame.TargetItem.transform.localPosition) <= viewDist)
        {
            yield return InGameCursor.Instance.CoMoveTo(InGameCursor.Instance.Position - (Vector2) minigame.Background.transform.localPosition - (Vector2) minigame.TargetItem.transform.localPosition, snapSpeed);
            InGameCursor.Instance.StopHoldingLMB();
            yield break;
        }

        float randAngle = Random.RandomRange(0f, 360f);
        Vector2 position = InGameCursor.Instance.Position;
        const float radius = 5.5f;
        const float speed = 5;
        Vector2 offset = new Vector2(-0.8f, -0.8f);

        while (!task.IsComplete)
        {
            yield return InGameCursor.Instance.CoMoveToPositionOnCircle(position + offset, radius, randAngle, 0.7f);
            for (float t = 0; t < speed; t += Time.deltaTime)
            {
                // if in view snap to target
                if (Vector2.Distance(minigame.Background.transform.localPosition, -minigame.TargetItem.transform.localPosition) <= viewDist)
                {
                    yield return InGameCursor.Instance.CoMoveTo(InGameCursor.Instance.Position - (Vector2) minigame.Background.transform.localPosition - (Vector2) minigame.TargetItem.transform.localPosition, snapSpeed);
                    InGameCursor.Instance.StopHoldingLMB();
                    yield break;
                }

                float angle = Mathf.Lerp(randAngle, randAngle + 360f, t / speed);
                InGameCursor.Instance.SnapToPositionOnCircle(position + offset, radius, angle);
                yield return null;
            }
        }

        InGameCursor.Instance.StopHoldingLMB();
    }
}
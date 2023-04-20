using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(KeyMinigame))]
public sealed class InsertKeysSolver : GeneralMinigameSolver<KeyMinigame>
{
    public override IEnumerator CompleteMinigame(KeyMinigame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.key);
        InGameCursor.Instance.StartHoldingLMB(minigame);
        yield return InGameCursor.Instance.CoMoveTo(minigame.Slots[minigame.targetSlotId]);
        InGameCursor.Instance.StopHoldingLMB();
        yield return new WaitForSeconds(0.1f);

        const float radius = 0.5f;
        Vector2 keyPosition = minigame.key.transform.position;
        yield return InGameCursor.Instance.CoMoveTo(keyPosition + new Vector2(0, 1) * radius, 0.5f);

        InGameCursor.Instance.StartHoldingLMB(minigame);
        const float spinDuration = 0.2f;
        for (float t = 0; t < spinDuration; t += Time.deltaTime)
        {
            float angle = Mathf.Lerp(90, 0, t / spinDuration);
            float angleInRadians = angle * Mathf.Deg2Rad;

            Vector2 positionOnCircle = keyPosition + new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)) * radius;

            InGameCursor.Instance.SnapTo(positionOnCircle);
            yield return null;
        }
        InGameCursor.Instance.StopHoldingLMB();
    }
}

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

        InGameCursor.Instance.StartHoldingLMB(minigame);
        const float spinDuration = 0.2f;
        for (float t = 0; t < spinDuration; t += Time.deltaTime)
        {
            Vector2 keyPosition = minigame.key.transform.position;

            // This can also probably be done with some trigonometry but i'm too stupid for that
            Vector2 positionOnChord = Vector2.Lerp(keyPosition + Vector2.up, keyPosition + Vector2.left, t / spinDuration);
            Vector2 unnormalizedDirection = positionOnChord - keyPosition;
            Vector2 normalizedDirection = unnormalizedDirection.normalized;
            Vector2 positionOnCircle = keyPosition + normalizedDirection * 0.5f;

            InGameCursor.Instance.SnapTo(positionOnCircle);
            yield return null;
        }
        InGameCursor.Instance.StopHoldingLMB();
    }
}

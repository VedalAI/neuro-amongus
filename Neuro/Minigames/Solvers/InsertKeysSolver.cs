using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(KeyMinigame))]
public class InsertKeysSolver : MinigameSolver<KeyMinigame>
{
    protected override IEnumerator CompleteMinigame(KeyMinigame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.key);
        InGameCursor.Instance.StartHoldingLMB(minigame.key);
        yield return InGameCursor.Instance.CoMoveTo(minigame.Slots[minigame.targetSlotId]);
        InGameCursor.Instance.StopHoldingLMB();
        yield return new WaitForSeconds(0.1f);
        InGameCursor.Instance.StartHoldingLMB(minigame.key);
        for (float t = 0; t < 0.5f; t += Time.deltaTime) 
        {
            minigame.prevHadInput = true;
            // TODO: make the cursor follow the topside of the key (its bounds are massive)
            InGameCursor.Instance.SnapTo(minigame.key);
            Vector3 currentAngles = minigame.key.transform.localEulerAngles;
            currentAngles.z = Mathf.LerpAngle(0f, 90f, t / 0.5f);
            minigame.key.transform.localEulerAngles = currentAngles;
            yield return null;
        }
        InGameCursor.Instance.StopHoldingLMB();
    }
}

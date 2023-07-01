using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(KeyMinigame))]
public sealed class InsertKeysSolver : GeneralMinigameSolver<KeyMinigame>
{
    public override float CloseTimout => 5;

    public override IEnumerator CompleteMinigame(KeyMinigame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.key);
        InGameCursor.Instance.StartHoldingLMB(minigame);
        yield return InGameCursor.Instance.CoMoveTo(minigame.Slots[minigame.targetSlotId]);
        InGameCursor.Instance.StopHoldingLMB();
        yield return new WaitForSeconds(0.1f);

        yield return InGameCursor.Instance.CoMoveToPositionOnCircle(minigame.key, 0.5f, 90, 0.5f);
        InGameCursor.Instance.StartHoldingLMB(minigame);
        yield return InGameCursor.Instance.CoMoveCircle(minigame.key, 0.5f, 90, 0, 0.2f);
        InGameCursor.Instance.StopHoldingLMB();
    }
}

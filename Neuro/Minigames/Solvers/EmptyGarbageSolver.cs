using System.Collections;
using System.Linq;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(EmptyGarbageMinigame))]
public sealed class EmptyGarbageSolver : GeneralMinigameSolver<EmptyGarbageMinigame>
{
    public override float CloseTimout => 7;

    public override IEnumerator CompleteMinigame(EmptyGarbageMinigame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.Handle);
        InGameCursor.Instance.StartHoldingLMB(minigame);

        yield return InGameCursor.Instance.CoMoveTo(InGameCursor.Instance.Position + new Vector2(0, -2), 0.5f);
        while (minigame.Objects.Any(o => o)) yield return null;

        InGameCursor.Instance.StopHoldingLMB();
    }
}
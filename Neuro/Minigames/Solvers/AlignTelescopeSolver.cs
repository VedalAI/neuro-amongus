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
        InGameCursor.Instance.StartHoldingLMB(minigame);

        // Add a bit of human
        Vector2 rand = new Vector2(Random.RandomRange(-0.4f, 0.4f), Random.RandomRange(-0.4f, 0.4f));
        float speed = Random.RandomRange(0.1f, 0.4f);

        // Cursor controls background so it needs to be moved opposite of the target
        yield return InGameCursor.Instance.CoMoveTo(InGameCursor.Instance.Position + -(Vector2)minigame.TargetItem.transform.localPosition + rand, speed);

        InGameCursor.Instance.StopHoldingLMB();
    }
}

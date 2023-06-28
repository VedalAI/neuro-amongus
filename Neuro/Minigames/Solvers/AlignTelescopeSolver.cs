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

        float speed = 0.2f;

        // Randomly order the targets
        var targets = minigame.Items;
        int count = targets.Count;
        int last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            (targets[r], targets[i]) = (targets[i], targets[r]);
        }

        // Loop through the targets until minigame completes
        while (!task.IsComplete)
        {
            Vector2 position = minigame.initialPos;
            foreach (var target in targets)
            {
                // Cursor controls background so it needs to be moved opposite of the target
                yield return InGameCursor.Instance.CoMoveTo(InGameCursor.Instance.Position + position - (Vector2)target.transform.localPosition, speed);
                position = (Vector2)target.transform.localPosition;
                yield return new WaitForSeconds(0.5f);
            }
        }

        InGameCursor.Instance.StopHoldingLMB();
    }
}

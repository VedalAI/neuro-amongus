using System.Collections;
using UnityEngine;
using Neuro.Cursor;
using System.Linq;
using Rewired.Utils;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(LeafMinigame))]
public sealed class CleanO2FilterSolver : GeneralMinigameSolver<LeafMinigame>
{
    public override float CloseTimout => 11;

    public override IEnumerator CompleteMinigame(LeafMinigame minigame, NormalPlayerTask task)
    {
        // ValidArea is somewhere offscreen so easier to just move towards the arrows on the chute
        Vector2 exit = minigame.Arrows[0].transform.position;
        while (!task.IsComplete)
        {
            Collider2D leaf = minigame.Leaves
                .Where(leaf => !leaf.IsNullOrDestroyed())
                .MaxBy(leaf => (InGameCursor.Instance.Position - (Vector2) leaf.transform.position).sqrMagnitude);

            if (!leaf)
            {
                yield return null;
                continue;
            }

            yield return InGameCursor.Instance.CoMoveTo(leaf);
            InGameCursor.Instance.StartHoldingLMB(minigame);
            yield return InGameCursor.Instance.CoMoveTo(exit);
            InGameCursor.Instance.StopHoldingLMB();
            yield return new WaitForSeconds(0.1f);
        }
    }
}

using System.Collections;
using UnityEngine;
using Neuro.Cursor;
using System.Linq;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(LeafMinigame))]
public class CleanO2FilterSolver : TaskMinigameSolver<LeafMinigame>
{
    protected override IEnumerator CompleteMinigame(LeafMinigame minigame, NormalPlayerTask task)
    {
        // ValidArea is somewhere offscreen so easier to just move towards the arrows on the chute
        Vector3 exit = minigame.Arrows[0].transform.position;
        Collider2D lastLeaf = null;
        while (!task.IsComplete)
        {
            Collider2D _lastLeafCopy = lastLeaf;
            Collider2D leaf = minigame.Leaves
                .Where(l => l && l.enabled && l.gameObject.active && l != _lastLeafCopy
                    && Vector3.Distance(l.transform.position, exit) > 4f)
                .MaxBy(l => (InGameCursor.Instance.Position - (Vector2)l.transform.position).magnitude);

            if (!leaf)
            {
                yield return null;
                continue;
            }

            // get a different leaf every time to avoid already moving leaves
            lastLeaf = leaf;
            yield return InGameCursor.Instance.CoMoveTo(leaf);
            InGameCursor.Instance.StartHoldingLMB(leaf);
            yield return InGameCursor.Instance.CoMoveTo(exit);
            InGameCursor.Instance.StopHoldingLMB();
            yield return new WaitForSeconds(0.1f);
        }
    }
}

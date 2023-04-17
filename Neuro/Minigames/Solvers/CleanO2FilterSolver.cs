using System.Collections;
using UnityEngine;
using Neuro.Cursor;
using System.Linq;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(LeafMinigame))]
public sealed class CleanO2FilterSolver : GeneralMinigameSolver<LeafMinigame>
{
    public override IEnumerator CompleteMinigame(LeafMinigame minigame, NormalPlayerTask task)
    {
        Vector3 exit = minigame.Arrows[0].transform.position;
        while (!task.IsComplete)
        {
            foreach (var leaf in minigame.Leaves.Where(leaf => leaf && leaf.enabled && leaf.gameObject.active))
            {
                yield return InGameCursor.Instance.CoMoveTo(leaf);
                InGameCursor.Instance.StartHoldingLMB(leaf);
                yield return InGameCursor.Instance.CoMoveTo(exit);
                InGameCursor.Instance.StopHoldingLMB();
                yield return new WaitForSeconds(0.25f);
            }
        }
    }
}

using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(typeof(VentCleaningMinigame))]
public sealed class CleanVentSolver : MinigameSolver<VentCleaningMinigame>
{
    public override IEnumerator CompleteMinigame(VentCleaningMinigame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.ventLidClosed);
        yield return minigame.CoOpenVent();

        foreach (PoolableBehavior dirt in minigame.dirtPool.activeChildren.ToArray())
        {
            yield return InGameCursor.Instance.CoMoveTo(dirt);
            minigame.CleanUp(dirt.Cast<VentDirt>());
            yield return Sleep(0.1f);
        }
    }
}

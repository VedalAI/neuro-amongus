using System.Collections;
using Neuro.Cursor;
using Neuro.Utilities;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(VentCleaningMinigame))]
public sealed class CleanVentMinigameSolver : TaskMinigameSolver<VentCleaningMinigame>
{
    protected override IEnumerator CompleteMinigame(VentCleaningMinigame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.ventLidClosed);
        yield return minigame.CoOpenVent();

        foreach (VentDirt dirt in minigame.dirtPool.activeChildren.ToArray().OfIl2CppType<VentDirt>())
        {
            yield return InGameCursor.Instance.CoMoveTo(dirt);
            minigame.CleanUp(dirt);
            yield return new WaitForSeconds(0.1f);
        }
    }
}

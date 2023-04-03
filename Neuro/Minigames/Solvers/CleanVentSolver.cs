using System.Collections;
using Neuro.Cursor;
using Neuro.Utilities;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(typeof(VentCleaningMinigame))]
public sealed class CleanVentSolver : MinigameSolver<VentCleaningMinigame>
{
    public override IEnumerator CompleteMinigame(VentCleaningMinigame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.ventLidClosed);
        yield return minigame.CoOpenVent();

        foreach (VentDirt dirt in minigame.dirtPool.activeChildren.ToArray().OfIl2CppType<VentDirt>())
        {
            yield return InGameCursor.Instance.CoMoveTo(dirt);
            minigame.CleanUp(dirt);
            yield return Sleep(0.1f);
        }
    }
}

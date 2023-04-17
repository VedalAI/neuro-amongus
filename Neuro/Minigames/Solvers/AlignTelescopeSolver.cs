using Neuro.Cursor;
using System.Collections;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(TelescopeGame))]
public sealed class AlignTelescopeSolver : GeneralMinigameSolver<TelescopeGame>
{
    public override IEnumerator CompleteMinigame(TelescopeGame minigame, NormalPlayerTask task)
    {
        InGameCursor.Instance.StartHoldingLMB(minigame);
        yield return InGameCursor.Instance.CoMoveTo(minigame.TargetPosition);
    }
}

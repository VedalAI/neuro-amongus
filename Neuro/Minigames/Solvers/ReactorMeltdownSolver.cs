using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(ReactorMinigame))]
public sealed class ReactorMeltdownSolver : SabotageMinigameSolver<ReactorMinigame>
{
    protected override IEnumerator CompleteMinigame(ReactorMinigame minigame, SabotageTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.hand);
        minigame.ButtonDown();
    }
}

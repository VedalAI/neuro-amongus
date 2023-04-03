using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(typeof(ReactorMinigame))]
public sealed class ReactorMeltdownSolver : TasklessMinigameSolver<ReactorMinigame>
{
    public override IEnumerator CompleteMinigame(ReactorMinigame minigame)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.hand);
        minigame.ButtonDown();
    }
}

using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(typeof(ReactorMinigame))]
public sealed class ReactorMeltdownSolver : MinigameSolver<ReactorMinigame>
{
    public override IEnumerator CompleteMinigame(ReactorMinigame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.hand);
        minigame.ButtonDown();
    }
}

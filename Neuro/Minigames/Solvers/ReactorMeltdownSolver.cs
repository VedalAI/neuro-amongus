using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(ReactorMinigame))]
public sealed class ReactorMeltdownSolver : IMinigameSolver<ReactorMinigame>, IMinigameOpener
{
    public float CloseTimout => 99999;

    // TODO: Don't open consoles that are already fixed
    public bool ShouldOpenConsole(Console console, PlayerTask task) => true;

    public IEnumerator CompleteMinigame(ReactorMinigame minigame)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.hand);
        minigame.ButtonDown();
    }
}

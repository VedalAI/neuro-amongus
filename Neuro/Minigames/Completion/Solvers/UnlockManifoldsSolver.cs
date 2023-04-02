using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(typeof(UnlockManifoldsMinigame))]
public sealed class UnlockManifoldsSolver : MinigameSolver<UnlockManifoldsMinigame>
{
    public override IEnumerator CompleteMinigame(UnlockManifoldsMinigame minigame, NormalPlayerTask task)
    {
        while (minigame.buttonCounter < minigame.Buttons.Length)
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.Buttons[minigame.buttonCounter]);
            minigame.HitButton(minigame.buttonCounter);
            yield return Sleep(0.2f);
        }
    }
}

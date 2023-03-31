using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(typeof(UnlockManifoldsMinigame))]
public sealed class UnlockManifoldsSolver : MinigameSolver<UnlockManifoldsMinigame>
{
    public override IEnumerator CompleteMinigame(UnlockManifoldsMinigame minigame, NormalPlayerTask task)
    {
        while (minigame.buttonCounter < minigame.Buttons.Length)
        {
            InGameCursor.Instance.MoveTo(minigame.Buttons[minigame.buttonCounter]);
            minigame.HitButton(minigame.buttonCounter);
            yield return new WaitForSeconds(0.1f);
        }
    }
}

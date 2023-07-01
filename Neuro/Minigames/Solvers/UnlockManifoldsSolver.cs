using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(UnlockManifoldsMinigame))]
public sealed class UnlockManifoldsSolver : GeneralMinigameSolver<UnlockManifoldsMinigame>
{
    public override float CloseTimout => 10;

    public override IEnumerator CompleteMinigame(UnlockManifoldsMinigame minigame, NormalPlayerTask task)
    {
        while (minigame.buttonCounter < minigame.Buttons.Length)
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.Buttons[minigame.buttonCounter]);
            minigame.HitButton(minigame.buttonCounter);
            yield return new WaitForSeconds(0.1f);
        }
    }
}

using System.Collections;
using UnityEngine;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(TaskTypes.UnlockManifolds)]
public sealed class UnlockManifoldsSolver : MinigameSolver<UnlockManifoldsMinigame>
{
    protected override IEnumerator CompleteMinigame()
    {
        while (MyMinigame.buttonCounter < MyMinigame.Buttons.Length)
        {
            MyMinigame.HitButton(MyMinigame.buttonCounter);
            yield return new WaitForSeconds(0.1f);
        }
    }
}

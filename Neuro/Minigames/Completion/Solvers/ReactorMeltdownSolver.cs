using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(typeof(ReactorMinigame))]
public sealed class ReactorMeltdownSolver : MinigameSolver<ReactorMinigame>
{
    public override IEnumerator CompleteMinigame(ReactorMinigame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.hand);
        // sit and wait for someone to hold the other button
        while (minigame.reactor.IsActive)
        {
            minigame.isButtonDown = true;
            yield return new WaitForFixedUpdate();
        }
    }
}

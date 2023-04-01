using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(typeof(DoorBreakerGame))]
public sealed class DoorSabotageSwitchesSolver : TasklessMinigameSolver<DoorBreakerGame>
{
    public override IEnumerator CompleteMinigame(DoorBreakerGame minigame)
    {
        foreach (SpriteRenderer button in minigame.Buttons)
        {
            if (button.flipX)
            {
                InGameCursor.Instance.MoveTo(button.transform.position + new Vector3(0.35f, 0));
                minigame.FlipSwitch(button);
                yield return new WaitForSeconds(0.2f * DELAY_MULTIPLIER);
            }
        }
    }
}

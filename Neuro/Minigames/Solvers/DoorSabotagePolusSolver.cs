using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(DoorBreakerGame), false)]
public sealed class DoorSabotagePolusSolver : IMinigameSolver<DoorBreakerGame>
{
    public float CloseTimout => 5;

    public IEnumerator CompleteMinigame(DoorBreakerGame minigame)
    {
        foreach (SpriteRenderer button in minigame.Buttons)
        {
            if (button.flipX)
            {
                yield return InGameCursor.Instance.CoMoveTo(button.transform.position + new Vector3(0.35f, 0));
                minigame.FlipSwitch(button);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}

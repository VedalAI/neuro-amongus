using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(SimonSaysGame))]
public sealed class StartReactorSolver : GeneralMinigameSolver<SimonSaysGame>
{
    public override float CloseTimout => 25;

    public override IEnumerator CompleteMinigame(SimonSaysGame minigame, NormalPlayerTask task)
    {
        while (!task.IsComplete)
        {
            while (minigame.Buttons[0].color != Color.white) yield return null;

            int indexCount = minigame.IndexCount;
            for (int i = 0; i < indexCount; i++)
            {
                int button = minigame[i];
                yield return InGameCursor.Instance.CoMoveTo(minigame.Buttons[button], 0.5f);
                minigame.HitButton(button);
                yield return new WaitForSeconds(0.2f);
            }

            if (!task.IsComplete) yield return InGameCursor.Instance.CoMoveTo(minigame.Buttons[minigame[0]], 0.5f);
        }

        InGameCursor.Instance.StopMovement();
    }
}

using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(SimonSaysGame))]
public sealed class StartReactorSolver : MinigameSolver<SimonSaysGame>
{
    protected override IEnumerator CompleteMinigame(SimonSaysGame minigame, NormalPlayerTask task)
    {
        int currentSequenceLength = minigame.IndexCount;
        yield return Sleep(5f);
        while (currentSequenceLength <= 5)
        {
            for (int i = 0; i < currentSequenceLength; i++)
            {
                int button = (int)(minigame[i]);
                yield return InGameCursor.Instance.CoMoveTo(minigame.Buttons[button].transform.position, 0.5f);
                minigame.HitButton(button);
                yield return Sleep(0.5f);
            }
            yield return Sleep(2f * currentSequenceLength);
            currentSequenceLength++;
        }
        InGameCursor.Instance.StopMovement();
        yield return Sleep(1f);
    }
}
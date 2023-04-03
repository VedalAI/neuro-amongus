using System;
using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(KeypadGame))]
public sealed class OxygenDepletedSolver : TasklessMinigameSolver<KeypadGame>
{
    protected override IEnumerator CompleteMinigame(KeypadGame minigame)
    {
        // Important TODO: Fix this. Currently only works on one of the keypads and incorrectly marks completions

        int[] numbers = Array.ConvertAll(minigame.oxyTask.targetNumber.ToString().ToCharArray(), x => (int)char.GetNumericValue(x));
        UiElement[] buttons = minigame.ControllerSelectable.ToArray();
        foreach (int number in numbers)
        {
            // luckily for us the buttons are indexed correctly
            if (number == 0)
                yield return InGameCursor.Instance.CoMoveTo(buttons[9]);
            else
                yield return InGameCursor.Instance.CoMoveTo(buttons[number - 1]);
            minigame.ClickNumber(number);
            yield return Sleep(0.2f);
        }
        yield return InGameCursor.Instance.CoMoveTo(minigame.AcceptButton);
        minigame.Enter();
    }
}

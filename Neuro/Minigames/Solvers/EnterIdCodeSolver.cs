using System;
using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(EnterCodeMinigame))]
public class EnterIdCodeSolver : MinigameSolver<EnterCodeMinigame>
{
    protected override IEnumerator CompleteMinigame(EnterCodeMinigame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.Card);
        minigame.ShowCard();

        // the cardOut bool gets set immediately so give some time for it to appear
        yield return Sleep(1.2f);
        int[] numbers = Array.ConvertAll(minigame.targetNumber.ToString().ToCharArray(), x => (int)char.GetNumericValue(x));
        UiElement[] buttons = minigame.ControllerSelectable.ToArray();
        foreach (int number in numbers)
        {
            // luckily for us the buttons are indexed correctly
            if (number == 0)
                yield return InGameCursor.Instance.CoMoveTo(buttons[9]);
            else
                yield return InGameCursor.Instance.CoMoveTo(buttons[number - 1]);
            minigame.EnterDigit(number);
            yield return Sleep(0.2f);
        }
        yield return InGameCursor.Instance.CoMoveTo(buttons[11]);
        
        minigame.AcceptDigits();
    }
}
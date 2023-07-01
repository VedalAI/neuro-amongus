using System;
using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(KeypadGame))]
public sealed class OxygenDepletedSolver : IMinigameSolver<KeypadGame>, IMinigameOpener
{
    public float CloseTimout => 15;

    public bool ShouldOpenConsole(Console console, PlayerTask task) => true;

    public IEnumerator CompleteMinigame(KeypadGame minigame)
    {
        int[] numbers = Array.ConvertAll(minigame.oxyTask.targetNumber.ToString().ToCharArray(), x => (int) char.GetNumericValue(x));
        UiElement[] buttons = minigame.ControllerSelectable.ToArray();
        foreach (int number in numbers)
        {
            // luckily for us the buttons are indexed correctly
            if (number == 0)
                yield return InGameCursor.Instance.CoMoveTo(buttons[9]);
            else
                yield return InGameCursor.Instance.CoMoveTo(buttons[number - 1]);
            minigame.ClickNumber(number);
            yield return new WaitForSeconds(0.2f);
        }

        yield return InGameCursor.Instance.CoMoveTo(minigame.AcceptButton);
        minigame.Enter();
    }
}

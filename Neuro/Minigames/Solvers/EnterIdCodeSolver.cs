using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Neuro.Cursor;
using Neuro.Utilities;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(EnterCodeMinigame))]
public sealed class EnterIdCodeSolver : GeneralMinigameSolver<EnterCodeMinigame>
{
    public override IEnumerator CompleteMinigame(EnterCodeMinigame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.Card);
        yield return minigame.CoShowCard();

        IEnumerable<int> numbers = minigame.targetNumber.ToString().Select(c => c - '0');
        foreach (int number in numbers)
        {
            if (number == 0) yield return InGameCursor.Instance.CoMoveTo(minigame.ControllerSelectable.At(9));
            else yield return InGameCursor.Instance.CoMoveTo(minigame.ControllerSelectable.At(number - 1));

            minigame.EnterDigit(number);
            yield return new WaitForSeconds(0.2f);
        }
        yield return InGameCursor.Instance.CoMoveTo(minigame.ControllerSelectable.At(11));

        minigame.AcceptDigits();
    }
}

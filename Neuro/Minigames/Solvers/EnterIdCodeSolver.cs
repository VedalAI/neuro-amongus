using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Neuro.Cursor;
using Neuro.Extensions;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(EnterCodeMinigame))]
public sealed class EnterIdCodeSolver : GeneralMinigameSolver<EnterCodeMinigame>
{
    public override float CloseTimout => 10;

    public override IEnumerator CompleteMinigame(EnterCodeMinigame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.Card);
        yield return minigame.CoShowCard();

        IEnumerable<int> numbers = minigame.targetNumber.ToString().PadLeft(5, '0').Select(c => c - '0');
        foreach (int number in numbers)
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.ControllerSelectable.At(number > 0 ? number - 1 : 9));
            minigame.EnterDigit(number);
            yield return new WaitForSeconds(0.2f);
        }

        yield return InGameCursor.Instance.CoMoveTo(minigame.ControllerSelectable.At(11));

        minigame.AcceptDigits();
    }
}
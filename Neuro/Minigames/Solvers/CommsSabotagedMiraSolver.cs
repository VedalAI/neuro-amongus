using Neuro.Cursor;
using System.Collections;
using System.Linq;
using Neuro.Extensions;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(AuthGame))]
public sealed class CommsSabotagedMiraSolver : IMinigameSolver<AuthGame>, IMinigameOpener
{
    public float CloseTimout => 15f;

    public bool ShouldOpenConsole(Console console, PlayerTask task) => true;

    public IEnumerator CompleteMinigame(AuthGame minigame)
    {
        do
        {
            yield return new WaitForSeconds(0.5f);
            int codeNumber = minigame.system.TargetNumber;
            int[] code = codeNumber.ToString().PadLeft(5, '0').Select(c => c - '0').ToArray();
            foreach (int number in code)
            {
                yield return InGameCursor.Instance.CoMoveTo(minigame.ControllerSelectable.At(number > 0 ? number - 1 : 9));
                minigame.ClickNumber(number);
                yield return new WaitForSeconds(0.25f);

                if (codeNumber != minigame.system.TargetNumber)
                {
                    yield return InGameCursor.Instance.CoMoveTo(minigame.ControllerSelectable.At(^2));
                    minigame.ClearEntry();
                    yield return new WaitForSeconds(0.5f);
                    break;
                }
            }

            if (codeNumber == minigame.system.TargetNumber)
            {
                yield return InGameCursor.Instance.CoMoveTo(minigame.ControllerSelectable.At(^1));
                minigame.Enter();
                yield return new WaitForSeconds(0.5f);
                yield return new WaitForSeconds(minigame.system.Timer);
            }
        } while (!minigame.MyTask.IsComplete);
    }
}
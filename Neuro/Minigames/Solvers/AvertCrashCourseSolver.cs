using Neuro.Cursor;
using System.Collections;
using System.Linq;
using Neuro.Extensions;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(AirshipAuthGame))]
public sealed class AvertCrashCourseSolver : IMinigameSolver<AirshipAuthGame>, IMinigameOpener
{
    public float CloseTimout => 99999;

    // TODO: Don't open consoles that are already fixed
    public bool ShouldOpenConsole(Console console, PlayerTask task) => true;

    public IEnumerator CompleteMinigame(AirshipAuthGame minigame)
    {
        do
        {
            yield return new WaitForSeconds(0.5f);
            int codeNumber = minigame.system.TargetCode;
            int[] code = codeNumber.ToString().PadLeft(5, '0').Select(c => c - '0').ToArray();
            foreach (int number in code)
            {
                yield return InGameCursor.Instance.CoMoveTo(minigame.selectableButtons.At(number > 0 ? number - 1 : 9));
                minigame.ClickNumber(number);
                yield return new WaitForSeconds(0.25f);

                if (codeNumber != minigame.system.TargetCode)
                {
                    yield return InGameCursor.Instance.CoMoveTo(minigame.selectableButtons.At(^2));
                    minigame.ClearEntry();
                    yield return new WaitForSeconds(0.5f);
                    break;
                }
            }

            if (codeNumber == minigame.system.TargetCode)
            {
                yield return InGameCursor.Instance.CoMoveTo(minigame.selectableButtons.At(^1));
                minigame.Enter();
                yield return new WaitForSeconds(0.5f);
                yield return new WaitForSeconds(minigame.system.codeResetTimer);
            }
        } while (!minigame.MyTask.IsComplete);
    }
}
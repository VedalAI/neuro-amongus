using Neuro.Cursor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Neuro.Utilities;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(AirshipAuthGame))]
public sealed class AvertCrashCourseSolver : IMinigameSolver<AirshipAuthGame>, IMinigameOpener
{
    public bool ShouldOpenConsole(Console console, PlayerTask task) => true;

    public IEnumerator CompleteMinigame(AirshipAuthGame minigame)
    {
        do
        {
            yield return new WaitForSeconds(0.5f);
            IEnumerable<int> code = minigame.system.TargetCode.ToString().PadLeft(5, '0').Select(c => c - '0');
            foreach (int number in code)
            {
                yield return InGameCursor.Instance.CoMoveTo(minigame.selectableButtons.At(number > 0 ? number - 1 : 9));
                minigame.ClickNumber(number);
                yield return new WaitForSeconds(0.25f);
            }
            yield return InGameCursor.Instance.CoMoveTo(minigame.selectableButtons.At(minigame.selectableButtons.Count - 1));
            minigame.Enter();
            yield return new WaitForSeconds(0.5f);
            yield return new WaitForSeconds(minigame.system.codeResetTimer);
        }
        while (!minigame.MyTask.IsComplete);
    }
}

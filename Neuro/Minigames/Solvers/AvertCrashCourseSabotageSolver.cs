using Neuro.Cursor;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(AirshipAuthGame))]
public sealed class AvertCrashCourseSabotageSolver : IMinigameSolver<AirshipAuthGame, SabotageTask>, IMinigameOpener<SabotageTask>
{
    public bool ShouldOpenConsole(Console console, SabotageTask task) => true;

    public IEnumerator CompleteMinigame(AirshipAuthGame minigame, SabotageTask task)
    {
        do
        {
            yield return new WaitForSeconds(0.5f);
            var code = minigame.system.TargetCode.ToString().Select(c => c - '0');
            foreach (var number in code)
            {
                yield return InGameCursor.Instance.CoMoveTo(minigame.selectableButtons[number > 0 ? number - 1 : 9]);
                minigame.ClickNumber(number);
                yield return new WaitForSeconds(0.5f);
            }
            yield return InGameCursor.Instance.CoMoveTo(minigame.selectableButtons[minigame.selectableButtons.Count - 1]);
            minigame.Enter();
            yield return new WaitForSeconds(0.5f);
            yield return new WaitForSeconds(minigame.system.codeResetTimer);
        }
        while (!minigame.MyTask.IsComplete);
    }
}

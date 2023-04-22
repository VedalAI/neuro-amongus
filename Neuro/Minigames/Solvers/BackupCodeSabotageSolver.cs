using Neuro.Cursor;
using System.Collections;
using System.Linq;
using Neuro.Utilities;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(AuthGame))]
public sealed class BackupCodeSabotageSolver : IMinigameSolver<AuthGame, SabotageTask>, IMinigameOpener<SabotageTask>
{
    public bool ShouldOpenConsole(Console console, SabotageTask task) => true;

    public IEnumerator CompleteMinigame(AuthGame minigame, SabotageTask task)
    {
        do
        {
            yield return new WaitForSeconds(0.5f);
            var code = minigame.system.TargetNumber.ToString().Select(c => c - '0');
            foreach (var number in code)
            {
                yield return InGameCursor.Instance.CoMoveTo(minigame.ControllerSelectable.At(number > 0 ? number - 1 : 9));
                minigame.ClickNumber(number);
                yield return new WaitForSeconds(0.5f);
            }
            yield return InGameCursor.Instance.CoMoveTo(minigame.ControllerSelectable.At(minigame.ControllerSelectable.Count - 1));
            minigame.Enter();
            yield return new WaitForSeconds(0.5f);
            yield return new WaitForSeconds(minigame.system.Timer);
        }
        while (!minigame.MyTask.IsComplete);
    }
}

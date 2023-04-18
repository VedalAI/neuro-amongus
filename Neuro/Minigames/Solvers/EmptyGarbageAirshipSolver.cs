using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(AirshipGarbageGame), false)]
[MinigameOpener(typeof(AutoMultistageMinigame))]
public class EmptyGarbageAirshipSolver : IMinigameSolver<AirshipGarbageGame>, IMinigameOpener
{
    public bool ShouldOpenConsole(Console console, Minigame minigame, PlayerTask task)
    {
        return task.TaskType == TaskTypes.EmptyGarbage;
    }

    public IEnumerator CompleteMinigame(AirshipGarbageGame minigame)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.can.Handle);
        InGameCursor.Instance.StartHoldingLMB(minigame);
        // yank that shit out of there
        yield return InGameCursor.Instance.CoMoveTo(new Vector2(Screen.width / 2, Screen.height), 3f);
        InGameCursor.Instance.StopHoldingLMB();
    }
}

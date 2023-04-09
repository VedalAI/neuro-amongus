using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(AirshipGarbageGame))]
public class EmptyGarbageAirshipSolver : TaskMinigameSolver<AirshipGarbageGame>
{
    protected override IEnumerator CompleteMinigame(AirshipGarbageGame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.can.Handle);
        InGameCursor.Instance.StartHoldingLMB(minigame.can.Handle);
        // yank that shit out of there
        yield return InGameCursor.Instance.CoMoveTo(new Vector2(Screen.width / 2, Screen.height), 3f);
        InGameCursor.Instance.StopHoldingLMB();
    }
}

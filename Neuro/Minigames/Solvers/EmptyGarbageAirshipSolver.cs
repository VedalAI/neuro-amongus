using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(AirshipGarbageGame), false)]
[MinigameOpener(typeof(AutoMultistageMinigame))]
public sealed class EmptyGarbageAirshipSolver : IMinigameSolver<AirshipGarbageGame>, IMinigameOpener
{
    public float CloseTimout => 7;

    public bool ShouldOpenConsole(Console console, PlayerTask task)
    {
        return task.TaskType == TaskTypes.EmptyGarbage;
    }

    public IEnumerator CompleteMinigame(AirshipGarbageGame minigame)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.can.Handle);
        InGameCursor.Instance.StartHoldingLMB(minigame);
        while (minigame.can.Body.IsTouching(minigame.can.Success))
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.can.Handle.transform.position + new Vector3(0f, 2f), 2);
            yield return new WaitForSeconds(0.05f);
            yield return InGameCursor.Instance.CoMoveTo(minigame.can.Handle.transform.position + new Vector3(0f, -1f), 2);
            yield return new WaitForSeconds(0.05f);
        }

        InGameCursor.Instance.StopHoldingLMB();
    }
}
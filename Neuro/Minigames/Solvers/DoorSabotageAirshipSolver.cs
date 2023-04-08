using System.Collections;
using Il2CppMono.Security.Interface;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(DoorCardSwipeGame))]
public sealed class DoorSabotageAirshipSolver : TasklessMinigameSolver<DoorCardSwipeGame>
{
    protected override IEnumerator CompleteMinigame(DoorCardSwipeGame minigame)
    {
        yield return minigame.InsertCard();

        yield return InGameCursor.Instance.CoMoveTo(minigame.col);

        InGameCursor.Instance.StartHoldingLMB(minigame);

        yield return InGameCursor.Instance.CoMoveTo(minigame.col.transform.position - new Vector3(0, 4f), minigame.minAcceptedTime);

        InGameCursor.Instance.StopHolding();

        yield return Sleep(0.1f);
    }
}

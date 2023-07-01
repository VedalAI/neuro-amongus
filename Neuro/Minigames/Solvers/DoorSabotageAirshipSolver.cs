using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(DoorCardSwipeGame))]
public sealed class DoorSabotageAirshipSolver : IMinigameSolver<DoorCardSwipeGame>
{
    public float CloseTimout => 11;

    public IEnumerator CompleteMinigame(DoorCardSwipeGame minigame)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.col);
        yield return new WaitForSeconds(0.1f);

        InGameCursor.Instance.StartFollowing(minigame.col);
        yield return minigame.InsertCard();
        yield return new WaitForSeconds(0.1f);
        InGameCursor.Instance.StopMovement();

        if (Random.Range(0f, 1f) < 0.02)
        {
            InGameCursor.Instance.StartHoldingLMB(minigame);
            yield return InGameCursor.Instance.CoMoveTo(InGameCursor.Instance.Position + new Vector2(0, -4), 0.8f);
            InGameCursor.Instance.StopHoldingLMB();
            yield return new WaitForSeconds(1f);
            yield return InGameCursor.Instance.CoMoveTo(minigame.col, 1.2f);
        }

        InGameCursor.Instance.StartHoldingLMB(minigame);
        yield return InGameCursor.Instance.CoMoveTo(InGameCursor.Instance.Position + new Vector2(0, -4), 0.5f);
        InGameCursor.Instance.StopHoldingLMB();
    }
}

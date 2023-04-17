using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(CardSlideGame))]
public sealed class SwipeCardSolver : GeneralMinigameSolver<CardSlideGame>
{
    public override IEnumerator CompleteMinigame(CardSlideGame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.col);
        yield return new WaitForSeconds(0.1f);

        InGameCursor.Instance.StartFollowing(minigame.col);
        yield return minigame.InsertCard();
        yield return new WaitForSeconds(0.1f);

        InGameCursor.Instance.StopMovement();
        InGameCursor.Instance.StartHoldingLMB(minigame);
        yield return InGameCursor.Instance.CoMoveTo(InGameCursor.Instance.Position + new Vector2(5, 0), 0.7f);
        InGameCursor.Instance.StopHoldingLMB();
    }
}

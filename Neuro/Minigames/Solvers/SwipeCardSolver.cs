using System.Collections;
using Neuro.Cursor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(CardSlideGame))]
public sealed class SwipeCardSolver : GeneralMinigameSolver<CardSlideGame>
{
    public override float CloseTimout => 11;

    public override IEnumerator CompleteMinigame(CardSlideGame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.col);
        yield return new WaitForSeconds(0.1f);

        InGameCursor.Instance.StartFollowing(minigame.col);
        yield return minigame.InsertCard();
        yield return new WaitForSeconds(0.1f);
        InGameCursor.Instance.StopMovement();

        if (Random.Range(0f, 1f) < 0.05)
        {
            InGameCursor.Instance.StartHoldingLMB(minigame);
            yield return InGameCursor.Instance.CoMoveTo(InGameCursor.Instance.Position + new Vector2(5, 0), 1.2f);
            InGameCursor.Instance.StopHoldingLMB();
            yield return new WaitForSeconds(1f);
            yield return InGameCursor.Instance.CoMoveTo(minigame.col, 1.2f);
        }

        InGameCursor.Instance.StartHoldingLMB(minigame);
        yield return InGameCursor.Instance.CoMoveTo(InGameCursor.Instance.Position + new Vector2(5, 0), 0.7f);
        InGameCursor.Instance.StopHoldingLMB();
    }
}

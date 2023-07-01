using Neuro.Cursor;
using System.Collections;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(TowelMinigame))]
public sealed class PickUpTowelsSolver : GeneralMinigameSolver<TowelMinigame>
{
    public override float CloseTimout => 21;

    public override IEnumerator CompleteMinigame(TowelMinigame minigame, NormalPlayerTask task)
    {
        Vector2 aboveBasket = minigame.BasketHitbox.transform.position + new Vector3(0f, 5f);

        foreach (Collider2D towel in minigame.Towels)
        {
            yield return InGameCursor.Instance.CoMoveTo(towel);
            InGameCursor.Instance.StartHoldingLMB(minigame);
            yield return InGameCursor.Instance.CoMoveTo(aboveBasket);
            yield return new WaitForSeconds(0.25f);
            yield return InGameCursor.Instance.CoMoveTo(minigame.BasketHitbox);
            InGameCursor.Instance.StopHoldingLMB();
            yield return new WaitForSeconds(0.25f);
        }
    }
}

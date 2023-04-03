using System.Collections;
using Neuro.Cursor;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(CardSlideGame))]
public sealed class SwipeCardSolver : MinigameSolver<CardSlideGame>
{
    public override IEnumerator CompleteMinigame(CardSlideGame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.col);
        yield return Sleep(0.1f);

        InGameCursor.Instance.StartFollowing(minigame.col);
        yield return minigame.InsertCard();
        yield return Sleep(0.1f);

        PlayAudio(minigame);

        Vector3 originalPosition = minigame.col.transform.localPosition;
        Vector3 targetPosition = new(minigame.XRange.max, originalPosition.y);
        for (float t = 0; t <= 0.5f; t += Time.deltaTime)
        {
            minigame.col.transform.localPosition = (Vector3) Vector2.Lerp(originalPosition, targetPosition, t / 0.5f) with {z = originalPosition.z};
            yield return null;
        }
        minigame.col.transform.localPosition = targetPosition with {z = originalPosition.z};

        InGameCursor.Instance.StopMovement();
        SolveMinigame(minigame);
    }

    // Taken from CardSlideGame.Update
    private void PlayAudio(CardSlideGame minigame)
    {
        if (minigame.moving) return;
        minigame.moving = true;

        if (Constants.ShouldPlaySfx())
        {
            SoundManager.Instance.PlaySound(minigame.CardMove.Random(), false);
        }
    }

    // Taken from CardSlideGame.Update
    private void SolveMinigame(CardSlideGame minigame)
    {
        if (Constants.ShouldPlaySfx())
        {
            SoundManager.Instance.PlaySound(minigame.AcceptSound, false);
        }
        minigame.State = CardSlideGame.TaskStages.After;
        minigame.StatusText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.SwipeCardAccepted);
        minigame.StartCoroutine(minigame.PutCardBack());
        if (minigame.MyNormTask) minigame.MyNormTask.NextStep();
        minigame.redLight.color = minigame.gray;
        minigame.greenLight.color = minigame.green;
    }
}

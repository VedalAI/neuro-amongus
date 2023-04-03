using System.Collections;
using System.Linq;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(EmptyGarbageMinigame))]
public sealed class EmptyGarbageMainStageSolver : MinigameSolver<EmptyGarbageMinigame>
{
    protected override IEnumerator CompleteMinigame(EmptyGarbageMinigame minigame, NormalPlayerTask task)
    {
        InGameCursor.Instance.StartFollowing(minigame.Handle);

        Vector3 holdPosition = minigame.Handle.transform.localPosition;
        for (float t = 0; t < 0.2f; t += Time.deltaTime)
        {
            minigame.Handle.transform.localPosition = holdPosition = holdPosition with {y = Mathf.Lerp(0.7f, -0.6f, t / 0.2f)};
            UpdateBars(minigame, holdPosition);

            yield return null;
        }

        while (!minigame.finished)
        {
            minigame.Handle.transform.localPosition = holdPosition;
            UpdateBars(minigame, holdPosition);

            DoHold(minigame);
            CheckForFinish(minigame);

            yield return null;
        }

        yield return Sleep(0.5f);

        minigame.StopCoroutines();
        StopSfx(minigame);
    }

    // This is taken from EmptyGarbageMinigame.Update
    private void UpdateBars(EmptyGarbageMinigame minigame, Vector3 handlePosition)
    {
        Vector3 localScale = minigame.Bars.transform.localScale;
        localScale.y = minigame.HandleRange.ChangeRange(handlePosition.y, -1f, 1f);
        minigame.Bars.transform.localScale = localScale;
    }

    // This is taken from EmptyGarbageMinigame.Update
    private void DoHold(EmptyGarbageMinigame minigame)
    {
        if (minigame.Blocker.enabled)
        {
            if (Constants.ShouldPlaySfx())
            {
                SoundManager.Instance.PlaySound(minigame.LeverDown, false);
                SoundManager.Instance.PlaySound(minigame.GrinderStart, false, 0.8f);
                SoundManager.Instance.StopSound(minigame.GrinderEnd);
                SoundManager.Instance.StopSound(minigame.GrinderLoop);
            }
            minigame.Blocker.enabled = false;
            minigame.StopCoroutines();
            minigame.popCoroutine = minigame.StartCoroutine(minigame.PopObjects());
            minigame.animateCoroutine = minigame.StartCoroutine(minigame.AnimateObjects());
        }
    }

    // This is taken from EmptyGarbageMinigame.Update
    private void CheckForFinish(EmptyGarbageMinigame minigame)
    {
        if (minigame.Objects.All(o => !o))
        {
            minigame.finished = true;
            minigame.MyNormTask.NextStep();
            minigame.StartCoroutine(minigame.CoStartClose());
        }
    }

    // This is taken from EmptyGarbageMinigame.Update
    private void StopSfx(EmptyGarbageMinigame minigame)
    {
        if (Constants.ShouldPlaySfx())
        {
            SoundManager.Instance.PlaySound(minigame.LeverUp, false);
            SoundManager.Instance.StopSound(minigame.GrinderStart);
            SoundManager.Instance.StopSound(minigame.GrinderLoop);
            SoundManager.Instance.PlaySound(minigame.GrinderEnd, false, 0.8f);
        }
    }
}

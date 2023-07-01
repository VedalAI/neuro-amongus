using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(TuneRadioMinigame))]
public sealed class CommsSabotagedSolver : IMinigameSolver<TuneRadioMinigame>, IMinigameOpener
{
    public float CloseTimout => 11;

    public bool ShouldOpenConsole(Console console, PlayerTask task) => true;

    public IEnumerator CompleteMinigame(TuneRadioMinigame minigame)
    {
        float flip = Mathf.Sign(minigame.targetAngle);

        const float radius = 0.6f;
        const float speed = 2f;
        float angle = 90f;
        float currAngle;
        float newAngle;

        yield return InGameCursor.Instance.CoMoveToPositionOnCircle(minigame.dial.DialTrans, radius, 90f);
        InGameCursor.Instance.StartHoldingLMB(minigame);

        // 50% chance to "choose wrong", or search in the opposite direction slightly
        if (Random.value >= 0.5)
        {
            currAngle = angle;
            newAngle = angle - (flip * 180f);
            for (float t = 0; t < speed; t += Time.deltaTime)
            {
                angle = Mathf.Lerp(currAngle, newAngle, t / speed);
                if (Mathf.Abs(angle - 90f) >= 60f) break;

                InGameCursor.Instance.SnapToPositionOnCircle(minigame.dial.DialTrans, radius, angle);
                yield return null;
            }
        }

        // Sweep through correct direction
        currAngle = angle;
        newAngle = angle + (flip * 180f);
        for (float t = 0; t < speed; t += Time.deltaTime)
        {
            // exit if we find the solution
            if (minigame.actualSignal.NoiseLevel <= minigame.Tolerance) yield break;

            angle = Mathf.Lerp(currAngle, newAngle, t / speed);
            InGameCursor.Instance.SnapToPositionOnCircle(minigame.dial.DialTrans, radius, angle);
            yield return null;
        }
    }
}

using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(TuneRadioMinigame))]
public sealed class CommsSabotagedSolver : IMinigameSolver<TuneRadioMinigame>, IMinigameOpener
{
    public bool ShouldOpenConsole(Console console, PlayerTask task) => true;

    public IEnumerator CompleteMinigame(TuneRadioMinigame minigame)
    {
        const float radius = 0.6f;

        yield return InGameCursor.Instance.CoMoveToPositionOnCircle(minigame.dial.DialTrans, radius, 90f);
        InGameCursor.Instance.StartHoldingLMB(minigame);

        // if target angle is positive the signal is on the left
        if (minigame.targetAngle > 0)
        {
            // move through the left side of the dial
            for (float t = 0; t < 1f; t += Time.deltaTime)
            {
                // exit if we find the solution
                if (minigame.actualSignal.NoiseLevel <= minigame.Tolerance) yield break;

                float angle = Mathf.Lerp(90f, 210f, t);
                InGameCursor.Instance.SnapToPositionOnCircle(minigame.dial.DialTrans, radius, angle);
                yield return null;
            }
        }
        // if target angle is negative the signal is on the right
        else if (minigame.targetAngle < 0)
        {
            // move through the right side of the dial
            for (float t = 0; t < 2f; t += Time.deltaTime)
            {
                // exit if we find the solution
                if (minigame.actualSignal.NoiseLevel <= minigame.Tolerance) yield break;

                float angle = Mathf.Lerp(90f, -30f, t);
                InGameCursor.Instance.SnapToPositionOnCircle(minigame.dial.DialTrans, radius, angle);
                yield return null;
            }
        }
        else
        {
            yield break;
        }
    }
}

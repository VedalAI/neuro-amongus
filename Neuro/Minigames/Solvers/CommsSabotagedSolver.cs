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

        // check the left side of the dial
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            // exit if we find the solution
            if (minigame.actualSignal.NoiseLevel <= minigame.Tolerance - 0.03f) yield break;

            float angle = Mathf.Lerp(90f, 210f, t);
            InGameCursor.Instance.SnapToPositionOnCircle(minigame.dial.DialTrans, radius, angle);
            yield return null;
        }

        // if the correct side is on the right, move to the other side
        for (float t = 0; t < 2f; t += Time.deltaTime)
        {
            // exit if we find the solution
            if (minigame.actualSignal.NoiseLevel <= minigame.Tolerance) yield break;

            float angle = Mathf.Lerp(210f, -210f, t / 2f);
            InGameCursor.Instance.SnapToPositionOnCircle(minigame.dial.DialTrans, radius, angle);
            yield return null;
        }
    }
}

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
        float radius = 0.5f;
        Vector2 origin = minigame.dial.DialTrans.transform.position;
        yield return InGameCursor.Instance.CoMoveToCircleStart(origin, radius, -30f);
        InGameCursor.Instance.StartHoldingLMB(minigame);
        // check the left side of the dial
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            // exit early if we find the solution
            if (minigame.actualSignal.NoiseLevel <= minigame.Tolerance)
            {
                yield break;
            }
            float angle = Mathf.Lerp(90f, 210f, t);
            float radians = angle * Mathf.Deg2Rad;

            Vector2 positionOnCircle = origin + new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
            InGameCursor.Instance.SnapTo(positionOnCircle);
            yield return null;
        }
        // if the correct side is on the right, move to the other side
        for (float t = 0; t < 2f; t += Time.deltaTime)
        {
            // exit early if we find the solution
            if (minigame.actualSignal.NoiseLevel <= minigame.Tolerance)
            {
                yield break;
            }
            float angle = Mathf.Lerp(210f, -210f, t / 2f);
            float radians = angle * Mathf.Deg2Rad;

            Vector2 positionOnCircle = origin + new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
            InGameCursor.Instance.SnapTo(positionOnCircle);
            yield return null;
        }
    }
}
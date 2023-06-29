using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(SafeMinigame))]
public sealed class SafeSolver : GeneralMinigameSolver<SafeMinigame>
{
    public override IEnumerator CompleteMinigame(SafeMinigame minigame, NormalPlayerTask task)
    {
        if (!task) yield break;

        yield return SolveTumbler(minigame, task);
        yield return SolveSpinner(minigame, task);
    }

    private IEnumerator SolveTumbler(SafeMinigame minigame, NormalPlayerTask task)
    {
        while (!minigame.latched[1])
        {
            // Tumbler (combo)
            const float TumblerRadius = 0.6f;
            const float TumblerSpeed = 4f;
            float TumblerAngle = 90f;
            float currTumblerAngle;
            float newTumblerAngle;

            yield return InGameCursor.Instance.CoMoveToPositionOnCircle(minigame.Tumbler, TumblerRadius, TumblerAngle);

            // Failsafe (if tumbler starts already at first combo)
            if (minigame.AngleNear(minigame.Tumbler.transform.eulerAngles.z + 45f, minigame.lastTumDir, (float)minigame.combo[0] * 45, 3f))
            {
                currTumblerAngle = TumblerAngle;
                newTumblerAngle = TumblerAngle - 180f;
                InGameCursor.Instance.StartHoldingLMB(minigame);
                for (float t = 0; t < 1f; t += Time.deltaTime)
                {
                    TumblerAngle = Mathf.Lerp(currTumblerAngle, newTumblerAngle, t * 2);
                    InGameCursor.Instance.SnapToPositionOnCircle(minigame.Tumbler, TumblerRadius, TumblerAngle);
                    yield return null;
                }

                InGameCursor.Instance.StopHoldingLMB();
            }

            // Combo 1
            currTumblerAngle = TumblerAngle;
            newTumblerAngle = TumblerAngle - 360f;
            InGameCursor.Instance.StartHoldingLMB(minigame);
            for (float t = 0; t < TumblerSpeed; t += Time.deltaTime)
            {
                // exit if we find the solution
                if (minigame.AngleNear(minigame.Tumbler.transform.eulerAngles.z + 45f, minigame.lastTumDir, (float)minigame.combo[0] * 45, 3f))
                {
                    InGameCursor.Instance.StopHoldingLMB();
                    minigame.CheckTumblr(0f, minigame.Tumbler.transform.eulerAngles.z, 0, minigame.combo[0] * 45);
                    break;
                }

                TumblerAngle = Mathf.Lerp(currTumblerAngle, newTumblerAngle, t / TumblerSpeed);
                InGameCursor.Instance.SnapToPositionOnCircle(minigame.Tumbler, TumblerRadius, TumblerAngle);
                yield return null;
            }

            InGameCursor.Instance.StopHoldingLMB();

            // Combo 2
            currTumblerAngle = TumblerAngle;
            newTumblerAngle = TumblerAngle + 360f;
            InGameCursor.Instance.StartHoldingLMB(minigame);
            for (float t = 0; t < TumblerSpeed; t += Time.deltaTime)
            {
                // exit if we find the solution
                if (minigame.AngleNear(minigame.Tumbler.transform.eulerAngles.z + 45f, minigame.lastTumDir, (float)minigame.combo[1] * 45, 3f))
                {
                    InGameCursor.Instance.StopHoldingLMB();
                    minigame.CheckTumblr(0f, minigame.Tumbler.transform.eulerAngles.z, 1, minigame.combo[1] * 45);
                    break;
                }

                TumblerAngle = Mathf.Lerp(currTumblerAngle, newTumblerAngle, t / TumblerSpeed);
                InGameCursor.Instance.SnapToPositionOnCircle(minigame.Tumbler, TumblerRadius, TumblerAngle);
                yield return null;
            }

            InGameCursor.Instance.StopHoldingLMB();

            if (minigame.latched[0])
            {
                // Combo 3
                currTumblerAngle = TumblerAngle;
                newTumblerAngle = TumblerAngle - 360f;
                InGameCursor.Instance.StartHoldingLMB(minigame);
                for (float t = 0; t < TumblerSpeed; t += Time.deltaTime)
                {
                    // exit if we find the solution
                    if (minigame.AngleNear(minigame.Tumbler.transform.eulerAngles.z + 45f, minigame.lastTumDir, (float)minigame.combo[2] * 45, 3f))
                    {
                        InGameCursor.Instance.StopHoldingLMB();
                        minigame.CheckTumblr(0f, minigame.Tumbler.transform.eulerAngles.z, 2, minigame.combo[2] * 45);
                        break;
                    }

                    TumblerAngle = Mathf.Lerp(currTumblerAngle, newTumblerAngle, t / TumblerSpeed);
                    InGameCursor.Instance.SnapToPositionOnCircle(minigame.Tumbler, TumblerRadius, TumblerAngle);
                    yield return null;
                }

                InGameCursor.Instance.StopHoldingLMB();
            }
        }
    }

    private IEnumerator SolveSpinner(SafeMinigame minigame, NormalPlayerTask task)
    {
        const float SpinnerRadius = 0.6f;

        yield return InGameCursor.Instance.CoMoveToPositionOnCircle(minigame.Spinner, SpinnerRadius, 90f);

        InGameCursor.Instance.StartHoldingLMB(minigame);
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            // exit if complete
            if (Mathf.Abs(minigame.spinDel) > 720f + 10f)
            {
                InGameCursor.Instance.StopHoldingLMB();
                yield break;
            }

            float SpinnerTumblerAngle = Mathf.Lerp(90f, 360f, t);
            InGameCursor.Instance.SnapToPositionOnCircle(minigame.Spinner, SpinnerRadius, SpinnerTumblerAngle);
            yield return null;
        }
    }
}

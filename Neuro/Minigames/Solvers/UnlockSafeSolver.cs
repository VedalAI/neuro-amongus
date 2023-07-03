using System.Collections;
using HarmonyLib;
using Neuro.Cursor;
using Neuro.Extensions.Harmony;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(SafeMinigame))]
[FullHarmonyPatch]
public sealed class UnlockSafeSolver : GeneralMinigameSolver<SafeMinigame>
{
    public override float CloseTimout => 20;

    public override IEnumerator CompleteMinigame(SafeMinigame minigame, NormalPlayerTask task)
    {
        if (!task) yield break;

        yield return SolveTumbler(minigame);
        yield return SolveSpinner(minigame, task);
    }

    private IEnumerator SolveTumbler(SafeMinigame minigame)
    {
        while (!minigame.vibration[0] || !minigame.vibration[1] || !minigame.vibration[2])
        {
            const float TumblerRadius = 0.6f;
            const float TumblerSpeed = 6f;
            float TumblerAngle = 90f;
            float currTumblerAngle;
            float newTumblerAngle;

            yield return InGameCursor.Instance.CoMoveToPositionOnCircle(minigame.Tumbler, TumblerRadius, TumblerAngle);
            InGameCursor.Instance.StartHoldingLMB(minigame);

            // Failsafe (if tumbler starts already at first combo)
            if (AngleNear(minigame, minigame.Tumbler.transform.eulerAngles.z + 45f, minigame.lastTumDir, (float) minigame.combo[0] * 45, 3f))
            {
                currTumblerAngle = TumblerAngle;
                newTumblerAngle = TumblerAngle - 180f;
                for (float t = 0; t < 1f; t += Time.deltaTime)
                {
                    TumblerAngle = Mathf.Lerp(currTumblerAngle, newTumblerAngle, t * 2);
                    InGameCursor.Instance.SnapToPositionOnCircle(minigame.Tumbler, TumblerRadius, TumblerAngle);
                    minigame.lastTumDir = 1;
                    yield return null;
                }
            }

            // Combo 1
            currTumblerAngle = TumblerAngle;
            newTumblerAngle = TumblerAngle - 720f;
            for (float t = 0; t < TumblerSpeed; t += Time.deltaTime)
            {
                // exit if we find the solution
                if (AngleNear(minigame, minigame.Tumbler.transform.eulerAngles.z + 45f, minigame.lastTumDir, (float) minigame.combo[0] * 45, 3f))
                {
                    minigame.lastTumDir = 1;
                    minigame.CheckTumblr(0f, minigame.Tumbler.transform.eulerAngles.z, 0, minigame.combo[0] * 45);
                    minigame.lastTumDir = 1;
                    break;
                }

                TumblerAngle = Mathf.Lerp(currTumblerAngle, newTumblerAngle, t / TumblerSpeed);
                InGameCursor.Instance.SnapToPositionOnCircle(minigame.Tumbler, TumblerRadius, TumblerAngle);
                yield return null;
            }

            if (minigame.vibration[0])
            {
                // Combo 2
                currTumblerAngle = TumblerAngle;
                newTumblerAngle = TumblerAngle + 720f;
                for (float t = 0; t < TumblerSpeed; t += Time.deltaTime)
                {
                    // exit if we find the solution
                    if (AngleNear(minigame, minigame.Tumbler.transform.eulerAngles.z + 45f, minigame.lastTumDir, (float) minigame.combo[1] * 45, 3f))
                    {
                        minigame.lastTumDir = -1;
                        minigame.CheckTumblr(0f, minigame.Tumbler.transform.eulerAngles.z, 1, minigame.combo[1] * 45);
                        minigame.lastTumDir = -1;
                        break;
                    }

                    TumblerAngle = Mathf.Lerp(currTumblerAngle, newTumblerAngle, t / TumblerSpeed);
                    InGameCursor.Instance.SnapToPositionOnCircle(minigame.Tumbler, TumblerRadius, TumblerAngle);
                    yield return null;
                }
            }

            if (minigame.vibration[1])
            {
                // Combo 3
                currTumblerAngle = TumblerAngle;
                newTumblerAngle = TumblerAngle - 720f;
                for (float t = 0; t < TumblerSpeed; t += Time.deltaTime)
                {
                    // exit if we find the solution
                    if (AngleNear(minigame, minigame.Tumbler.transform.eulerAngles.z + 45f, minigame.lastTumDir, (float) minigame.combo[2] * 45, 3f))
                    {
                        minigame.lastTumDir = 1;
                        minigame.CheckTumblr(0f, minigame.Tumbler.transform.eulerAngles.z, 2, minigame.combo[2] * 45);
                        minigame.lastTumDir = 1;
                        break;
                    }

                    TumblerAngle = Mathf.Lerp(currTumblerAngle, newTumblerAngle, t / TumblerSpeed);
                    InGameCursor.Instance.SnapToPositionOnCircle(minigame.Tumbler, TumblerRadius, TumblerAngle);
                    yield return null;
                }
            }

            InGameCursor.Instance.StopHoldingLMB();
        }
    }

    private IEnumerator SolveSpinner(SafeMinigame minigame, NormalPlayerTask task)
    {
        const float SpinnerRadius = 1.2f;

        float SpinnerTumblerAngle = -90f;
        yield return InGameCursor.Instance.CoMoveToPositionOnCircle(minigame.Spinner, SpinnerRadius, SpinnerTumblerAngle);

        InGameCursor.Instance.StartHoldingLMB(minigame);
        while (!task.IsComplete)
        {
            // exit if complete
            if (Mathf.Abs(minigame.spinDel) > 720f + 10f)
            {
                InGameCursor.Instance.StopHoldingLMB();
                yield break;
            }

            SpinnerTumblerAngle += Time.deltaTime * 100f;
            InGameCursor.Instance.SnapToPositionOnCircle(minigame.Spinner, SpinnerRadius, SpinnerTumblerAngle);

            if (minigame.spinVel < 200f) minigame.spinVel = 200f;

            yield return null;
        }
    }

    private static bool AngleNear(SafeMinigame minigame, float actual, float dir, float expected, float Threshold)
    {
        try
        {
            _enablePatch = false;
            return minigame.AngleNear(actual, dir, expected, Threshold);
        }
        finally
        {
            _enablePatch = true;
        }
    }

    private static bool _enablePatch = true;

    [HarmonyPatch(typeof(SafeMinigame), nameof(SafeMinigame.AngleNear))]
    [HarmonyPrefix]
    public static bool AlwaysCorrectAnglePatch(SafeMinigame __instance, out bool __result)
    {
        __result = true;
        return !_enablePatch;
    }
}

using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(SafeMinigame))]
public class UnlockSafeSolver : IMinigameSolver, IMinigameOpener
{
    public bool ShouldOpenConsole(Console console, PlayerTask task) => true;

    public IEnumerator CompleteMinigame(Minigame minigame, PlayerTask task)
    {
        if (!task) yield break;
        // throw new System.Exception();

        var safeMinigame = minigame.Cast<SafeMinigame>();
        yield return EnterTumblerCode(safeMinigame);

        yield return SpinUnlockSpinner(safeMinigame);
    }

    private static IEnumerator EnterTumblerCode(SafeMinigame minigame)
    {
        int[] combo = minigame.combo;
        Transform tumblerTransform = minigame.Tumbler.transform;
        Vector2 tumblerPosition = tumblerTransform.position;
        float startAngle = tumblerTransform.eulerAngles.z;

        // Input the combination to the tumbler
        for (int i = 0; i < combo.Length; i++)
        {
            int desiredRotation = combo[i] * 45;
            Info($"sA: {startAngle}");
            Info($"dR: {desiredRotation}");

            const float RADIUS = 0.8f;
            yield return InGameCursor.Instance.CoMoveToPositionOnCircle(tumblerPosition, RADIUS, startAngle);
            yield return new WaitForSeconds(1f);

            InGameCursor.Instance.StartHoldingLMB(minigame);
            if (Mathf.Sign(desiredRotation) < 0)
            {
                Info("n");
                yield return InGameCursor.Instance.CoMoveCircle(tumblerPosition, RADIUS, startAngle, -desiredRotation, 3f);
                startAngle = -desiredRotation;
            }
            else
            {
                Info("p");
                yield return InGameCursor.Instance.CoMoveCircle(tumblerPosition, RADIUS, startAngle, desiredRotation, 3f);
                startAngle = desiredRotation;
            }

            yield return new WaitForSeconds(0.2f);
        }

        InGameCursor.Instance.StopHoldingLMB();
    }

    private static IEnumerator SpinUnlockSpinner(SafeMinigame minigame)
    {
        Transform spinnerTransform = minigame.Spinner.transform;
        Vector2 spinnerPosition = spinnerTransform.position;

        float lastAngle = 0f;
        do
        {
            yield return InGameCursor.Instance.CoMoveCircle(spinnerPosition, 1.6f, lastAngle, lastAngle + 8, 0.01f);
            lastAngle += 8;

            // Continually grab and let go of the spinner to toss it
            if (Time.time * 4 % 1 > 0.5f) InGameCursor.Instance.StopHoldingLMB();
            else if (!InGameCursor.Instance.IsLeftButtonPressed) InGameCursor.Instance.StartHoldingLMB(minigame);
        } while (Mathf.Abs(minigame.spinDel) < 720f);

        InGameCursor.Instance.StopHoldingLMB();
    }
}

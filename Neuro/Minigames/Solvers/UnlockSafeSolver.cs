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

            const float RADIUS = 0.85f;
            yield return InGameCursor.Instance.CoMoveToPositionOnCircle(tumblerPosition, RADIUS, startAngle);
            yield return new WaitForSeconds(0.3f);

            if (!InGameCursor.Instance.IsLeftButtonPressed) InGameCursor.Instance.StartHoldingLMB(minigame);
            do
            {
                yield return InGameCursor.Instance.CoMoveCircle(tumblerPosition, RADIUS, startAngle, startAngle + 3 * Mathf.Sign(desiredRotation * -1), 0.01f);
                startAngle += 3 * Mathf.Sign(desiredRotation * -1);
            } while (!minigame.AngleNear(tumblerTransform.eulerAngles.z + 45, Mathf.Sign(desiredRotation), desiredRotation, 2f));

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

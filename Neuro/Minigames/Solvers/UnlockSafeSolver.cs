using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(SafeMinigame))]
public class UnlockSafeSolver : IMinigameSolver<SafeMinigame>, IMinigameOpener
{
    public bool ShouldOpenConsole(Console console, PlayerTask task) => true;

    public IEnumerator CompleteMinigame(SafeMinigame minigame)
    {
        int[] combo = minigame.combo;
        Transform tumblerTransform = minigame.Tumbler.transform;
        Transform spinnerTransform = minigame.Spinner.transform;
        Vector2 tumblerPosition = tumblerTransform.position;
        Vector2 spinnerPosition = spinnerTransform.position;
        Vector2 mousePosition = tumblerPosition;
        float time = 0f;

        // TODO: Why doesn't this work sometimes??
        // Input the combination to the tumbler
        for (int i = 0; i < combo.Length; i++)
        {
            int desiredRotation = combo[i] * 45;
            int direction = i == 1
                ? -1
                : 1;

            do
            {
                time += Time.deltaTime * direction;

                yield return InGameCursor.Instance.CoMoveTo(mousePosition, 10);

                const int SPEED_MULTIPLIER = 5;
                mousePosition.x = tumblerPosition.x + Mathf.Sin(time * SPEED_MULTIPLIER) * 0.75f;
                mousePosition.y = tumblerPosition.y + Mathf.Cos(time * SPEED_MULTIPLIER) * 0.75f;

                if (!InGameCursor.Instance.IsLeftButtonPressed) InGameCursor.Instance.StartHoldingLMB(minigame);
            } while (!minigame.AngleNear(tumblerTransform.eulerAngles.z + 45, direction, desiredRotation, 2f));

            yield return new WaitForSeconds(0.2f);
        }

        // Spin the unlock spinner
        InGameCursor.Instance.StopHoldingLMB();
        mousePosition = spinnerPosition;
        do
        {
            time += Time.deltaTime;
            yield return InGameCursor.Instance.CoMoveTo(mousePosition);

            const int SPEED_MULTIPLIER = 30;
            mousePosition.x = spinnerPosition.x + Mathf.Sin(time * SPEED_MULTIPLIER) * 1.6f;
            mousePosition.y = spinnerPosition.y + Mathf.Cos(time * SPEED_MULTIPLIER) * 1.6f;

            // Continually grab and let go of the spinner to toss it
            if (time * 4 % 1 > 0.5f) InGameCursor.Instance.StopHoldingLMB();
            else if (!InGameCursor.Instance.IsLeftButtonPressed) InGameCursor.Instance.StartHoldingLMB(minigame);
        } while (Mathf.Abs(minigame.spinDel) < 720f);

        InGameCursor.Instance.StopHoldingLMB();
    }
}
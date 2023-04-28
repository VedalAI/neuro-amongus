using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(TuneRadioMinigame))]
public class TuneRadioSabotageSolver : IMinigameSolver<TuneRadioMinigame>, IMinigameOpener
{
    public bool ShouldOpenConsole(Console console, PlayerTask task) => true;

    public IEnumerator CompleteMinigame(TuneRadioMinigame minigame)
    {
        const float radius = 0.6f;
        const float angleOffset = 90f * Mathf.Deg2Rad;
        const float smoothTurnSpeed = 0.15f; //Speed of the dail turning. Smaller is faster.

        float angleStep = 11.25f * Mathf.Deg2Rad;
        if (Random.Range(0, 2) > 0)
        {
            angleStep = -angleStep;
        }

        float angle = minigame.dial.transform.rotation.z + angleOffset;
        float targetAngle = angle + angleStep;
        float currentVelocity = 0f;
        float noiseLevel = minigame.actualSignal.NoiseLevel;
        bool canSwitch = true;// Only so it will not be stuck switching angles

        yield return InGameCursor.Instance.CoMoveToCircleStart(minigame.dial.DialTrans, radius, angle);

        InGameCursor.Instance.StartHoldingLMB(minigame);        

        while (!minigame.finished)
        {
            // Lerp was not working for me, Using SmoothDampAngle
            while (Mathf.Abs(angle - targetAngle) > 0.1f)
            {
                angle = Mathf.SmoothDampAngle(angle, targetAngle, ref currentVelocity, smoothTurnSpeed);
                yield return InGameCursor.Instance.CoMoveToCircleStart(minigame.dial.DialTrans, radius, angle);

                noiseLevel = minigame.actualSignal.NoiseLevel;
                if (noiseLevel < 0.1f)
                {
                    break;
                }
            }

            if (noiseLevel < 0.1f)
            {
                break;
            }
            else if (!canSwitch && noiseLevel < 0.7f)
            {
                canSwitch = true;
            }
            else if (canSwitch && noiseLevel == 1f) // Way off, go other way
            {
                canSwitch = false;
                angleStep = -angleStep;
            }

            targetAngle = angle + angleStep;
        }

        InGameCursor.Instance.StopHoldingLMB();
    }
}

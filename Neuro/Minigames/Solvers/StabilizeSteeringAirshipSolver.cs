using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(AdjustSteeringGame))]
public class StabilizeSteeringAirshipSolver : MinigameSolver<AdjustSteeringGame>
{
    protected override IEnumerator CompleteMinigame(AdjustSteeringGame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.Thrust);
        float startingThrust = minigame.Thrust.transform.localPosition.y;
        for (float t = 0; t < 0.5f; t += Time.deltaTime)
        {
            InGameCursor.Instance.SnapTo(minigame.Steering);

            Vector3 position = minigame.Thrust.transform.localPosition;
            position.y = Mathf.Lerp(startingThrust, minigame.TargetThrustY, t / 0.5f);
            minigame.Thrust.transform.localPosition = position;
            yield return null;
        }
        minigame.thrustLocked = true;

        // TODO: make the cursor stick to one of the steering wheel grips
        yield return InGameCursor.Instance.CoMoveTo(minigame.Steering);
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            InGameCursor.Instance.SnapTo(minigame.Steering);

            Vector3 eulerAngles = minigame.Steering.transform.localEulerAngles;
            eulerAngles.z = Mathf.LerpAngle(minigame.startAngle, minigame.TargetSteeringRot, t);
            minigame.Steering.transform.localEulerAngles = eulerAngles;
            yield return null;
        }
        minigame.steeringLocked = true;
    }
}
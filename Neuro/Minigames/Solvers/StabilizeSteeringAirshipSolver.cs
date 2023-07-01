using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(AdjustSteeringGame))]
public sealed class StabilizeSteeringAirshipSolver : GeneralMinigameSolver<AdjustSteeringGame>
{
    public override float CloseTimout => 6;

    public override IEnumerator CompleteMinigame(AdjustSteeringGame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.Thrust);
        InGameCursor.Instance.StartHoldingLMB(minigame);
        yield return InGameCursor.Instance.CoMoveTo(minigame.transform.TransformPoint(minigame.Thrust.transform.localPosition with {y = minigame.TargetThrustY}));
        InGameCursor.Instance.StopHoldingLMB();

        yield return InGameCursor.Instance.CoMoveTo(minigame.Steering);
        InGameCursor.Instance.StartHoldingLMB(minigame);
        float localEulerAngles2z = minigame.Steering.transform.localEulerAngles.z;
        if (localEulerAngles2z > 180f) localEulerAngles2z -= 360f;
        Vector3 targetOffset = (minigame.TargetSteeringRot - localEulerAngles2z) / -15f * Vector2.right;
        yield return InGameCursor.Instance.CoMoveTo(minigame.Steering.transform.position + targetOffset, 0.5f);
        yield return new WaitForSeconds(0.2f);
        InGameCursor.Instance.StopHoldingLMB();
    }
}

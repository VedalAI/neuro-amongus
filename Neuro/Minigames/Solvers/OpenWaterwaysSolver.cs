using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(WaterWheelGame))]
public sealed class OpenWaterwaysSolver : GeneralMinigameSolver<WaterWheelGame>
{
    public override IEnumerator CompleteMinigame(WaterWheelGame minigame, NormalPlayerTask task)
    {
        Transform wheelTransform = minigame.Wheel.transform;
        Vector2 wheelPosition = wheelTransform.position;
        Vector2 position = wheelPosition;
        float time = 0f;

        do
        {
            time += Time.deltaTime;
            yield return InGameCursor.Instance.CoMoveTo(position);

            const int SPEED_MULTIPLIER = 80;
            position.x = wheelPosition.x + Mathf.Sin(time * SPEED_MULTIPLIER) * -2.1f;
            position.y = wheelPosition.y + Mathf.Cos(time * SPEED_MULTIPLIER) * 2.1f;

            if (!InGameCursor.Instance.IsLeftButtonPressed) InGameCursor.Instance.StartHoldingLMB(minigame);
        } while (wheelTransform.localEulerAngles.z < 358.99f);

        InGameCursor.Instance.StopHoldingLMB();
    }
}
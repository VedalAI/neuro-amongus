using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(WaterWheelGame))]
public sealed class OpenWaterwaysSolver : GeneralMinigameSolver<WaterWheelGame>
{
    public override float CloseTimout => 15;

    public override IEnumerator CompleteMinigame(WaterWheelGame minigame, NormalPlayerTask task)
    {
        Transform wheelTransform = minigame.Wheel.transform;
        Vector2 wheelPosition = wheelTransform.position;

        yield return InGameCursor.Instance.CoMoveToPositionOnCircle(wheelPosition, 2f, 0);
        InGameCursor.Instance.StartHoldingLMB(minigame);
        do
        {
            yield return InGameCursor.Instance.CoMoveCircle(wheelPosition, 2f, 0, 359.5f, 0.3f);
        } while (wheelTransform.localEulerAngles.z < 358.99f);

        InGameCursor.Instance.StopHoldingLMB();
    }
}

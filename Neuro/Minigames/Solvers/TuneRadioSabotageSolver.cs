using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(TuneRadioMinigame))]
public class TuneRadioSabotageSolver : MinigameSolver<TuneRadioMinigame>
{
    protected override IEnumerator CompleteMinigame(TuneRadioMinigame minigame, NormalPlayerTask task)
    {
        float radius = 0.6f;
        float angle = 90f;
        float angleStep = 1f;

        Vector3 moveTo = PositionInCircumference(minigame.dial.DialTrans.transform.position, radius, angle);
        yield return InGameCursor.Instance.CoMoveTo(moveTo);
        InGameCursor.Instance.StartHoldingLMB(minigame.dial.DialTrans);

        while (!minigame.finished)
        {
            if (minigame.targetAngle < minigame.dial.Value)
            {
                angle -= angleStep;
            } else {
                angle += angleStep;
            }

            moveTo = PositionInCircumference(minigame.dial.DialTrans.transform.position, radius, angle);
            yield return InGameCursor.Instance.CoMoveTo(moveTo);
        }

        InGameCursor.Instance.StopHolding();
    }

    // https://answers.unity.com/questions/759542/get-coordinate-with-angle-and-distance.html
    protected Vector3 PositionInCircumference(Vector3 center, float radius, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float x = Mathf.Cos(radians);
        float y = Mathf.Sin(radians);
        Vector3 position = new Vector3(x, y, 0);
        position *= radius;
        position += center;

        return position;
    }
}

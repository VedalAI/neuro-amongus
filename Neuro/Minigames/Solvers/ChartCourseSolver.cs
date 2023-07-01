using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(CourseMinigame))]
public sealed class ChartCourseSolver : GeneralMinigameSolver<CourseMinigame>
{
    public override float CloseTimout => 7;

    public override IEnumerator CompleteMinigame(CourseMinigame minigame, NormalPlayerTask task)
    {
        // for some reason this minigame uses localPositions for everything so we have to convert them into world positions

        yield return InGameCursor.Instance.CoMoveTo(minigame.Ship);
        InGameCursor.Instance.StartHoldingLMB(minigame);

        int start = (int) minigame.Converter.GetFloat(minigame.MyNormTask.Data);
        for (int i = start; i < minigame.NumPoints; i++)
        {
            Vector3 point = minigame.PathPoints[i];
            Vector3 worldPos = minigame.transform.TransformPoint(point);
            if (i == minigame.NumPoints - 1)
            {
                // minigame requires you to move the mouse slightly past the final point
                // so we handle that here
                worldPos += new Vector3(0.2f, 0.2f, 0f);
                yield return InGameCursor.Instance.CoMoveTo(worldPos, 0.7f);
                InGameCursor.Instance.StopHoldingLMB();
                break;
            }

            yield return InGameCursor.Instance.CoMoveTo(worldPos, 0.7f);
            yield return new WaitForSeconds(0.1f);
        }
    }
}

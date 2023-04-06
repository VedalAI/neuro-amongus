using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(CourseMinigame))]
public class ChartCourseSolver : MinigameSolver<CourseMinigame>
{
    protected override IEnumerator CompleteMinigame(CourseMinigame minigame, NormalPlayerTask task)
    {
        // for some reason this minigame uses localPositions for everything so we have to convert them into world positions

        yield return InGameCursor.Instance.CoMoveTo(minigame.transform.TransformPoint(minigame.Ship.transform.localPosition));
        InGameCursor.Instance.StartHoldingLMB(minigame.Ship);
        for (int i = 0; i < minigame.NumPoints; i++)
        {
            Vector3 point = minigame.PathPoints[i];
            Vector3 worldPos = minigame.transform.TransformPoint(point);
            if (i + 1 == minigame.NumPoints)
            {
                // minigame requires you to move the mouse slightly past the final point
                // so we handle that here
                worldPos += new Vector3(0.1f, 0.1f, 0f);
                yield return InGameCursor.Instance.CoMoveTo(worldPos, 0.33f);
                InGameCursor.Instance.StopHolding();
                yield break;
            }
            yield return InGameCursor.Instance.CoMoveTo(worldPos, 0.33f);
            yield return Sleep(0.1f);
        }
    }
}
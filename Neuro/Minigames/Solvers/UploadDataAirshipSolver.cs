using System.Collections;
using UnityEngine;
using Neuro.Cursor;
using Neuro.Utilities;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(AirshipUploadGame), false)]
[MinigameOpener(typeof(AutoMultistageMinigame))]
public sealed class UploadDataAirshipSolver : IMinigameSolver<AirshipUploadGame, NormalPlayerTask>, IMinigameOpener
{
    public bool ShouldOpenConsole(Console console, PlayerTask task)
    {
        return task.TaskType == TaskTypes.UploadData;
    }

    public IEnumerator CompleteMinigame(AirshipUploadGame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.Phone);
        InGameCursor.Instance.StartHoldingLMB(minigame);
        Vector2 target;

        // before we know where poor is, just try random points on the screen
        bool poorFound = false;
        while (!poorFound)
        {
            Vector2 start = minigame.Phone.transform.position;
            target = NeuroUtilities.MainCamera.ScreenToWorldPoint(new Vector2(Random.Range(10, Screen.width - 10), Random.Range(10, Screen.height - 10)));
            float time = Vector2.Distance(start, target) / 6f;
            for (float t = 0; t < time; t += Time.deltaTime)
            {
                InGameCursor.Instance.SnapTo(Vector2.Lerp(start, target, t / time));
                // if we find poor while moving to our target, break out early
                if (minigame.Poor.IsTouching(minigame.Hotspot))
                {
                    poorFound = true;
                    break;
                }
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.2f);

        // afterward, good must be somewhere nearby, so just check in a radius near the hotspot
        while (!minigame.Good.IsTouching(minigame.Hotspot))
        {
            target = (Vector2)minigame.Hotspot.transform.position + Random.insideUnitCircle * minigame.Good.bounds.extents;
            yield return InGameCursor.Instance.CoMoveTo(target, 0.2f);
        }

        yield return new WaitForSeconds(0.2f);

        // do the same for perfect
        while (!minigame.Perfect.IsTouching(minigame.Hotspot))
        {
            target = (Vector2)minigame.Hotspot.transform.position + Random.insideUnitCircle * minigame.Perfect.bounds.extents;
            yield return InGameCursor.Instance.CoMoveTo(target, 0.1f);
        }

        InGameCursor.Instance.StopHoldingLMB();
    }

}

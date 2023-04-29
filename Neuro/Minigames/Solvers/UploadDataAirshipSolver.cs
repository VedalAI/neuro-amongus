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

        // Find poor zone
        bool poorFound = false;
        while (!poorFound)
        {
            Vector2 start = minigame.Phone.transform.position;
            Vector2 target = NeuroUtilities.MainCamera.ScreenToWorldPoint(new Vector2(Random.Range(10, Screen.width - 10), Random.Range(10, Screen.height - 10)));
            float time = Vector2.Distance(start, target) / 6f;
            for (float t = 0; t < time; t += Time.deltaTime)
            {
                InGameCursor.Instance.SnapTo(Vector2.Lerp(start, target, t / time));
                if (minigame.Poor.IsTouching(minigame.Hotspot))
                {
                    poorFound = true;
                    break;
                }

                yield return null;
            }
        }

        Vector2 lastPosition = InGameCursor.Instance.transform.position;
        float searchRadius = minigame.Good.bounds.extents.magnitude;

        // Find good zone
        while (!minigame.Good.IsTouching(minigame.Hotspot))
        {
            Vector2 target = (Vector2) InGameCursor.Instance.transform.position + Random.insideUnitCircle * searchRadius;
            yield return InGameCursor.Instance.CoMoveTo(target, 0.2f);

            // Check if the quality has decreased
            if (!minigame.Poor.IsTouching(minigame.Hotspot))
            {
                // Go back to the last position
                yield return InGameCursor.Instance.CoMoveTo(lastPosition, 0.2f);
            }
            else
            {
                lastPosition = InGameCursor.Instance.transform.position;
                searchRadius += 0.1f; // Gradually increase the search radius
            }
        }

        yield return new WaitForSeconds(0.2f);

        lastPosition = InGameCursor.Instance.transform.position;
        searchRadius = minigame.Perfect.bounds.extents.magnitude;

        // Find perfect zone
        while (!minigame.Perfect.IsTouching(minigame.Hotspot))
        {
            Vector2 target = (Vector2) InGameCursor.Instance.transform.position + Random.insideUnitCircle * searchRadius;
            yield return InGameCursor.Instance.CoMoveTo(target, 0.1f);

            // Check if the quality has decreased
            if (!minigame.Good.IsTouching(minigame.Hotspot))
            {
                // Go back to the last position
                yield return InGameCursor.Instance.CoMoveTo(lastPosition, 0.1f);
            }
            else
            {
                lastPosition = InGameCursor.Instance.transform.position;
                searchRadius += 0.05f; // Gradually increase the search radius
            }
        }

        InGameCursor.Instance.StopHoldingLMB();
    }
}

using System.Collections;
using Neuro.Cursor;
using UnityEngine;
using static Il2CppSystem.Uri;

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

        float randAngle = Random.RandomRange(0f, 360f);
        Vector2 position = InGameCursor.Instance.Position;
        const float radius = 2.5f;
        const float speed = 5f;
        yield return InGameCursor.Instance.CoMoveToPositionOnCircle(position, radius, randAngle, 0.5f);
        for (float t = 0; t < speed; t += Time.deltaTime)
        {
            // if perfect stop
            if (minigame.Hotspot.IsTouching(minigame.Perfect))
            {
                InGameCursor.Instance.StopHoldingLMB();
                yield break;
            }

            float angle = Mathf.Lerp(randAngle, randAngle + 360f, t / speed);
            InGameCursor.Instance.SnapToPositionOnCircle(position, radius, angle);
            yield return null;
        }

        //yield return InGameCursor.Instance.CoMoveTo(InGameCursor.Instance.Position + (Vector2)minigame.Hotspot.transform.localPosition, 0.2f);
    }
}

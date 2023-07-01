using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(AirshipUploadGame), false)]
[MinigameOpener(typeof(AutoMultistageMinigame))]
public sealed class UploadDataAirshipSolver : IMinigameSolver<AirshipUploadGame, NormalPlayerTask>, IMinigameOpener
{
    public float CloseTimout => 13;

    public bool ShouldOpenConsole(Console console, PlayerTask task)
    {
        return task.TaskType == TaskTypes.UploadData;
    }

    public IEnumerator CompleteMinigame(AirshipUploadGame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.Phone);
        InGameCursor.Instance.StartHoldingLMB(minigame);

        float angle = Random.RandomRange(0f, 360f);
        Vector2 position = InGameCursor.Instance.Position;
        const float radius = 2.5f;
        const float speed = 60f;

        bool flip = false;
        bool poorFlag = false;

        yield return InGameCursor.Instance.CoMoveToPositionOnCircle(position, radius, angle, 0.5f);
        while (!task.IsComplete)
        {
            // if perfect stop
            if (minigame.Hotspot.IsTouching(minigame.Perfect))
            {
                InGameCursor.Instance.StopHoldingLMB();
                yield break;
            }

            // bounce back if edge of zone is reached
            if (minigame.Hotspot.IsTouching(minigame.Poor))
            {
                poorFlag = true;
            }
            else if (poorFlag)
            {
                poorFlag = false;
                flip = !flip;
            }

            angle += Time.deltaTime * speed * (flip ? -1 : 1);
            InGameCursor.Instance.SnapToPositionOnCircle(position, radius, angle);
            yield return null;
        }
    }
}
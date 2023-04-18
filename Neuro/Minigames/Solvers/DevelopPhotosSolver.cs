using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(PhotosMinigame))]
public class DevelopPhotosSolver : GeneralMinigameSolver<PhotosMinigame>
{
    public override IEnumerator CompleteMinigame(PhotosMinigame minigame, NormalPlayerTask task)
    {
        if (task.TimerStarted == NormalPlayerTask.TimerState.NotStarted) yield return CompleteStage1(minigame);
        else if (task.TimerStarted == NormalPlayerTask.TimerState.Finished) yield return CompleteStage2(minigame);
        else
        {
            yield return new WaitForSeconds(0.5f);
            minigame.Close();
        }
    }

    private IEnumerator CompleteStage1(PhotosMinigame minigame)
    {
        foreach (GamePhotoBehaviour photo in minigame.photos)
        {
            yield return InGameCursor.Instance.CoMoveTo(photo);
            InGameCursor.Instance.StartHoldingLMB(photo);
            Bounds bounds = minigame.PoolHitbox.bounds;
            float xOffset = Random.Range(bounds.center.x - 1, bounds.center.x + 1);
            float yOffset = Random.Range(bounds.center.y - 1, bounds.center.y + 1);
            yield return InGameCursor.Instance.CoMoveTo(new Vector2(xOffset, yOffset));
            InGameCursor.Instance.StopHoldingLMB();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator CompleteStage2(PhotosMinigame minigame)
    {
        foreach (GamePhotoBehaviour photo in minigame.photos)
        {
            yield return InGameCursor.Instance.CoMoveTo(photo);
            InGameCursor.Instance.StartHoldingLMB(photo);
            Rect bounds = minigame.PolaroidBounds;
            Vector2 position = new(bounds.xMin, Random.Range(bounds.yMin, bounds.yMax));
            yield return InGameCursor.Instance.CoMoveTo(minigame.transform.TransformPoint(position));
            InGameCursor.Instance.StopHoldingLMB();
            yield return new WaitForSeconds(0.1f);
        }
    }
}

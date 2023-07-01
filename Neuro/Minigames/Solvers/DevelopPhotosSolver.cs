using System.Collections;
using Neuro.Cursor;
using UnityEngine;
using static NormalPlayerTask;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(PhotosMinigame))]
public sealed class DevelopPhotosSolver : IMinigameSolver<PhotosMinigame, NormalPlayerTask>, IMinigameOpener<NormalPlayerTask>
{
    public float CloseTimout => 6;

    public bool ShouldOpenConsole(Console console, NormalPlayerTask task)
    {
        return task.TimerStarted is TimerState.NotStarted or TimerState.Finished;
    }

    public IEnumerator CompleteMinigame(PhotosMinigame minigame, NormalPlayerTask task)
    {
        switch (task.TimerStarted)
        {
            case TimerState.NotStarted:
                yield return CompleteStep1(minigame);
                break;
            case TimerState.Finished:
                yield return CompleteStep2(minigame);
                break;
            default:
                minigame.CoStartClose(0.5f);
                break;
        }
    }

    private IEnumerator CompleteStep1(PhotosMinigame minigame)
    {
        foreach (GamePhotoBehaviour photo in minigame.photos)
        {
            yield return InGameCursor.Instance.CoMoveTo(photo);
            InGameCursor.Instance.StartHoldingLMB(minigame);
            Bounds bounds = minigame.PoolHitbox.bounds;
            float xOffset = Random.Range(bounds.center.x - 1, bounds.center.x + 1);
            float yOffset = Random.Range(bounds.center.y - 1, bounds.center.y + 1);
            yield return InGameCursor.Instance.CoMoveTo(new Vector2(xOffset, yOffset));
            InGameCursor.Instance.StopHoldingLMB();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator CompleteStep2(PhotosMinigame minigame)
    {
        foreach (GamePhotoBehaviour photo in minigame.photos)
        {
            yield return InGameCursor.Instance.CoMoveTo(photo);
            InGameCursor.Instance.StartHoldingLMB(minigame);
            Rect bounds = minigame.PolaroidBounds;
            Vector2 position = new(bounds.xMin, Random.Range(bounds.yMin, bounds.yMax));
            yield return InGameCursor.Instance.CoMoveTo(minigame.transform.TransformPoint(position));
            InGameCursor.Instance.StopHoldingLMB();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
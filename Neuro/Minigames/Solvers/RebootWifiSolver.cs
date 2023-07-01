using System.Collections;
using Neuro.Cursor;
using UnityEngine;
using static NormalPlayerTask;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(WifiGame))]
public sealed class RebootWifiSolver : IMinigameSolver<WifiGame, NormalPlayerTask>, IMinigameOpener<NormalPlayerTask>
{
    public float CloseTimout => 10;

    public bool ShouldOpenConsole(Console console, NormalPlayerTask task)
    {
        return task.TimerStarted is TimerState.NotStarted or TimerState.Finished;
    }

    public IEnumerator CompleteMinigame(WifiGame minigame, NormalPlayerTask task)
    {
        switch (task.TimerStarted)
        {
            case TimerState.NotStarted:
                yield return CompleteStep(minigame, 1, 0);
                yield return new WaitForSeconds(0.5f);
                minigame.Close();
                break;
            case TimerState.Finished:
                yield return CompleteStep(minigame, 0, 1);
                break;
            default:
                yield return new WaitForSeconds(0.5f);
                minigame.Close();
                break;
        }
    }

    private IEnumerator CompleteStep(WifiGame minigame, float from, float to)
    {
        const float slideDuration = 0.2f;
        yield return InGameCursor.Instance.CoMoveTo(minigame.Slider);
        for (float t = 0; t < slideDuration; t += Time.deltaTime)
        {
            InGameCursor.Instance.SnapTo(minigame.Slider);
            minigame.Slider.Value = Mathf.Lerp(from, to, t / slideDuration);
            minigame.Slider.UpdateValue();
            yield return null;
        }

        minigame.Slider.Value = to;
        minigame.Slider.UpdateValue();
        InGameCursor.Instance.SnapTo(minigame.Slider);
    }
}

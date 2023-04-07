using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(WifiGame))]
public class RebootWifiSolver : MinigameSolver<WifiGame>
{
    protected override IEnumerator CompleteMinigame(WifiGame minigame, NormalPlayerTask task)
    {
        if (task.TimerStarted == NormalPlayerTask.TimerState.NotStarted)
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.Slider);
            for (float t = 0; t < 0.75f; t += Time.deltaTime)
            {
                InGameCursor.Instance.SnapTo(minigame.Slider);
                minigame.Slider.Value = Mathf.Lerp(1f, 0f, t / 0.75f);
                minigame.Slider.UpdateValue();
                yield return null;
            }
            yield return Sleep(0.5f);
            minigame.Close();
        }
        else if (task.TimerStarted == NormalPlayerTask.TimerState.Finished)
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.Slider);
            for (float t = 0; t < 0.75f; t += Time.deltaTime)
            {
                InGameCursor.Instance.SnapTo(minigame.Slider);
                minigame.Slider.Value = Mathf.Lerp(0f, 1f, t / 0.75f);
                minigame.Slider.UpdateValue();
                yield return null;
            }
        }
        else
        {
            yield return Sleep(0.5f);
            minigame.Close();
        }
    }
}

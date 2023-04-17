using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(WeatherSwitchGame))]
public class WeatherSwitchSolver : GeneralMinigameSolver<WeatherSwitchGame>
{
    public override IEnumerator CompleteMinigame(WeatherSwitchGame minigame, NormalPlayerTask task)
    {
        var desiredSwitch = minigame.Controls[minigame.WeatherTask.NodeId];

        yield return InGameCursor.Instance.CoMoveTo(desiredSwitch.Switch);
        InGameCursor.Instance.StartHoldingLMB(desiredSwitch.Switch);
        yield return new WaitForSeconds(0.1f);
        InGameCursor.Instance.StopHoldingLMB();
    }
}
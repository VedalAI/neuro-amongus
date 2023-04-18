using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(WeatherSwitchGame))]
public sealed class WeatherSwitchSolver : GeneralMinigameSolver<WeatherSwitchGame>
{
    public override IEnumerator CompleteMinigame(WeatherSwitchGame minigame, NormalPlayerTask task)
    {
        var desiredSwitch = minigame.Controls[minigame.WeatherTask.NodeId];

        yield return InGameCursor.Instance.CoMoveTo(desiredSwitch.Switch);
        yield return InGameCursor.Instance.CoPressLMB();
    }
}
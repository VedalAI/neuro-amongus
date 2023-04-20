using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(WeatherSwitchGame))]
public sealed class WeatherSwitchSolver : GeneralMinigameSolver<WeatherSwitchGame>
{
    public override IEnumerator CompleteMinigame(WeatherSwitchGame minigame, NormalPlayerTask task)
    {
        WeatherControl desiredSwitch = minigame.Controls[minigame.WeatherTask.NodeId];

        yield return InGameCursor.Instance.CoMoveTo(desiredSwitch.Switch);
        yield return InGameCursor.Instance.CoPressLMB();
    }
}
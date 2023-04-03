using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(WeatherMinigame))]
public sealed class MeasureWeatherSolver : MinigameSolver<WeatherMinigame>
{
    protected override IEnumerator CompleteMinigame(WeatherMinigame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.StartButton);
        minigame.StartStopFill();
    }
}

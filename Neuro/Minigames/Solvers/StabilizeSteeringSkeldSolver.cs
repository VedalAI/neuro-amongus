using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(NavigationMinigame))]
public class StabilizeSteeringSkeldSolver : MinigameSolver<NavigationMinigame>
{
    protected override IEnumerator CompleteMinigame(NavigationMinigame minigame, NormalPlayerTask task)
    {
        InGameCursor.Instance.SnapToCenter();
        yield return InGameCursor.Instance.CoPressLMB();
    }
}

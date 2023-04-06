using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(NavigationMinigame))]
public class StabilizeSteeringSkeldSolver : MinigameSolver<NavigationMinigame>
{
    protected override IEnumerator CompleteMinigame(NavigationMinigame minigame, NormalPlayerTask task)
    {
        InGameCursor.Instance.SnapToCenter();
        InGameCursor.Instance.StartHoldingLMB(minigame.CrossHairImage);
        yield return Sleep(0.5f);
        InGameCursor.Instance.StopHolding();
    }
}
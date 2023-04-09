using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(NavigationMinigame))]
public class StabilizeSteeringSkeldSolver : TaskMinigameSolver<NavigationMinigame>
{
    protected override IEnumerator CompleteMinigame(NavigationMinigame minigame, NormalPlayerTask task)
    {
        InGameCursor.Instance.SnapTo(minigame.CrossHairImage);
        InGameCursor.Instance.StartHoldingLMB(minigame.CrossHairImage);
        yield return InGameCursor.Instance.CoMoveToCenter(0.75f);
        InGameCursor.Instance.StopHoldingLMB();
    }
}

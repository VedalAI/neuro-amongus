using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(NavigationMinigame))]
public sealed class StabilizeSteeringSkeldSolver : GeneralMinigameSolver<NavigationMinigame>
{
    public override float CloseTimout => 6;

    public override IEnumerator CompleteMinigame(NavigationMinigame minigame, NormalPlayerTask task)
    {
        InGameCursor.Instance.SnapTo(minigame.CrossHairImage);
        InGameCursor.Instance.StartHoldingLMB(minigame);
        yield return InGameCursor.Instance.CoMoveToCenter(0.75f);
        yield return new WaitForSeconds(0.1f);
        InGameCursor.Instance.StopHoldingLMB();
    }
}

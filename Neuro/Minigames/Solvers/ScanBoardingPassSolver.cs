using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(BoardPassGame))]
public sealed class ScanBoardingPassSolver : GeneralMinigameSolver<BoardPassGame>
{
    public override float CloseTimout => 9;

    public override IEnumerator CompleteMinigame(BoardPassGame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.pullButton);
        yield return minigame.CoPullPass();
        yield return InGameCursor.Instance.CoMoveTo(minigame.flipButton);
        yield return minigame.CoFlipPass();
        yield return InGameCursor.Instance.CoMoveTo(minigame.pass);
        yield return new WaitForSeconds(0.1f);

        InGameCursor.Instance.StartHoldingLMB(minigame);
        yield return InGameCursor.Instance.CoMoveTo(minigame.Sensor);
        InGameCursor.Instance.StopHoldingLMB();
    }
}

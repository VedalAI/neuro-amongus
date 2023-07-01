using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(AlignGame))]
public sealed class AlignEngineOutputSolver : GeneralMinigameSolver<AlignGame>
{
    public override float CloseTimout => 5;

    public override IEnumerator CompleteMinigame(AlignGame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.col);
        InGameCursor.Instance.StartHoldingLMB(minigame);
        yield return InGameCursor.Instance.CoMoveTo(minigame.transform.TransformPoint(new Vector2(1.5f, 0)));
        InGameCursor.Instance.StopHoldingLMB();
    }
}

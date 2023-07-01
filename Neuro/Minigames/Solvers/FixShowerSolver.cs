using Neuro.Cursor;
using System;
using System.Collections;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(FixShowerMinigame))]
public sealed class FixShowerSolver : GeneralMinigameSolver<FixShowerMinigame>
{
    public override float CloseTimout => 10;

    public override IEnumerator CompleteMinigame(FixShowerMinigame minigame, NormalPlayerTask task)
    {
        do
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.showerHead);
            InGameCursor.Instance.StartHoldingLMB(minigame);
            yield return new WaitForSeconds(Math.Abs(minigame.showerPos - 0.5f));
            InGameCursor.Instance.StopHoldingLMB();
            yield return new WaitForSeconds(0.5f);
        } while (Math.Abs(minigame.showerPos - 0.55f) <= 0.5f);
    }
}

using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(MonitorOxyMinigame))]
public sealed class MonitorTreeSolver : GeneralMinigameSolver<MonitorOxyMinigame>
{
    public override float CloseTimout => 7;

    public override IEnumerator CompleteMinigame(MonitorOxyMinigame minigame, NormalPlayerTask task)
    {
        for (int i = 0; i < minigame.Sliders.Count; i++)
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.Sliders[i], 0.5f);
            InGameCursor.Instance.StartHoldingLMB(minigame);
            yield return InGameCursor.Instance.CoMoveTo(minigame.Targets[i], 0.5f);
            InGameCursor.Instance.StopHoldingLMB();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
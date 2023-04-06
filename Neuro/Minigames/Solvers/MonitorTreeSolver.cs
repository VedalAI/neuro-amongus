using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(MonitorOxyMinigame))]
public class MonitorTreeSolver : MinigameSolver<MonitorOxyMinigame>
{
    protected override IEnumerator CompleteMinigame(MonitorOxyMinigame minigame, NormalPlayerTask task)
    {
        for (int i = 0; i < minigame.Sliders.Count; i++)
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.Sliders[i], 0.5f);
            InGameCursor.Instance.StartHoldingLMB(minigame.Sliders[i]);
            yield return InGameCursor.Instance.CoMoveTo(minigame.Targets[i].bounds.center, 0.5f);
            InGameCursor.Instance.StopHolding();
            yield return Sleep(0.1f);
        }
    }
}
using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(RefuelStage))]
public sealed class FuelEnginesSolver : TaskMinigameSolver<RefuelStage>
{
    protected override IEnumerator CompleteMinigame(RefuelStage minigame, NormalPlayerTask task)
    {
        Vector3 position = Vector3.Lerp(minigame.greenLight.transform.position, minigame.redLight.transform.position, 0.5f) + new Vector3(0f, -0.6f);
        yield return InGameCursor.Instance.CoMoveTo(position);
        InGameCursor.Instance.StartHoldingLMB(minigame);
        while (!minigame.complete) yield return null;
        InGameCursor.Instance.StopHoldingLMB();
    }
}

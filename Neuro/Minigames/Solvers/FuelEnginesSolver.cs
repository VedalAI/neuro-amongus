using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(RefuelStage))]
public sealed class FuelEnginesSolver : MinigameSolver<RefuelStage>
{
    public override IEnumerator CompleteMinigame(RefuelStage minigame, NormalPlayerTask task)
    {
        Vector3 position = Vector3.Lerp(minigame.greenLight.transform.position, minigame.redLight.transform.position, 0.5f) - new Vector3(0f, 0.5f, 0f);
        yield return InGameCursor.Instance.CoMoveTo(position);
        minigame.Refuel();

        while (!minigame.complete) yield return new WaitForFixedUpdate();

        yield return minigame.CoStartClose(0.5f);
    }
}

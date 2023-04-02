using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(typeof(RefuelStage))]
public sealed class FuelEnginesSolver : MinigameSolver<RefuelStage>
{
    public override IEnumerator CompleteMinigame(RefuelStage minigame, NormalPlayerTask task)
    {
        // TODO: Figure out if the button is stored somewhere so we dont have to do this
        Vector3 position = Vector3.Lerp(minigame.greenLight.transform.position, minigame.redLight.transform.position, 0.5f) - new Vector3(0f, 0.5f, 0f);
        yield return InGameCursor.Instance.CoMoveTo(position);
        minigame.Refuel();
        while (!minigame.complete)
            yield return new WaitForFixedUpdate();
        minigame.Close();
    }
}

using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(SpecimenGame))]
public sealed class StoreArtifactsSolver : GeneralMinigameSolver<SpecimenGame>
{
    public override IEnumerator CompleteMinigame(SpecimenGame minigame, NormalPlayerTask task)
    {
        // Reorder artifacts so they are handled top-to-bottom
        (minigame.Specimens[1], minigame.Specimens[2], minigame.Specimens[3]) = (minigame.Specimens[3], minigame.Specimens[2], minigame.Specimens[1]);
        (minigame.Slots[1], minigame.Slots[2], minigame.Slots[3]) = (minigame.Slots[3], minigame.Slots[2], minigame.Slots[1]);

        for (int i = 0; i < minigame.Specimens.Count; i++)
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.Specimens[i], 0.75f);
            InGameCursor.Instance.StartHoldingLMB(minigame.Specimens[i]);
            yield return InGameCursor.Instance.CoMoveTo(minigame.Slots[i], 0.75f);
            yield return new WaitForSeconds(0.1f);
            InGameCursor.Instance.StopHoldingLMB();
        }
    }
}

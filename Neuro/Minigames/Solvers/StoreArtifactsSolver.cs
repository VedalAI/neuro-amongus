using System.Collections;
using System.Linq;
using Neuro.Cursor;
using Neuro.Extensions;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(SpecimenGame))]
public sealed class StoreArtifactsSolver : GeneralMinigameSolver<SpecimenGame>
{
    public override float CloseTimout => 8;

    public override IEnumerator CompleteMinigame(SpecimenGame minigame, NormalPlayerTask task)
    {
        // Reorder artifacts so they are handled top-to-bottom
        minigame.Specimens = minigame.Specimens.ReverseSection(1..).ToArray();
        minigame.Slots = minigame.Slots.ReverseSection(1..).ToArray();

        for (int i = 0; i < minigame.Specimens.Count; i++)
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.Specimens[i], 0.75f);
            InGameCursor.Instance.StartHoldingLMB(minigame);
            yield return InGameCursor.Instance.CoMoveTo(minigame.Slots[i], 0.75f);
            yield return new WaitForSeconds(0.1f);
            InGameCursor.Instance.StopHoldingLMB();
        }
    }
}
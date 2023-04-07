using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(SpecimenGame))]
public class StoreArtifactsSolver : MinigameSolver<SpecimenGame>
{
    protected override IEnumerator CompleteMinigame(SpecimenGame minigame, NormalPlayerTask task)
    {
        for (int i = 0; i < minigame.Specimens.Count; i++)
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.Specimens[i], 0.75f);
            InGameCursor.Instance.StartHoldingLMB(minigame.Specimens[i]);
            yield return InGameCursor.Instance.CoMoveTo(minigame.Slots[i], 0.75f);
            yield return Sleep(0.1f);
            InGameCursor.Instance.StopHolding();
        }
    }
}
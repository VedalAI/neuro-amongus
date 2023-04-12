using Neuro.Cursor;
using System.Collections;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(CrystalMinigame))]
public sealed class AssembleArtifactSolver : MinigameSolver<CrystalMinigame>
{
    protected override IEnumerator CompleteMinigame(CrystalMinigame minigame, NormalPlayerTask task)
    {
        foreach (var crystal in minigame.CrystalPieces)
        {
            yield return InGameCursor.Instance.CoMoveTo(crystal);
            InGameCursor.Instance.StartHoldingLMB(minigame);
            yield return InGameCursor.Instance.CoMoveTo(minigame.CrystalSlots[(int)crystal.PieceIndex]);
            InGameCursor.Instance.StopHolding();
        }
    }
}

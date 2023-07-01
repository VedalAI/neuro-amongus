using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(CrystalMinigame))]
public sealed class AssembleArtifactSolver : GeneralMinigameSolver<CrystalMinigame>
{
    public override float CloseTimout => 10;

    public override IEnumerator CompleteMinigame(CrystalMinigame minigame, NormalPlayerTask task)
    {
        CrystalBehaviour[] crystals = minigame.CrystalPieces;
        Transform[] crystalSlots = minigame.CrystalSlots;

        for (int i = 0; i < crystals.Length; i++)
        {
            yield return InGameCursor.Instance.CoMoveTo(crystals[i]);
            InGameCursor.Instance.StartHoldingLMB(minigame);
            yield return InGameCursor.Instance.CoMoveTo(crystalSlots[i]);
            InGameCursor.Instance.StopHoldingLMB();
            yield return new WaitForSeconds(0.2f);
        }
    }
}

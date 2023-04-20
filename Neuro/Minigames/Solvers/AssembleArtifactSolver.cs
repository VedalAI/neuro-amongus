using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(CrystalMinigame))]
public sealed class AssembleArtifactSolver : GeneralMinigameSolver<CrystalMinigame>
{
    public override IEnumerator CompleteMinigame(CrystalMinigame minigame, NormalPlayerTask task)
    {
        CrystalBehaviour[] crystals = minigame.CrystalPieces;
        Transform[] crystalSlots = minigame.CrystalSlots;

        WaitForSeconds wait = new(0.2f);
        for (int i = 0; i < crystals.Length; i++)
        {
            yield return wait;
            yield return InGameCursor.Instance.CoMoveTo(crystals[i]);
            InGameCursor.Instance.StartHoldingLMB(minigame);
            yield return InGameCursor.Instance.CoMoveTo(crystalSlots[i]);
            InGameCursor.Instance.StopHoldingLMB();
        }
    }
}
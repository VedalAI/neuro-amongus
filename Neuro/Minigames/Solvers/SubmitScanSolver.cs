using System.Collections;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(MedScanMinigame))]

public sealed class SubmitScanSolver : GeneralMinigameSolver<MedScanMinigame>
{
    public override IEnumerator CompleteMinigame(MedScanMinigame minigame, NormalPlayerTask task)
    {
        yield return new WaitForSeconds(minigame.ScanDuration);
    }
}

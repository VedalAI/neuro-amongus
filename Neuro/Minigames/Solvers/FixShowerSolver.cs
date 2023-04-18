using System;
using System.Collections;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(FixShowerMinigame))]
public sealed class FixShowerSolver : GeneralMinigameSolver<FixShowerMinigame>
{
    public override IEnumerator CompleteMinigame(FixShowerMinigame minigame, NormalPlayerTask task)
    {
        throw new NotImplementedException();
    }
}

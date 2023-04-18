﻿using System.Collections;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(ShowerMinigame))]
public class DecontaminateSolver : GeneralMinigameSolver<ShowerMinigame>
{
    public override IEnumerator CompleteMinigame(ShowerMinigame minigame, NormalPlayerTask task)
    {
        yield break;
    }
}
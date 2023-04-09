﻿using System.Collections;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(ShowerMinigame))]
public class DecontaminateSolver : MinigameSolver<ShowerMinigame>
{
    protected override IEnumerator CompleteMinigame(ShowerMinigame minigame, NormalPlayerTask task)
    {
        yield break;
    }
}
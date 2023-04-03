using System.Collections;
using UnityEngine;

namespace Neuro.Minigames;

public abstract class MinigameSolver
{
    // The larger this constant is, the longer the delays are when solving minigames.
    private const float DELAY_MULTIPLIER = 1;

    public abstract IEnumerator CompleteMinigame(Minigame minigame, PlayerTask task);

    protected static WaitForSeconds Sleep(float seconds) => new(seconds * DELAY_MULTIPLIER);
}

public abstract class MinigameSolver<TMinigame> : MinigameSolver where TMinigame : Minigame
{
    public sealed override IEnumerator CompleteMinigame(Minigame minigame, PlayerTask task)
        => CompleteMinigame(minigame.TryCast<TMinigame>(), task.TryCast<NormalPlayerTask>());

    protected abstract IEnumerator CompleteMinigame(TMinigame minigame, NormalPlayerTask task);
}

public abstract class TasklessMinigameSolver<TMinigame> : MinigameSolver where TMinigame : Minigame
{
    public sealed override IEnumerator CompleteMinigame(Minigame minigame, PlayerTask _)
        => CompleteMinigame(minigame.TryCast<TMinigame>());

    protected abstract IEnumerator CompleteMinigame(TMinigame minigame);
}

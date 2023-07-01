using System.Collections;

namespace Neuro.Minigames;

public interface IMinigameSolver
{
    float CloseTimout { get; }

    IEnumerator CompleteMinigame(Minigame minigame, PlayerTask task);
}

public interface IMinigameSolver<in TMinigame> : IMinigameSolver where TMinigame : Minigame
{
    IEnumerator CompleteMinigame(TMinigame minigame);

    IEnumerator IMinigameSolver.CompleteMinigame(Minigame minigame, PlayerTask task) => CompleteMinigame(minigame.Cast<TMinigame>());
}

public interface IMinigameSolver<in TMinigame, in TTask> : IMinigameSolver where TMinigame : Minigame where TTask : PlayerTask
{
    IEnumerator CompleteMinigame(TMinigame minigame, TTask task);

    IEnumerator IMinigameSolver.CompleteMinigame(Minigame minigame, PlayerTask task) => CompleteMinigame(minigame.Cast<TMinigame>(), task!?.Cast<TTask>());
}
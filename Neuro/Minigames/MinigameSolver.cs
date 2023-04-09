using System.Collections;

namespace Neuro.Minigames;

public abstract class MinigameSolver
{
    public abstract IEnumerator CompleteMinigame(Minigame minigame, PlayerTask task);

    public virtual bool CanUseConsole(Console console, PlayerTask task) => true;
}

public abstract class MinigameSolver<TMinigame> : MinigameSolver where TMinigame : Minigame
{
    public sealed override IEnumerator CompleteMinigame(Minigame minigame, PlayerTask task)
        => CompleteMinigame(minigame.Cast<TMinigame>());

    public sealed override bool CanUseConsole(Console console, PlayerTask task)
        => true;

    protected abstract IEnumerator CompleteMinigame(TMinigame minigame);
}

public abstract class TaskMinigameSolver<TMinigame> : MinigameSolver where TMinigame : Minigame
{
    public sealed override IEnumerator CompleteMinigame(Minigame minigame, PlayerTask task)
        => CompleteMinigame(minigame.Cast<TMinigame>(), task.Cast<NormalPlayerTask>());

    public sealed override bool CanUseConsole(Console console, PlayerTask task)
        => CanUseConsole(console, task.Cast<NormalPlayerTask>());

    protected abstract IEnumerator CompleteMinigame(TMinigame minigame, NormalPlayerTask task);

    protected virtual bool CanUseConsole(Console console, NormalPlayerTask task) => true;
}

public abstract class SabotageMinigameSolver<TMinigame> : MinigameSolver where TMinigame : Minigame
{
    public sealed override IEnumerator CompleteMinigame(Minigame minigame, PlayerTask task)
        => CompleteMinigame(minigame.Cast<TMinigame>(), task.Cast<SabotageTask>());

    public sealed override bool CanUseConsole(Console console, PlayerTask task)
        => CanUseConsole(console, task.Cast<SabotageTask>());

    protected abstract IEnumerator CompleteMinigame(TMinigame minigame, SabotageTask task);

    protected virtual bool CanUseConsole(Console console, SabotageTask task) => true;
}

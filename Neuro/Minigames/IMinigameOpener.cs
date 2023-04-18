namespace Neuro.Minigames;

public interface IMinigameOpener
{
    bool ShouldOpenConsole(Console console, Minigame minigame, PlayerTask task);
}

public interface IMinigameOpener<in TTask> : IMinigameOpener where TTask : PlayerTask
{
    bool ShouldOpenConsole(Console console, TTask task);

    bool IMinigameOpener.ShouldOpenConsole(Console console, Minigame minigame, PlayerTask task) => ShouldOpenConsole(console, task.Cast<TTask>());
}

public interface IMinigameOpener<in TMinigame, in TTask> : IMinigameOpener where TMinigame : Minigame where TTask : PlayerTask
{
    bool ShouldOpenConsole(Console console, TMinigame minigame, TTask task);

    bool IMinigameOpener.ShouldOpenConsole(Console console, Minigame minigame, PlayerTask task) => ShouldOpenConsole(console, minigame.Cast<TMinigame>(), task.Cast<TTask>());
}

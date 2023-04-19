namespace Neuro.Minigames;

public interface IMinigameOpener
{
    bool ShouldOpenConsole(Console console, PlayerTask task);
}

public interface IMinigameOpener<in TTask> : IMinigameOpener where TTask : PlayerTask
{
    bool ShouldOpenConsole(Console console, TTask task);

    bool IMinigameOpener.ShouldOpenConsole(Console console, PlayerTask task) => ShouldOpenConsole(console, task.Cast<TTask>());
}

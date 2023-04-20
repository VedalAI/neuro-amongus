namespace Neuro.Minigames.Solvers;

[MinigameOpener(typeof(ShowerMinigame))]
public sealed class DecontaminateSolver : IMinigameOpener
{
    public bool ShouldOpenConsole(Console console, PlayerTask task) => true;
}

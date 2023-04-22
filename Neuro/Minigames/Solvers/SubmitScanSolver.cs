namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(MedScanMinigame))]
public sealed class SubmitScanSolver : IMinigameOpener
{
    public bool ShouldOpenConsole(Console console, PlayerTask task) => true;
}

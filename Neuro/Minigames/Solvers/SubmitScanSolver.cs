namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(MedScanMinigame))]

public sealed class SubmitScanSolver : IMinigameOpener<NormalPlayerTask>
{
    public bool ShouldOpenConsole(Console console, NormalPlayerTask task) => true;
}

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(MedScanMinigame))]

public sealed class SubmitScanSolver : IMinigameOpener<MedScanMinigame, NormalPlayerTask>
{
    public bool ShouldOpenConsole(Console console, MedScanMinigame minigame, NormalPlayerTask task) => true;
}

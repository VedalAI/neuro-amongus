using System.Collections;

namespace Neuro.Minigames;

public abstract class GeneralMinigameSolver<TMinigame> : IMinigameSolver<TMinigame, NormalPlayerTask>, IMinigameOpener
    where TMinigame : Minigame
{
    public abstract float CloseTimout { get; }
    public abstract IEnumerator CompleteMinigame(TMinigame minigame, NormalPlayerTask task);
    public bool ShouldOpenConsole(Console console, PlayerTask task) => true;
}

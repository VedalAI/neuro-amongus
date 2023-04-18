using System.Collections;

namespace Neuro.Minigames;

public abstract class GeneralMinigameSolver<TMinigame> : IMinigameSolver<TMinigame, NormalPlayerTask>, IMinigameOpener<TMinigame, NormalPlayerTask>
    where TMinigame : Minigame
{
    public virtual bool ShouldOpenConsole(Console console, TMinigame minigame, NormalPlayerTask task) => true;
    public abstract IEnumerator CompleteMinigame(TMinigame minigame, NormalPlayerTask task);
}

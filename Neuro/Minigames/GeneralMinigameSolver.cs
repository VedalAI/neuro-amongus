using System.Collections;

namespace Neuro.Minigames;

public abstract class GeneralMinigameSolver<TMinigame> : IMinigameSolver<TMinigame, NormalPlayerTask>, IMinigameOpener<TMinigame, NormalPlayerTask>
    where TMinigame : Minigame
{
    public abstract IEnumerator CompleteMinigame(TMinigame minigame, NormalPlayerTask task);
    public bool ShouldOpenConsole(Console console, TMinigame minigame, NormalPlayerTask task)
    {
        return (task.TimerStarted != NormalPlayerTask.TimerState.Started) && !task.IsComplete;
    }
}

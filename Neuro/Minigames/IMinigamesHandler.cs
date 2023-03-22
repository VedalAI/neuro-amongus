using System.Collections;
using Neuro.DependencyInjection;

namespace Neuro.Minigames;

public interface IMinigamesHandler : IContextAccepter
{
    public IEnumerator CompleteMinigame(PlayerTask task, Minigame minigame);
}

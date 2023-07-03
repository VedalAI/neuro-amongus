using System.Collections.Generic;
using System.Linq;
using Neuro.Minigames;

namespace Neuro.Movement;

public static class ConsoleFinder
{
    public static List<Console> GetOpenableConsoles(bool includeSabotages)
    {
        return PlayerControl.LocalPlayer.myTasks._items.Where(t => t && !t.IsComplete && !t.TryCast<ImportantTextTask>())
            .Where(t => includeSabotages || !PlayerTask.TaskIsEmergency(t))
            .SelectMany(t => t.FindConsoles()._items.Where(c => c).Select(c => (t, c)))
            .Where(e => MinigameHandler.ShouldOpenConsole(e.c, e.t.MinigamePrefab, e.t))
            .Select(e => e.c).ToList();
    }
}

using System.Linq;
using Neuro.Minigames;
using Neuro.Pathfinding;
using Neuro.Recording.Common;

namespace Neuro.Recording.Tasks;

public partial class TaskData
{
    public static TaskData Create(PlayerTask task)
    {
        TaskData data = new()
        {
            Id = task.Id,
            Type = task.TaskType.ForMessage(),
        };

        bool minigameHasNoSolver = !MinigameOpenerAttribute.MinigameOpeners.TryGetValue(task.MinigamePrefab.GetIl2CppType().FullName, out _);
        foreach (Console consoleOfInterest in task.FindConsoles()._items
                     .Where(c => c && (minigameHasNoSolver || MinigameHandler.ShouldOpenConsole(c, task.MinigamePrefab, task)))
                     .OrderBy(Closest).Take(2))
        {
            data.ConsolesOfInterest.Add(PositionData.Create(consoleOfInterest, consoleOfInterest));
        }

        return data;
    }

    private static float Closest(Console console)
    {
        return PathfindingHandler.Instance.GetPathLength(PlayerControl.LocalPlayer, console, console);
    }
}

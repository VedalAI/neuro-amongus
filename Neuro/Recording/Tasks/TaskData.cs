using System;
using System.Linq;
using Neuro.Minigames;
using Neuro.Pathfinding;
using static Neuro.Recording.Tasks.TaskData.Types;

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

        foreach (Console consoleOfInterest in task.FindConsoles()._items.Where(c => c && MinigameHandler.ShouldOpenConsole(c, task.MinigamePrefab, task)).OrderBy(Closest).Take(2))
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

public static class TaskDataExtensions
{
    public static TaskType ForMessage(this TaskTypes taskType)
    {
        return Enum.TryParse(taskType.ToString(), out TaskType result) ? result : TaskType.Unset;
    }
}

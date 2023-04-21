using System.Linq;
using Neuro.Minigames;
using Neuro.Pathfinding;
using Neuro.Recording.Common;
using Neuro.Utilities;
using UnityEngine;
using Random = System.Random;
using Vector2 = UnityEngine.Vector2;

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
            data.ConsolesOfInterest.Add(PositionData.Create(consoleOfInterest));

            // TODO: Move this thing out of here
            Vector2[] path = PathfindingHandler.Instance.GetPath(consoleOfInterest);
            DrawPath(path, consoleOfInterest.GetInstanceID());
        }

        return data;
    }

    private static void DrawPath(Vector2[] path, int id)
    {
        GameObject.Destroy(GameObject.Find("Neuro Path " + id));
        GameObject test = new("Neuro Path " + id);
        //Info(test.transform);
        test.transform.position = PlayerControl.LocalPlayer.transform.position;

        LineRenderer renderer = test.AddComponent<LineRenderer>();
        renderer.positionCount = path.Length;
        for (int i = 0; i < path.Length; i++)
        {
            renderer.SetPosition(i, path[i]);
        }

        renderer.material = NeuroUtilities.MaskShaderMat;
        renderer.widthMultiplier = 0.2f;

        Random random = new(id);
        renderer.startColor = new Color(random.NextSingle(), random.NextSingle(), random.NextSingle());
    }

    private static float Closest(Console console)
    {
        return PathfindingHandler.Instance.GetPathLength(console);
    }
}

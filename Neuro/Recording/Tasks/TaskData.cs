using System.Collections;
using System.Diagnostics;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils;
using Neuro.Gizmos;
using Neuro.Minigames;
using Neuro.Pathfinding;
using Neuro.Recording.Common;
using Neuro.Utilities;
using Reactor.Utilities.Extensions;
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

        // TODO: minigameHasNoSolver is only used for recording training data for tasks with no solvers, we need to remove that once all tasks have solvers
        bool minigameHasNoSolver = !MinigameOpenerAttribute.MinigameOpeners.TryGetValue(task.MinigamePrefab.GetIl2CppType().FullName, out _);
        foreach (Console consoleOfInterest in task.FindConsoles()._items
                     .Where(c => c && (minigameHasNoSolver || MinigameHandler.ShouldOpenConsole(c, task.MinigamePrefab, task)))
                     .OrderBy(Closest).Take(2))
        {
            data.ConsolesOfInterest.Add(PositionData.Create(consoleOfInterest));

            if (!GizmosDebugTab.EnableTaskPaths) continue;

            // TODO: Move this thing out of here
            Vector2[] path = PathfindingHandler.Instance.GetPath(consoleOfInterest, false);
            DrawPath(path, consoleOfInterest.GetInstanceID());
        }

        return data;
    }

    // TODO: Fix path Z index
    [Conditional("FULL")]
    private static void DrawPath(Vector2[] path, int id)
    {
        GameObject.Destroy(GameObject.Find("Neuro Path " + id));
        GameObject pathObj = new("Neuro Path " + id)
        {
            transform =
            {
                position = PlayerControl.LocalPlayer.transform.position
            }
        };

        LineRenderer renderer = pathObj.AddComponent<LineRenderer>();
        renderer.positionCount = path.Length;
        for (int i = 0; i < path.Length; i++)
        {
            renderer.SetPosition(i, new Vector3(path[i].x, path[i].y, path[i].y / 1000f + 0.0005f));
        }

        renderer.material = NeuroUtilities.MaskShaderMat;
        renderer.widthMultiplier = 0.2f;

        Random random = new(id);
        renderer.startColor = new Color(random.NextSingle(), random.NextSingle(), random.NextSingle());

        pathObj.AddComponent<DivertPowerMetagame>().StartCoroutine(DestroyAfter(pathObj, 5));
    }

    private static float Closest(Console console)
    {
        return PathfindingHandler.Instance.GetPathLength(console);
    }

    private static IEnumerator DestroyAfter(GameObject obj, float timeoutSeconds)
    {
        yield return new WaitForSeconds(timeoutSeconds);
        obj.Destroy();
    }
}

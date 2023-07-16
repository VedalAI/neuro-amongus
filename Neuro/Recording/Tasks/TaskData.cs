using System.Collections;
using System.Diagnostics;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils;
using Neuro.Caching;
using Neuro.Debugging.Tabs;
using Neuro.Minigames;
using Neuro.Pathfinding;
using Neuro.Recording.Common;
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

        foreach (Console consoleOfInterest in task.FindConsoles()._items
                     .Where(c => c && MinigameHandler.ShouldOpenConsole(c, task.MinigamePrefab, task))
                     .OrderBy(Closest).Take(2))
        {
            data.ConsolesOfInterest.Add(PositionData.Create(consoleOfInterest));

            if (GizmosDebugTab.EnableTaskPaths) DrawPath(PathfindingHandler.Instance.GetPath(consoleOfInterest, false), consoleOfInterest.GetInstanceID());
        }

        return data;
    }

    public static TaskData CreateFake(Component component)
    {
        TaskData data = new();
        data.ConsolesOfInterest.Add(PositionData.Create(component));

        if (GizmosDebugTab.EnableTaskPaths) DrawPath(PathfindingHandler.Instance.GetPath(component, false), component.GetInstanceID());

        return data;
    }

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

        renderer.material = UnityCache.MaskShaderMat;
        renderer.widthMultiplier = 0.2f;

        Random random = new(id);
        renderer.startColor = new Color(random.NextSingle(), random.NextSingle(), random.NextSingle());

        pathObj.AddComponent<DivertPowerMetagame>().StartCoroutine(DestroyAfter(pathObj, 5));
    }

    private static float Closest(Console console)
    {
        return PathfindingHandler.Instance.GetPathLength(console);
    }

    private static IEnumerator DestroyAfter(GameObject obj, int timeoutSeconds)
    {
        for (int i = 0; i < timeoutSeconds; i++)
        {
            if (!GizmosDebugTab.EnableTaskPaths) break;
            yield return new WaitForSeconds(1);
        }

        obj.Destroy();
    }
}

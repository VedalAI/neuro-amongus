using System;
using System.Collections;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Tasks;

[RegisterInIl2Cpp]
public class TasksHandler : MonoBehaviour
{
    public TasksHandler(IntPtr ptr) : base(ptr) { }

    // TODO: Move this to movement class
    [HideFromIl2Cpp]
    public Vector2[] CurrentPath { get; set; } = Array.Empty<Vector2>();
    public int PathIndex { get; set; } = -1;

    [HideFromIl2Cpp]
    public void UpdatePathToTask(PlayerTask task = null)
    {
        if (!task) task = PlayerControl.LocalPlayer.myTasks.At(0);

        PlayerTask nextTask = null;
        if (task.IsComplete)
        {
            Info("Task is complete, getting next one.");
            PlayerTask closestTask = null;
            float closestDistance = Mathf.Infinity;

            foreach (PlayerTask t in PlayerControl.LocalPlayer.myTasks)
            {
                if (!t.IsComplete && t.HasLocation)
                {
                    Vector2[] path = NeuroPlugin.Instance.Pathfinding.FindPath(PlayerControl.LocalPlayer.transform.position, t.Locations.At(0));
                    // Evaluate length of path
                    float distance = 0f;
                    for (int i = 0; i < path.Length - 1; i++)
                    {
                        distance += Vector2.Distance(path[i], path[i + 1]);
                    }

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTask = t;
                    }
                }
            }

            if (closestTask == null)
            {
                PathIndex = -1;
                Destroy(GameObject.Find("Arrow")); // TODO: Cache this object or something
                return;
            }

            nextTask = closestTask;
        }
        else
        {
            nextTask = task;
        }

        if (nextTask != null)
        {
            // Info("Next task isn't null");
            CurrentPath = NeuroPlugin.Instance.Pathfinding.FindPath(PlayerControl.LocalPlayer.transform.position, nextTask.Locations.At(0));
            PathIndex = 0;

            //pathfinding.DrawPath(currentPath);
        }
    }

    [HideFromIl2Cpp]
    public IEnumerator UpdatePathToFirstTask(NormalPlayerTask initial)
    {
        CurrentPath = NeuroPlugin.Instance.Pathfinding.FindPath(PlayerControl.LocalPlayer.transform.position, initial.Locations.At(0));
        PathIndex = 0;

        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            UpdatePathToTask();

            // TODO: Is this while loop purposefully infinite? If so, we should have a stop condition for example when the game ends.
        }
    }

    private void Update()
    {
        if (!ShipStatus.Instance) return;
        if (Minigame.Instance) return;
        if (!PlayerControl.LocalPlayer) return;

        foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
        {
            if (task.IsComplete || task.Locations == null) continue;

            // TODO: Invoke Console.Use directly
            foreach (Vector2 location in task.Locations)
            {
                if (Vector2.Distance(location, PlayerControl.LocalPlayer.transform.position) < 0.8f)
                {
                    if (task.MinigamePrefab)
                    {
                        Minigame minigame = Instantiate(task.GetMinigamePrefab(), Camera.main!.transform, false);
                        minigame.transform.localPosition = new Vector3(0f, 0f, -50f);
                        // minigame.Console = FindObjectOfType<Console>(); // TODO: What the fuck is this
                        minigame.Begin(task);
                    }
                }
            }
        }
    }
}

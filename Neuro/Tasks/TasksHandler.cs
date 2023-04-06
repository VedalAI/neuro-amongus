using System;
using System.Collections;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Tasks;

[RegisterInIl2Cpp]
public sealed class TasksHandler : MonoBehaviour
{
    public TasksHandler(IntPtr ptr) : base(ptr) { }

    // TODO: Move this to movement class
    [HideFromIl2Cpp]
    public Vector2[] CurrentPath { get; set; } = Array.Empty<Vector2>();
    public int PathIndex { get; set; } = -1;

    [HideFromIl2Cpp]
    public void UpdatePathToTask(PlayerTask task = null)
    {
        if (!task)
        {
            // skip fake tasks that cannot be completed (for instance, the red text as impostor)
            foreach (PlayerTask t in PlayerControl.LocalPlayer.myTasks)
            {
                if (!t.TryCast<NormalPlayerTask>()) continue;
                task = t;
                break;
            }
        }

        PlayerTask nextTask;
        if (task.IsComplete)
        {
            Info("Task is complete, getting next one.");
            PlayerTask targetTask;

            // as impostor, getting the furthest task instead of the closest one is good for finding players
            // also helps trying to do a task right next to someone we killed
            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
            {
                targetTask = GetFurthestTask();
            }
            else
            {
                targetTask = GetClosestTask();
            }

            if (targetTask == null)
            {
                PathIndex = -1;
                Destroy(NeuroPlugin.Instance.Movement.Arrow);
                return;
            }

            nextTask = targetTask;
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
    public PlayerTask GetClosestTask()
    {
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

        return closestTask;
    }

    [HideFromIl2Cpp]
    public PlayerTask GetFurthestTask()
    {
        PlayerTask furthestTask = null;
        float furthestDistance = 0f;

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

                if (distance > furthestDistance)
                {
                    furthestDistance = distance;
                    furthestTask = t;
                }
            }
        }

        return furthestTask;
    }

    // TODO: Find a better place for this
    [HideFromIl2Cpp]
    public void UpdatePathToKillTarget()
    {
        NeuroPlugin.Instance.Pathfinding.FindPath(PlayerControl.LocalPlayer.transform.position, NeuroPlugin.Instance.Impostor.killTarget.transform.position);
        PathIndex = 0;
    }

    // TODO: Find a better place for this
    [HideFromIl2Cpp]
    public void UpdatePathToVent()
    {
        NeuroPlugin.Instance.Pathfinding.FindPath(PlayerControl.LocalPlayer.transform.position, NeuroPlugin.Instance.Impostor.ClosestVent.transform.position);
        PathIndex = 0;
    }

    [HideFromIl2Cpp]
    public IEnumerator UpdatePathToFirstTask(NormalPlayerTask initial)
    {
        CurrentPath = NeuroPlugin.Instance.Pathfinding.FindPath(PlayerControl.LocalPlayer.transform.position, initial.Locations.At(0));
        PathIndex = 0;

        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            // TODO: Possibly run multiple coroutines for each handler that deals with pathfinding. Would need a way to block each other's when necessary.
            if (NeuroPlugin.Instance.Impostor.attemptingVent)
                UpdatePathToVent();
            else if (NeuroPlugin.Instance.Impostor.killTarget)
                UpdatePathToKillTarget();
            else
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

            // TODO: HOLY FUCK DON'T UNCOMMENT THIS THIS IS BROKEN AF
            // INVOKE CONSOLE.USE DIRECTLY PLEEEEEEASE

            /*// TODO: Invoke Console.Use directly and check if we can open it before trying
            foreach (Vector2 location in task.Locations)
            {
                if (Vector2.Distance(location, PlayerControl.LocalPlayer.transform.position) < 0.8f)
                {
                    // TODO: Check if we should open the task with the MinigameSolver before actually opening it
                    if (task.MinigamePrefab)
                    {
                        Minigame minigame = Instantiate(task.GetMinigamePrefab(), Camera.main!.transform, false);
                        minigame.transform.localPosition = new Vector3(0f, 0f, -50f);
                        // minigame.Console = FindObjectOfType<Console>(); // TODO: What the fuck is this
                        minigame.Begin(task);
                    }
                }
            }*/
        }
    }
}

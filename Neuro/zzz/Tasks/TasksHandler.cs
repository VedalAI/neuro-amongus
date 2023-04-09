using System;
using System.Collections;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Pathfinding;
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
            CurrentPath = PathfindingHandler.Instance.FindPath(PlayerControl.LocalPlayer.transform.position, nextTask.Locations.At(0));
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
                Vector2[] path = PathfindingHandler.Instance.FindPath(PlayerControl.LocalPlayer.transform.position, t.Locations.At(0));
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
                Vector2[] path = PathfindingHandler.Instance.FindPath(PlayerControl.LocalPlayer.transform.position, t.Locations.At(0));
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
        PathfindingHandler.Instance.FindPath(PlayerControl.LocalPlayer.transform.position, NeuroPlugin.Instance.Impostor.killTarget.transform.position);
        PathIndex = 0;
    }

    // TODO: Find a better place for this
    [HideFromIl2Cpp]
    public void UpdatePathToVent()
    {
        PathfindingHandler.Instance.FindPath(PlayerControl.LocalPlayer.transform.position, NeuroPlugin.Instance.Impostor.ClosestVent.transform.position);
        PathIndex = 0;
    }

    [HideFromIl2Cpp]
    public IEnumerator UpdatePathToFirstTask(NormalPlayerTask initial)
    {
        CurrentPath = PathfindingHandler.Instance.FindPath(PlayerControl.LocalPlayer.transform.position, initial.Locations.At(0));
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

        //Open anything nearby, speedrun.
        foreach (var tasky in PlayerControl.LocalPlayer.myTasks)
        {
            if (tasky.IsComplete || tasky.Locations == null) continue;

            foreach (Console con in tasky.FindConsoles()) {
                var currentDistance = Vector2.Distance(con.transform.position, PlayerControl.LocalPlayer.transform.position);

                if (currentDistance < con.usableDistance)
                {
                    // Info("We found " + con.name + " with distance " + con.usableDistance + " " + currentDistance);
                    con.CanUse(PlayerControl.LocalPlayer.Data, out bool canUse,out _);
                    if (canUse)
                    {
                        con.Use();
                        Info("Using task console: " + con.name);
                    }
                }
            }
        }
    }
}

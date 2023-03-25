using System;
using System.Collections;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Tasks;

[RegisterInIl2Cpp]
public class TasksHandler : MonoBehaviour
{
    public TasksHandler(IntPtr ptr) : base(ptr) { }

    // TODO: Move this to movement class
    public Vector2[] CurrentPath { get; set; } = Array.Empty<Vector2>();
    public int PathIndex { get; set; } = -1;

    public IEnumerator EvaluatePath(NormalPlayerTask initial)
    {
        CurrentPath = NeuroPlugin.Instance.PathfindingHandler.FindPath(PlayerControl.LocalPlayer.transform.position, initial.Locations.At(0));
        PathIndex = 0;

        while (true)
        {
            yield return new WaitForSeconds(1);

            PlayerTask task = PlayerControl.LocalPlayer.myTasks.At(0);

            // TODO: Get the nearest location from all tasks instead of the first location of the next task
            PlayerTask nextTask = null;
            if (task.IsComplete)
            {
                Info("Task is complete");
                foreach (PlayerTask t in PlayerControl.LocalPlayer.myTasks)
                {
                    if (!t.IsComplete && t.HasLocation)
                    {
                        nextTask = t;
                        Info(nextTask.name);
                        break;
                    }
                }
            }
            else
            {
                nextTask = task;
            }

            if (nextTask != null)
            {
                Info("Next task isn't null");
                CurrentPath = NeuroPlugin.Instance.PathfindingHandler.FindPath(PlayerControl.LocalPlayer.transform.position, nextTask.Locations.At(0));
                PathIndex = 0;

                //pathfinding.DrawPath(currentPath);
            }
        }
    }

    private void Update()
    {
        if (Minigame.Instance) return;

        foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
        {
            if (task.Locations == null || task.IsComplete) continue;

            // TODO: Invoke Console.Use directly
            foreach (Vector2 location in task.Locations)
            {
                if (Vector2.Distance(location, PlayerControl.LocalPlayer.transform.position) < 0.8f)
                {
                    if (task.MinigamePrefab)
                    {
                        Minigame minigame = Instantiate(task.GetMinigamePrefab(), Camera.main!.transform, false);
                        minigame.transform.localPosition = new Vector3(0f, 0f, -50f);
                        minigame.Console = FindObjectOfType<Console>(); // TODO: What the fuck is this
                        minigame.Begin(task);
                    }
                }
            }
        }
    }
}

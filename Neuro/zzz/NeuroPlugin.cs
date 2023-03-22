using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Neuro.Utils;
using Reactor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Neuro;

// [BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public class NeuroPlugind : BasePlugin
{
    public LineRenderer arrow;

    public Vector2[] currentPath = Array.Empty<Vector2>();

    public Vector2 directionToNearestTask;
    public Vector2 moveDirection;

    public Pathfinding pathfinding = new();
    public int pathIndex = -1;
    public Recorder recorder = new();

    public Vision vision = new();

    public override void Load()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), Id);
    }

    public void StartMap(ShipStatus shipStatus)
    {
        Debug.Log("OnShipLoad");
        pathfinding.GenerateNodeGrid();

        pathfinding.FloodFill(shipStatus.MeetingSpawnCenter + Vector2.up * shipStatus.SpawnRadius + new Vector2(0f, 0.3636f));

        GameObject arrowGO = new("Arrow");
        arrow = arrowGO.AddComponent<LineRenderer>();
    }

    public void FixedUpdate()
    {
        // TODO: This method should be split into multiple MonoBehaviours - one for vision, one for recording and one for doing tasks

        if (MeetingHud.Instance) return;

        vision.UpdateVision();

        foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
        {
            if (Minigame.Instance) break;

            if (task == null || task.Locations == null) continue;
            if (task.IsComplete) continue;
            foreach (Vector2 location in task.Locations)
                if (Vector2.Distance(location, PlayerControl.LocalPlayer.transform.position) < 0.8f)
                    if (task.MinigamePrefab)
                    {
                        Minigame minigame = Object.Instantiate(task.GetMinigamePrefab(), Camera.main!.transform, false);
                        minigame.transform.localPosition = new Vector3(0f, 0f, -50f);
                        minigame.Console = Object.FindObjectOfType<Console>();
                        minigame.Begin(task);
                    }
        }

        // Record values
        Frame frame = new(
            PlayerControl.LocalPlayer.Data.Role.IsImpostor,
            PlayerControl.LocalPlayer.killTimer,
            directionToNearestTask,
            PlayerControl.LocalPlayer.myTasks.ToArray().Any(PlayerTask.TaskIsEmergency),
            Vector2.zero,
            vision.directionToNearestBody,
            GameManager.Instance.CanReportBodies() && HudManager.Instance.ReportButton.gameObject.activeInHierarchy,
            new List<PlayerRecord>(),
            moveDirection,
            false,
            false,
            false,
            false,
            false
        );
        string frameString = JsonSerializer.Serialize(frame);
        Debug.Log(frameString);
        recorder.Frames.Add(frame);
    }

    public bool MovePlayer(ref Vector2 direction)
    {
        // TODO: Move to separate MonoBehaviour for handling movement

        moveDirection = direction;
        if (currentPath.Length > 0 && pathIndex != -1)
        {
            Vector2 nextWaypoint = currentPath[pathIndex];

            while (Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), nextWaypoint) < 0.75f)
            {
                pathIndex++;
                if (pathIndex > currentPath.Length - 1)
                {
                    pathIndex = currentPath.Length - 1;
                    nextWaypoint = currentPath[pathIndex];
                    break;
                }

                nextWaypoint = currentPath[pathIndex];
            }

            directionToNearestTask = (nextWaypoint - PlayerControl.LocalPlayer.GetTruePosition()).normalized;


            LineRenderer renderer = arrow;
            renderer.SetPosition(0, PlayerControl.LocalPlayer.GetTruePosition());
            renderer.SetPosition(1, PlayerControl.LocalPlayer.GetTruePosition() + directionToNearestTask);
            renderer.widthMultiplier = 0.1f;
            renderer.positionCount = 2;
            renderer.startColor = Color.red;
        }
        else
        {
            directionToNearestTask = Vector2.zero;
        }

        return true;
    }

    public void MeetingEnd()
    {
        // TODO: Use events to invoke vision.MeetingEnd or invoke it directly from the patch, don't route through NeuroPlugin
        Debug.Log("NEURO: MEETING IS FINISHED");
        vision.MeetingEnd();
    }

    public IEnumerator EvaluatePath(NormalPlayerTask initial)
    {
        // TODO: Move to a separate MonoBehaviour for handling tasks

        currentPath = pathfinding.FindPath(PlayerControl.LocalPlayer.transform.position, initial.Locations.At(0));
        pathIndex = 0;

        while (true)
        {
            yield return new WaitForSeconds(1);

            PlayerTask task = PlayerControl.LocalPlayer.myTasks.At(0);

            // TODO: Get the nearest location from all tasks instead of the first location of the next task
            PlayerTask nextTask = null;
            if (task.IsComplete)
            {
                Debug.Log("Task is complete");
                foreach (PlayerTask t in PlayerControl.LocalPlayer.myTasks)
                    if (!t.IsComplete && t.HasLocation)
                    {
                        nextTask = t;
                        Debug.Log(nextTask.name);
                        break;
                    }
            }
            else
            {
                nextTask = task;
            }

            if (nextTask != null)
            {
                Debug.Log("Next task isn't null");
                currentPath = pathfinding.FindPath(PlayerControl.LocalPlayer.transform.position, nextTask.Locations.At(0));
                pathIndex = 0;

                //pathfinding.DrawPath(currentPath);
            }
        }
    }
}

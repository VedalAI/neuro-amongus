using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Text.Json;

namespace Neuro;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class NeuroPlugin : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);

    public ConfigEntry<string> ConfigName { get; private set; }

    public Recorder recorder = new Recorder();

    public Vision vision = new Vision();

    public bool inMinigame = false;

    public bool hasStarted = false;

    public Pathfinding pathfinding = new Pathfinding();

    public Vector2 directionToNearestTask = new Vector2();
    public Vector2 moveDirection = new Vector2();

    public Vector2[] currentPath = new Vector2[0];
    public int pathIndex = -1;

    public LineRenderer arrow;

    public List<PlayerTask> tasks = new List<PlayerTask>();

    public override void Load()
    {
        ConfigName = Config.Bind("Neuro", "Name", "Neuro-sama");

        Harmony.PatchAll();
    }

    public void StartMap(ShipStatus shipStatus)
    {
        Debug.Log("OnShipLoad");
        pathfinding.GenerateNodeGrid();

        pathfinding.FloodFill(shipStatus.MeetingSpawnCenter + (Vector2.up * shipStatus.SpawnRadius) + new Vector2(0f, 0.3636f));

        GameObject arrowGO = new GameObject("Arrow");
        arrow = arrowGO.AddComponent<LineRenderer>();
    }

    public void FixedUpdate(PlayerControl localPlayer)
    {
        if (MeetingHud.Instance != null && MeetingHud.Instance.enabled) return;

        vision.UpdateVision();

        if (localPlayer.myTasks != null)
        {
            foreach (PlayerTask task in localPlayer.myTasks)
            {
                if (task == null || task.Locations == null) continue;
                if (task.IsComplete || inMinigame) continue;
                foreach (Vector2 location in task.Locations)
                {
                    if (Vector2.Distance(location, PlayerControl.LocalPlayer.transform.position) < 0.8f)
                    {
                        if (task.MinigamePrefab)
                        {
                            var minigame = GameObject.Instantiate(task.GetMinigamePrefab());
                            minigame.transform.SetParent(Camera.main.transform, false);
                            minigame.transform.localPosition = new Vector3(0f, 0f, -50f);
                            minigame.Begin(task);
                            inMinigame = true;
                        }
                    }
                }
            }
        }

        bool sabotageActive = false;
        foreach (PlayerTask task in localPlayer.myTasks)
            if (task.TaskType == TaskTypes.FixLights || task.TaskType == TaskTypes.RestoreOxy || task.TaskType == TaskTypes.ResetReactor || task.TaskType == TaskTypes.ResetSeismic || task.TaskType == TaskTypes.FixComms)
                sabotageActive = true;

        List<PlayerRecord> playerRecords = new List<PlayerRecord>();


        // Record values
        Frame frame = new Frame(
            localPlayer.Data.RoleType == AmongUs.GameOptions.RoleTypes.Impostor,
            localPlayer.killTimer,
            directionToNearestTask,
            sabotageActive,
            Vector2.zero,
            vision.directionToNearestBody,
            GameManager.Instance.CanReportBodies() && HudManager.Instance.ReportButton.gameObject.activeInHierarchy,
            playerRecords,
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

            directionToNearestTask = (nextWaypoint - (Vector2)PlayerControl.LocalPlayer.GetTruePosition()).normalized;


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

    public void MeetingBegin()
    {
        Debug.Log("NEURO: MEETING CALLED");
        vision.ReportFindings();
    }

    public void MeetingEnd()
    {
        Debug.Log("NEURO: MEETING IS FINISHED");
        vision.MeetingEnd();
    }
    public IEnumerator EvaluatePath(NormalPlayerTask initial)
    {
        currentPath = pathfinding.FindPath(PlayerControl.LocalPlayer.transform.position, initial.Locations[0]);
        pathIndex = 0;

        while (true)
        {
            yield return new WaitForSeconds(1);

            PlayerTask task = PlayerControl.LocalPlayer.myTasks[0];

            PlayerTask nextTask = null;
            if (task.IsComplete)
            {
                Debug.Log("Task is complete");
                foreach (PlayerTask t in PlayerControl.LocalPlayer.myTasks)
                {
                    if (!t.IsComplete && t.HasLocation)
                    {
                        nextTask = t;
                        Debug.Log(nextTask.name);
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
                Debug.Log("Next task isn't null");
                currentPath = pathfinding.FindPath(PlayerControl.LocalPlayer.transform.position, nextTask.Locations[0]);
                pathIndex = 0;

                //pathfinding.DrawPath(currentPath);
            }
        }
    }
}

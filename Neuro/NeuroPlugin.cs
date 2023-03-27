using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Utils;
using HarmonyLib;
using Reactor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

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

    public List<Vent> vents = new List<Vent>();

    public bool isImpostor = false;
    public bool goingForKill = false;
    public PlayerControl killTarget = null;
    public int ventCooldown = 0;

    public bool didKill = false;
    public bool didReport = false;
    public bool didVent = false;

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

        GameObject arrowGo = new GameObject("Arrow");
        arrow = arrowGo.AddComponent<LineRenderer>();
        arrow.startWidth = 0.4f;
        arrow.endWidth = 0.05f;
        arrow.positionCount = 2;
        arrow.material = new Material(Shader.Find("Sprites/Default"));
        arrow.startColor = Color.blue;
        arrow.endColor = Color.cyan;

        killTarget = null;

        // only run once on game start so negligable performance impact
        vents = GameObject.FindObjectsOfType<Vent>().ToList();
    }

    public void FixedUpdate(PlayerControl localPlayer)
    {
        if (MeetingHud.Instance != null && MeetingHud.Instance.enabled) return;

        vision.UpdateVision();

        // only do tasks when we aren't trying to kill anyone
        // prevents weird interactions such as stopping to do a task during a chase
        if (localPlayer.myTasks != null && killTarget == null)
        {
            foreach (PlayerTask task in localPlayer.myTasks)
            {
                // Must be in this order or else breaks Wires task
                if (task == null) continue;
                if (task.IsComplete || inMinigame) continue;
                if (task.Locations == null) continue;

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


        isImpostor = localPlayer.Data.RoleType == AmongUs.GameOptions.RoleTypes.Impostor;
        // for testing purposes

        // in practice area go to the laptop and click "Be_Impostor.exe" to be impostor
        if (isImpostor && localPlayer.killTimer == 0f)
        {
            if (killTarget == null && !localPlayer.inVent)
            {
                PlayerControl potentialKillTarget = null;

                foreach (KeyValuePair<PlayerControl, Utils.LastSeenPlayer> player in vision.playerLocations)
                {
                    // ignore ourselves, dead players, and other impostors
                    if (localPlayer == player.Key) continue;
                    if (player.Key.Data.IsDead) continue;
                    if (player.Key.Data.RoleType == AmongUs.GameOptions.RoleTypes.Impostor) continue;

                    if (player.Value.time > Time.timeSinceLevelLoad - (2 * vision.lastPlayerUpdateDuration))
                    {
                        // if there are multiple players in view, avoid trying to kill so we dont give ourselves up
                        if (potentialKillTarget != null)
                        {
                            potentialKillTarget = null;
                            break;
                        }
                        potentialKillTarget = player.Key;
                    }
                }
                // if the target ends up being the only target in the room, mark them a kill target and pathfind to them
                if (potentialKillTarget != null)
                {
                    killTarget = potentialKillTarget;
                }
            }
            else
            {
                if (!goingForKill && HudManager.Instance.KillButton.currentTarget != null)
                {
                    goingForKill = true;
                    localPlayer.StartCoroutine(MurderTarget());
                }
            }
        }

        // currently, this will just vent whenever we're near a vent
        // in practice it should only activate under certain circumstances, such as after a kill
        // TODO: Pathfind to vents
        if (isImpostor && ventCooldown == 0 && !localPlayer.inVent && !localPlayer.walkingToVent)
        {
            foreach (Vent vent in vents)
            {
                float distance = Vector2.Distance(vent.transform.position, localPlayer.transform.position);
                if (distance < 0.4f)
                {
                    // vent.EnterVent() and vent.Use() dont actually put you in the vent for whatever reason so just click the button virtually
                    HudManager.Instance.ImpostorVentButton.DoClick();
                    break;
                }
            }
        }

        // cooldown so we dont just hop right back in the vent we just exited
        // can probably be removed if the above commented conditions are implemented
        if (ventCooldown > 0)
            ventCooldown--;

        bool sabotageActive = false;
        foreach (PlayerTask task in localPlayer.myTasks)
            if (task.TaskType == TaskTypes.FixLights || task.TaskType == TaskTypes.RestoreOxy || task.TaskType == TaskTypes.ResetReactor || task.TaskType == TaskTypes.ResetSeismic || task.TaskType == TaskTypes.FixComms)
                sabotageActive = true;

        // Record values
        Frame frame = new Frame(
            isImpostor,
            localPlayer.killTimer,
            directionToNearestTask,
            sabotageActive,
            Vector2.zero,
            vision.directionToNearestBody,
            GameManager.Instance.CanReportBodies() && HudManager.Instance.ReportButton.canInteract,
            vision.playerRecords,
            moveDirection,
            didReport,
            didVent,
            didKill,
            // TODO: Implement these two
            false,
            false
        );

        didKill = false;
        didReport = false;
        didVent = false;

        //string frameString = JsonSerializer.Serialize(frame);
        //Debug.Log(frameString);

        recorder.Frames.Add(frame);
    }

    public IEnumerator MurderTarget()
    {
        // wait a random amount of time so we dont just instantly kill them the second we are in range
        yield return new WaitForSeconds(UnityEngine.Random.RandomRange(0.2f, 0.6f));
        // this actually works in practice area which is pretty nice
        PlayerControl.LocalPlayer.MurderPlayer(killTarget);
        Debug.Log(String.Format("I just killed {0}!", killTarget.Data.PlayerName));
        didKill = true;
        goingForKill = false;
        killTarget = null;
        UpdatePathToTask(GetFurthestTask());
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
        }
        else
        {
            directionToNearestTask = Vector2.zero;
        }

        return true;
    }

    public IEnumerator Vent(Vent original)
    {
        Debug.Log("I entered a vent!");
        // there is a vent.NearbyVents variable, but seems to break randomly
        // so currently we just go to random vent
        Vent current = vents[UnityEngine.Random.RandomRangeInt(0, vents.Count)];
        yield return new WaitForSeconds(UnityEngine.Random.RandomRange(0.8f, 1.2f));
        original.MoveToVent(current);
        while (true)
        {
            // use a random time between vent moves to make it more realistic
            yield return new WaitForSeconds(UnityEngine.Random.RandomRange(0.8f, 1.2f));
            bool playerFound = false;
            foreach (KeyValuePair<PlayerControl, Utils.LastSeenPlayer> player in vision.playerLocations)
            {
                if (PlayerControl.LocalPlayer == player.Key) continue;
                if (player.Key.Data.IsDead) continue;
                if (player.Key.Data.RoleType == AmongUs.GameOptions.RoleTypes.Impostor) continue;

                // if we see a player in our radius, try a different vent
                if (player.Value.time > Time.timeSinceLevelLoad - (2 * vision.lastPlayerUpdateDuration))
                {
                    Debug.Log(String.Format("Spotted {0}, trying a different exit vent...", player.Key.name));
                    Vent next;
                    while (true) {
                        next = vents[UnityEngine.Random.RandomRangeInt(0, vents.Count)];
                        if (current == next) continue;
                        break;
                    }
                    current.MoveToVent(next);
                    current = next;
                    playerFound = true;
                    break;
                }
            }
            // if we dont see anyone, exit the vent we're currently in
            // also reset the kill target out of the vent to prevent any crossmap targeting
            if (!playerFound)
            {
                HudManager.Instance.ImpostorVentButton.DoClick();
                ventCooldown = 60;
                killTarget = null;
                yield break;
            }
        }
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

    public void UpdatePathToPlayer()
    {
        if (killTarget == null) return;
        currentPath = pathfinding.FindPath(PlayerControl.LocalPlayer.transform.position, killTarget.transform.position);
        pathIndex = 0;
    }

    public IEnumerator EvaluatePath(NormalPlayerTask initial)
    {
        currentPath = pathfinding.FindPath(PlayerControl.LocalPlayer.transform.position, initial.Locations[0]);
        pathIndex = 0;

        while (true)
        {
            Debug.Log("EvaulatePath Ran");
            yield return new WaitForSeconds(0.5f);

            foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
            {
                Debug.Log("" + task.TaskType + " " + task.HasLocation);
            }

            // if we are attempting to kill someone, dont pathfind to tasks
            if (killTarget == null)
            {
                UpdatePathToTask();
            }
            else
            {
                UpdatePathToPlayer();
            }
        }

    }

    public void UpdatePathToTask(PlayerTask task = null)
    {
        if (task == null)
        {
            // as impostor, among us adds a fake task to the top of the list with no position
            // presumably to display the red text at the top and prevent you from "completing" all your tasks
            // therefore skip it as it cannot be completed
            if (isImpostor)
                task = PlayerControl.LocalPlayer.myTasks[1];
            else
                task = PlayerControl.LocalPlayer.myTasks[0];
        }

        PlayerTask nextTask = null;
        if (task.IsComplete)
        {
            Debug.Log("Task is complete, getting next one.");
            PlayerTask targetTask;
            // as impostor, getting the furthest task instead of the closest one is good for post-kills
            // as it prevents neuro from doing a task in the same room as a dead body
            // it also helps her find more people to kill by traversing the entire map
            if (isImpostor)
            {
                targetTask = GetFurthestTask();
            }
            else
            {
                targetTask = GetClosestTask();
            }
            if(targetTask == null)
            {
                pathIndex = -1;
                GameObject.Destroy(GameObject.Find("Arrow"));
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
            Debug.Log("Next task isn't null");
            currentPath = pathfinding.FindPath(PlayerControl.LocalPlayer.transform.position, nextTask.Locations[0]);
            pathIndex = 0;

            //pathfinding.DrawPath(currentPath);
        }
    }

    private PlayerTask GetClosestTask()
    {
        PlayerTask closestTask = null;
        float closestDistance = Mathf.Infinity;

        foreach (PlayerTask t in PlayerControl.LocalPlayer.myTasks)
        {
            if (!t.IsComplete && t.HasLocation)
            {
                Vector2[] path = pathfinding.FindPath(PlayerControl.LocalPlayer.transform.position, t.Locations[0]);
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

    private PlayerTask GetFurthestTask()
    {
        PlayerTask furthestTask = null;
        float furthestDistance = 0f;

        foreach (PlayerTask t in PlayerControl.LocalPlayer.myTasks)
        {
            if (!t.IsComplete && t.HasLocation)
            {
                Vector2[] path = pathfinding.FindPath(PlayerControl.LocalPlayer.transform.position, t.Locations[0]);
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
}

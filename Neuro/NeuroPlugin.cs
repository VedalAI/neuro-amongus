using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor;
using Reactor.Utilities;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;
using System.Security;
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

    public DeadBody[] deadBodies = null;

    public PlayerControl mainPlayer = null;

    public bool inMinigame = false;

    public bool hasStarted = false;

    public Pathfinding pathfinding = new Pathfinding();

    public Vector2 directionToNearestTask = new Vector2();
    public Vector2 moveDirection = new Vector2();

    public Vector2[] currentPath = new Vector2[0];
    public int pathIndex = -1;

    public LineRenderer arrow;

    public List<PlayerTask> tasks = new List<PlayerTask>();

    public Dictionary<PlayerControl, LastSeenPlayer> playerLocations = new Dictionary<PlayerControl, LastSeenPlayer>();

    public Dictionary<byte, PlayerControl> playerControls = new Dictionary<byte, PlayerControl>();

    public Dictionary<Vector2, string> locationNames = new Dictionary<Vector2, string>()
    {
        { new Vector2 { x = 0, y = 0 }, "Cafeteria" },
        { new Vector2 { x = 9, y = 1 }, "Weapons" },
        { new Vector2 { x = 6.6f, y = -3.7f }, "O2" },
        { new Vector2 { x = 17, y = -5 }, "Navigation" },
        { new Vector2 { x = 9, y = -12.5f }, "Shields" },
        { new Vector2 { x = 4, y = 15.6f }, "Communications" },
        { new Vector2 { x = 0, y = -17 }, "Trash Chute" },
        { new Vector2 { x = 0, y = -12 }, "Storage" },
        { new Vector2 { x = -9, y = -10f }, "Electrical" },
        { new Vector2 { x = -15.6f, y = -10.6f }, "Lower Engine" },
        { new Vector2 { x = -13f, y = -4.4f }, "Security" },
        { new Vector2 { x = -21f, y = -5.5f }, "Reactor" },
        { new Vector2 { x = -15.4f, y = 1 }, "Upper Engine" },
        { new Vector2 { x = -8f, y = -3.5f }, "MedBay" },
        { new Vector2 { x = 5f, y = -8 }, "Admin" },
    };

    public float roundStartTime = 0f; // in seconds
    public float lastPlayerUpdateTime = 0f; // in seconds, records the last time PlayerControl.FixedUpdate was called

    public static string GetLocationFromPosition(Vector2 position)
    {
        float closestDistance = Mathf.Infinity;
        string closestLocation = "";

        foreach(KeyValuePair<Vector2, string> keyValuePair in PluginSingleton<NeuroPlugin>.Instance.locationNames)
        {
            float distance = Vector2.Distance(keyValuePair.Key, position);
            if (distance < 2f)
            {
                return keyValuePair.Value;
            } else
            {
                if(distance < closestDistance)
                {
                    closestDistance = distance;
                    closestLocation = keyValuePair.Value;
                }
            }
        }

        return closestLocation;
    }

    public override void Load()
    {
        ConfigName = Config.Bind("Neuro", "Name", "Neuro-sama");

        Harmony.PatchAll();
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
    public static class UpdatePlayerControlArray
    {
        public static void Postfix(PlayerControl __instance)
        {
            if(PlayerControl.LocalPlayer == null) PlayerControl.LocalPlayer = __instance;
            
            foreach(PlayerControl playerControl in PlayerControl.AllPlayerControls.ToArray())
            {
                if(!PluginSingleton<NeuroPlugin>.Instance.playerControls.ContainsKey(playerControl.PlayerId))
                    PluginSingleton<NeuroPlugin>.Instance.playerControls.Add(playerControl.PlayerId, playerControl);
            }
            foreach(PlayerControl playerControl in PluginSingleton<NeuroPlugin>.Instance.playerControls.Values)
            {
                if (PluginSingleton<NeuroPlugin>.Instance.playerLocations.ContainsKey(playerControl)) continue;
                PluginSingleton<NeuroPlugin>.Instance.playerLocations.Add(playerControl, new LastSeenPlayer("", 0f, false));
            }
            Debug.Log("Updating playerControls: " + PluginSingleton<NeuroPlugin>.Instance.playerControls.Count.ToString());
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
    public static class OnShipLoad
    {
        public static void Postfix(ShipStatus __instance)
        {
            if (!PluginSingleton<NeuroPlugin>.Instance.hasStarted)
            {
                PluginSingleton<NeuroPlugin>.Instance.hasStarted = true;

                Debug.Log("OnShipLoad");
                PluginSingleton<NeuroPlugin>.Instance.pathfinding.GenerateNodeGrid();

                PluginSingleton<NeuroPlugin>.Instance.pathfinding.FloodFill(__instance.MeetingSpawnCenter + (Vector2.up * __instance.SpawnRadius) + new Vector2(0f, 0.3636f));

                GameObject arrow = new GameObject("Arrow");
                PluginSingleton<NeuroPlugin>.Instance.arrow = arrow.AddComponent<LineRenderer>();
            }
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnDestroy))]
    public static class OnShipUnload
    {
        public static void Postfix(ShipStatus __instance)
        {
            PluginSingleton<NeuroPlugin>.Instance.hasStarted = false;
        }
    }

    [HarmonyPatch(typeof(Minigame), nameof(Minigame.Begin))]
    public static class MinigameAutocomplete
    {
        public static void Postfix(Minigame __instance, PlayerTask task)
        {
            Debug.Log("Started task + " + __instance.name);
            __instance.StartCoroutine(MinigameWait(__instance, task, 2f));
            //__instance.Complete();
        }

        public static IEnumerator MinigameWait(Minigame minigame, PlayerTask task, float time)
        {
            yield return new WaitForSeconds(time);

            //task.Complete();
            if(task.TryCast<NormalPlayerTask>() is NormalPlayerTask normalPlayerTask)
            {
                normalPlayerTask.NextStep();
            } else
            {
                Debug.Log("Not Normal Player Task");
                task.Complete();
            }
            minigame.Close();
            PluginSingleton<NeuroPlugin>.Instance.inMinigame = false;
            /*PlayerControl.LocalPlayer.CompleteTask(task.Id);*/
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
            } else {
                nextTask = task;
            }
            if (nextTask != null)
            {
                Debug.Log("Next task isn't null");
                PluginSingleton<NeuroPlugin>.Instance.currentPath = PluginSingleton<NeuroPlugin>.Instance.pathfinding.FindPath(PlayerControl.LocalPlayer.transform.position, nextTask.Locations[0]);
                PluginSingleton<NeuroPlugin>.Instance.pathIndex = 0;

                /*GameObject.Destroy(GameObject.Find("Neuro Path"));

                GameObject test = new GameObject("Neuro Path");
                //Debug.Log(test.transform);
                test.transform.position = PlayerControl.LocalPlayer.transform.position;

                LineRenderer renderer = test.AddComponent<LineRenderer>();
                renderer.positionCount = PluginSingleton<NeuroPlugin>.Instance.currentPath.Length;
                for (int i = 0; i < PluginSingleton<NeuroPlugin>.Instance.currentPath.Length; i++)
                {
                    Debug.Log(PluginSingleton<NeuroPlugin>.Instance.currentPath[i].ToString());
                    renderer.SetPosition(i, PluginSingleton<NeuroPlugin>.Instance.currentPath[i]);
                }
                renderer.widthMultiplier = 0.2f;
                renderer.startColor = Color.blue;*/
            }

        }
    }

    [HarmonyPatch(typeof(NormalPlayerTask), nameof(NormalPlayerTask.Initialize))]
    public static class PlayerTaskInitialize
    {
        static bool done = false;
        public static void Postfix(NormalPlayerTask __instance)
        {
            if (done) return;
            done = true;

            PlayerControl.LocalPlayer.StartCoroutine(PlayerTaskInitialize.EvaluatePath(__instance));

            /*GameObject test = new GameObject("Neuro Path");
            //Debug.Log(test.transform);
            test.transform.position = PlayerControl.LocalPlayer.transform.position;

            LineRenderer renderer = test.AddComponent<LineRenderer>();
            renderer.positionCount = PluginSingleton<NeuroPlugin>.Instance.currentPath.Length;
            for (int i = 0; i < PluginSingleton<NeuroPlugin>.Instance.currentPath.Length; i++)
            {
                Debug.Log(PluginSingleton<NeuroPlugin>.Instance.currentPath[i].ToString());
                renderer.SetPosition(i, PluginSingleton<NeuroPlugin>.Instance.currentPath[i]);
            }
            renderer.widthMultiplier = 0.2f; 
            renderer.startColor = Color.blue;*/
        }

        public static IEnumerator EvaluatePath(NormalPlayerTask initial)
        {
            PluginSingleton<NeuroPlugin>.Instance.currentPath = PluginSingleton<NeuroPlugin>.Instance.pathfinding.FindPath(PlayerControl.LocalPlayer.transform.position, initial.Locations[0] - new Vector2(0, 0.5f));
            PluginSingleton<NeuroPlugin>.Instance.pathIndex = 0;

            while (true) {
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
                    PluginSingleton<NeuroPlugin>.Instance.currentPath = PluginSingleton<NeuroPlugin>.Instance.pathfinding.FindPath(PlayerControl.LocalPlayer.transform.position, nextTask.Locations[0]);
                    PluginSingleton<NeuroPlugin>.Instance.pathIndex = 0;

                    /*GameObject.Destroy(GameObject.Find("Neuro Path"));

                    GameObject test = new GameObject("Neuro Path");
                    //Debug.Log(test.transform);
                    test.transform.position = PlayerControl.LocalPlayer.transform.position;

                    LineRenderer renderer = test.AddComponent<LineRenderer>();
                    renderer.positionCount = PluginSingleton<NeuroPlugin>.Instance.currentPath.Length;
                    for (int i = 0; i < PluginSingleton<NeuroPlugin>.Instance.currentPath.Length; i++)
                    {
                        Debug.Log(PluginSingleton<NeuroPlugin>.Instance.currentPath[i].ToString());
                        renderer.SetPosition(i, PluginSingleton<NeuroPlugin>.Instance.currentPath[i]);
                    }
                    renderer.widthMultiplier = 0.2f;
                    renderer.startColor = Color.blue;*/
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.SetNormalizedVelocity))]
    public static class MovePlayer
    {
        public static bool Prefix(PlayerPhysics __instance, ref Vector2 direction)
        {
            PluginSingleton<NeuroPlugin>.Instance.moveDirection = direction;
            if (PluginSingleton<NeuroPlugin>.Instance.currentPath.Length > 0 && PluginSingleton<NeuroPlugin>.Instance.pathIndex != -1)
            {
                Vector2 nextWaypoint = PluginSingleton<NeuroPlugin>.Instance.currentPath[PluginSingleton<NeuroPlugin>.Instance.pathIndex];

                while (Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), nextWaypoint) < 0.75f)
                {
                    PluginSingleton<NeuroPlugin>.Instance.pathIndex++;
                    if(PluginSingleton<NeuroPlugin>.Instance.pathIndex > PluginSingleton<NeuroPlugin>.Instance.currentPath.Length - 1)
                    {
                        PluginSingleton<NeuroPlugin>.Instance.pathIndex = PluginSingleton<NeuroPlugin>.Instance.currentPath.Length - 1;
                        nextWaypoint = PluginSingleton<NeuroPlugin>.Instance.currentPath[PluginSingleton<NeuroPlugin>.Instance.pathIndex];
                        break;
                    }

                    nextWaypoint = PluginSingleton<NeuroPlugin>.Instance.currentPath[PluginSingleton<NeuroPlugin>.Instance.pathIndex];
                }

                PluginSingleton<NeuroPlugin>.Instance.directionToNearestTask = (nextWaypoint - (Vector2)PlayerControl.LocalPlayer.GetTruePosition()).normalized;


                //Debug.Log(test.transform);

                LineRenderer renderer = PluginSingleton<NeuroPlugin>.Instance.arrow;
                renderer.SetPosition(0, PlayerControl.LocalPlayer.GetTruePosition());
                renderer.SetPosition(1, PlayerControl.LocalPlayer.GetTruePosition() + PluginSingleton<NeuroPlugin>.Instance.directionToNearestTask);
                renderer.widthMultiplier = 0.1f;
                renderer.positionCount = 2;
                renderer.startColor = Color.red;

                //direction = newDirection;

            } else
            {
                PluginSingleton<NeuroPlugin>.Instance.directionToNearestTask = Vector2.zero;
            }
            
            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class PlayerControlFixedUpdate
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (PlayerControl.LocalPlayer != __instance) return;

            if (MeetingHud.Instance != null && MeetingHud.Instance.enabled) return;

            /*if (PluginSingleton<NeuroPlugin>.Instance.currentPath.Length > 0 && PluginSingleton<NeuroPlugin>.Instance.pathIndex != -1)
            {
                Vector2 nextWaypoint = PluginSingleton<NeuroPlugin>.Instance.currentPath[PluginSingleton<NeuroPlugin>.Instance.pathIndex];

                if (Vector2.Distance(PlayerControl.LocalPlayer.transform.position, nextWaypoint) < 0.05f)
                {
                    PluginSingleton<NeuroPlugin>.Instance.pathIndex++;
                }
                else
                {
                    Vector2 direction = (nextWaypoint - (Vector2)PlayerControl.LocalPlayer.transform.position).normalized;
                    PlayerControl.LocalPlayer.MyPhysics.SetNormalizedVelocity(direction);
                }
            }*/

            // Keep track of the amount of time it has been since the last time we were in this function
            float timeSinceLastUpdate = Time.timeSinceLevelLoad - PluginSingleton<NeuroPlugin>.Instance.lastPlayerUpdateTime;
            PluginSingleton<NeuroPlugin>.Instance.lastPlayerUpdateTime = Time.timeSinceLevelLoad;

            // TODO: Fix this
            PluginSingleton<NeuroPlugin>.Instance.deadBodies = GameObject.FindObjectsOfType<DeadBody>();

            Vector2 directionToNearestBody = Vector2.zero;
            float nearestBodyDistance = Mathf.Infinity;

            foreach(DeadBody deadBody in PluginSingleton<NeuroPlugin>.Instance.deadBodies)
            {
                float distance = Vector2.Distance(deadBody.transform.position, PlayerControl.LocalPlayer.transform.position);
                if(distance < nearestBodyDistance)
                {
                    nearestBodyDistance = distance;
                    directionToNearestBody = (deadBody.transform.position - PlayerControl.LocalPlayer.transform.position).normalized;
                }
                if (distance < 3f)
                {
                    PlayerControl playerControl = PluginSingleton<NeuroPlugin>.Instance.playerControls[deadBody.ParentId];
                    PluginSingleton<NeuroPlugin>.Instance.playerLocations[playerControl].location = NeuroPlugin.GetLocationFromPosition(playerControl.transform.position);
                    if(!PluginSingleton<NeuroPlugin>.Instance.playerLocations[playerControl].dead)
                    {
                        PluginSingleton<NeuroPlugin>.Instance.playerLocations[playerControl].time = Time.timeSinceLevelLoad;
                        List<PlayerControl> witnesses = new List<PlayerControl>();
                        foreach (PlayerControl potentialWitness in PluginSingleton<NeuroPlugin>.Instance.playerControls.Values)
                        {
                            if (PlayerControl.LocalPlayer == potentialWitness) continue;

                            if (potentialWitness.inVent || potentialWitness.Data.IsDead) continue;

                            if (Vector2.Distance(potentialWitness.transform.position, deadBody.transform.position) < 3f)
                            {
                                witnesses.Add(potentialWitness);
                            }
                        }
                        PluginSingleton<NeuroPlugin>.Instance.playerLocations[playerControl].witnesses = witnesses.ToArray();
                    }
                    PluginSingleton<NeuroPlugin>.Instance.playerLocations[playerControl].dead = true;

                    Debug.Log(playerControl.name + " is dead in " + NeuroPlugin.GetLocationFromPosition(playerControl.transform.position));
                }
            }

            foreach (PlayerControl playerControl in PluginSingleton<NeuroPlugin>.Instance.playerControls.Values)
            {
                if (PlayerControl.LocalPlayer == playerControl) continue;

                if (playerControl.Data.IsDead) continue;

                // Watch for players venting right in front of us
                if (playerControl.inVent)
                {
                    // Check the last place we saw the player
                    LastSeenPlayer previousSighting = PluginSingleton<NeuroPlugin>.Instance.playerLocations[playerControl];

                    // If we were able to see them during our last update (~30 ms ago), and now they're in a vent, we must have seen them enter the vent
                    if (previousSighting.time > Time.timeSinceLevelLoad - (2 * timeSinceLastUpdate))
                    {
                        previousSighting.sawVent = true; // Remember that we saw this player vent
                        Debug.Log(playerControl.name + " vented right in front of me!");
                    }

                    continue; // Do not consider players in vents as recently seen
                }

                if(Vector2.Distance(playerControl.transform.position, PlayerControl.LocalPlayer.transform.position) < 5f)
                {
                    // raycasting
                    int layerSolid = LayerMask.GetMask(new[] { "Ship", "Shadow" });
                    ContactFilter2D filter = new()
                    {
                        layerMask = layerSolid
                    };

                    Il2CppSystem.Collections.Generic.List<RaycastHit2D> hits = new();

                    if(PlayerControl.LocalPlayer.Collider.RaycastList_Internal((playerControl.GetTruePosition() - PlayerControl.LocalPlayer.GetTruePosition()).normalized, 100f, filter, hits) > 0) {
                        if (hits[0].collider == playerControl.Collider)
                        {
                            PluginSingleton<NeuroPlugin>.Instance.playerLocations[playerControl].location = NeuroPlugin.GetLocationFromPosition(playerControl.transform.position);
                            PluginSingleton<NeuroPlugin>.Instance.playerLocations[playerControl].time = Time.timeSinceLevelLoad;
                            PluginSingleton<NeuroPlugin>.Instance.playerLocations[playerControl].dead = false;
                            PluginSingleton<NeuroPlugin>.Instance.playerLocations[playerControl].gameTimeVisible += timeSinceLastUpdate; // Keep track of total time we've been able to see this player
                            PluginSingleton<NeuroPlugin>.Instance.playerLocations[playerControl].roundTimeVisible += timeSinceLastUpdate; // Keep track of time this round we've been able to see this player

                            Debug.Log(playerControl.name + " is in " + NeuroPlugin.GetLocationFromPosition(playerControl.transform.position));
                        }
                    }
                }
            }

            if(__instance.myTasks != null)
            {
                foreach (PlayerTask task in __instance.myTasks)
                {
                    if (task == null || task.Locations == null) continue;
                    if (task.IsComplete || PluginSingleton<NeuroPlugin>.Instance.inMinigame) continue;
                    foreach (Vector2 location in task.Locations)
                    {
                        if (Vector2.Distance(location, PlayerControl.LocalPlayer.transform.position) < 0.8f)
                        {
                            if (task.MinigamePrefab)
                            {
                                var minigame = GameObject.Instantiate(task.GetMinigamePrefab());
                                minigame.transform.SetParent(Camera.main.transform, false);
                                minigame.transform.localPosition = new Vector3(0f, 0f, -50f);
                                minigame.Console = GameObject.FindObjectOfType<Console>();
                                minigame.Begin(task);
                                PluginSingleton<NeuroPlugin>.Instance.inMinigame = true;
                            }
                        }
                    }
                }
            }

            bool sabotageActive = false;
            foreach (PlayerTask task in __instance.myTasks)
                if (task.TaskType == TaskTypes.FixLights || task.TaskType == TaskTypes.RestoreOxy || task.TaskType == TaskTypes.ResetReactor || task.TaskType == TaskTypes.ResetSeismic || task.TaskType == TaskTypes.FixComms)
                    sabotageActive = true;

            List<PlayerRecord> playerRecords = new List<PlayerRecord>();


            // Record values
            Frame frame = new Frame(
                __instance.Data.RoleType == AmongUs.GameOptions.RoleTypes.Impostor,
                __instance.killTimer,
                PluginSingleton<NeuroPlugin>.Instance.directionToNearestTask,
                sabotageActive,
                Vector2.zero,
                directionToNearestBody,
                GameManager.Instance.CanReportBodies() && HudManager.Instance.ReportButton.gameObject.activeInHierarchy,
                playerRecords,
                PluginSingleton<NeuroPlugin>.Instance.moveDirection,
                false,
                false,
                false,
                false,
                false
            );
            string frameString = JsonSerializer.Serialize(frame);
            Debug.Log(frameString);
            PluginSingleton<NeuroPlugin>.Instance.recorder.Frames.Add(frame);
        }
    }


    [HarmonyPatch(typeof(EmergencyMinigame), nameof(EmergencyMinigame.CallMeeting))]
    public static class MeetingPatch
    {
        public static void Postfix(EmergencyMinigame __instance)
        {
            Debug.Log("NEURO: MEETING CALLED");
            foreach(KeyValuePair<PlayerControl, LastSeenPlayer> playerLocation in PluginSingleton<NeuroPlugin>.Instance.playerLocations)
            {
                if (playerLocation.Key == PlayerControl.LocalPlayer) continue;
                if (playerLocation.Value.location == "") continue;
                if(playerLocation.Value.dead)
                {
                    Debug.Log(playerLocation.Key.name + " was found dead in " + playerLocation.Value.location + " " + (Mathf.Round(Time.timeSinceLevelLoad - playerLocation.Value.time)).ToString() + " seconds ago.");
                    Debug.Log("Witnesses:");
                    foreach(PlayerControl witness in playerLocation.Value.witnesses)
                    {
                        Debug.Log(witness.name);
                    }
                }
                else
                {
                    Debug.Log(playerLocation.Key.name + " was last seen in " + playerLocation.Value.location + " " + (Mathf.Round(Time.timeSinceLevelLoad - playerLocation.Value.time)).ToString() + " seconds ago.");

                    // Report if we saw the player vent right in front of us
                    if (playerLocation.Value.sawVent)
                        Debug.Log("I saw " + playerLocation.Key.name + " vent right in front of me!");

                    // Determine how much time the player was visible to Neuro-sama for
                    float gamePercentage = playerLocation.Value.gameTimeVisible / Time.timeSinceLevelLoad;
                    float roundPercentage = playerLocation.Value.roundTimeVisible / (Time.timeSinceLevelLoad - PluginSingleton<NeuroPlugin>.Instance.roundStartTime);
                    TimeSpan gameTime = new TimeSpan(0, 0, (int) Math.Floor(playerLocation.Value.gameTimeVisible));
                    TimeSpan roundTime = new TimeSpan(0, 0, (int) Math.Floor(playerLocation.Value.roundTimeVisible));
                    Debug.Log(String.Format("{0} has spent {1} minutes and {2} seconds near me this game ({3:0.0}% of the game)", playerLocation.Key.name, gameTime.Minutes,  gameTime.Seconds,  gamePercentage * 100.0f));
                    Debug.Log(String.Format("{0} has spent {1} minutes and {2} seconds near me this round ({3:0.0}% of the round)", playerLocation.Key.name, roundTime.Minutes, roundTime.Seconds, roundPercentage * 100.0f));
                }
                }
            }
        }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static class MeetingEndPatch
    {
        public static void Postfix (ExileController __instance)
        {
            Debug.Log("NEURO: MEETING IS FINISHED");

            // Keep track of what time the round started
            PluginSingleton<NeuroPlugin>.Instance.roundStartTime = Time.timeSinceLevelLoad;

            // Reset our count of how much time per round we've spent near each other player
            foreach (var playerLocation in PluginSingleton<NeuroPlugin>.Instance.playerLocations)
            {
                if (playerLocation.Key == PlayerControl.LocalPlayer || playerLocation.Value.location == "") continue;
                playerLocation.Value.roundTimeVisible = 0f;
            }
        }
    }

    public class LastSeenPlayer
    {
        public string location;
        public float time;
        public bool dead;
        public PlayerControl[] witnesses;
        public float gameTimeVisible; // Total time that we've been able to see this player for
        public float roundTimeVisible; // Total time this round that we've been able to see this player for
        public bool sawVent; // If we've caught this player venting in front of us before

        public LastSeenPlayer(string location, float time, bool dead)
        {
            this.location = location;
            this.time = time;
            this.dead = dead;
            this.gameTimeVisible = 0f;
            this.roundTimeVisible = 0f;
            this.sawVent = false;
            this.witnesses = null;
        }
    }
}

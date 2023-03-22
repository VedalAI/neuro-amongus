using System;
using System.Collections.Generic;
using Neuro.DependencyInjection;
using Neuro.Utilities;
using Neuro.Vision.DataStructures;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Vision;

// TODO: Refactor this entire class
[RegisterInIl2Cpp]
public class VisionHandler : MonoBehaviour, IVisionHandler
{
    public VisionHandler(IntPtr ptr) : base(ptr) { }

    public IContextProvider Context { get; set; }

    public Vector2 DirectionToNearestBody { get; set; }

    private DeadBody[] deadBodies;
    private readonly Dictionary<byte, PlayerControl> playerControls = new(); // TODO: Use GameData.GetPlayerById
    private readonly Dictionary<PlayerControl, LastSeenPlayer> playerLocations = new(); // TODO: Don't use PlayerControls as dictionary keys,
                                                                                        // TODO#2: These dictionaries arent cleaned after games
    private float roundStartTime; // in seconds

    public void FixedUpdate()
    {
        if (MeetingHud.Instance) return;

        // TODO: Fix this
        // I know what the problem is: Line 40 you are getting the position of the player which is not the same as the position of the body
        deadBodies = FindObjectsOfType<DeadBody>();

        DirectionToNearestBody = Vector2.zero;
        float nearestBodyDistance = Mathf.Infinity;

        foreach (DeadBody deadBody in deadBodies)
        {
            float distance = Vector2.Distance(deadBody.transform.position, PlayerControl.LocalPlayer.transform.position);
            if (distance < nearestBodyDistance)
            {
                nearestBodyDistance = distance;
                DirectionToNearestBody = (deadBody.transform.position - PlayerControl.LocalPlayer.transform.position).normalized;
            }

            if (distance < 3f)
            {
                PlayerControl playerControl = playerControls[deadBody.ParentId];
                playerLocations[playerControl].location = GetLocationFromPosition(playerControl.transform.position);
                if (!playerLocations[playerControl].dead)
                {
                    playerLocations[playerControl].time = Time.timeSinceLevelLoad;
                    List<PlayerControl> witnesses = new();
                    foreach (PlayerControl potentialWitness in playerControls.Values)
                    {
                        if (PlayerControl.LocalPlayer == potentialWitness) continue;

                        if (potentialWitness.inVent || potentialWitness.Data.IsDead) continue;

                        if (Vector2.Distance(potentialWitness.transform.position, deadBody.transform.position) < 3f) witnesses.Add(potentialWitness);
                    }

                    playerLocations[playerControl].witnesses = witnesses.ToArray();
                }

                playerLocations[playerControl].dead = true;

                Debug.Log(playerControl.name + " is dead in " + GetLocationFromPosition(playerControl.transform.position));
            }
        }

        foreach (PlayerControl playerControl in playerControls.Values)
        {
            if (PlayerControl.LocalPlayer == playerControl) continue;

            if (playerControl.Data.IsDead) continue;

            // Watch for players venting right in front of us
            if (playerControl.inVent)
            {
                // Check the last place we saw the player
                LastSeenPlayer previousSighting = playerLocations[playerControl];

                // If we were able to see them during our last update (~30 ms ago), and now they're in a vent, we must have seen them enter the vent
                if (previousSighting.time > Time.timeSinceLevelLoad - 2 * Time.fixedDeltaTime)
                {
                    previousSighting.sawVent = true; // Remember that we saw this player vent
                    Debug.Log(playerControl.name + " vented right in front of me!");
                }

                continue; // Do not consider players in vents as recently seen
            }

            if (Vector2.Distance(playerControl.transform.position, PlayerControl.LocalPlayer.transform.position) < 5f)
            {
                // raycasting
                int layerSolid = LayerMask.GetMask("Ship", "Shadow");
                ContactFilter2D filter = new()
                {
                    layerMask = layerSolid
                };

                Il2CppSystem.Collections.Generic.List<RaycastHit2D> hits = new();

                if (PlayerControl.LocalPlayer.Collider.RaycastList_Internal((playerControl.GetTruePosition() - PlayerControl.LocalPlayer.GetTruePosition()).normalized, 100f, filter, hits) > 0)
                {
                    if (hits.At(0).collider == playerControl.Collider)
                    {
                        playerLocations[playerControl].location = GetLocationFromPosition(playerControl.transform.position);
                        playerLocations[playerControl].time = Time.timeSinceLevelLoad;
                        playerLocations[playerControl].dead = false;
                        playerLocations[playerControl].gameTimeVisible += Time.fixedDeltaTime; // Keep track of total time we've been able to see this player
                        playerLocations[playerControl].roundTimeVisible += Time.fixedDeltaTime; // Keep track of time this round we've been able to see this player

                        Debug.Log(playerControl.name + " is in " + GetLocationFromPosition(playerControl.transform.position));
                    }
                }
            }
        }
    }

    public void StartTrackingPlayer(PlayerControl player)
    {
        // TODO: This entire implementation is just terrible.

        foreach (PlayerControl playerControl in PlayerControl.AllPlayerControls.ToArray())
            if (!playerControls.ContainsKey(playerControl.PlayerId))
                playerControls.Add(playerControl.PlayerId, playerControl);

        foreach (PlayerControl playerControl in playerControls.Values)
        {
            if (playerLocations.ContainsKey(playerControl)) continue;
            playerLocations.Add(playerControl, new LastSeenPlayer("", 0f, false));
        }

        Debug.Log("Updating playerControls: " + playerControls.Count);
    }

    public void ReportFindings()
    {
        foreach (KeyValuePair<PlayerControl, LastSeenPlayer> playerLocation in playerLocations)
        {
            if (playerLocation.Key == PlayerControl.LocalPlayer) continue;
            if (playerLocation.Value.location == "") continue;
            if (playerLocation.Value.dead)
            {
                Debug.Log(playerLocation.Key.name + " was found dead in " + playerLocation.Value.location + " " + Mathf.Round(Time.timeSinceLevelLoad - playerLocation.Value.time) + " seconds ago.");
                Debug.Log("Witnesses:");
                foreach (PlayerControl witness in playerLocation.Value.witnesses) Debug.Log(witness.name);
            }
            else
            {
                Debug.Log(playerLocation.Key.name + " was last seen in " + playerLocation.Value.location + " " + Mathf.Round(Time.timeSinceLevelLoad - playerLocation.Value.time) + " seconds ago.");

                // Report if we saw the player vent right in front of us
                if (playerLocation.Value.sawVent)
                    Debug.Log("I saw " + playerLocation.Key.name + " vent right in front of me!");

                // Determine how much time the player was visible to Neuro-sama for
                float gamePercentage = playerLocation.Value.gameTimeVisible / Time.timeSinceLevelLoad;
                float roundPercentage = playerLocation.Value.roundTimeVisible / (Time.timeSinceLevelLoad - roundStartTime);
                TimeSpan gameTime = new(0, 0, (int) Math.Floor(playerLocation.Value.gameTimeVisible));
                TimeSpan roundTime = new(0, 0, (int) Math.Floor(playerLocation.Value.roundTimeVisible));
                Debug.Log($"{playerLocation.Key.name} has spent {gameTime.Minutes} minutes and {gameTime.Seconds} seconds near me this game ({gamePercentage * 100.0f:0.0}% of the game)");
                Debug.Log($"{playerLocation.Key.name} has spent {roundTime.Minutes} minutes and {roundTime.Seconds} seconds near me this round ({roundPercentage * 100.0f:0.0}% of the round)");
            }
        }
    }

    public void ResetAfterMeeting()
    {
        // Keep track of what time the round started
        roundStartTime = Time.timeSinceLevelLoad;

        // Reset our count of how much time per round we've spent near each other player
        foreach (KeyValuePair<PlayerControl, LastSeenPlayer> playerLocation in playerLocations)
        {
            if (playerLocation.Key == PlayerControl.LocalPlayer || playerLocation.Value.location == "") continue;
            playerLocation.Value.roundTimeVisible = 0f;
        }
    }

    #region This is moved here temporarily until the changes from #7 are merged

    private static readonly Dictionary<Vector2, string> locationNames = new Dictionary<Vector2, string>()
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

    private static string GetLocationFromPosition(Vector2 position)
    {
        float closestDistance = Mathf.Infinity;
        string closestLocation = "";

        foreach (KeyValuePair<Vector2, string> keyValuePair in locationNames)
        {
            float distance = Vector2.Distance(keyValuePair.Key, position);
            if (distance < 2f)
            {
                return keyValuePair.Value;
            }
            else
            {
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestLocation = keyValuePair.Value;
                }
            }
        }

        return closestLocation;
    }

    #endregion
}

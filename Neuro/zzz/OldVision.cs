using System;
using System.Collections.Generic;
using Neuro.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Neuro;

public class OldVision
{
    public DeadBody[] deadBodies;

    public Vector2 directionToNearestBody;
    public float lastPlayerUpdateTime; // in seconds, records the last time PlayerControl.FixedUpdate was called

    public Dictionary<byte, PlayerControl> playerControls = new();

    public Dictionary<PlayerControl, LastSeenPlayer> playerLocations = new();

    public float roundStartTime; // in seconds

    public void UpdatePlayerControlArray(PlayerControl newPlayerControl)
    {
        if (PlayerControl.LocalPlayer == null) PlayerControl.LocalPlayer = newPlayerControl;

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

    public void MeetingEnd()
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

    // Called from FixedUpdate
    public void UpdateVision()
    {
        // Keep track of the amount of time it has been since the last time we were in this function
        float timeSinceLastUpdate = Time.timeSinceLevelLoad - lastPlayerUpdateTime;
        lastPlayerUpdateTime = Time.timeSinceLevelLoad;

        // TODO: Fix this
        deadBodies = Object.FindObjectsOfType<DeadBody>();

        directionToNearestBody = Vector2.zero;
        float nearestBodyDistance = Mathf.Infinity;

        foreach (DeadBody deadBody in deadBodies)
        {
            float distance = Vector2.Distance(deadBody.transform.position, PlayerControl.LocalPlayer.transform.position);
            if (distance < nearestBodyDistance)
            {
                nearestBodyDistance = distance;
                directionToNearestBody = (deadBody.transform.position - PlayerControl.LocalPlayer.transform.position).normalized;
            }

            if (distance < 3f)
            {
                PlayerControl playerControl = playerControls[deadBody.ParentId];
                playerLocations[playerControl].location = Methods.GetLocationFromPosition(playerControl.transform.position);
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

                Debug.Log(playerControl.name + " is dead in " + Methods.GetLocationFromPosition(playerControl.transform.position));
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
                if (previousSighting.time > Time.timeSinceLevelLoad - 2 * timeSinceLastUpdate)
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
                    if (hits.At(0).collider == playerControl.Collider)
                    {
                        playerLocations[playerControl].location = Methods.GetLocationFromPosition(playerControl.transform.position);
                        playerLocations[playerControl].time = Time.timeSinceLevelLoad;
                        playerLocations[playerControl].dead = false;
                        playerLocations[playerControl].gameTimeVisible += timeSinceLastUpdate; // Keep track of total time we've been able to see this player
                        playerLocations[playerControl].roundTimeVisible += timeSinceLastUpdate; // Keep track of time this round we've been able to see this player

                        Debug.Log(playerControl.name + " is in " + Methods.GetLocationFromPosition(playerControl.transform.position));
                    }
            }
        }
    }
}
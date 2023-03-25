using Neuro.Utils;
using Reactor.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Neuro;

public class Vision
{
    public List<DeadBody> deadBodies = new List<DeadBody>();

    public Dictionary<PlayerControl, LastSeenPlayer> playerLocations = new Dictionary<PlayerControl, LastSeenPlayer>();

    public Dictionary<byte, PlayerControl> playerControls = new Dictionary<byte, PlayerControl>();

    public Dictionary<byte, int> playerRecordIndexes = new Dictionary<byte, int>();

    public List<PlayerRecord> playerRecords = new();

    public float roundStartTime = 0f; // in seconds
    public float lastPlayerUpdateTime = 0f; // in seconds, records the last time PlayerControl.FixedUpdate was called
    public float lastPlayerUpdateDuration = 0f; // in seconds, records the time elapsed since last PlayerControl.FixedUpdate was called

    public Vector2 directionToNearestBody;

    public void UpdatePlayerControlArray(PlayerControl newPlayerControl)
    {
        if (PlayerControl.LocalPlayer == null) PlayerControl.LocalPlayer = newPlayerControl;

        playerRecords.Clear();
        playerRecordIndexes.Clear();

        int i = 0;
        foreach (PlayerControl playerControl in PlayerControl.AllPlayerControls.ToArray())
        {
            if (!playerControls.ContainsKey(playerControl.PlayerId))
                playerControls.Add(playerControl.PlayerId, playerControl);
            playerRecords.Add(new PlayerRecord(new MyVector2(0, 0), -1));
            playerRecordIndexes.Add(playerControl.PlayerId, i);
            i++;
        }
        foreach (PlayerControl playerControl in playerControls.Values)
        {
            if (playerLocations.ContainsKey(playerControl)) continue;
            playerLocations.Add(playerControl, new LastSeenPlayer("", 0f, false));
        }
        Debug.Log("Updating playerControls: " + playerControls.Count.ToString());
    }

    public void ReportFindings()
    {
        foreach (KeyValuePair<PlayerControl, LastSeenPlayer> playerLocation in playerLocations)
        {
            if (playerLocation.Key == PlayerControl.LocalPlayer) continue;
            if (playerLocation.Value.location == "") continue;
            if (playerLocation.Value.dead)
            {
                Debug.Log(playerLocation.Key.name + " was found dead in " + playerLocation.Value.location + " " + (Mathf.Round(Time.timeSinceLevelLoad - playerLocation.Value.time)).ToString() + " seconds ago.");
                Debug.Log("Witnesses:");
                foreach (PlayerControl witness in playerLocation.Value.witnesses)
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
                float roundPercentage = playerLocation.Value.roundTimeVisible / (Time.timeSinceLevelLoad - roundStartTime);
                TimeSpan gameTime = new TimeSpan(0, 0, (int)Math.Floor(playerLocation.Value.gameTimeVisible));
                TimeSpan roundTime = new TimeSpan(0, 0, (int)Math.Floor(playerLocation.Value.roundTimeVisible));
                Debug.Log(String.Format("{0} has spent {1} minutes and {2} seconds near me this game ({3:0.0}% of the game)", playerLocation.Key.name, gameTime.Minutes, gameTime.Seconds, gamePercentage * 100.0f));
                Debug.Log(String.Format("{0} has spent {1} minutes and {2} seconds near me this round ({3:0.0}% of the round)", playerLocation.Key.name, roundTime.Minutes, roundTime.Seconds, roundPercentage * 100.0f));
            }
        }
    }

    public void MeetingEnd()
    {
        // Keep track of what time the round started
        roundStartTime = Time.timeSinceLevelLoad;

        // Reset our count of how much time per round we've spent near each other player
        foreach (var playerLocation in playerLocations)
        {
            if (playerLocation.Key == PlayerControl.LocalPlayer || playerLocation.Value.location == "") continue;
            playerLocation.Value.roundTimeVisible = 0f;
        }

        // Reset dead bodies
        deadBodies.Clear();

        // Clear out the target player
        PluginSingleton<NeuroPlugin>.Instance.killTarget = null;
    }

    public void DeadBodyAppeared(DeadBody deadBody)
    {
        Debug.Log(String.Format("{0} has been killed", playerControls[deadBody.ParentId].Data.PlayerName));
        deadBodies.Add(deadBody);
    }

    // Called from FixedUpdate
    public void UpdateVision()
    {
        // Keep track of the amount of time it has been since the last time we were in this function
        lastPlayerUpdateDuration = Time.timeSinceLevelLoad - lastPlayerUpdateTime;
        lastPlayerUpdateTime = Time.timeSinceLevelLoad;

        UpdateDeadBodiesVision();
        UpdateNearbyPlayersVision();
    }

    void UpdateDeadBodiesVision() {
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

            if (!CheckVisibility(deadBody.TruePosition))
            {
                continue;
            }

            if (distance < 3f)
            {
                PlayerControl playerControl = playerControls[deadBody.ParentId];
                playerLocations[playerControl].location = Methods.GetLocationFromPosition(playerControl.transform.position);
                if (!playerLocations[playerControl].dead)
                {
                    playerLocations[playerControl].time = Time.timeSinceLevelLoad;
                    List<PlayerControl> witnesses = new List<PlayerControl>();
                    foreach (PlayerControl potentialWitness in playerControls.Values)
                    {
                        if (PlayerControl.LocalPlayer == potentialWitness) continue;

                        if (potentialWitness.inVent || potentialWitness.Data.IsDead) continue;

                        if (Vector2.Distance(potentialWitness.transform.position, deadBody.transform.position) < 3f)
                        {
                            witnesses.Add(potentialWitness);
                        }
                    }
                    playerLocations[playerControl].witnesses = witnesses.ToArray();
                }
                playerLocations[playerControl].dead = true;

                Debug.Log(playerControl.name + " is dead in " + Methods.GetLocationFromPosition(playerControl.transform.position));
            }
        }
    }

    void UpdateNearbyPlayersVision() {
        foreach (PlayerControl playerControl in playerControls.Values)
        {
            int playerIndex = playerRecordIndexes[playerControl.PlayerId];
            PlayerRecord playerRecord = playerRecords[playerIndex];
            playerRecord.distance = -1;
            playerRecord.relativeDirection = new MyVector2(0, 0);

            playerRecords[playerIndex] = playerRecord;

            if (PlayerControl.LocalPlayer == playerControl) continue;

            if (playerControl.Data.IsDead) continue;

            // Watch for players venting right in front of us
            if (playerControl.inVent)
            {
                // Check the last place we saw the player
                LastSeenPlayer previousSighting = playerLocations[playerControl];

                // If we were able to see them during our last update (~30 ms ago), and now they're in a vent, we must have seen them enter the vent
                if (previousSighting.time > Time.timeSinceLevelLoad - (2 * lastPlayerUpdateDuration))
                {
                    previousSighting.sawVent = true; // Remember that we saw this player vent
                    Debug.Log(playerControl.name + " vented right in front of me!");
                }

                continue; // Do not consider players in vents as recently seen
            }

            if (Vector2.Distance(playerControl.transform.position, PlayerControl.LocalPlayer.transform.position) < 5f)
            {
                if (CheckVisibility(playerControl.GetTruePosition()))
                {
                    playerLocations[playerControl].location = Methods.GetLocationFromPosition(playerControl.transform.position);
                    playerLocations[playerControl].time = Time.timeSinceLevelLoad;
                    playerLocations[playerControl].dead = false;
                    playerLocations[playerControl].gameTimeVisible += lastPlayerUpdateDuration; // Keep track of total time we've been able to see this player
                    playerLocations[playerControl].roundTimeVisible += lastPlayerUpdateDuration; // Keep track of time this round we've been able to see this player

                    playerRecord.distance = (playerControl.GetTruePosition() - PlayerControl.LocalPlayer.GetTruePosition()).magnitude;
                    playerRecord.relativeDirection = (MyVector2)(playerControl.GetTruePosition() - PlayerControl.LocalPlayer.GetTruePosition()).normalized;
                    playerRecords[playerIndex] = playerRecord;

                    Debug.Log(playerControl.name + " is in " + Methods.GetLocationFromPosition(playerControl.transform.position));
                }
                else
                {
                    Debug.Log(String.Format("{0} is close, but out of sight", playerControl.Data.PlayerName));
                }
            }
        }
    }

    bool CheckVisibility(Vector2 rayEnd)
    {
        // Raycasting
        // If raycast hits shadow, this usually means that player is not visible
        // So check that there is no shadow
        int layerShadow = LayerMask.GetMask(new[] { "Shadow" });
        Vector2 rayStart = PlayerControl.LocalPlayer.GetTruePosition();
        RaycastHit2D hit = Physics2D.Raycast(rayStart, (rayEnd - rayStart).normalized, (rayEnd - rayStart).magnitude, layerShadow);
        return !hit;
    }
}
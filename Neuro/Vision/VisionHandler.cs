using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Neuro.Recording.DataStructures;
using Neuro.Vision.DataStructures;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Vision;

// TODO: Refactor this entire class
[RegisterInIl2Cpp]
public class VisionHandler : MonoBehaviour
{
    public VisionHandler(IntPtr ptr) : base(ptr) { }

    public Vector2 DirectionToNearestBody { get; set; }
    public readonly Dictionary<byte, PlayerRecord> PlayerRecords = new(); // TODO: Store this data using a monobehaviour on the player

    // TODO: Handle players disconnecting
    private readonly List<DeadBody> deadBodies = new();
    private readonly Dictionary<byte, PlayerControl> playerControls = new(); // TODO: Use GameData.GetPlayerById
    private readonly Dictionary<PlayerControl, LastSeenPlayer> playerLocations = new(); // TODO: Don't use PlayerControls as dictionary keys,
    // TODO#2: These dictionaries arent cleaned after games

    private float roundStartTime; // in seconds

    public void FixedUpdate()
    {
        if (!ShipStatus.Instance) return;
        if (MeetingHud.Instance) return;

        UpdateDeadBodiesVision();
        UpdateNearbyPlayersVision();
    }

    public void StartTrackingPlayer(PlayerControl player)
    {
        // TODO: I don't like this

        Il2CppArrayBase<PlayerControl> @base = PlayerControl.AllPlayerControls.ToArray();
        for (int i = 0; i < @base.Count; i++)
        {
            PlayerControl playerControl = @base[i];
            if (!playerControls.ContainsKey(playerControl.PlayerId))
                playerControls.Add(playerControl.PlayerId, playerControl);

            PlayerRecords.Add(playerControl.PlayerId, new PlayerRecord(-1, new MyVector2(0, 0)));
        }

        foreach (PlayerControl playerControl in playerControls.Values)
        {
            if (playerLocations.ContainsKey(playerControl)) continue;
            playerLocations.Add(playerControl, new LastSeenPlayer("", 0f, false));
        }

        Info("Updating playerControls: " + playerControls.Count);
    }

    public void AddDeadBody(DeadBody body)
    {
        if (deadBodies.Any(b => b.ParentId == body.ParentId)) return;

        Info($"{GameData.Instance.GetPlayerById(body.ParentId).PlayerName} has been killed");
        deadBodies.Add(body);
    }

    public void ReportFindings()
    {
        foreach (KeyValuePair<PlayerControl, LastSeenPlayer> playerLocation in playerLocations)
        {
            if (playerLocation.Key == PlayerControl.LocalPlayer) continue;
            if (playerLocation.Value.location == "") continue;
            if (playerLocation.Value.dead)
            {
                Info(playerLocation.Key.name + " was found dead in " + playerLocation.Value.location + " " + Mathf.Round(Time.timeSinceLevelLoad - playerLocation.Value.time) + " seconds ago.");
                Info("Witnesses:");
                foreach (PlayerControl witness in playerLocation.Value.witnesses) Info(witness.name);
            }
            else
            {
                Info(playerLocation.Key.name + " was last seen in " + playerLocation.Value.location + " " + Mathf.Round(Time.timeSinceLevelLoad - playerLocation.Value.time) + " seconds ago.");

                // Report if we saw the player vent right in front of us
                if (playerLocation.Value.sawVent)
                    Info("I saw " + playerLocation.Key.name + " vent right in front of me!");

                // Determine how much time the player was visible to Neuro-sama for
                float gamePercentage = playerLocation.Value.gameTimeVisible / Time.timeSinceLevelLoad;
                float roundPercentage = playerLocation.Value.roundTimeVisible / (Time.timeSinceLevelLoad - roundStartTime);
                TimeSpan gameTime = new(0, 0, (int) Math.Floor(playerLocation.Value.gameTimeVisible));
                TimeSpan roundTime = new(0, 0, (int) Math.Floor(playerLocation.Value.roundTimeVisible));
                Info($"{playerLocation.Key.name} has spent {gameTime.Minutes} minutes and {gameTime.Seconds} seconds near me this game ({gamePercentage * 100.0f:0.0}% of the game)");
                Info($"{playerLocation.Key.name} has spent {roundTime.Minutes} minutes and {roundTime.Seconds} seconds near me this round ({roundPercentage * 100.0f:0.0}% of the round)");
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

        deadBodies.Clear();
    }

    private void UpdateDeadBodiesVision()
    {
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

            if (!IsVisible(deadBody.TruePosition))
            {
                continue;
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

                Info(playerControl.name + " is dead in " + GetLocationFromPosition(playerControl.transform.position));
            }
        }
    }

    private void UpdateNearbyPlayersVision()
    {
        foreach (PlayerControl playerControl in playerControls.Values)
        {
            PlayerRecords[playerControl.PlayerId] = new PlayerRecord();

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
                    Info(playerControl.name + " vented right in front of me!");
                }

                continue; // Do not consider players in vents as recently seen
            }

            if (Vector2.Distance(playerControl.transform.position, PlayerControl.LocalPlayer.transform.position) < 5f)
            {
                if (IsVisible(playerControl.GetTruePosition()))
                {
                    playerLocations[playerControl].location = GetLocationFromPosition(playerControl.transform.position);
                    playerLocations[playerControl].time = Time.timeSinceLevelLoad;
                    playerLocations[playerControl].dead = false;
                    playerLocations[playerControl].gameTimeVisible += Time.fixedDeltaTime; // Keep track of total time we've been able to see this player
                    playerLocations[playerControl].roundTimeVisible += Time.fixedDeltaTime; // Keep track of time this round we've been able to see this player

                    float distance = (playerControl.GetTruePosition() - PlayerControl.LocalPlayer.GetTruePosition()).magnitude;
                    Vector2 direction = (playerControl.GetTruePosition() - PlayerControl.LocalPlayer.GetTruePosition()).normalized;
                    PlayerRecords[playerControl.PlayerId] = new PlayerRecord(distance, direction);

                    Info(playerControl.name + " is in " + GetLocationFromPosition(playerControl.transform.position));
                }
                else
                {
                    Info($"{playerControl.Data.PlayerName} is close, but out of sight");
                }
            }
        }
    }

    private static bool IsVisible(Vector2 rayEnd)
    {
        // Raycasting
        // If raycast hits shadow, this usually means that player is not visible
        // So check that there is no shadow
        int layerShadow = LayerMask.GetMask(new[] { "Shadow" });
        Vector2 rayStart = PlayerControl.LocalPlayer.GetTruePosition();
        RaycastHit2D hit = Physics2D.Raycast(rayStart, (rayEnd - rayStart).normalized, (rayEnd - rayStart).magnitude, layerShadow);
        return !hit;
    }

    private static string GetLocationFromPosition(Vector2 position)
    {
        float closestDistance = Mathf.Infinity;
        PlainShipRoom closestLocation = null;
        string nearPrefix = "outside near "; // If we're not in any rooms/hallways, we're "outside"

        if (!ShipStatus.Instance) // In case this is called from the lobby
            return "the lobby";

        foreach (PlainShipRoom room in ShipStatus.Instance.AllRooms)
        {
            Collider2D collider = room.roomArea;
            if (collider.OverlapPoint(position))
            {
                if (room.RoomId == SystemTypes.Hallway)
                    nearPrefix = "a hallway near "; // keep looking for the nearest room
                else
                    return TranslationController.Instance.GetString(room.RoomId); // If we're inside a proper room, ignore the nearPrefix
            }
            else if (room.RoomId != SystemTypes.Hallway)
            {
                float distance = Vector2.Distance(position, collider.ClosestPoint(position));
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestLocation = room;
                }
            }
        }

        if (!closestLocation)
            return "";

        // We're not in an actual room, so say which room we're nearest to
        return nearPrefix + TranslationController.Instance.GetString(closestLocation.RoomId);
    }
}

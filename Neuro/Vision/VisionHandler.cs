using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Recording.DataStructures;
using Neuro.Vision.DataStructures;
using Reactor.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Vision;

[RegisterInIl2Cpp]
public class VisionHandler : MonoBehaviour
{
    public VisionHandler(IntPtr ptr) : base(ptr) { }

    public Vector2 DirectionToNearestBody { get; set; }

    [HideFromIl2Cpp]
    public Dictionary<PlayerControl, PlayerRecord> PlayerRecords { get; } = new(Il2CppEqualityComparer<PlayerControl>.Instance);

    // TODO: These dictionaries arent cleaned after games
    // TODO: Handle players disconnecting
    private readonly List<DeadBody> _deadBodies = new();
    private readonly Dictionary<PlayerControl, LastSeenPlayer> _playerLocations = new(Il2CppEqualityComparer<PlayerControl>.Instance);

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
        foreach (PlayerControl playerControl in PlayerControl.AllPlayerControls)
        {
            PlayerRecords[playerControl] = new PlayerRecord(-1, new MyVector2(0, 0));
            _playerLocations[playerControl] = new LastSeenPlayer("", 0f, false);
        }

        Info("Updating playerControls");
    }

    public void AddDeadBody(DeadBody body)
    {
        if (_deadBodies.Any(b => b.ParentId == body.ParentId)) return;

        Info($"{GameData.Instance.GetPlayerById(body.ParentId).PlayerName} has been killed");
        _deadBodies.Add(body);
    }

    public void ReportFindings()
    {
        foreach ((PlayerControl player, LastSeenPlayer lastSeen) in _playerLocations)
        {
            if (!player || player.AmOwner) continue;

            if (lastSeen.location == "") continue;
            if (lastSeen.dead)
            {
                Info(player.name + " was found dead in " + lastSeen.location + " " + Mathf.Round(Time.timeSinceLevelLoad - lastSeen.time) + " seconds ago.");
                Info("Witnesses:");
                foreach (PlayerControl witness in lastSeen.witnesses) Info(witness.name);
            }
            else
            {
                Info(player.name + " was last seen in " + lastSeen.location + " " + Mathf.Round(Time.timeSinceLevelLoad - lastSeen.time) + " seconds ago.");

                // Report if we saw the player vent right in front of us
                if (lastSeen.sawVent)
                    Info("I saw " + player.name + " vent right in front of me!");

                // Determine how much time the player was visible to Neuro-sama for
                float gamePercentage = lastSeen.gameTimeVisible / Time.timeSinceLevelLoad;
                float roundPercentage = lastSeen.roundTimeVisible / (Time.timeSinceLevelLoad - roundStartTime);
                TimeSpan gameTime = new(0, 0, (int)Math.Floor(lastSeen.gameTimeVisible));
                TimeSpan roundTime = new(0, 0, (int)Math.Floor(lastSeen.roundTimeVisible));
                Info($"{player.name} has spent {gameTime.Minutes} minutes and {gameTime.Seconds} seconds near me this game ({gamePercentage * 100.0f:0.0}% of the game)");
                Info($"{player.name} has spent {roundTime.Minutes} minutes and {roundTime.Seconds} seconds near me this round ({roundPercentage * 100.0f:0.0}% of the round)");
            }
        }
    }

    public void ResetAfterMeeting()
    {
        // Keep track of what time the round started
        roundStartTime = Time.timeSinceLevelLoad;

        // Reset our count of how much time per round we've spent near each other player
        foreach ((PlayerControl player, LastSeenPlayer lastSeen) in _playerLocations)
        {
            if (player.AmOwner || lastSeen.location == "") continue;
            lastSeen.roundTimeVisible = 0f;
        }

        _deadBodies.Clear();
    }

    private void UpdateDeadBodiesVision() // TODO: Refactor
    {
        DirectionToNearestBody = Vector2.zero;
        float nearestBodyDistance = Mathf.Infinity;
        Vector2 localPlayerTruePosition = PlayerControl.LocalPlayer.GetTruePosition();

        // TODO: This logic is probably incorrect
        foreach (DeadBody deadBody in _deadBodies)
        {
            float distance = Vector2.Distance(deadBody.transform.position, PlayerControl.LocalPlayer.transform.position);
            if (distance < nearestBodyDistance)
            {
                nearestBodyDistance = distance;
                DirectionToNearestBody = (deadBody.transform.position - PlayerControl.LocalPlayer.transform.position).normalized;
            }

            if (!IsVisible(localPlayerTruePosition, deadBody.TruePosition))
            {
                continue;
            }

            if (distance < 3f)
            {
                PlayerControl playerControl = GameData.Instance.GetPlayerById(deadBody.ParentId).Object;
                _playerLocations[playerControl].location = GetLocationFromPosition(playerControl.transform.position);
                if (!_playerLocations[playerControl].dead)
                {
                    _playerLocations[playerControl].time = Time.timeSinceLevelLoad;
                    List<PlayerControl> witnesses = new();
                    foreach (PlayerControl potentialWitness in PlayerControl.AllPlayerControls)
                    {
                        if (PlayerControl.LocalPlayer == potentialWitness) continue;

                        if (potentialWitness.inVent || potentialWitness.Data.IsDead) continue;

                        if (Vector2.Distance(potentialWitness.transform.position, deadBody.transform.position) < 3f) witnesses.Add(potentialWitness);
                    }

                    _playerLocations[playerControl].witnesses = witnesses.ToArray();
                }

                _playerLocations[playerControl].dead = true;

                Info(playerControl.name + " is dead in " + GetLocationFromPosition(playerControl.transform.position));
            }
        }
    }

    private void UpdateNearbyPlayersVision() // TODO: Refactor
    {
        Vector2 localPlayerTruePosition = PlayerControl.LocalPlayer.GetTruePosition();

        foreach (PlayerControl playerControl in PlayerControl.AllPlayerControls)
        {
            
            PlayerRecords[playerControl] = new PlayerRecord();
            if (PlayerControl.LocalPlayer == playerControl) continue;

            if (playerControl.Data.IsDead) continue;

            // Watch for players venting right in front of us
            if (playerControl.inVent)
            {
                // Check the last place we saw the player
                LastSeenPlayer previousSighting = _playerLocations[playerControl];

                // If we were able to see them during our last update (~30 ms ago), and now they're in a vent, we must have seen them enter the vent
                if (previousSighting.time > Time.timeSinceLevelLoad - 2 * Time.fixedDeltaTime)
                {
                    previousSighting.sawVent = true; // Remember that we saw this player vent
                    Info(playerControl.name + " vented right in front of me!");
                }

                continue; // Do not consider players in vents as recently seen
            }

            Vector3 otherPlayerPosition = playerControl.transform.position;
            if (Vector2.Distance(otherPlayerPosition, PlayerControl.LocalPlayer.transform.position) < 5f)
            {
                var otherPlayerTruePosition = playerControl.GetTruePosition();
                if (IsVisible(localPlayerTruePosition, otherPlayerTruePosition))
                {
                    LastSeenPlayer lastSeenPlayer = _playerLocations[playerControl];
                    lastSeenPlayer.location = GetLocationFromPosition(otherPlayerPosition);
                    lastSeenPlayer.time = Time.timeSinceLevelLoad;
                    lastSeenPlayer.dead = false;
                    lastSeenPlayer.gameTimeVisible += Time.fixedDeltaTime; // Keep track of total time we've been able to see this player
                    lastSeenPlayer.roundTimeVisible += Time.fixedDeltaTime; // Keep track of time this round we've been able to see this player

                    Vector2 vectorToPlayer = otherPlayerTruePosition - localPlayerTruePosition;
                    PlayerRecords[playerControl] = new PlayerRecord(vectorToPlayer.magnitude, vectorToPlayer.normalized);

                    Info(playerControl.name + " is in " + GetLocationFromPosition(otherPlayerPosition));
                }
                else
                {
                    Info($"{playerControl.Data.PlayerName} is close, but out of sight");
                }
            }
        }
    }

    private static bool IsVisible(Vector2 rayStart, Vector2 rayEnd)
    {
        // Raycasting
        // If raycast hits shadow, this usually means that player is not visible
        // So check that there is no shadow
        int layerShadow = LayerMask.GetMask("Shadow");
        Vector2 ray = rayEnd - rayStart;
        RaycastHit2D hit = Physics2D.Raycast(rayStart, ray.normalized, ray.magnitude, layerShadow);
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
            if (collider && collider.OverlapPoint(position))
            {
                if (room.RoomId == SystemTypes.Hallway)
                    nearPrefix = "a hallway near "; // keep looking for the nearest room
                else
                    return TranslationController.Instance.GetString(room.RoomId); // If we're inside a proper room, ignore the nearPrefix
            }
            else if (room.RoomId != SystemTypes.Hallway)
            {
                float distance = collider
                    ? Vector2.Distance(position, collider.ClosestPoint(position))
                    : Mathf.Infinity;
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

    public IReadOnlyDictionary<PlayerControl, LastSeenPlayer> GetPlayerLocations()
    {
        return _playerLocations;
    }
}

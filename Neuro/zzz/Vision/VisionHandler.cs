using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Recording.DataStructures;
using Reactor.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Vision;

[RegisterInIl2Cpp]
public sealed class OldVisionHandler : MonoBehaviour
{
    public VisionHandler(IntPtr ptr) : base(ptr) { }

    public Vector2 DirectionToNearestBody { get; set; }

    [HideFromIl2Cpp]
    public Dictionary<PlayerControl, PlayerRecord> PlayerRecords { get; } = new(Il2CppEqualityComparer<PlayerControl>.Instance);

    // TODO: These dictionaries arent cleaned after games
    // TODO: Handle players disconnecting
    private readonly List<DeadBody> _deadBodies = new();
    private readonly Dictionary<PlayerControl, LastSeenPlayer> _playerLocations = new(Il2CppEqualityComparer<PlayerControl>.Instance);
    public IReadOnlyDictionary<PlayerControl, LastSeenPlayer> PlayerLocations => _playerLocations;

    private float roundStartTime; // in seconds

    public void StartTrackingPlayer(PlayerControl player)
    {
        foreach (PlayerControl playerControl in PlayerControl.AllPlayerControls)
        {
            PlayerRecords[playerControl] = new PlayerRecord(-1, new MyVector2(0, 0));
            _playerLocations[playerControl] = new LastSeenPlayer("", 0f, false);
        }

        Info("Updating playerControls");
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
    }
}

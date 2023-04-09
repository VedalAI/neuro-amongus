using System;
using System.IO;
using Neuro.Communication.AmongUsAI;
using Neuro.Events;
using Neuro.Utilities;
using Neuro.Utilities.Collections;
using Neuro.Vision;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording.OtherPlayers;

[RegisterInIl2Cpp]
public sealed class OtherPlayersRecorder : MonoBehaviour, ISerializable
{
    public static OtherPlayersRecorder Instance { get; private set; }

    public OtherPlayersRecorder(IntPtr ptr) : base(ptr) { }

    public AnchoredUnstableDictionary<byte, OtherPlayerData> Data { get; } = new();

    public void Serialize(BinaryWriter writer)
    {
        for (byte i = 0; i < 15; i++)
        {
            if (PlayerControl.LocalPlayer.PlayerId == i) continue;

            if (!Data.TryGetValue(i, out OtherPlayerData data)) data = default;
            data.Serialize(writer);
        }
    }

    private void Awake()
    {
        if (Instance)
        {
            NeuroUtilities.WarnDoubleSingletonInstance();
            Destroy(this);
            return;
        }

        Instance = this;
        EventManager.RegisterHandler(this);
    }

    private void FixedUpdate()
    {
        if (MeetingHud.Instance || Minigame.Instance) return;

        foreach (PlayerControl playerControl in PlayerControl.AllPlayerControls)
        {
            if (playerControl.AmOwner || playerControl.Data.IsDead) continue;
            if (!Visibility.IsVisible(playerControl)) continue;

            if (!Data.TryGetValue(playerControl.PlayerId, out OtherPlayerData visionData))
            {
                visionData = OtherPlayerData.Create(playerControl);
            }

            Data[playerControl, playerControl.PlayerId] = visionData;
        }
    }

    [EventHandler(EventTypes.MeetingEnded)]
    public void ResetAfterMeeting()
    {
        foreach (byte id in Data.Keys)
        {
            PlayerControl player = GameData.Instance.GetPlayerById(id).Object;
            Data[player, id] = Data[id].ResetAfterMeeting();
        }
    }

    [EventHandler(EventTypes.PlayerDied)]
    public void OnPlayerDied(PlayerControl player, DeathReason _)
    {
        Data.Remove(player.PlayerId);
    }

    [EventHandler(EventTypes.GameStarted)]
    private static void OnGameStarted(ShipStatus shipStatus)
    {
        shipStatus.gameObject.AddComponent<OtherPlayersRecorder>();
    }
}

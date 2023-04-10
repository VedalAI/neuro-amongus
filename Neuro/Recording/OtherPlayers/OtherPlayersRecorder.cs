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

    public AnchoredUnstableDictionary<byte, OtherPlayerData> LastSeen { get; } = new();

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
        if (MeetingHud.Instance || Minigame.Instance || !PlayerControl.LocalPlayer) return;

        foreach (PlayerControl playerControl in PlayerControl.AllPlayerControls)
        {
            if (playerControl.AmOwner || playerControl.Data.IsDead) continue;
            if (!Visibility.IsVisible(playerControl)) continue;

            if (!LastSeen.TryGetValue(playerControl.PlayerId, out OtherPlayerData visionData))
            {
                visionData = OtherPlayerData.Create(playerControl);
            }

            LastSeen[playerControl, playerControl.PlayerId] = visionData.UpdateVisible(playerControl);
        }
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write((byte) LastSeen.Count);
        foreach (OtherPlayerData player in LastSeen.Values)
        {
            player.Serialize(writer);
        }
    }

    [EventHandler(EventTypes.MeetingEnded)]
    public void ResetAfterMeeting()
    {
        foreach (byte id in LastSeen.Keys)
        {
            PlayerControl player = GameData.Instance.GetPlayerById(id).Object;
            if (!player || player.Data.IsDead) LastSeen.Remove(id);
            LastSeen[player, id] = LastSeen[id].ResetAfterMeeting();
        }
    }

    [EventHandler(EventTypes.GameStarted)]
    private static void OnGameStarted(ShipStatus shipStatus)
    {
        shipStatus.gameObject.AddComponent<OtherPlayersRecorder>();
    }
}

using System;
using System.Linq;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Events;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording.OtherPlayers;

[RegisterInIl2Cpp]
public sealed class OtherPlayersRecorder : MonoBehaviour
{
    public static OtherPlayersRecorder Instance { get; private set; }

    public OtherPlayersRecorder(IntPtr ptr) : base(ptr) { }

    [HideFromIl2Cpp]
    public OtherPlayersFrame Frame { get; } = new();

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

            if (Frame.LastSeenPlayers.FirstOrDefault(p => p.Id == playerControl.PlayerId) is not { } player)
            {
                OtherPlayerData newData = OtherPlayerData.Create(playerControl);
                Frame.LastSeenPlayers.Add(newData);
            }
            else
            {
                player.UpdateVisible(playerControl);
            }
        }
    }

    [EventHandler(EventTypes.MeetingEnded)]
    public void ResetAfterMeeting()
    {
        for (int i = 0; i < Frame.LastSeenPlayers.Count; i++)
        {
            OtherPlayerData data = Frame.LastSeenPlayers[i];

            PlayerControl player = GameData.Instance.GetPlayerById((byte) data.Id).Object;
            if (!player || player.Data.IsDead)
            {
                Frame.LastSeenPlayers.RemoveAt(i);
                i--;
                continue;
            }

            data.ResetAfterMeeting();
        }
    }

    [EventHandler(EventTypes.GameStarted)]
    private static void OnGameStarted()
    {
        ShipStatus.Instance.gameObject.AddComponent<OtherPlayersRecorder>();
    }
}

using System;
using Neuro.Utilities;
using Neuro.Utilities.Collections;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Vision.Players;

[RegisterInIl2Cpp]
public sealed class PlayerControlVisionHandler : MonoBehaviour
{
    public static PlayerControlVisionHandler Instance { get; private set; }

    public PlayerControlVisionHandler(IntPtr ptr) : base(ptr) { }

    public SelfUnstableDictionary<PlayerControl, PlayerControlVisionData> Data { get; } = new();

    private void Awake()
    {
        if (Instance)
        {
            LogUtils.WarnDoubleSingletonInstance();
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void FixedUpdate()
    {
        if (MeetingHud.Instance || Minigame.Instance) return;

        foreach (PlayerControl playerControl in PlayerControl.AllPlayerControls)
        {
            if (playerControl.AmOwner || playerControl.Data.IsDead) continue;
            if (!Visibility.IsVisible(playerControl)) continue;

            if (!Data.TryGetValue(playerControl, out PlayerControlVisionData visionData))
            {
                visionData = Data[playerControl] = PlayerControlVisionData.Create();
            }

            visionData.UpdateVisible(playerControl);
        }
    }

    public void ResetAfterMeeting()
    {
        foreach (PlayerControlVisionData visionData in Data.Values)
        {
            visionData.ResetAfterMeeting();
        }
    }
}

using System;
using System.Collections.Generic;
using Neuro.Events;
using Neuro.Utilities;
using Neuro.Vision;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.zzz.Vision.DeadBodies;

[RegisterInIl2Cpp]
public sealed class DeadBodyVisionHandler : MonoBehaviour
{
    public static DeadBodyVisionHandler Instance { get; private set; }

    public DeadBodyVisionHandler(IntPtr ptr) : base(ptr) { }

    public DeadBody Nearest { get; private set; }

    public Dictionary<byte, DeadBodyVisionData> Data { get; } = new();

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

        Nearest = null;
        float nearestBodyDistance = Mathf.Infinity;

        foreach (DeadBody deadBody in ComponentCache<DeadBody>.Cached)
        {
            if (!Visibility.IsVisible(deadBody)) continue;

            float distance = Vector2.Distance(deadBody.TruePosition, PlayerControl.LocalPlayer.GetTruePosition());
            if (distance < nearestBodyDistance)
            {
                nearestBodyDistance = distance;
                Nearest = deadBody;
            }

            if (!Data.ContainsKey(deadBody.ParentId))
            {
                Data[deadBody.ParentId] = DeadBodyVisionData.Create(deadBody);
            }
        }
    }

    [EventHandler(EventTypes.MeetingEnded)]
    public void ResetAfterMeeting()
    {
        Data.Clear();
    }

    [EventHandler(EventTypes.GameStarted)]
    private static void OnGameStarted()
    {
        ShipStatus.Instance.gameObject.AddComponent<DeadBodyVisionHandler>();
    }
}
